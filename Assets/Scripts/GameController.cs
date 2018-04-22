using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ConstructionMode
{
    None = 0,
    Road,
    Building,
    Nature,
    Defense,
    Bulldozer
}

public class GameController : MonoBehaviour {
    public static GameController instance;

    public Text InfoLabel;
    public Text MoneyLabel;

    public int MapSize = 10;
    public float MeshSize = 10;

    public City city;

    public Map map;

    public ConstructionMode Mode = ConstructionMode.None;

    public ToolbarController ToolbarController;

    public AnimationCurve InitialPlacementCurve;

    float[] rotations = new float[] { 0, 90, 180, 270 };

    private void Awake()
    {
        instance = this;
    }

    void Start () {
        city = new City();
        city.map = new Map(MapSize);

        PlaceRandomBuildings();

        ToolbarController.OnChangeMode += (mode) => Mode = mode;
	}
	
	// Update is called once per frame
	void Update () {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector2 gridPos = Vector2.zero;
        Vector2Int twoPos = Vector2Int.zero;
        bool IsOverMap = false;
        bool IsOverConstruction = false;

        InfoLabel.text = "";
        MoneyLabel.text = "$" + city.Money.ToString("N2");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Constructed")))
        {
            InfoLabel.text = hit.collider.transform.parent.gameObject.GetComponent<NamedThing>().ThingName();
            IsOverConstruction = true;

            Vector3 worldPos = (hit.point + (Vector3.one * (MeshSize / 2))) / MeshSize;
            twoPos = new Vector2Int(Mathf.FloorToInt(worldPos.x * MapSize), Mathf.FloorToInt(worldPos.z * MapSize));
        }
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            Vector3 worldPos = (hit.point + (Vector3.one * (MeshSize / 2))) / MeshSize;
            twoPos = new Vector2Int(Mathf.FloorToInt(worldPos.x * MapSize), Mathf.FloorToInt(worldPos.z * MapSize));

            float cellSize = MeshSize / MapSize;

            gridPos.x = (((float)twoPos.x / MapSize) * MeshSize) - (MeshSize / 2) + (cellSize / 2);
            gridPos.y = (((float)twoPos.y / MapSize) * MeshSize) - (MeshSize / 2) + (cellSize / 2);

            IsOverMap = true;
        }

        if (!IsOverMap && !IsOverConstruction)
        {
            return;
        }

        if (Input.GetMouseButton(0) && !IsOverConstruction)
        {
            if (Mode == ConstructionMode.Building)
            {
                PlaceBuilding(twoPos);
            }
            else if (Mode == ConstructionMode.Road)
            {
                if (city.PlaceRoad(twoPos.x, twoPos.y))
                {
                    GameObject bPrefab = Resources.Load<GameObject>("Prefabs/RoadGeneric");
                    GameObject v = Instantiate(bPrefab, new Vector3(gridPos.x, 0, gridPos.y), Quaternion.identity, transform);
                    v.GetComponent<CityThingView>().thing = city.map.CellAtPosition(twoPos.x, twoPos.y).thing;
                    city.map.CellAtPosition(twoPos.x, twoPos.y).thing.view = v.GetComponent<CityThingView>();
                }
            }
            else if (Mode == ConstructionMode.Nature)
            {
                PlaceTree(twoPos);
            }
        }
        else if (Input.GetMouseButton(0) && IsOverConstruction)
        {
            if (Mode == ConstructionMode.Bulldozer)
            {
                if (city.Money - 500 >= 0)
                {
                    city.Money -= 500;
                    Destroy(city.map.CellAtPosition(twoPos.x, twoPos.y).thing.view.gameObject);
                    city.DestroyAt(twoPos.x, twoPos.y);
                }
            }
        }
    }

    void PlaceRandomBuildings()
    {
        for (int y = 0; y < MapSize; y++)
        {
            float yMod = InitialPlacementCurve.Evaluate((float)y / MapSize);

            for (int x = 0; x < MapSize; x++)
            {
                float xMod = InitialPlacementCurve.Evaluate((float)x / MapSize);

                if (Random.Range(0f, 1f) < xMod && Random.Range(0f, 1f) < yMod)
                {
                    PlaceBuilding(new Vector2Int(x, y), true);
                }
                else if (Random.Range(0f, 1f) > 0.7f)
                {
                    PlaceTree(new Vector2Int(x, y), true);
                }
            }
        }
    }

    public void PlaceBuilding(Vector2Int twoPos, bool free=false)
    {
        float cellSize = MeshSize / MapSize;
        Vector2 gridPos = Vector2.zero;

        gridPos.x = (((float)twoPos.x / MapSize) * MeshSize) - (MeshSize / 2) + (cellSize / 2);
        gridPos.y = (((float)twoPos.y / MapSize) * MeshSize) - (MeshSize / 2) + (cellSize / 2);

        if (city.PlaceBuilding(twoPos.x, twoPos.y, BuildingTypes.Small, free))
        {
            GameObject bPrefab = Resources.Load<GameObject>(Random.Range(0f, 1f) > 0.5f ? "Prefabs/Building-1" : (Random.Range(0f, 1f) > 0.5f ? "Prefabs/Building-2" : "Prefabs/Building-3"));

            GameObject v = Instantiate(bPrefab, new Vector3(gridPos.x, 0, gridPos.y), Quaternion.identity, transform);
            v.transform.rotation = Quaternion.Euler(0, rotations[Random.Range(0, 4)], 0);

            v.GetComponent<CityThingView>().thing = city.map.CellAtPosition(twoPos.x, twoPos.y).thing;
            city.map.CellAtPosition(twoPos.x, twoPos.y).thing.view = v.GetComponent<CityThingView>();
        }
    }

    public void PlaceTree(Vector2Int twoPos, bool free=false)
    {
        float cellSize = MeshSize / MapSize;
        Vector2 gridPos = Vector2.zero;

        gridPos.x = (((float)twoPos.x / MapSize) * MeshSize) - (MeshSize / 2) + (cellSize / 2);
        gridPos.y = (((float)twoPos.y / MapSize) * MeshSize) - (MeshSize / 2) + (cellSize / 2);

        if (city.PlaceTree(twoPos.x, twoPos.y, free))
        {
            GameObject bPrefab = Resources.Load<GameObject>(Random.Range(0f, 1f) > 0.5f ? "Prefabs/Tree-1" : (Random.Range(0f, 1f) > 0.5f ? "Prefabs/Tree-2" : "Prefabs/Tree-3"));
            GameObject v = Instantiate(bPrefab, new Vector3(gridPos.x, 0, gridPos.y), Quaternion.identity, transform);

            v.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            v.GetComponent<CityThingView>().thing = city.map.CellAtPosition(twoPos.x, twoPos.y).thing;
            city.map.CellAtPosition(twoPos.x, twoPos.y).thing.view = v.GetComponent<CityThingView>();
        }
    }

    public void OnBomb(Vector3 pos, float damage)
    {
        Vector3 worldPos = (pos + (Vector3.one * (MeshSize / 2))) / MeshSize;
        Vector2Int twoPos = new Vector2Int(Mathf.FloorToInt(worldPos.x * MapSize), Mathf.FloorToInt(worldPos.z * MapSize));

        if (twoPos.x < 0 || twoPos.x > MapSize || twoPos.y < 0 || twoPos.y > MapSize)
            return;

        CityThing t = null;
        if (city.map.CellAtPosition(twoPos.x, twoPos.y) != null)
            t = city.map.CellAtPosition(twoPos.x, twoPos.y).thing;

        if (t == null || t.type == CellType.Empty)
            return;

        t.Damage += damage;

        Vector2 gridPos = Vector2.zero;
        float cellSize = MeshSize / MapSize;

        gridPos.x = (((float)twoPos.x / MapSize) * MeshSize) - (MeshSize / 2) + (cellSize / 2);
        gridPos.y = (((float)twoPos.y / MapSize) * MeshSize) - (MeshSize / 2) + (cellSize / 2);

        if (t.type == CellType.Building && t.state != ThingState.Damaged)
        {
            Destroy(t.view.gameObject);

            GameObject bPrefab = Resources.Load<GameObject>(Random.Range(0f, 1f) > 0.5f ? "Prefabs/Bombed-1" : "Prefabs/Bombed-2");
            GameObject v = Instantiate(bPrefab, new Vector3(gridPos.x, 0, gridPos.y), Quaternion.identity, transform);
            v.transform.rotation = Quaternion.Euler(0, rotations[Random.Range(0, 4)], 0);
            v.GetComponent<CityThingView>().thing = t;
            t.view = v.GetComponent<CityThingView>();
            t.state = ThingState.Damaged;
            t.Name = "A damaged building";

        } else if (t.view != null)
        {
            //Destroy(t.view);
            //t.view.gameObject.SetActive(false);

            t.state = ThingState.Damaged;
        }
    }
}

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
    public Text InfoLabel;

    public int MapSize = 10;
    public float MeshSize = 10;

    public City city;

    public Map map;

    public ConstructionMode Mode = ConstructionMode.None;

    public ToolbarController ToolbarController;

	// Use this for initialization
	void Start () {
        city = new City();
        city.map = new Map(MapSize);

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

        InfoLabel.text = "";

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Constructed")))
        {
            InfoLabel.text = hit.collider.transform.parent.gameObject.GetComponent<NamedThing>().ThingName();
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

        if (!IsOverMap)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            if (Mode == ConstructionMode.Building)
            {
                if (city.PlaceBuilding(twoPos.x, twoPos.y, BuildingTypes.Small))
                {
                    GameObject bPrefab = Resources.Load<GameObject>(Random.Range(0f, 1f) > 0.5f ? "Prefabs/TallBuilding" : "Prefabs/ShortBuilding");
                    GameObject v = Instantiate(bPrefab, new Vector3(gridPos.x, 0, gridPos.y), Quaternion.identity, transform);
                    v.GetComponent<CityThingView>().thing = city.map.CellAtPosition(twoPos.x, twoPos.y).thing;
                }
            }
            else if (Mode == ConstructionMode.Road)
            {
                if (city.PlaceRoad(twoPos.x, twoPos.y))
                {
                    GameObject bPrefab = Resources.Load<GameObject>("Prefabs/RoadGeneric");
                    GameObject v = Instantiate(bPrefab, new Vector3(gridPos.x, 0, gridPos.y), Quaternion.identity, transform);
                    v.GetComponent<CityThingView>().thing = city.map.CellAtPosition(twoPos.x, twoPos.y).thing;
                }
            }
            else if (Mode == ConstructionMode.Nature)
            {
                if (city.PlaceRoad(twoPos.x, twoPos.y))
                {
                    GameObject bPrefab = Resources.Load<GameObject>(Random.Range(0f, 1f) > 0.5f ? "Prefabs/Tree-1" : (Random.Range(0f, 1f) > 0.5f ? "Prefabs/Tree-2" : "Prefabs/Tree-3"));
                    GameObject v = Instantiate(bPrefab, new Vector3(gridPos.x, 0, gridPos.y), Quaternion.identity, transform);

                    v.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

                    v.GetComponent<CityThingView>().thing = city.map.CellAtPosition(twoPos.x, twoPos.y).thing;
                }
            }

        }
    }
}

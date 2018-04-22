using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InteractionController : MonoBehaviour {

    public Text InfoLabel;

    public AudioSource Audio;
    public AudioClip[] Sounds;

    // Use this for initialization
    void Start () {
        Audio = GetComponent<AudioSource>();
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

        float MeshSize = GameController.instance.MeshSize;
        int MapSize = GameController.instance.MapSize;

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
            bool didSomething = false;

            if (GameController.instance.Mode == ConstructionMode.Building)
            {
                didSomething = GameController.instance.PlaceBuilding(twoPos);
            }
            else if (GameController.instance.Mode == ConstructionMode.Defense)
            {
                didSomething = GameController.instance.PlaceDefense(twoPos);
            }
            else if (GameController.instance.Mode == ConstructionMode.Road)
            {
                City city = GameController.instance.city;

                if (city.PlaceRoad(twoPos.x, twoPos.y))
                {
                    GameObject bPrefab = Resources.Load<GameObject>("Prefabs/RoadGeneric");
                    GameObject v = Instantiate(bPrefab, new Vector3(gridPos.x, 0, gridPos.y), Quaternion.identity, transform);
                    v.GetComponent<CityThingView>().thing = city.map.CellAtPosition(twoPos.x, twoPos.y).thing;
                    city.map.CellAtPosition(twoPos.x, twoPos.y).thing.view = v.GetComponent<CityThingView>();
                }
            }
            else if (GameController.instance.Mode == ConstructionMode.Nature)
            {
                didSomething = GameController.instance.PlaceTree(twoPos);
            }

            if (didSomething)
            {
                Audio.clip = Sounds[0];
                Audio.Play(200);
            }
        }
        else if (Input.GetMouseButton(0) && IsOverConstruction)
        {
            if (GameController.instance.Mode == ConstructionMode.Bulldozer)
            {
                if (GameController.instance.city.Money - 500 >= 0)
                {
                    GameController.instance.city.Money -= 500;
                    Destroy(GameController.instance.city.map.CellAtPosition(twoPos.x, twoPos.y).thing.view.gameObject);
                    GameController.instance.city.DestroyAt(twoPos.x, twoPos.y);

                    Audio.clip = Sounds[1];
                    Audio.Play(200);
                }
            }
        }
    }
}

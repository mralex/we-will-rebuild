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

    public Text MoneyLabel;

    public GameObject NextWaveContainer;
    public Text NextWaveLabel;
    public Text SmallNextWaveLabel;
    float nextWaveTime;
    bool IsWaveCountdown = false;

    public GameObject Scoreboard;
    public Text DamageReceived;
    public Text DamageDealt;
    public Text ThingsOnFire;
    public Text NewMoney;

    public GameObject GameOverBoard;
    public Text TotalDamageReceived;
    public Text TotalDamageDealt;
    public Text TotalWaves;
    public Text TotalMoney;


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

        WaveController.instance.OnWaveAlert += OnWaveAlert;
        WaveController.instance.OnWaveBegin += OnWaveBegin;
        WaveController.instance.OnWaveEnded += OnWaveEnded;

        WaveController.instance.Invoke("EnqueueWave", 4);
	}

	// Update is called once per frame
	void Update () {
        MoneyLabel.text = "$" + city.Money.ToString("N2");

        if (IsWaveCountdown)
        {
            nextWaveTime -= Time.deltaTime;
            SmallNextWaveLabel.text = "Wave: " + WaveController.instance.WaveCount + "\nNext Wave: " + nextWaveTime.ToString("N2");
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
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

    public bool PlaceBuilding(Vector2Int twoPos, bool free=false)
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

            return true;
        }

        return false;
    }

    public bool PlaceTree(Vector2Int twoPos, bool free=false)
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
            return true;
        }

        return false;
    }

    public bool PlaceDefense(Vector2Int twoPos, bool free = false)
    {
        float cellSize = MeshSize / MapSize;
        Vector2 gridPos = Vector2.zero;

        gridPos.x = (((float)twoPos.x / MapSize) * MeshSize) - (MeshSize / 2) + (cellSize / 2);
        gridPos.y = (((float)twoPos.y / MapSize) * MeshSize) - (MeshSize / 2) + (cellSize / 2);

        if (city.PlaceDefense(twoPos.x, twoPos.y, free))
        {
            GameObject bPrefab = Resources.Load<GameObject>("Prefabs/Launcher-1");

            GameObject v = Instantiate(bPrefab, new Vector3(gridPos.x, 0, gridPos.y), Quaternion.identity, transform);
            v.transform.rotation = Quaternion.Euler(0, rotations[Random.Range(0, 4)], 0);

            v.GetComponent<CityThingView>().thing = city.map.CellAtPosition(twoPos.x, twoPos.y).thing;
            city.map.CellAtPosition(twoPos.x, twoPos.y).thing.view = v.GetComponent<CityThingView>();
            return true;
        }

        return false;
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
        city.damageReceived += damage;

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

    bool ScoreGame()
    {
        int buildingCount = city.BuildingCount;
        int damagedBuildingCount = city.DamagedBuildingCount;

        if (buildingCount == 0 || damagedBuildingCount == buildingCount || damagedBuildingCount > (buildingCount * 0.75f))
        {
            // GAME OVER
            TotalDamageReceived.text = city.totalDamageReceived.ToString("N2");
            TotalDamageDealt.text = city.totalDamageDealt.ToString("N2");
            TotalMoney.text = city.totalMoney.ToString("N2");
            TotalWaves.text = WaveController.instance.WaveCount.ToString();

            return false;
        }

        float gross = buildingCount * 100;
        float money;

        if (damagedBuildingCount < 1)
        {
            money = gross * 1.2f;
        } else
        {
            money = ((buildingCount / damagedBuildingCount) * 1.3f) * gross;
        }

        money += city.damageDealt * 1.6f;
        money -= city.damageReceived * 0.3f;

        city.Money += money;
        city.totalMoney += money;

        DamageReceived.text = city.damageReceived.ToString("N2");
        city.totalDamageReceived += city.damageReceived;
        city.damageReceived = 0;

        DamageDealt.text = city.damageDealt.ToString("N2");
        city.totalDamageDealt += city.damageDealt;
        city.damageDealt = 0;

        ThingsOnFire.text = damagedBuildingCount.ToString() + "/" + buildingCount.ToString();
        NewMoney.text = "$" + money.ToString("N2");

        return true;
    }

    void OnWaveAlert(int wave, float time)
    {
        nextWaveTime = time;
        IsWaveCountdown = true;

        StartCoroutine(ShowWaveDetail(wave, time));
    }

    IEnumerator ShowWaveDetail(int wave, float time)
    {
        NextWaveLabel.text = "NEXT WAVE IN\n" + time + " SECONDS";
        NextWaveContainer.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        NextWaveContainer.SetActive(false);
    }

    void OnWaveBegin()
    {
        IsWaveCountdown = false;
        SmallNextWaveLabel.text = "Wave: " + WaveController.instance.WaveCount;

        city.EngageLaunchers();

        StartCoroutine(ShowWaveIncoming());
    }

    IEnumerator ShowWaveIncoming()
    {
        NextWaveLabel.text = "NEXT WAVE\nINCOMING";
        NextWaveContainer.SetActive(true);

        yield return new WaitForSeconds(3);

        NextWaveContainer.SetActive(false);
    }

    void OnWaveEnded(int wave)
    {
        city.DisengageLaunchers();

        if (ScoreGame())
        {
            Scoreboard.SetActive(true);
        } else
        {
            GameOverBoard.SetActive(true);
        }
        
    }

    public void OnQuit()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void OnContinue()
    {
        Scoreboard.SetActive(false);
        WaveController.instance.Invoke("EnqueueWave", 4);
    }

    public void OnNewGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}

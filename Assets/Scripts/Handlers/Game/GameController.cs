using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public enum State
{
    Menu,
    Game
}

public class GameController : MonoBehaviour{

    static Action onGameOpened;
    static Action onGameClosed;
    static Action onGameStarted;
    static Action onGameTick;

    static Action onMenuOpened;
    static Action onMenuClosed;
    static Action onMenuStarted;

    public static float gameTick = 1;
    static bool isRunning = false;
    public static bool paused = false;

    public static bool debug = false;

    public static State state = State.Menu;

    public AnimationCurve lightLevels;

    static GameController instance;

    static GameData save;

    public static int day
    {
        get
        {
            return 1 + Mathf.FloorToInt(time / 1440);
        }
    }
    //Time in minutes
    static int time;
    public static int Time
    {
        get
        {
            return (time % 1440);
        }
    }
    public Light sun;
    public Text clock;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            SceneManager.sceneLoaded += LevelLoaded;
            path = Application.persistentDataPath + "/saves.sav";
            DontDestroyOnLoad(gameObject);
            Init();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        OpenMenu();
    }

    static void OpenMenu()
    {
        state = State.Menu;
        LoadFromFile();
        if (onMenuOpened != null)
            onMenuOpened();
        SceneManager.LoadScene("MainMenu");
    }

    static void CloseMenu()
    {
        if (onMenuClosed != null)
            onMenuClosed();
    }

    static void StartMenu()
    {
        if (onMenuStarted != null)
            onMenuStarted();
    }

    public static void OpenGame(GameData data)
    {
        CloseMenu();
        save = data;
        if (onGameOpened != null)
            onGameOpened();
        SceneManager.LoadScene("World");
    }

    public static void CloseGame()
    {
        SaveGame();
        isRunning = false;
        if (onGameClosed != null)
            onGameClosed();
        OpenMenu();
    }

    void StartGame()
    {
        OnPreStart();
        RegisterCallbacks();

        Civilization.SetName(save.name);
        time = save.time;
        World.instance.GenerateWorld(save.world);
        Technology.Load(save.technologies);
        if (save.currentTech != -1)
            Technology.SetCurrentTechnology(Technology.GetTechnology(save.currentTech));
        if (save.resources != null)
            ResourcePool.Load(save.resources);
        City.LoadCities(save.cities);
        Building.Load(save.buildings);
        City.LoadWorkers(save.cities);
        if (save.armies != null)
            Army.Load(save.armies);

        OnPostLoad();

        Worker.UpdatePopulation();
        state = State.Game;
        if (sun == null)
            sun = GameObject.FindWithTag("Sun").GetComponent<Light>();
        if (clock == null)
            clock = GameObject.Find("Clock").GetComponent<Text>();
        if (onGameStarted != null)
            onGameStarted();
        isRunning = true;

        OnStart();

        StartCoroutine("GameTick");
    }

    void Init()
    {
        FindObjectOfType<World>().Init();
        Building.Init();
        Technology.Init();
        NameGenerator.Init();
        Worker.Init();
    }

    void OnPreStart()
    {
        ResourcePool.inventory.inventory.Clear();
        Technology.PreStart();
        Building.PreStart();
        Residential.PreStart();
        Worker.PreStart();
        Unit.PreStart();
        Army.PreStart();
        City.PreStart();
        Age.PreStart();
        FindObjectOfType<CityViewUI>().PreStart();
        FindObjectOfType<ChunkViewUI>().PreStart();
        FindObjectOfType<ResourceViewUI>().PreStart();
        FindObjectOfType<BuildingListUI>().PreStart();
    }

    void RegisterCallbacks()
    {
        FindObjectOfType<UIHandler>().RegisterCallbacks();
        FindObjectOfType<SoundHandler>().RegisterCallbacks();
        FindObjectOfType<ArmyViewUI>().RegisterCallbacks();
        FindObjectOfType<BattleViewUI>().RegisterCallbacks();
        FindObjectOfType<ChunkViewUI>().RegisterCallbacks();
        FindObjectOfType<CivInfoPanelUI>().RegisterCallbacks();
        FindObjectOfType<CityViewUI>().RegisterCallbacks();
        FindObjectOfType<ResourceViewUI>().RegisterCallbacks();
        FindObjectOfType<BuildingListUI>().RegisterCallbacks();
        FindObjectOfType<TechnologyViewUI>().RegisterCallbacks();
        FindObjectOfType<TechProgressUI>().RegisterCallbacks();
    }

    void OnPostLoad()
    {
        FindObjectOfType<TechnologyViewUI>().OnPostLoad();
    }

    void OnStart()
    {
        Technology.Start();
        Building.Start();
        FindObjectOfType<UIHandler>().OnStart();

        SetGameSpeed(1);
        debug = false;
    }

    void LevelLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        if (instance != this)
            return;
        switch (scene.name)
        {
            case "MainMenu":
                StartMenu();
                break;
            case "World":
                StartGame();
                break;
        }
    }

    public static void ExitGame()
    {
        Application.Quit();
    }

    public static void SetGameSpeed(int speed)
    {
        gameTick = 1f / speed;
    }

    IEnumerator GameTick()
    {
        while (isRunning)
        {
            if (!paused)
            {
                if (onGameTick != null)
                    onGameTick();
                time++;
                clock.text = "Day " + day + " " + string.Format("{0}:{1:00}", Mathf.FloorToInt(Time / 60), Time % 60);
                sun.intensity = lightLevels.Evaluate(Time / 1440f);
                sun.transform.rotation = Quaternion.Euler(Mathf.Lerp(270f, -90f, Time / 1440f), 90f, 0f);
            }
            yield return new WaitForSeconds(gameTick);
        }
    }

    #region Saving / Loading

    public static GameData NewGame(string name)
    {
        GameData gameData = new GameData();
        GameData.WorldData worldData = new GameData.WorldData();

        gameData.ID = GameController.SavesCount;
        gameData.lastPlayed = DateTime.Now;
        gameData.name = name;
        gameData.time = 480;

        worldData.seed = World.seed;
        worldData.generatedChunks = new Coord[] {
            new Coord(0, 0),
            new Coord(-1, 0),
            new Coord(1, 0),
            new Coord(0, -1),
            new Coord(0, 1)
        };
        worldData.claimedChunks = new Coord[]
        {
            new Coord(0, 0)
        };

        gameData.world = worldData;

        gameData.currentTech = -1;
        gameData.technologies = new GameData.TechData[] {
            new GameData.TechData(Technology.fire)
        };

        gameData.resources = GameData.ResourceData.Convert(ResourceStack.Add(
                Building.campfire.cost,
                Building.hut.cost,
                Building.stockpile.cost,
                Building.gatherersHut.cost,
                new ResourceStack[]
                {
                    new ResourceStack(Resource.berries, 20)
                }
            ));

        gameData.cities = new GameData.CityData[0];

        return gameData;
    }

    public static void SaveGame()
    {
        GameData gameData = new GameData();

        gameData.ID = save.ID;
        gameData.lastPlayed = DateTime.Now;
        gameData.name = Civilization.name;
        gameData.time = GameController.time;

        GameData.WorldData worldData = World.instance.SaveWorld();
        gameData.world = worldData;

        GameData.BuildingData[] buildingData = Building.Save();
        gameData.buildings = buildingData;

        gameData.resources = ResourcePool.Save();

        if (Technology.currentTech == null)
            gameData.currentTech = -1;
        else
            gameData.currentTech = Technology.currentTech.ID;
        gameData.technologies = Technology.Save();

        gameData.cities = City.Save();

        gameData.armies = Army.Save();

        SaveData(gameData);
    }

    public static void DeleteSave(GameData data)
    {
        saves.Remove(data.ID);
        SaveToFile();
    }

    public static Dictionary<int, GameData> saves;

    public static int SavesCount
    {
        get
        {
            return saves.Count;
        }
    }

    static string path;

    static void SaveData(GameData data)
    {
        if (saves.ContainsKey(data.ID))
            saves[data.ID] = data;
        else
            saves.Add(data.ID, data);

        SaveToFile();
    }

    static void SaveToFile()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(path);
        formatter.Serialize(file, saves);
        file.Close();
    }

    static void LoadFromFile()
    {
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            saves = (Dictionary<int, GameData>)formatter.Deserialize(file);
            file.Close();
        }
        else
        {
            saves = new Dictionary<int, GameData>();
        }
    }

    #endregion

    #region Callbacks

    public static void RegisterOnGameOpened(Action callback)
    {
        onGameOpened += callback;
    }

    public static void RegisterOnGameClosed(Action callback)
    {
        onGameClosed += callback;
    }

    public static void RegisterOnGameTick(Action callback)
    {
        onGameTick += callback;
    }

    public static void UnregisterOnGameTick(Action callback)
    {
        onGameTick -= callback;
    }

    public static void RegisterOnGameStarted(Action callback)
    {
        onGameStarted += callback;
    }

    public static void RegisterOnMenuOpened(Action callback)
    {
        onMenuOpened += callback;
    }

    public static void RegisterOnMenuClosed(Action callback)
    {
        onMenuClosed += callback;
    }

    public static void RegisterOnMenuStarted(Action callback)
    {
        onMenuStarted += callback;
    }

    #endregion

}

[System.Serializable]
public class GameData
{
    public int ID;
    public DateTime lastPlayed;

    public string name;
    public int time;

    public WorldData world;

    public BuildingData[] buildings;

    public ResourceData[] resources;

    public int currentTech;
    public TechData[] technologies;

    public CityData[] cities;

    public ArmyData[] armies;

    [System.Serializable]
    public struct SerializeVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializeVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator Vector3(SerializeVector3 v3)
        {
            return new Vector3(v3.x, v3.y, v3.z);
        }

        public static implicit operator SerializeVector3(Vector3 v3)
        {
            return new SerializeVector3(v3.x, v3.y, v3.z);
        }
    }

    [System.Serializable]
    public struct SerializeQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public SerializeQuaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static implicit operator Quaternion(SerializeQuaternion q)
        {
            return new Quaternion(q.x, q.y, q.z, q.w);
        }

        public static implicit operator SerializeQuaternion(Quaternion q)
        {
            return new SerializeQuaternion(q.x, q.y, q.z, q.w);
        }
    }

    [System.Serializable]
    public class WorldData
    {
        public int seed;
        public Coord[] generatedChunks;
        public Coord[] claimedChunks;
    }

    [System.Serializable]
    public class ResourceData
    {
        public string name;
        public int amount;

        public ResourceData(string name, int amount)
        {
            this.name = name;
            this.amount = amount;
        }

        public ResourceData(ResourceStack stack)
        {
            this.name = stack.resource.name;
            this.amount = stack.amount;
        }

        public static ResourceData[] Convert(ResourceStack[] stacks)
        {
            ResourceData[] data = new ResourceData[stacks.Length];
            for (int i = 0; i < stacks.Length; i++)
                data[i] = new ResourceData(stacks[i]);
            return data;
        }
    }

    [System.Serializable]
    public class BuildingData
    {
        public string name;
        public string city;
        public SerializeVector3 location;
        public SerializeQuaternion rotation;
        public string toolType;
        public int durability;

        public virtual Building Load()
        {
            Building building = Building.buildingPrototypes[name].Copy();
            building.location = location;
            building.rotation = rotation;
            if (toolType != "")
            {
                building.tool = Resource.GetResource(toolType) as Tool;
                building.durability = durability;
            }
            building.SetCity(City.GetCity(city));
            building.OnBuildingPlaced();
            return building;
        }
    }

    [System.Serializable]
    public class ResidentialData : BuildingData
    {
        public ConsumableData[] consumables;

        public ResidentialData(BuildingData data)
        {
            this.name = data.name;
            this.city = data.city;
            this.location = data.location;
            this.rotation = data.rotation;
            this.toolType = data.toolType;
            this.durability = data.durability;
        }

        public override Building Load()
        {
            Residential building = base.Load() as Residential;
            foreach (ConsumableData data in consumables) {
                Consumable consumable = building.myConsumables[data.ID];
                consumable.progress = data.progress;
                consumable.satisfaction = data.satisfaction;
            }
            return building;
        }
    }

    [System.Serializable]
    public class ConsumableData
    {
        public int ID;
        public int progress;
        public int satisfaction;
    }

    [System.Serializable]
    public class TechData
    {
        public int ID;
        public int progress;
        public bool discovered;

        public TechData(int ID, int progress, bool discovered)
        {
            this.ID = ID;
            this.progress = progress;
            this.discovered = discovered;
        }

        public TechData(Technology tech)
        {
            ID = tech.ID;
            progress = tech.progress;
            discovered = tech.discovered;
        }
    }

    [System.Serializable]
    public class WorkerData
    {
        public string name;
        public bool employed;
        public int workBuilding;
        public int workOrder;
    }

    [System.Serializable]
    public class CityData
    {
        public string name;
        public float happiness;
        public float loyalty;
        public CityCenter[] centers;
        public WorkerData[] workers;
    }

    [System.Serializable]
    public class UnitData
    {
        public int ID;
        public string[] workers;
    }

    [System.Serializable]
    public class ArmyData
    {
        public string name;
        public string commander;
        public UnitData[] units;
        public int center;
    }
}



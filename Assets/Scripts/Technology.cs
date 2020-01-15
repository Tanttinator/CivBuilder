using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Technology {

    static List<Technology> technologies = new List<Technology>();

    public static Technology fire = new Technology("Fire", 0, Age.stoneAge);
    public static Technology tools = new Technology("Tools", 10, Age.stoneAge);
    public static Technology hunting = new Technology("Hunting", 20, Age.stoneAge);
    public static Technology mining = new Technology("Mining", 20, Age.stoneAge);
    public static Technology weaving = new Technology("Weaving", 20, Age.stoneAge);
    public static Technology fishing = new Technology("Fishing", 20, Age.stoneAge);
    public static Technology pottery = new Technology("Pottery", 30, Age.stoneAge);
    public static Technology archery = new Technology("Archery", 50, Age.stoneAge);
    public static Technology agriculture = new Technology("Agriculture", 50, Age.stoneAge);
    public static Technology animalHusbandry = new Technology("Animal Husbandry", 50, Age.stoneAge);
    public static Technology casting = new Technology("Casting", 70, Age.stoneAge);
    public static Technology socialStructure = new Technology("Social Structure", 70, Age.stoneAge);
    public static Technology leatherWorking = new Technology("Leather Working", 100, Age.stoneAge);
    public static Technology military = new Technology("Military", 100, Age.stoneAge);
    public static Technology goldWorking = new Technology("Gold Working", 100, Age.stoneAge);
    //public static Technology bronzeWorking = new Technology("Bronze Working", 150, Age.BronzeAge);
    //public static Technology ironWorking = new Technology("Iron Working", 200, Age.IronAge);

    public int ID
    {
        get
        {
            return technologies.IndexOf(this);
        }
    }
    public string name;
    public int cost;
    public string description;
    public Age age;
    public Technology[] techUnlocks;
    public Building[] buildingUnlocks;
    public UnlockWorkOrder[] workOrderUnlocks;
    public Consumable[] consumableUnlocks;
    public bool discovered = false;
    public List<Technology> requiredTechs = new List<Technology>();
    public int progress = 0;

    static List<Technology> discoveredTechs = new List<Technology>();
    static List<Technology> unlockedTechs = new List<Technology>();
    public static Technology currentTech;

    Action onProgress;

    static Action<Technology> onTechDiscovered;
    static Action<Technology> onTechUnlocked;
    static Action<Technology> onCurrentTechSet;

	public Technology(string name, int cost, Age age)
    {
        this.name = name;
        this.cost = cost;
        this.age = age;

        technologies.Add(this);
    }

    public static void Init()
    {
        fire.SetUnlockedTechnologies(tools);
        fire.SetUnlockedBuildings(Building.campfire);
        tools.SetUnlockedTechnologies(hunting, mining, pottery, weaving, fishing);
        tools.SetUnlockedBuildings(Building.toolmaker);
        hunting.SetUnlockedTechnologies(archery, leatherWorking);
        hunting.SetUnlockedBuildings(Building.huntersCabin);
        mining.SetUnlockedTechnologies(casting);
        mining.SetUnlockedBuildings(Building.mine);
        fishing.SetUnlockedBuildings(Building.fishingHut);
        weaving.SetUnlockedBuildings(Building.weavery, Building.tailor);
        weaving.SetUnlockedWorkOrders(new UnlockWorkOrder(Building.toolmaker, Building.makeStoneTool), new UnlockWorkOrder(Building.gatherersHut, Building.gatherHemp));
        pottery.SetUnlockedTechnologies(agriculture, animalHusbandry);
        pottery.SetUnlockedBuildings(Building.potter, Building.kiln, Building.well);
        //archery.SetUnlockedTechnologies(military);
        archery.SetUnlockedBuildings(Building.archeryRange, Building.bowyer);
        archery.SetUnlockedWorkOrders(new UnlockWorkOrder(Building.weavery, Building.weaveBowstring));
        agriculture.SetUnlockedTechnologies(socialStructure);
        agriculture.SetUnlockedBuildings(Building.farm, Building.bakery, Building.grindstone);
        animalHusbandry.SetUnlockedBuildings(Building.pasture);
        casting.SetUnlockedTechnologies(goldWorking);
        casting.SetUnlockedWorkOrders(new UnlockWorkOrder(Building.kiln, Building.kilnSmeltCopper), new UnlockWorkOrder(Building.mine, Building.mineCopper));
        //socialStructure.SetUnlockedTechnologies(military);
        socialStructure.SetUnlockedBuildings(Building.chieftainsHut);
        military.SetUnlockedBuildings(Building.stagingGrounds);
        leatherWorking.SetUnlockedBuildings(Building.tannery);
        goldWorking.SetUnlockedBuildings(Building.goldsmith);
        goldWorking.SetUnlockedWorkOrders(new UnlockWorkOrder(Building.mine, Building.mineGold), new UnlockWorkOrder(Building.kiln, Building.kilnSmeltGold));
        //bronzeWorking.SetUnlockedTechnologies(ironWorking);
        //bronzeWorking.SetUnlockedWorkOrders(new UnlockWorkOrder(Building.kiln, Building.kilnSmeltBronze));
        //ironWorking.SetUnlockedWorkOrders(new UnlockWorkOrder(Building.mine, Building.mineIron));
    }

    public static void PreStart()
    {
        unlockedTechs.Clear();
        discoveredTechs.Clear();
        foreach (Technology tech in technologies)
        {
            tech.discovered = false;
            tech.progress = 0;
        }
    }

    public static void Start()
    {

    }

    #region Global functions

    public static void SetCurrentTechnology(Technology tech)
    {
        currentTech = tech;
        if (onCurrentTechSet != null)
            onCurrentTechSet(tech);
        if (tech.cost == 0 || GameController.debug)
            tech.Discover();
    }

    public static Technology GetTechnology(int ID)
    {
        return technologies[ID];
    }

    #endregion

    #region Local Functions

    public void Progress(int amount)
    {
        progress = Mathf.Min(progress += amount, cost);
        if (onProgress != null)
            onProgress();
        if (progress >= cost)
            Discover();
    }
    
    public void Discover()
    {
        currentTech = null;
        discoveredTechs.Add(this);
        discovered = true;
        /*if(!GameController.debug)
            ResourcePool.inventory.Remove(new ResourceStack(Resource.research, cost));*/
        if (Technology.onTechDiscovered != null)
            Technology.onTechDiscovered(this);
        if (techUnlocks != null)
        {
            foreach (Technology tech in techUnlocks)
            {
                if (tech.isUnlocked())
                    tech.Unlock();
            }
        }
        if(buildingUnlocks != null)
        {
            foreach(Building building in buildingUnlocks)
            {
                Building.DiscoverBuilding(building);
            }
        }
        if(workOrderUnlocks != null)
        {
            foreach (UnlockWorkOrder workOrder in workOrderUnlocks)
                workOrder.Unlock();
        }
        if (Age.currentAge == null || age.ID > Age.currentAge.ID)
            Age.EnterAge(age);
    }

    public void Unlock()
    {
        if (unlockedTechs.Contains(this))
            return;
        unlockedTechs.Add(this);
        if (Technology.onTechUnlocked != null)
            Technology.onTechUnlocked(this);
    }

    public bool isUnlocked()
    {
        foreach(Technology tech in requiredTechs)
        {
            if (!tech.discovered)
            {
                return false;
            }
        }

        return true;
    }

    #endregion

    #region Properties

    public Technology SetUnlockedTechnologies(params Technology[] techs)
    {
        techUnlocks = techs;
        foreach (Technology tech in techs)
        {
            tech.requiredTechs.Add(this);
        }
        return this;
    }

    public Technology SetUnlockedBuildings(params Building[] buildings)
    {
        buildingUnlocks = buildings;
        description += "\nUnlocked Buildings:";
        foreach (Building building in buildings)
            description += "\n" + building.name;
        return this;
    }

    public Technology SetUnlockedWorkOrders(params UnlockWorkOrder[] workOrders)
    {
        workOrderUnlocks = workOrders;
        description += "\nUnlocked Work Orders:";
        foreach (UnlockWorkOrder workOrder in workOrders)
            description += "\n" + workOrder.workOrder.description + " for " + workOrder.building.name;
        return this;
    }

    public Technology SetUnlockedConsumables(params Consumable[] consumables)
    {
        consumableUnlocks = consumables;
        return this;
    }

    #endregion

    #region Saving / Loading

    public static void Load(GameData.TechData[] techs)
    {
        foreach(GameData.TechData data in techs)
        {
            Technology tech = technologies[data.ID];
            tech.Unlock();
            tech.progress = data.progress;
            if (data.discovered)
                tech.Discover();
        }
    }

    public static GameData.TechData[] Save()
    {
        List<GameData.TechData> techData = new List<GameData.TechData>();
        foreach (Technology tech in unlockedTechs)
            techData.Add(new GameData.TechData(tech));
        return techData.ToArray();
    }

    #endregion

    #region Callbacks

    public static void RegisterOnTechDiscovered(Action<Technology> callback)
    {
        onTechDiscovered += callback;
    }

    public static void UnregisterOnTechDiscovered(Action<Technology> callback)
    {
        onTechDiscovered -= callback;
    }

    public static void RegisterOnTechUnlocked(Action<Technology> callback)
    {
        onTechUnlocked += callback;
    }

    public static void UnregisterOnTechUnlocked(Action<Technology> callback)
    {
        onTechUnlocked -= callback;
    }

    public static void RegisterOnCurrentTechSet(Action<Technology> callback)
    {
        onCurrentTechSet += callback;
    }

    public static void UnregisterOnCurrentTechSet(Action<Technology> callback)
    {
        onCurrentTechSet -= callback;
    }

    public void RegisterOnProgress(Action callback)
    {
        onProgress += callback;
    }

    public void UnregisterOnProgress(Action callback)
    {
        onProgress -= callback;
    }

    #endregion
}

public class Age
{
    /*StoneAge,       //-3000 eaa
    BronzeAge,      //3000 - 1000 eaa
    IronAge,        //1000 - 700 eaa
    ClassicalAge,   //700 eaa - 500
    MiddleAge,      //500 - 1400
    Renaissance,    //1400 - 1700
    Enlightenment,  //1700 - 1800
    IndustrialAge,  //1800 - 1950
    AtomicAge,      //1950 - 2000
    SpaceAge        //2000 - */

    public static Age currentAge;

    public static Age stoneAge = new Age(0, "Stone Age", new Unit[] { Unit.wolf, Unit.bear }, new Consumable[] { new Consumable(ResourceType.Food, 60), new Consumable(Resource.water, 40), new Consumable(Resource.jewelry, 320), new Consumable(Resource.clothing, 240) });

    public int ID;
    public string name;
    public Unit[] enemies;
    public Consumable[] consumables;

    public Age(int ID, string name, Unit[] enemies, Consumable[] consumables)
    {
        this.ID = ID;
        this.name = name;
        this.enemies = enemies;
        this.consumables = consumables;
    }

    public static void PreStart()
    {
        currentAge = null;
    }

    public static void EnterAge(Age age)
    {
        currentAge = age;
        Residential.AddConsumable(age.consumables);
    }
}

public struct UnlockWorkOrder
{
    public Building building;
    public WorkOrder workOrder;

    public UnlockWorkOrder(Building building, WorkOrder workOrder)
    {
        this.building = building;
        this.workOrder = workOrder;
    }

    public void Unlock()
    {
        Building.UnlockWorkOrder(building, workOrder);
    }
}

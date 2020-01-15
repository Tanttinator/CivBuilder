using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum BuildingClass
{
    Central,
    Governance,
    Logistics,
    Military,
    MilitaryCentral,
    Production,
    Research,
    Residential
}

public enum BuildingType
{
    Governance,
    Logistics,
    Military,
    Production,
    Residential,
    Research
}

public class Building {

    public enum PlaceTerrain
    {
        Land,
        Coast,
        Water
    }

    public static Dictionary<string, Building> buildingPrototypes = new Dictionary<string, Building>();
    public static List<Building> buildings = new List<Building>();
    public static Dictionary<GameObject, Building> buildingGOMap = new Dictionary<GameObject, Building>();

    #region Costs
    static ResourceStack[] campfireCost =
    {
        new ResourceStack(Resource.influence, 20),
        new ResourceStack(Resource.stone, 20),
        new ResourceStack(Resource.wood, 20)
    };
    static ResourceStack[] hutCost = {
        new ResourceStack(Resource.stone, 10),
        new ResourceStack(Resource.wood, 10)
    };
    static ResourceStack[] stockpileCost =
    {
        new ResourceStack(Resource.clay, 15),
        new ResourceStack(Resource.stone, 15),
        new ResourceStack(Resource.wood, 15)
    };
    static ResourceStack[] gatherersHutCost =
    {
        new ResourceStack(Resource.clay, 10),
        new ResourceStack(Resource.stone, 10),
        new ResourceStack(Resource.wood, 10)
    };
    static ResourceStack[] clayPitCost =
    {
        new ResourceStack(Resource.stone, 15),
        new ResourceStack(Resource.wood, 15)
    };
    static ResourceStack[] shamansHutCost =
    {
        new ResourceStack(Resource.clay, 15),
        new ResourceStack(Resource.stone, 15),
        new ResourceStack(Resource.wood, 15)
    };
    static ResourceStack[] toolmakerCost =
    {
        new ResourceStack(Resource.clay, 10),
        new ResourceStack(Resource.stone, 20),
        new ResourceStack(Resource.wood, 10)
    };
    static ResourceStack[] huntersCabinCost =
    {
        new ResourceStack(Resource.clay, 15),
        new ResourceStack(Resource.stone, 25),
        new ResourceStack(Resource.wood, 10)
    };
    static ResourceStack[] mineCost =
    {
        new ResourceStack(Resource.clay, 35),
        new ResourceStack(Resource.stone, 35),
        new ResourceStack(Resource.wood, 30)
    };
    static ResourceStack[] weaveryCost =
    {
        new ResourceStack(Resource.clay, 15),
        new ResourceStack(Resource.stone, 25),
        new ResourceStack(Resource.wood, 10)
    };
    static ResourceStack[] tailorCost =
    {
        new ResourceStack(Resource.clay, 15),
        new ResourceStack(Resource.stone, 25),
        new ResourceStack(Resource.wood, 10)
    };
    static ResourceStack[] fishingHutCost =
    {
        new ResourceStack(Resource.clay, 15),
        new ResourceStack(Resource.stone, 25),
        new ResourceStack(Resource.wood, 10)
    };
    static ResourceStack[] kilnCost =
    {
        new ResourceStack(Resource.clay, 50),
        new ResourceStack(Resource.stone, 30),
        new ResourceStack(Resource.wood, 10)
    };
    static ResourceStack[] potterCost =
    {
        new ResourceStack(Resource.clay, 15),
        new ResourceStack(Resource.stone, 30),
        new ResourceStack(Resource.wood, 15)
    };
    static ResourceStack[] wellCost =
    {
        new ResourceStack(Resource.stone, 50),
        new ResourceStack(Resource.wood, 10)
    };
    static ResourceStack[] archeryRangeCost =
    {
        new ResourceStack(Resource.wood, 40),
        new ResourceStack(Resource.textile, 20)
    };
    static ResourceStack[] bowyerCost =
    {
        new ResourceStack(Resource.clay, 25),
        new ResourceStack(Resource.stone, 35),
        new ResourceStack(Resource.wood, 20)
    };
    static ResourceStack[] farmCost =
    {
        new ResourceStack(Resource.wood, 50),
        new ResourceStack(Resource.stone, 10)
    };
    static ResourceStack[] grindstoneCost =
    {
        new ResourceStack(Resource.stone, 45),
        new ResourceStack(Resource.wood, 15)
    };
    static ResourceStack[] bakeryCost =
    {
        new ResourceStack(Resource.clay, 25),
        new ResourceStack(Resource.stone, 35),
        new ResourceStack(Resource.wood, 20)
    };
    static ResourceStack[] pastureCost =
    {
        new ResourceStack(Resource.wood, 50),
        new ResourceStack(Resource.stone, 10)
    };
    static ResourceStack[] chieftainsHutCost =
    {
        new ResourceStack(Resource.clay, 35),
        new ResourceStack(Resource.stone, 35),
        new ResourceStack(Resource.wood, 30)
    };
    static ResourceStack[] stagingGroundsCost =
    {
        new ResourceStack(Resource.textile, 30),
        new ResourceStack(Resource.stone, 50),
        new ResourceStack(Resource.wood, 20)
    };
    static ResourceStack[] tanneryCost =
    {
        new ResourceStack(Resource.clay, 50),
        new ResourceStack(Resource.stone, 60),
        new ResourceStack(Resource.wood, 40)
    };
    static ResourceStack[] goldSmithCost =
    {
        new ResourceStack(Resource.clay, 50),
        new ResourceStack(Resource.stone, 60),
        new ResourceStack(Resource.wood, 40)
    };
    #endregion

    #region WorkOrders
    public static Gather gatherWood = new Gather("Gather Wood", 20, new ResourceStack[] {
        new ResourceStack(Resource.wood, 2)
    });
    public static Gather gatherStone = new Gather("Gather Stone", 20, new ResourceStack[] {
        new ResourceStack(Resource.stone, 2)
    });
    public static Gather gatherFood = new Gather("Gather Food", 20, new ResourceStack[] {
        new ResourceStack(Resource.berries, 2)
    });
    public static Gather gatherHemp = new Gather("Gather Hemp", 20, new ResourceStack[] {
        new ResourceStack(Resource.hemp, 3)
    });
    public static Gather clayPitGather = new Gather("Gather Clay", 20, new ResourceStack[] {
        new ResourceStack(Resource.clay, 1)
    });
    public static Retrieve stockpileRetrieve = new Retrieve("Retireve", 5);
    public static Supply stockpileSupply = new Supply("Supply", 5);
    public static Research shamansHutResearch = new Research("Research", 20, 1);
    public static Gather fishingHutGather = new Gather("Fish", 20, new ResourceStack[] {
        new ResourceStack(Resource.fish, 2)
    });
    public static Gather chieftainsHutGather = new Command("Rule", 20, 1);
    public static Gather mineCopper = new Gather("Mine Copper", 20, new ResourceStack[] {
        new ResourceStack(Resource.copperOre, 1),
        new ResourceStack(Resource.stone, 2)
    }, 1);
    public static Gather mineTin = new Gather("Mine Tin", 20, new ResourceStack[] {
        new ResourceStack(Resource.tinOre, 1),
        new ResourceStack(Resource.stone, 2)
    }, 1);
    public static Gather mineGold = new Gather("Mine Gold", 20, new ResourceStack[] {
        new ResourceStack(Resource.goldOre, 1),
        new ResourceStack(Resource.stone, 2)
    }, 1);
    public static Gather mineIron = new Gather("Mine Iron", 20, new ResourceStack[] {
        new ResourceStack(Resource.ironOre, 1),
        new ResourceStack(Resource.stone, 2)
    }, 2);
    public static Gather mineSalt = new Gather("Mine Salt", 20, new ResourceStack[] {
        new ResourceStack(Resource.salt, 1),
        new ResourceStack(Resource.stone, 2)
    }, 0);
    public static Produce kilnSmeltCopper = new Produce("Smelt Copper", 30, new ResourceStack[] {
        new ResourceStack(Resource.copperOre, 1),
        new ResourceStack(Resource.wood, 2)
    }, new ResourceStack[] {
        new ResourceStack(Resource.copper, 1)
    });
    public static Produce kilnSmeltTin = new Produce("Smelt Tin", 30, new ResourceStack[] {
        new ResourceStack(Resource.tinOre, 1),
        new ResourceStack(Resource.wood, 2)
    }, new ResourceStack[] {
        new ResourceStack(Resource.tin, 1)
    });
    public static Produce kilnSmeltGold = new Produce("Smelt Gold", 30, new ResourceStack[] {
        new ResourceStack(Resource.goldOre, 1),
        new ResourceStack(Resource.wood, 2)
    }, new ResourceStack[] {
        new ResourceStack(Resource.gold, 1)
    });
    public static Produce kilnSmeltBronze = new Produce("Smelt Bronze", 30, new ResourceStack[] {
        new ResourceStack(Resource.copperOre, 1),
        new ResourceStack(Resource.tinOre, 1),
        new ResourceStack(Resource.wood, 4)
    }, new ResourceStack[] {
        new ResourceStack(Resource.bronze, 2)
    });
    public static Produce kilnFirePottery = new Produce("Fire Pottery", 30, new ResourceStack[] {
        new ResourceStack(Resource.unfiredPottery, 1)
    }, new ResourceStack[] {
        new ResourceStack(Resource.pottery, 1)
    });
    public static Gather huntersCabinGather = new Gather("Hunt", 40, new ResourceStack[] {
        new ResourceStack(Resource.meat, 1),
        new ResourceStack(Resource.hide, 1)
    });
    public static Produce weaveTextile = new Produce("Weave Textile", 40, new ResourceStack[] {
        new ResourceStack(Resource.twine, 3)
    }, new ResourceStack[] {
        new ResourceStack(Resource.textile, 1)
    });
    public static Produce weaveTwine = new Produce("Weave Twine", 20, new ResourceStack[] {
        new ResourceStack(Resource.hemp, 2)
    }, new ResourceStack[] {
        new ResourceStack(Resource.twine, 1)
    });
    public static Produce weaveYarn = new Produce("Weave Yarn", 20, new ResourceStack[]
    {
        new ResourceStack(Resource.hemp, 1)
    }, new ResourceStack[] {
        new ResourceStack(Resource.yarn, 1)
    });
    public static Produce weaveBowstring = new Produce("Weave Bowstring", 30, new ResourceStack[] {
        new ResourceStack(Resource.hemp, 2)
    }, new ResourceStack[] {
        new ResourceStack(Resource.bowstring, 1)
    });
    public static Produce createClothing = new Produce("Create Clothing", 30, new ResourceStack[]
    {
        new ResourceStack(Resource.yarn, 2),
        new ResourceStack(Resource.leather, 1),
        new ResourceStack(Resource.textile, 1)
    }, new ResourceStack[] {
        new ResourceStack(Resource.clothing, 1)
    });
    public static Command stagingGroundsCommand = new Command("Lead Army", 40, 1);
    public static Gather farmGather = new Gather("Farm", 50, new ResourceStack[] {
        new ResourceStack(Resource.wheat, 5)
    });
    public static Produce pastureProduce = new Produce("Herd", 30, new ResourceStack[] {
        new ResourceStack(Resource.wheat, 2)
    }, new ResourceStack[] {
        new ResourceStack(Resource.meat, 2),
        new ResourceStack(Resource.hide, 2)
    });
    public static Produce potterProduce = new Produce("Create Pottery", 20, new ResourceStack[] {
        new ResourceStack(Resource.clay, 2)
    }, new ResourceStack[] {
        new ResourceStack(Resource.unfiredPottery, 1)
    });
    public static Produce tanneryProduce = new Produce("Tan Hides", 40, new ResourceStack[] {
        new ResourceStack(Resource.hide, 1),
        new ResourceStack(Resource.wood, 1),
        new ResourceStack(Resource.water, 1),
        new ResourceStack(Resource.salt, 2)
    }, new ResourceStack[] {
        new ResourceStack(Resource.leather, 1)
    });
    public static Produce grindWheat = new Produce("Grind Wheat", 20, new ResourceStack[] {
        new ResourceStack(Resource.wheat, 1)
    }, new ResourceStack[] {
        new ResourceStack(Resource.wheatFlour, 1)
    });
    public static Produce bakeBread = new Produce("Bake Bread", 30, new ResourceStack[] {
        new ResourceStack(Resource.wheatFlour, 1)
    }, new ResourceStack[] {
        new ResourceStack(Resource.bread, 1)
    });
    public static Produce makeCrudeTool = new Produce("Create Crude Tools", 20, new ResourceStack[] {
        new ResourceStack(Resource.stone, 1),
        new ResourceStack(Resource.wood, 1)
    }, new ResourceStack[] {
        new ResourceStack(Resource.crudeTool, 1)
    });
    public static Produce makeStoneTool = new Produce("Create Stone Tools", 25, new ResourceStack[] {
        new ResourceStack(Resource.stone, 1),
        new ResourceStack(Resource.wood, 1),
        new ResourceStack(Resource.twine, 1)
    }, new ResourceStack[] {
        new ResourceStack(Resource.stoneTool, 1)
    }, 0);
    public static Produce createBows = new Produce("Create Bows", 40, new ResourceStack[] {
        new ResourceStack(Resource.bowstring, 1),
        new ResourceStack(Resource.wood, 1)
    }, new ResourceStack[] {
        new ResourceStack(Resource.bow, 1)
    });
    public static Produce createJewelry = new Produce("Create Jewelry", 40, new ResourceStack[] {
        new ResourceStack(Resource.gold, 1)
    }, new ResourceStack[] {
        new ResourceStack(Resource.jewelry, 1)
    });
    public static Produce collectWater = new Produce("Collect Water", 20, new ResourceStack[] {
        new ResourceStack(Resource.pottery, 1)
    }, new ResourceStack[] {
        new ResourceStack(Resource.water, 10)
    }, -1, true);
    #endregion

    #region Buildings
    public static Building hut = new Residential("Hut", "Basic residential building", hutCost, 2);
    public static Building gatherersHut = new Building("Gatherer's Hut", "Gathers materials for early construction.", BuildingClass.Production, BuildingType.Production, gatherersHutCost).SetWorkOrders(2, gatherWood, gatherStone, gatherFood);
    public static Building campfire = new Central("Campfire", "Early game central building.", campfireCost, 15f);
    public static Building clayPit = new Building("Clay Pit", "Gathers clay.", BuildingClass.Production, BuildingType.Production, clayPitCost).SetWorkOrders(2, clayPitGather);
    public static Building stockpile = new Building("Stockpile", "Retrieves items made by buildings in its range.", BuildingClass.Logistics, BuildingType.Logistics, stockpileCost).SetWorkOrders(2, stockpileRetrieve, stockpileSupply).SetRange(10f);
    public static Building shamansHut = new Building("Shaman's Hut", "Generates research.", BuildingClass.Research, BuildingType.Research, shamansHutCost).SetWorkOrders(1, shamansHutResearch);
    public static Building fishingHut = new Building("Fishing Hut", "Gathers fish.", BuildingClass.Production, BuildingType.Production, fishingHutCost).SetWorkOrders(2, fishingHutGather).SetPlaceTerrain(PlaceTerrain.Coast).AlignWithTerrain();
    public static Building chieftainsHut = new MilitaryBase("Chieftain's Hut", "Generates influence.", chieftainsHutCost, 5f, 4, BuildingType.Governance).SetWorkOrders(1, chieftainsHutGather).Unique().SetUnlockedUnits(Unit.brute);
    public static Building mine = new Building("Mine", "Mines ores.", BuildingClass.Production, BuildingType.Production, mineCost).SetWorkOrders(4, mineSalt).SetAngle(30f, 90f).AlignWithTerrain();
    public static Building kiln = new Building("Kiln", "Smelts ores.", BuildingClass.Production, BuildingType.Production, kilnCost).SetWorkOrders(2, kilnFirePottery);
    public static Building archeryRange = new Building("Archery Range", "Unlocks the archer unit.", BuildingClass.Military, BuildingType.Military, archeryRangeCost).SetUnlockedUnits(Unit.archer);
    public static Building huntersCabin = new Building("Hunter's Cabin", "Hunts wild animals.", BuildingClass.Production, BuildingType.Production, huntersCabinCost).SetWorkOrders(2, huntersCabinGather);
    public static Building weavery = new Building("Weavery", "Weaves fibres.", BuildingClass.Production, BuildingType.Production, weaveryCost).SetWorkOrders(2, weaveTwine, weaveTextile, weaveYarn);
    public static Building tailor = new Building("Tailor", "Creates clothing.", BuildingClass.Production, BuildingType.Production, tailorCost).SetWorkOrders(1, createClothing);
    public static Building stagingGrounds = new MilitaryBase("Staging Grounds", "Allows creation of an army.", stagingGroundsCost, 10f, 6).SetWorkOrders(1, stagingGroundsCommand);
    public static Building farm = new Building("Farm", "Farms wheat.", BuildingClass.Production, BuildingType.Production, farmCost).SetWorkOrders(1, farmGather);
    public static Building pasture = new Building("Pasture", "Herds animals.", BuildingClass.Production, BuildingType.Production, pastureCost).SetWorkOrders(1, pastureProduce);
    public static Building potter = new Building("Potter", "Creates Unfired Pottery", BuildingClass.Production, BuildingType.Production, potterCost).SetWorkOrders(1, potterProduce);
    public static Building tannery = new Building("Tannery", "Turns hides into leather.", BuildingClass.Production, BuildingType.Production, tanneryCost).SetWorkOrders(1, tanneryProduce);
    public static Building grindstone = new Building("Grindstone", "Grinds stuff.", BuildingClass.Production, BuildingType.Production, grindstoneCost).SetWorkOrders(1, grindWheat);
    public static Building bakery = new Building("Bakery", "Bakes stuff.", BuildingClass.Production, BuildingType.Production, bakeryCost).SetWorkOrders(1, bakeBread);
    public static Building toolmaker = new Building("Toolmaker", "Creates tools.", BuildingClass.Production, BuildingType.Production, toolmakerCost).SetWorkOrders(1, makeCrudeTool);
    public static Building bowyer = new Building("Bowyer", "Creates bows.", BuildingClass.Production, BuildingType.Production, bowyerCost).SetWorkOrders(1, createBows);
    public static Building goldsmith = new Building("Goldsmith", "Creates various items with gold.", BuildingClass.Production, BuildingType.Production, goldSmithCost).SetWorkOrders(1, createJewelry);
    public static Building well = new Building("Well", "Generates water.", BuildingClass.Production, BuildingType.Production, wellCost).SetWorkOrders(1, collectWater);
    #endregion

    //Local variables
    public Vector3 location;
    public Quaternion rotation;
    public GameObject GO;
    public Inventory input;
    public Inventory output;
    public List<Resource> request;
    public List<Building> buildingsInRange;
    public List<Worker> workers;
    public bool[] isWorkerWorking;
    public bool isWorking;
    public Projector rangeDisplay;
    public City city;
    public Tool tool;
    public int durability = 0;
    public int toolTierRequest = -1;

    //Prototype variables
    public string name;
    public string description;
    public BuildingClass buildingClass;
    public BuildingType type;
    public GameObject prefab;
    public ResourceStack[] cost;
    public List<WorkOrder> workOrders;
    public int maxWorkers;
    public float range;
    public Building[] unlockedBuildings;
    public bool unique;
    public float minAngle;
    public float maxAngle;
    public PlaceTerrain placeTerrain;
    public Unit[] unlockedUnits;
    public bool flatten;

    public bool alignWithTerrain = false;
    bool discovered = false;

    //Callbacks
    Action<int> onWorkersChanged;
    Action<Worker> onWorkerAdded;
    Action<Worker> onWorkerRemoved;

    static Action<Building> onBuildingPlaced;
    static Action<Building> onBuildingDiscovered;

    public Building(string name, string description, BuildingClass buildingClass, BuildingType type, ResourceStack[] cost)
    {
        this.name = name;
        this.description = description;
        this.buildingClass = buildingClass;
        this.type = type;
        this.cost = cost;

        prefab = Resources.Load<GameObject>("Buildings/" + name);
        if (prefab == null)
            Debug.LogError("Couldn't load prefab for " + name);
        prefab.transform.position = new Vector3(0, 0, 0);
        prefab.tag = "Building";
        Rigidbody rb = prefab.GetComponent<Rigidbody>();
        if(rb == null)
        {
            rb = prefab.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.isKinematic = true;
        if(prefab.GetComponent<BuildingGO>() == null)
            prefab.AddComponent<BuildingGO>();

        workOrders = new List<WorkOrder>();
        maxWorkers = 0;
        unlockedBuildings = null;
        range = 0f;
        unique = false;
        minAngle = 0f;
        maxAngle = 45f;
        placeTerrain = PlaceTerrain.Land;
        flatten = true;

        if(!buildingPrototypes.ContainsKey(name))
            buildingPrototypes.Add(name, this);
    }

    public Building(Building prototype)
    {
        name = prototype.name;
        description = prototype.description;
        buildingClass = prototype.buildingClass;
        type = prototype.type;
        cost = prototype.cost;

        prefab = prototype.prefab;

        maxWorkers = prototype.maxWorkers;
        unlockedBuildings = prototype.unlockedBuildings;
        unlockedUnits = prototype.unlockedUnits;
        range = prototype.range * World.ScaleFactor;
        unique = prototype.unique;
        minAngle = prototype.minAngle;
        maxAngle = prototype.maxAngle;
        placeTerrain = prototype.placeTerrain;
        flatten = prototype.flatten;

        input = new Inventory();
        output = new Inventory();
        request = new List<Resource>();
        buildingsInRange = new List<Building>();

        workOrders = new List<WorkOrder>();
        if (prototype.workOrders != null)
        {
            foreach (WorkOrder workOrder in prototype.workOrders)
            {
                workOrders.Add(workOrder.Copy());
            }
        }
        isWorkerWorking = new bool[maxWorkers];
        isWorking = false;

        workers = new List<Worker>();
        toolTierRequest = -1;
    }

    public static void GameTick()
    {
        foreach (Building building in buildings)
        {
            building.OnGameTick();
        }
    }

    public static void Init()
    {
        hut.SetUnlockedBuildings(stockpile);
        campfire.SetUnlockedBuildings(hut);
        stockpile.SetUnlockedBuildings(gatherersHut, clayPit, shamansHut);

        GameController.RegisterOnGameTick(GameTick);
    }

    public static void PreStart()
    {
        foreach (Building building in buildingPrototypes.Values)
            building.discovered = false;

        buildings = new List<Building>();
        buildingGOMap = new Dictionary<GameObject, Building>();
    }

    public static void Start()
    {

    }

    #region Building Properties

    public Building SetWorkOrders(int workers, params WorkOrder[] workOrders)
    {
        maxWorkers = workers;
        this.workOrders = workOrders.ToList();

        return this;
    }

    public Building SetUnlockedBuildings(params Building[] unlockedBuildings)
    {
        this.unlockedBuildings = unlockedBuildings;

        return this;
    }

    public Building SetRange(float range)
    {
        this.range = range;

        return this;
    }

    public Building SetAngle(float min, float max)
    {
        minAngle = min;
        maxAngle = max;

        if (min > 0)
            flatten = false;

        return this;
    }

    public Building Unique()
    {
        this.unique = true;
        description += "\nCan only have one";
        return this;
    }

    public Building AlignWithTerrain()
    {
        this.alignWithTerrain = true;
        return this;
    }

    public Building SetPlaceTerrain(PlaceTerrain terrain)
    {
        this.placeTerrain = terrain;
        return this;
    }

    public Building SetUnlockedUnits(params Unit[] units)
    {
        this.unlockedUnits = units;
        return this;
    }

    #endregion

    #region Local Functions

    public virtual void OnBuildingPlaced()
    {
        GetBuildingsInRange(this);

        foreach (Building other in buildings)
        {
            if (other == this || other.range == 0)
                continue;
            if (Vector3.Distance(location, other.location) < other.range)
            {
                other.OnBuildingPlacedInRange(this);
            }
        }

        if (unlockedBuildings != null)
        {
            foreach (Building prototype in unlockedBuildings)
                DiscoverBuilding(prototype);
        }

        BuildingHandler.instance.PlaceBuildingGO(this);
        buildings.Add(this);

        if (onBuildingPlaced != null)
            onBuildingPlaced(this);
    }

    public virtual void OnNewBuildingPlaced()
    {

    }

    public virtual void OnBuildingPlacedInRange(Building building)
    {
        buildingsInRange.Add(building);
    }

    public virtual void OnGameTick()
    {
        Work();
    }

    public virtual Building Copy()
    {
        Building building = new Building(this);
        return building;
    }

    public void SetCity(City city)
    {
        this.city = city;
        city.buildings.Add(this);
    }

    public void Select()
    {
        if (range > 0)
            DisplayRange();
    }

    public void Deselect()
    {
        if (range > 0)
            HideRange();
    }

    public void Work()
    {
        if (workOrders == null)
            return;
        foreach (Worker worker in workers)
        {
            IsWorking(worker, worker.Work());
        }
    }

    public void IsWorking(Worker worker, bool isWorking)
    {
        isWorkerWorking[workers.IndexOf(worker)] = isWorking;
        bool work = false;
        foreach(bool b in this.isWorkerWorking)
        {
            if (b == true)
                work = true;
        }
        if (this.isWorking)
        {
            if (!work)
            {
                this.isWorking = false;
                GO.SendMessage("OnWorkEnd", SendMessageOptions.DontRequireReceiver);
            }
        }else if (work)
        {
            this.isWorking = true;
            GO.SendMessage("OnWorkStart", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void AddWorker()
    {
        if (workers.Count < maxWorkers && Worker.unemployed > 0)
        {
            Worker worker = city.Employ();
            AddWorker(worker, 0);
        }
    }

    public void AddWorker(Worker worker, int workID, bool onAssign = true)
    {
        worker.Employ(this);
        worker.SetWork(workOrders[workID]);
        if(onAssign)
            worker.work.OnAssign(this, worker);
        workers.Add(worker);
        if (onWorkersChanged != null)
            onWorkersChanged(workers.Count);
        if (onWorkerAdded != null)
            onWorkerAdded(worker);
    }

    public void UnemployWorker()
    {
        if (workers.Count > 0)
        {
            Worker worker = workers.Last();
            UnemployWorker(worker);
        }
    }

    public void UnemployWorker(Worker worker)
    {
        if (!workers.Contains(worker))
            return;
        IsWorking(worker, false);
        workers.Remove(worker);
        city.Unemploy(worker);
        if (onWorkersChanged != null)
            onWorkersChanged(workers.Count);
        if (onWorkerRemoved != null)
            onWorkerRemoved(worker);
    }

    public void RemoveWorker(Worker worker)
    {
        if (!workers.Contains(worker))
            return;
        workers.Remove(worker);
        if (onWorkersChanged != null)
            onWorkersChanged(workers.Count);
        if (onWorkerRemoved != null)
            onWorkerRemoved(worker);
    }

    public int GetWorkOrderIndex(WorkOrder workOrder)
    {
        for(int i = 0; i < workOrders.Count; i++)
        {
            if (workOrders[i].description == workOrder.description)
                return i;
        }

        return -1;
    }

    public void Supply(params ResourceStack[] stacks)
    {
        foreach(ResourceStack stack in stacks)
        {
            if (request.Contains(stack.resource))
            {
                request.Remove(stack.resource);
            }
            input.Add(stack);
        }
    }

    public void DisplayRange()
    {
        if(rangeDisplay != null)
            rangeDisplay.enabled = true;
    }

    public void HideRange()
    {
        if (rangeDisplay != null)
            rangeDisplay.enabled = false;
    }

    public virtual float GetIncome(Resource resource)
    {
        float income = 0f;
        foreach(Worker worker in workers)
        {
            income += worker.work.GetIncome(this, resource);
        }
        return income;
    }

    public void AddTool(Tool tool)
    {
        this.tool = tool;
        durability = tool.durability;
        toolTierRequest = -1;
    }

    #endregion

    #region Global Functions

    public static void CreateBuilding(Building prototype, Vector3 location, Quaternion rotation)
    {
        if (!GameController.debug)
            ResourcePool.inventory.Remove(prototype.cost);

        Building building = prototype.Copy();
        building.location = location;
        building.rotation = rotation;

        Building closestCentral = GetClosestOfTypes(building, BuildingClass.Central);
        if (!(building is Central) && closestCentral != null)
        {
            building.SetCity(closestCentral.city);
        }
        building.OnBuildingPlaced();
        building.OnNewBuildingPlaced();
    }

    public static void DiscoverBuilding(Building building)
    {
        if (building.discovered)
            return;
        building.discovered = true;
        if (onBuildingDiscovered != null)
            onBuildingDiscovered(building);
    }

    public static void GetBuildingsInRange(Building building)
    {
        if (building.range == 0)
            return;
        
        foreach (Building other in buildings)
        {
            if (other == building)
                continue;
            if (Vector3.Distance(building.location, other.location) < building.range)
            {
                building.OnBuildingPlacedInRange(other);
            }
        }
    }

    public static Building GetClosestBuildings(Building building, params Building[] types)
    {
        float minDist = Mathf.Infinity;
        Building closest = null;
        List<string> names = new List<string>();
        foreach(Building type in types)
        {
            if (!names.Contains(type.name))
                names.Add(type.name);
        }
            

        foreach(Building other in buildings)
        {
            if (other == building)
                continue;
            if (names.Contains(other.name))
            {
                if(Vector3.Distance(building.location, other.location) < minDist)
                {
                    minDist = Vector3.Distance(building.location, other.location);
                    closest = other;
                }
            }
        }

        return closest;
    }

    public static Building GetClosestBuildings(Vector3 point, params Building[] types)
    {
        float minDist = Mathf.Infinity;
        Building closest = null;
        List<string> names = new List<string>();
        foreach (Building type in types)
        {
            if (!names.Contains(type.name))
                names.Add(type.name);
        }


        foreach (Building other in buildings)
        {
            if (names.Contains(other.name))
            {
                if (Vector3.Distance(point, other.location) < minDist)
                {
                    minDist = Vector3.Distance(point, other.location);
                    closest = other;
                }
            }
        }

        return closest;
    }

    public static Building GetClosestOfTypes(Building building, params BuildingClass[] types)
    {
        float minDist = Mathf.Infinity;
        Building closest = null;
        foreach (Building other in buildings)
        {
            if (types.Contains(other.buildingClass) && other != building)
            {
                if (Vector3.Distance(building.location, other.location) < minDist)
                {
                    minDist = Vector3.Distance(building.location, other.location);
                    closest = other;
                }
            }
        }

        return closest;
    }

    public static Building GetClosestOfTypes(Vector3 point, params BuildingClass[] types)
    {
        float minDist = Mathf.Infinity;
        Building closest = null;
        foreach (Building other in buildings)
        {
            if (types.Contains(other.buildingClass))
            {
                if (Vector3.Distance(point, other.location) < minDist)
                {
                    minDist = Vector3.Distance(point, other.location);
                    closest = other;
                }
            }
        }

        return closest;
    }

    public static List<BuildingClass> GetShowRangeTypes(BuildingClass type)
    {
        List<BuildingClass> types = new List<BuildingClass>();

        switch (type)
        {
            case BuildingClass.Military:
                types.Add(BuildingClass.MilitaryCentral);
                types.Add(BuildingClass.Central);
                break;
            case BuildingClass.Production:
                types.Add(BuildingClass.Central);
                types.Add(BuildingClass.Logistics);
                break;
            default:
                types.Add(BuildingClass.Central);
                break;
        }

        return types;
    }

    public static int BuildingCount(string name)
    {
        int count = 0;
        foreach (Building building in buildings)
        {
            if (building.name == name)
                count++;
        }
        return count;
    }

    public static Building[] GetBuildingsOfType(Building prototype)
    {
        List<Building> returns = new List<Building>();
        foreach(Building building in buildings)
        {
            if (building.name == prototype.name)
                returns.Add(building);
        }

        return returns.ToArray();
    }

    public static Building GetBuildingFromGO(GameObject go)
    {
        return buildingGOMap[go];
    }

    public static void UnlockWorkOrder(Building prototype, WorkOrder workOrder)
    {
        prototype.workOrders.Add(workOrder);
        foreach(Building building in GetBuildingsOfType(prototype))
        {
            building.workOrders.Add(workOrder);
        }
    }

    #endregion

    #region Prototype Functions
    
    public bool CanBuild()
    {
        if (!ResourcePool.inventory.HasResources(cost))
        {
            UIHandler.DisplayTooltip("Not enough resources!");
            return false;
        }
        if (unique && BuildingCount(name) > 0)
        {
            UIHandler.DisplayTooltip("Can only have 1 " + name);
            return false;
        }
        return true;
    }

    #endregion

    #region Saving / Loading

    public virtual GameData.BuildingData GetData()
    {
        GameData.BuildingData data = new GameData.BuildingData();
        data.name = name;
        data.city = city.name;
        data.location = location;
        data.rotation = rotation;
        if (tool != null)
        {
            data.toolType = tool.name;
            data.durability = durability;
        }
        else
        {
            data.toolType = "";
            data.durability = 0;
        }
        return data;
    }

    public static void Load(GameData.BuildingData[] buildings)
    {
        if (buildings == null)
            return;
        foreach(GameData.BuildingData data in buildings)
        {
            data.Load();
        }
    }

    public static GameData.BuildingData[] Save()
    {
        List<GameData.BuildingData> dataList = new List<GameData.BuildingData>();

        foreach(Building building in buildings)
        {
            dataList.Add(building.GetData());
        }

        return dataList.ToArray();
    }

    #endregion

    #region Callbacks

    public void RegisterOnWorkersChanged(Action<int> callback)
    {
        onWorkersChanged += callback;
    }

    public void UnregisterOnWorkersChanged(Action<int> callback)
    {
        onWorkersChanged -= callback;
    }

    public void RegisterOnWorkerAdded(Action<Worker> callback)
    {
        onWorkerAdded += callback;
    }

    public void UnregisterOnWorkerAdded(Action<Worker> callback)
    {
        onWorkerAdded -= callback;
    }

    public void RegisterOnWorkerRemoved(Action<Worker> callback)
    {
        onWorkerRemoved += callback;
    }

    public void UnregisterOnWorkerRemoved(Action<Worker> callback)
    {
        onWorkerRemoved -= callback;
    }

    public static void RegisterOnBuildingPlaced(Action<Building> callback)
    {
        onBuildingPlaced += callback;
    }

    public static void UnregisterOnBuildingPlaced(Action<Building> callback)
    {
        onBuildingPlaced -= callback;
    }

    public static void RegisterOnBuildingDiscovered(Action<Building> callback)
    {
        onBuildingDiscovered += callback;
    }

    public static void UnregisterOnBuildingDiscovered(Action<Building> callback)
    {
        onBuildingDiscovered -= callback;
    }

    #endregion

}

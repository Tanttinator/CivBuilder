using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Residential : Building {

    public int population;
    static List<Consumable> consumables = new List<Consumable>();
    public List<Consumable> myConsumables = new List<Consumable>();
    public int happiness;

    public Residential(string name, string description, ResourceStack[] cost, int population) : base(name, description, BuildingClass.Residential, BuildingType.Residential, cost)
    {
        this.population = population;
    }

    public Residential(Residential prototype) : base(prototype)
    {
        population = prototype.population;
        foreach (Consumable consumable in consumables)
            AddConsumableLocal(consumable);
    }

    new public static void PreStart()
    {
        consumables.Clear();
    }

    public override void OnBuildingPlaced()
    {
        city.AddMaxPopulation(population);
        base.OnBuildingPlaced();
    }

    public override void OnGameTick()
    {
        Consume();
        base.OnGameTick();
    }

    public override float GetIncome(Resource resource)
    {
        float income = 0f;
        foreach (Consumable consumable in consumables)
            income += consumable.GetIncome(resource);
        return income + base.GetIncome(resource);
    }

    void Consume()
    {
        foreach (Consumable consumable in myConsumables)
            consumable.Consume();
        CalculateHappiness();
    }

    void CalculateHappiness()
    {
        int i = 0;
        int total = 0;
        foreach(Consumable consumable in myConsumables)
        {
            i++;
            total += consumable.satisfaction;
        }
        if(i != 0)
            happiness = total / i;
    }

    public static void AddConsumable(Consumable consumable)
    {
        consumables.Add(consumable);
        foreach(Building building in buildings)
        {
            Residential residential = null;
            if (building is Residential)
            {
                residential = building as Residential;
                residential.AddConsumableLocal(consumable);
            }
        }
    }

    public void AddConsumableLocal(Consumable consumable)
    {
        Consumable copy = consumable.Copy();
        copy.ID = myConsumables.Count;
        myConsumables.Add(copy);
    }

    public static void AddConsumable(Consumable[] consumables)
    {
        foreach (Consumable consumable in consumables)
            AddConsumable(consumable);
    }

    public override GameData.BuildingData GetData()
    {
        GameData.ResidentialData data = new GameData.ResidentialData(base.GetData());
        List<GameData.ConsumableData> consumables = new List<GameData.ConsumableData>();
        foreach (Consumable consumable in myConsumables)
            consumables.Add(consumable.GetData());
        data.consumables = consumables.ToArray();
        return data;
    }

    public override Building Copy()
    {
        return new Residential(this);
    }
}

public class Consumable
{
    public int ID;
    public Resource resource;
    public ResourceType type;
    public int rate;
    public int progress;
    public int satisfaction;

    public Consumable(Resource resource, int rate)
    {
        this.resource = resource;
        this.rate = rate;
        progress = 0;
        satisfaction = 50;
    }

    public Consumable(ResourceType type, int rate)
    {
        this.resource = null;
        this.type = type;
        this.rate = rate;
        this.progress = 0;
        satisfaction = 50;
    }

    public Consumable Copy()
    {
        if (resource == null)
            return new Consumable(type, rate);
        else
            return new Consumable(resource, rate);
    }

    public float GetIncome(Resource resource)
    {
        float income = 0f;
        if(this.resource == null)
        {
            if (resource == ResourcePool.inventory.GetMostOfType(type))
                income -= 60f / rate;
        }else if(this.resource == resource)
        {
            income -= 60f / rate;
        }
        return income;
    }

    public void Consume()
    {
        progress++;
        if(progress >= rate)
        {
            if(resource == null)
            {
                if (ResourcePool.inventory.HasType(type, 1))
                {
                    ResourcePool.inventory.Remove(new ResourceStack(ResourcePool.inventory.GetMostOfType(type), 1));
                    if(satisfaction != 100)
                        satisfaction++;
                }
                else
                {
                    if (satisfaction != 0)
                        satisfaction--;
                }
            }
            else
            {
                if (ResourcePool.inventory.HasResources(new ResourceStack(resource, 1)))
                {
                    ResourcePool.inventory.Remove(new ResourceStack(resource, 1));
                    if (satisfaction != 100)
                        satisfaction++;
                }
                else
                {
                    if (satisfaction != 0)
                        satisfaction--;
                }
            }
            progress = 0;
        }
    }

    public GameData.ConsumableData GetData()
    {
        GameData.ConsumableData data = new GameData.ConsumableData();
        data.ID = ID;
        data.progress = progress;
        data.satisfaction = satisfaction;
        return data;
    }
}

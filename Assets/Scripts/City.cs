using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class City {

    public string name;
    public float happiness = 1f;
    public float loyalty = 1f;
    public List<CityCenter> centers = new List<CityCenter>();

    public static List<City> cities = new List<City>();

    public int unemployed
    {
        get
        {
            return unemployedWorkers.Count;
        }
    }
    public int population
    {
        get
        {
            return workers.Count;
        }
    }
    public int maxPopulation = 0;
    public List<Worker> workers = new List<Worker>();
    public Queue<Worker> unemployedWorkers = new Queue<Worker>();
    public List<Building> buildings = new List<Building>();

    Action<int, int, int> onPopulationChanged;
    Action onUnemployedChanged;

    Action<Vector3> onCenterChanged;
    Action<float> onHappinessChanged;
    Action<float> onLoyaltyChanged;

    static Action<City> onCityCreated;

    public City(string name)
    {
        this.name = name;
        GameController.RegisterOnGameTick(CalculateHappiness);
        GameController.RegisterOnGameTick(AutobuyPopulation);
        if (onCityCreated != null)
            onCityCreated(this);
    }

    public static void PreStart()
    {
        cities = new List<City>();
    }

    public void AddCenter(CityCenter center)
    {
        centers.Add(center);
        Vector3 final = CalculateCenter();
        CityController.instance.PlaceBorderDisplay(center.center, center.range);
        if (onCenterChanged != null)
            onCenterChanged(final);
    }

    Vector3 CalculateCenter()
    {
        float x = 0;
        float z = 0;
        float y = 0;
        int i = 0;
        foreach(CityCenter center in centers)
        {
            Vector3 v3 = center.center;
            x += v3.x;
            z += v3.z;
            if (v3.y > y)
                y = v3.y;
            i++;
        }
        return new Vector3(x / i, y, z / i);
    }

    void CalculateHappiness()
    {
        int i = 0;
        int total = 0;
        foreach(Building building in buildings)
        {
            if(building is Residential)
            {
                i++;
                Residential residential = building as Residential;
                total += residential.happiness;
            }
        }
        if (i != 0)
        {
            happiness = (total / i) / 100f;
            if (onHappinessChanged != null)
                onHappinessChanged(happiness);
        }
    }

    void AutobuyPopulation()
    {
        if (maxPopulation > population)
            AddPopulation(maxPopulation - population);
    }

    public static City GetCityWithRoom()
    {
        foreach(City city in cities)
        {
            if (city.maxPopulation > city.population)
                return city;
        }

        return null;
    }

    public static City GetCity(string name)
    {
        foreach(City city in cities)
        {
            if (city.name == name)
                return city;
        }

        return null;
    }

    #region Population

    public void AddPopulation(int amount)
    {
        for (int i = 0; i < amount; i++)
            AddPopulation();
    }

    public void AddPopulation()
    {
        if (population >= maxPopulation && !GameController.debug)
            return;
        if (!ResourcePool.inventory.HasType(ResourceType.Food, 10) && !GameController.debug)
            return;
        if (!GameController.debug)
            ResourcePool.inventory.RemoveType(ResourceType.Food, 10);
        Worker worker = new Worker((UnityEngine.Random.Range(0f, 1f) > 0.5f ? NameGenerator.GenerateMaleName(NameGenerator.Style.Anglic) : NameGenerator.GenerateFemaleName(NameGenerator.Style.Anglic)) + " " + NameGenerator.GenerateSurname(NameGenerator.Style.Anglic), this);
        AddPopulation(worker);
    }

    public void AddPopulation(Worker worker, bool unemployed = true)
    {
        workers.Add(worker);
        if(unemployed)
            unemployedWorkers.Enqueue(worker);
        if (onPopulationChanged != null)
            onPopulationChanged(this.unemployed, population, maxPopulation);
        if (onUnemployedChanged != null)
            onUnemployedChanged();
    }

    public void RemoveWorker(Worker worker)
    {
        if (!workers.Contains(worker))
            return;

        workers.Remove(worker);
        if (onPopulationChanged != null)
            onPopulationChanged(unemployed, population, maxPopulation);
    }

    public void AddMaxPopulation(int amount)
    {
        maxPopulation += amount;
        if (onPopulationChanged != null)
            onPopulationChanged(unemployed, population, maxPopulation);
    }

    public Worker Employ()
    {
        Worker worker = null;
        if (unemployed > 0)
        {
            worker = unemployedWorkers.Dequeue();
        }
        if (onPopulationChanged != null)
            onPopulationChanged(unemployed, population, maxPopulation);
        if (onUnemployedChanged != null)
            onUnemployedChanged();
        return worker;
    }

    public void Unemploy(Worker worker)
    {
        worker.Unemploy();
        unemployedWorkers.Enqueue(worker);
        if (onPopulationChanged != null)
            onPopulationChanged(unemployed, population, maxPopulation);
        if (onUnemployedChanged != null)
            onUnemployedChanged();
    }

    public Worker GetWorker(string name)
    {
        foreach (Worker worker in workers)
        {
            if (worker.name == name)
                return worker;
        }
        return null;
    }

    #endregion

    #region Saving / Loading

    public static void LoadCities(GameData.CityData[] cityData)
    {
        foreach(GameData.CityData data in cityData)
        {
            City city = new City(data.name);
            city.happiness = data.happiness;
            city.loyalty = data.loyalty;
            CityController.instance.CreateCity(city, data.centers[0]);
            foreach (CityCenter center in data.centers)
                city.AddCenter(center);
        }
    }

    public static void LoadWorkers(GameData.CityData[] cityData)
    {
        foreach(GameData.CityData data in cityData)
        {
            City city = GetCity(data.name);
            foreach (GameData.WorkerData workerData in data.workers)
            {
                Worker worker = new Worker(workerData.name, city);
                if (workerData.employed)
                {
                    if (workerData.workBuilding != -1)
                    {
                        Building workBuilding = Building.buildings[workerData.workBuilding];
                        workBuilding.AddWorker(worker, workerData.workOrder, false);
                    }
                }
                city.AddPopulation(worker, !workerData.employed);
            }
        }
    }

    public static GameData.CityData[] Save()
    {
        List<GameData.CityData> cityData = new List<GameData.CityData>();

        foreach(City city in cities)
        {
            GameData.CityData data = new GameData.CityData();
            data.name = city.name;
            data.happiness = city.happiness;
            data.loyalty = city.loyalty;
            data.centers = city.centers.ToArray();
            List<GameData.WorkerData> workerData = new List<GameData.WorkerData>();
            foreach (Worker worker in city.workers)
                workerData.Add(worker.GetData());
            data.workers = workerData.ToArray();
            cityData.Add(data);
        }

        return cityData.ToArray();
    }

    #endregion

    #region Callbacks

    public void RegisterOnCenterChanged(Action<Vector3> callback)
    {
        onCenterChanged += callback;
    }

    public void RegisterOnHappinessChanged(Action<float> callback)
    {
        onHappinessChanged += callback;
    }

    public void UnregisterOnHappinessChanged(Action<float> callback)
    {
        onHappinessChanged -= callback;
    }

    public void RegisterOnLoyaltyChanged(Action<float> callback)
    {
        onLoyaltyChanged += callback;
    }

    public void UnregisterOnLoyaltyChanged(Action<float> callback)
    {
        onLoyaltyChanged -= callback;
    }

    public void RegisterOnPopulationChanged(Action<int, int, int> callback)
    {
        onPopulationChanged += callback;
    }

    public void UnregisterOnPopulationChanged(Action<int, int, int> callback)
    {
        onPopulationChanged -= callback;
    }

    public void RegisterOnUnemployedChanged(Action callback)
    {
        onUnemployedChanged += callback;
    }

    public void UnregisterOnUnemployedChanged(Action callback)
    {
        onUnemployedChanged -= callback;
    }

    public static void RegisterOnCityCreated(Action<City> callback)
    {
        onCityCreated += callback;
    }

    public static void UnregisterOnCityCreated(Action<City> callback)
    {
        onCityCreated -= callback;
    }

    #endregion
}

[System.Serializable]
public struct CityCenter
{
    public GameData.SerializeVector3 center;
    public float range;

    public CityCenter(Vector3 center, float range)
    {
        this.center = center;
        this.range = range;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Worker {

    //Population
    public static int unemployed
    {
        get
        {
            int unemployed = 0;
            foreach (City city in City.cities)
                unemployed += city.unemployed;
            return unemployed;
        }
    }
    public static int population
    {
        get
        {
            return workers.Count;
        }
    }
    public static int maxPopulation
    {
        get
        {
            int max = 0;
            foreach (City city in City.cities)
                max += city.maxPopulation;
            return max;
        }
    }

    //Local
    public string name;
    public City city;
    public WorkOrder work;
    public Building workBuilding;

    //Global
    public static List<Worker> workers = new List<Worker>();

    static Action<int, int, int> onPopulationChanged;

    public static void Init()
    {
        City.RegisterOnCityCreated(OnCityCreated);
    }

    public static void PreStart()
    {
        workers = new List<Worker>();
    }

    public Worker(string name, City city)
    {
        this.name = name;
        this.city = city;
        workers.Add(this);
    }

    #region Worker

    public bool Work()
    {
        if (work != null)
            return work.Work(workBuilding);
        else
            return false;
    }

    public void SetWork(WorkOrder work)
    {
        this.work = work.Copy();
    }

    public void Employ(Building building)
    {
        workBuilding = building;
    }

    public void Unemploy()
    {
        if (work == null)
            return;
        work.OnUnassign(workBuilding, this);
        work = null;
        workBuilding = null;
    }

    public void Delete(bool onUnassign = true)
    {
        RemoveWorker(this);
        if (work == null)
            return;
        workBuilding.RemoveWorker(this);
        work = null;
        workBuilding = null;
    }

    #endregion

    #region Population

    public static void AddPopulation(int amount = 1)
    {
        if (population + amount > maxPopulation && !GameController.debug)
            return;
        if (!ResourcePool.inventory.HasType(ResourceType.Food, 10 * amount) && !GameController.debug)
            return;
        for (int i = 0; i < amount; i++)
        {
            if (!GameController.debug)
                ResourcePool.inventory.RemoveType(ResourceType.Food, 10);
            City city = null;
            if (GameController.debug)
            {
                if (City.cities.Count > 0)
                    city = City.cities[0];
                else
                    return;
            }else
                city = City.GetCityWithRoom();
            if (city == null)
                return;
            Worker worker = new Worker((UnityEngine.Random.Range(0f, 1f) > 0.5f ? NameGenerator.GenerateMaleName(NameGenerator.Style.Anglic) : NameGenerator.GenerateFemaleName(NameGenerator.Style.Anglic)) + " " + NameGenerator.GenerateSurname(NameGenerator.Style.Anglic), city);
            city.AddPopulation(worker);
        }
        if (onPopulationChanged != null)
            onPopulationChanged(unemployed, population, maxPopulation);
    }

    public static void RemoveWorker(Worker worker)
    {
        if (!workers.Contains(worker))
            return;

        workers.Remove(worker);
        worker.city.RemoveWorker(worker);
        if (onPopulationChanged != null)
            onPopulationChanged(unemployed, population, maxPopulation);
    }

    public static Worker Employ()
    {
        foreach(City city in City.cities)
        {
            if (city.unemployed > 0)
                return city.Employ();
        }

        return null;
    }

    public static Worker GetWorker(string name)
    {
        foreach (Worker worker in workers)
        {
            if (worker.name == name)
                return worker;
        }
        return null;
    }

    static void OnCityCreated(City city)
    {
        city.RegisterOnPopulationChanged(OnPopulationChanged);
    }

    static void OnPopulationChanged(int unemployed, int total, int max)
    {
        if (onPopulationChanged != null)
            onPopulationChanged(Worker.unemployed, Worker.population, Worker.maxPopulation);
    }

    public static void UpdatePopulation()
    {
        if (onPopulationChanged != null)
            onPopulationChanged(unemployed, population, maxPopulation);
    }

    #endregion

    #region Saving / Loading

    public GameData.WorkerData GetData()
    {
        GameData.WorkerData data = new GameData.WorkerData();
        data.name = name;
        if (city.unemployedWorkers.Contains(this))
            data.employed = false;
        else
        {
            data.employed = true;
            if (workBuilding != null)
            {
                data.workBuilding = Building.buildings.IndexOf(workBuilding);
                data.workOrder = workBuilding.GetWorkOrderIndex(work);
            }
            else
            {
                data.workBuilding = -1;
                data.workOrder = -1;
            }
        }
        return data;
    }

    #endregion

    #region Callbacks

    public static void RegisterOnPopulationChanged(Action<int, int, int> callback)
    {
        onPopulationChanged += callback;
    }

    public static void UnregisterOnPopulationChanged(Action<int, int, int> callback)
    {
        onPopulationChanged -= callback;
    }

    #endregion

}

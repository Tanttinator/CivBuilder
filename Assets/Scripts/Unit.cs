using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit {

    //Prototype
    public string name;
    public int strength;
    public int size;
    public ResourceStack[] cost;

    //Global
    public static List<Unit> unitPrototypes = new List<Unit>();

    //Local
    public List<Worker> workers;
    

    #region Units

    public static Unit brute = new Unit("Brute", 2, 10);
    public static Unit archer = new Unit("Archer", 3, 8, new ResourceStack(Resource.bow, 8));

    public static Unit wolf = new Unit("Wolf", 1, 1);
    public static Unit bear = new Unit("Bear", 3, 1);

    #endregion

    public Unit(Unit prototype)
    {
        name = prototype.name;
        strength = prototype.strength;
        size = prototype.size;
        cost = prototype.cost;
        workers = new List<Worker>();
    }

    public Unit(string name, int strength, int size, params ResourceStack[] cost)
    {
        this.name = name;
        this.strength = strength;
        this.size = size;
        this.cost = cost;
        unitPrototypes.Add(this);
    }

    public static void PreStart()
    {

    }

    #region Global functions

    public static Unit CreateUnit(Unit unit)
    {
        Unit newUnit = new Unit(unit);
        for (int i = 0; i < unit.size; i++)
            newUnit.workers.Add(Worker.Employ());
        if (unit.cost != null)
            ResourcePool.inventory.Remove(unit.cost);
        return newUnit;
    }

    public static int GetStrength(List<Unit> units)
    {
        int strength = 0;
        foreach (Unit unit in units)
            strength += unit.strength;

        return strength;
    }

    public static List<Unit> GetEnemies(Age age, int strength)
    {
        int remaining = strength;
        List<Unit> units = new List<Unit>();
        if (age == null)
            age = Age.stoneAge;
        NewUnit:
        if(remaining > 0)
        {
            foreach(Unit unit in age.enemies)
            {
                if (unit.strength > remaining)
                    continue;

                units.Add(unit);
                remaining -= unit.strength;
                goto NewUnit;
            }
        }
        return units;
    }

    #endregion

    #region Local functions

    public int GetID()
    {
        foreach (Unit unit in unitPrototypes)
        {
            if (unit.name == name)
                return unitPrototypes.IndexOf(unit);
        }

        return -1;
    }

    public void Remove()
    {
        foreach (Worker worker in workers)
            worker.city.Unemploy(worker);
    }

    public void Delete()
    {
        foreach (Worker worker in workers)
            worker.Delete();
    }

    #endregion

    #region Saving / Loading

     public Unit(GameData.UnitData data)
    {
        Unit prototype = unitPrototypes[data.ID];
        name = prototype.name;
        strength = prototype.strength;
        size = prototype.size;
        workers = new List<Worker>();
        foreach (string worker in data.workers)
            workers.Add(Worker.GetWorker(worker));
    }

    public GameData.UnitData GetData()
    {
        GameData.UnitData data = new GameData.UnitData();
        data.ID = GetID();
        List<string> workers = new List<string>();
        foreach (Worker worker in this.workers)
            workers.Add(worker.name);
        data.workers = workers.ToArray();
        return data;
    }

    #endregion

}

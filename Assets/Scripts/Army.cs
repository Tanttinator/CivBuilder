using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Army {

    public string name;
    public Worker leader;
    public List<Unit> units;
    public MilitaryBase center;
    public int maxUnits;

    static int armyIndex = 1;

    public static List<Army> armies = new List<Army>();

    static Action<Army> onArmyCreated;
    static Action<Army> onArmyDeleted;
    Action<Unit> onUnitAdded;
    Action<Unit> onUnitRemoved;

    public Army(string name, Worker leader, MilitaryBase center)
    {
        this.name = name;
        this.leader = leader;
        this.center = center;
        maxUnits = center.armySize;
        units = new List<Unit>();
        armyIndex++;
        armies.Add(this);
        if (Army.onArmyCreated != null)
            Army.onArmyCreated(this);
    }

    public Army(Worker leader, MilitaryBase center) : this(armyIndex + "" + (armyIndex == 1? "st" : armyIndex == 2? "nd" : "rd") + " Army", leader, center)
    {

    }

    public static void PreStart()
    {
        armies.Clear();
        armyIndex = 1;
    }

    public int Strength
    {
        get
        {
            return Unit.GetStrength(units);
        }
    }

    public static void DeleteArmy(Army army)
    {
        foreach (Unit unit in army.units)
            unit.Remove();

        if (onArmyDeleted != null)
            onArmyDeleted(army);

        armies.Remove(army);
    }

    public static void DestroyArmy(Army army)
    {
        if (onArmyDeleted != null)
            onArmyDeleted(army);

        foreach (Unit unit in army.units)
            unit.Delete();

        army.leader.Delete();
        armies.Remove(army);
    }

    public void AddUnit(Unit unit)
    {
        if (units.Count < maxUnits)
        {
            units.Add(unit);
            if (onUnitAdded != null)
                onUnitAdded(unit);
        }
    }

    public void RemoveUnit(Unit unit)
    {
        if (units.Contains(unit))
        {
            units.Remove(unit);
            unit.Remove();
            if (onUnitRemoved != null)
                onUnitRemoved(unit);
        }
    }

    public static int GetOdds(Army army, List<Unit> enemy)
    {
        int enemyStrength = Unit.GetStrength(enemy);

        return Mathf.RoundToInt((army.Strength / ((enemyStrength + army.Strength) * 1f)) * 100f);
    }

    public static bool Battle(Army army, List<Unit> enemy)
    {
        float odds = army.Strength / ((Unit.GetStrength(enemy) + army.Strength) * 1f);
        return odds > UnityEngine.Random.Range(0f, 1f);
    }

    public override string ToString()
    {
        return name;
    }

    #region Saving / Loading

    public static void Load(GameData.ArmyData[] armyData)
    {
        foreach(GameData.ArmyData data in armyData)
        {
            Army army = new Army(data.name, Worker.GetWorker(data.commander), Building.buildings[data.center] as MilitaryBase);
            foreach (GameData.UnitData unit in data.units)
                army.AddUnit(new Unit(unit));
        }
    }

    public static GameData.ArmyData[] Save()
    {
        List<GameData.ArmyData> armyData = new List<GameData.ArmyData>();

        foreach(Army army in armies)
        {
            GameData.ArmyData data = new GameData.ArmyData();
            data.name = army.name;
            data.commander = army.leader.name;
            data.units = new GameData.UnitData[army.units.Count];
            for (int i = 0; i < army.units.Count; i++)
                data.units[i] = army.units[i].GetData();
            data.center = Building.buildings.IndexOf(army.center);
            armyData.Add(data);
        }
        return armyData.ToArray();
    }

    #endregion

    #region Callbacks

    public static void RegisterOnArmyCreated(Action<Army> callback)
    {
        onArmyCreated += callback;
    }

    public static void UnregisterOnArmyCreated(Action<Army> callback)
    {
        onArmyCreated -= callback;
    }

    public static void RegisterOnArmyDeleted(Action<Army> callback)
    {
        onArmyDeleted += callback;
    }

    public static void UnregisterOnArmyDeleted(Action<Army> callback)
    {
        onArmyDeleted -= callback;
    }

    public void RegisterOnUnitAdded(Action<Unit> callback)
    {
        onUnitAdded += callback;
    }

    public void UnregisterOnUnitAdded(Action<Unit> callback)
    {
        onUnitAdded -= callback;
    }

    public void RegisterOnUnitRemoved(Action<Unit> callback)
    {
        onUnitRemoved += callback;
    }

    public void UnregisterOnUnitRemoved(Action<Unit> callback)
    {
        onUnitRemoved -= callback;
    }

    #endregion
}

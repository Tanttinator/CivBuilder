using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilitaryBase : Building {

    public int armySize;
    public List<Unit> trainableUnits = new List<Unit>();

	public MilitaryBase(string name, string description, ResourceStack[] cost, float range, int armySize, BuildingType type = BuildingType.Military) : base(name, description, BuildingClass.MilitaryCentral, type, cost)
    {
        SetRange(range);
        this.armySize = armySize;
    }

    public MilitaryBase(MilitaryBase prototype) : base(prototype)
    {
        armySize = prototype.armySize;
        if (unlockedUnits != null)
            trainableUnits.AddRange(unlockedUnits);
    }

    public override void OnBuildingPlacedInRange(Building building)
    {
        if(building.unlockedUnits != null)
        {
            foreach(Unit unit in building.unlockedUnits)
            {
                if (!trainableUnits.Contains(unit))
                    trainableUnits.Add(unit);
            }
        }
        base.OnBuildingPlacedInRange(building);
    }

    public override Building Copy()
    {
        return new MilitaryBase(this);
    }
}

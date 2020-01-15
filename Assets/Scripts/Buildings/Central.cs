using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Central : Building {

    public Central(string name, string description, ResourceStack[] cost, float range) : base(name, description, BuildingClass.Central, BuildingType.Governance, cost)
    {
        SetRange(range);
    }

    public Central(Central prototype) : base(prototype)
    {

    }

    public override void OnBuildingPlaced()
    {
        Building other = GetClosestOfTypes(this, BuildingClass.Central);
        if (other != null)
            BuildingHandler.instance.BuildRoad(new Vector2(location.x, location.z), new Vector2(other.location.x, other.location.z));

        base.OnBuildingPlaced();
    }

    public override void OnNewBuildingPlaced()
    {
        Building closestCentral = GetClosestOfTypes(this, BuildingClass.Central);
        if (closestCentral == null || Vector3.Distance(closestCentral.location, location) > range + closestCentral.range)
        {
            city = new City(NameGenerator.GenerateCityName(NameGenerator.Style.Anglic));
            CityController.instance.CreateCity(city, new CityCenter(location, range));
        }
        else
        {
            city = closestCentral.city;
        }
        city.AddCenter(new CityCenter(location, range));
        city.buildings.Add(this);
        base.OnNewBuildingPlaced();
    }

    public override Building Copy()
    {
        return new Central(this);
    }

}

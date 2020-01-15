using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Retrieve : WorkOrder {
    
    public ResourceStack[] retrievedStacks;
    Queue<Inventory> targetQueue;

	public Retrieve(string description, int time) : base(description, time)
    {
        targetQueue = new Queue<Inventory>();
    }

    public override void OnCompletion(Building building)
    {
        ResourcePool.Add(building.location, retrievedStacks);
        base.OnCompletion(building);
    }

    public override void OnStart(Building building)
    {
        Inventory target;
        if (targetQueue.Count > 0)
        {

        }
        else
        {
            foreach(Building targetBuilding in building.buildingsInRange)
            {
                if(targetBuilding.output.inventory.Count > 0)
                {
                    targetQueue.Enqueue(targetBuilding.output);
                }
            }
        }

        target = targetQueue.Dequeue();
        
        retrievedStacks = target.RetrieveAll();
        base.OnStart(building);
    }

    public override bool CanStart(Building building)
    {
        return GetInventory(building) != null || targetQueue.Count > 0;
    }

    public override WorkOrder Copy()
    {
        return new Retrieve(description, time);
    }

    public Inventory GetInventory(Building building)
    {

        foreach(Building b in building.buildingsInRange)
        {
            if (b.output.inventory.Count > 0)
            {
                return b.output;
            }
        }

        return null;
    }
}

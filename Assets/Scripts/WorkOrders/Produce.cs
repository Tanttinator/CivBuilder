using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Produce : WorkOrder {

    public ResourceStack[] cost;
    public ResourceStack[] produce;
    public bool outputToResourcePool;

	public Produce(string description, int time, ResourceStack[] cost, ResourceStack[] produce, int toolTier = -1, bool outputToResourcePool = false) : base(description, time, toolTier)
    {
        this.cost = cost;
        this.produce = produce;
        this.outputToResourcePool = outputToResourcePool;
    }

    public override void OnCompletion(Building building)
    {
        if (outputToResourcePool)
            ResourcePool.Add(building.location, produce);
        else
            building.output.Add(produce);

        base.OnCompletion(building);
    }

    public override void OnStart(Building building)
    {
        if (building.input.HasResources(cost))
        {
            foreach (ResourceStack stack in cost)
            {
                building.input.Retrieve(stack);
            }

            base.OnStart(building);
        }
        else
        {
            foreach(ResourceStack stack in cost)
            {
                if (!building.input.HasResources(stack) && !building.request.Contains(stack.resource))
                    building.request.Add(stack.resource);
            }
        }
    }

    public override bool CanStart(Building building)
    {
        if(produce == null || produce.Length == 0)
        {
            Debug.LogError(description + " doesn't have a produce.");
            return false;
        }

        return base.CanStart(building);
    }

    public override float GetIncome(Building building, Resource resource)
    {
        float income = 0f;
        foreach(ResourceStack stack in cost)
        {
            if (stack.resource == resource)
                income -= stack.amount * (60f / time);
        }
        foreach (ResourceStack stack in produce)
        {
            if (stack.resource == resource)
                income += stack.amount * (60f / time);
        }
        return income * (building.city.happiness / 2 + 0.75f);
    }

    public override string GetDescription()
    {
        string costString = "";
        foreach (ResourceStack resource in cost)
        {
            costString += resource;
            if (System.Array.IndexOf(cost, resource) < cost.Length - 1)
                costString += ", ";
        }
        string produceString = "";
        foreach (ResourceStack resource in produce)
        {
            produceString += resource;
            if (System.Array.IndexOf(produce, resource) < produce.Length - 1)
                produceString += ", ";
        }
        return costString += " -> " + produceString + ", time: " + time;
    }

    public override WorkOrder Copy()
    {
        return new Produce(description, time, cost, produce, toolTier, outputToResourcePool);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkOrder {

    public string description;
    public int time;
    public int toolTier;

    [HideInInspector] public float progress;
    [HideInInspector] public bool isWorking;

    public WorkOrder(string description, int time, int toolTier = -1)
    {
        this.description = description;
        this.time = time;
        progress = 0;
        isWorking = false;
        this.toolTier = toolTier;
    }

    public virtual bool Work(Building building)
    {
        if (isWorking)
        {
            progress += building.city.happiness / 2 + 0.75f;
            if(progress >= time)
            {
                OnCompletion(building);
            }
        }
        else
        {
            if (CanStart(building))
            {
                OnStart(building);
            }
        }
        return isWorking;
    }

    public virtual void OnCompletion(Building building)
    {
        if (toolTier >= 0)
            building.durability--;
        progress = 0;
        isWorking = false;
    }

    public virtual void OnStart(Building building)
    {
        isWorking = true;
    }

    public virtual bool CanStart(Building building)
    {
        if(toolTier >= 0)
        {
            if(building.durability == 0 || building.tool.tier < toolTier)
            {
                if (building.toolTierRequest < toolTier)
                {
                    building.toolTierRequest = toolTier;
                }
                return false;
            }
        }
        return true;
    }

    public virtual void OnAssign(Building building, Worker worker)
    {

    }

    public virtual void OnUnassign(Building building, Worker worker)
    {

    }

    public virtual float GetIncome(Building building, Resource resource)
    {
        return 0f;
    }

    public virtual string GetDescription()
    {
        return description;
    }

    public virtual WorkOrder Copy()
    {
        return new WorkOrder(description, time);
    }

}

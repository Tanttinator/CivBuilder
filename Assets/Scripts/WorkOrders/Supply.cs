using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supply : WorkOrder {

    ResourceStack[] suppliedStack;
    Tool suppliedTool;
    Building target;
    Queue<SupplyRequest> targetQueue;

    public Supply(string description, int time) : base(description, time)
    {
        targetQueue = new Queue<SupplyRequest>();
    }

    public override void OnStart(Building building)
    {
        if(targetQueue.Count == 0)
        {
            foreach(Building other in building.buildingsInRange)
            {
                if(other.request.Count > 0 || other.toolTierRequest >= 0)
                {
                    targetQueue.Enqueue(new SupplyRequest(other, other.request.ToArray(), other.toolTierRequest));
                }
            }
        }

        SupplyRequest request = targetQueue.Dequeue();
        target = request.target;
        suppliedTool = null;
        List<ResourceStack> stack = new List<ResourceStack>();
        foreach(Resource resource in request.request)
        {
            stack.Add(ResourcePool.inventory.Retrieve(new ResourceStack(resource, 5)));
        }
        if (request.toolRequest >= 0)
            suppliedTool = ResourcePool.inventory.RetrieveTool(request.toolRequest);
        suppliedStack = stack.ToArray();

        base.OnStart(building);
    }

    public override void OnCompletion(Building building)
    {
        target.Supply(suppliedStack);
        if (suppliedTool != null)
            target.AddTool(suppliedTool);
        suppliedTool = null;
        suppliedStack = null;
        target = null;
        base.OnCompletion(building);
    }

    public override bool CanStart(Building building)
    {
        return GetInventory(building) != null || targetQueue.Count > 0;
    }

    public SupplyRequest GetInventory(Building building)
    {
        foreach(Building other in building.buildingsInRange)
        {
            if(other.request.Count > 0 || other.toolTierRequest >= 0)
            {
                return new SupplyRequest(other, other.request.ToArray(), other.toolTierRequest);
            }
        }

        return null;
    }

    public override WorkOrder Copy()
    {
        return new Supply(description, time);
    }

    public class SupplyRequest
    {
        public Building target;
        public Resource[] request;
        public int toolRequest;

        public SupplyRequest(Building target, Resource[] request, int toolRequest = -1)
        {
            this.target = target;
            this.request = request;
            this.toolRequest = toolRequest;
        }
    }
}

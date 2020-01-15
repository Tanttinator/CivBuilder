using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory {

    public Dictionary<Resource, int> inventory;

    public Action<Resource> onResourceAdded;
    public Action<Resource> onResourceRemoved;
    public Action<Resource, int> onResourceSet;

    bool keepResources;

    public Inventory(bool keepResources = false)
    {
        inventory = new Dictionary<Resource, int>();
        this.keepResources = keepResources;
    }

    public void Add(ResourceStack stack)
    {
        if (inventory.ContainsKey(stack.resource))
        {
            inventory[stack.resource] += stack.amount;
        }
        else
        {
            inventory.Add(stack.resource, stack.amount);
            if(onResourceAdded != null)
                onResourceAdded(stack.resource);
        }
        if(onResourceSet != null)
            onResourceSet(stack.resource, inventory[stack.resource]);
    }

    public void Add(ResourceStack[] stacks)
    {
        foreach(ResourceStack stack in stacks)
        {
            Add(stack);
        }
    }

    public void Add(List<ResourceStack> stacks)
    {
        foreach (ResourceStack stack in stacks)
        {
            Add(stack);
        }
    }

    public void Remove(ResourceStack stack)
    {
        if (inventory.ContainsKey(stack.resource))
        {
            if(AmountOf(stack.resource) >= stack.amount)
            {
                inventory[stack.resource] -= stack.amount;
                if(onResourceSet != null)
                    onResourceSet(stack.resource, inventory[stack.resource]);
                if(inventory[stack.resource] == 0 && !keepResources)
                {
                    inventory.Remove(stack.resource);
                    if(onResourceRemoved != null)
                        onResourceRemoved(stack.resource);
                }
            }
        }
    }

    public void Remove(ResourceStack[] stacks)
    {
        foreach(ResourceStack stack in stacks)
        {
            Remove(stack);
        }
    }

    public void Remove(List<ResourceStack> stacks)
    {
        foreach (ResourceStack stack in stacks)
        {
            Remove(stack);
        }
    }

    public void RemoveType(ResourceType type, int amount)
    {
        int gatheredResources = 0;
        List<ResourceStack> stacks = new List<ResourceStack>();
        foreach (KeyValuePair<Resource, int> resources in inventory)
        {
            if (resources.Key.type == type)
            {
                if(AmountOf(resources.Key) >= amount - gatheredResources)
                {
                    stacks.Add(new ResourceStack(resources.Key, amount - gatheredResources));
                    break;
                }
                else
                {
                    stacks.Add(new ResourceStack(resources.Key, resources.Value));
                    gatheredResources += resources.Value;
                }
            }
        }
        Remove(stacks);
    }

    public ResourceStack Retrieve(ResourceStack stack)
    {
        ResourceStack returns;
        if(AmountOf(stack.resource) == 0)
        {
            returns =  new ResourceStack(stack.resource, 0);
        }
        else if(AmountOf(stack.resource) < stack.amount)
        {
            returns = new ResourceStack(stack.resource, AmountOf(stack.resource));
        }
        else
        {
            returns = new ResourceStack(stack.resource, stack.amount);
        }
        Remove(returns);
        return returns;
    }

    public ResourceStack RetrieveAllOfResource(Resource resource)
    {
        return Retrieve(new ResourceStack(resource, AmountOf(resource)));
    }

    public ResourceStack[] RetrieveAll()
    {
        List<ResourceStack> stacks = new List<ResourceStack>();
        foreach(Resource resource in inventory.Keys)
        {
            stacks.Add(new ResourceStack(resource, AmountOf(resource)));
        }

        Remove(stacks);
        return stacks.ToArray();
    }

    public Tool RetrieveTool(int tier)
    {
        if (tier == -1)
            return null;
        Tool tool = null;
        Tool other = null;

        foreach(Resource resource in inventory.Keys)
        {
            if(resource is Tool)
            {
                other = resource as Tool;
                if ((tool == null && other.tier >= tier) || (other.tier >= tier && other.tier < tool.tier))
                    tool = other;
            }
        }

        if(tool != null)
            Remove(new ResourceStack(tool, 1));
        return tool;
    }

    public Resource GetMostOfType(ResourceType type)
    {
        Resource resource = null;
        int amount = 0;
        foreach(Resource r in inventory.Keys)
        {
            if (r.type == type && (inventory[r] > amount || resource == null))
            {
                resource = r;
                amount = inventory[r];
            }
        }

        return resource;
    }

    public int AmountOf(Resource resource)
    {
        if (inventory.ContainsKey(resource))
        {
            return inventory[resource];
        }
        else
        {
            return 0;
        }
    }

    public bool HasResources(params ResourceStack[] resources)
    {
        foreach(ResourceStack resource in resources)
        {
            if(AmountOf(resource.resource) < resource.amount)
            {
                return false;
            }
        }

        return true;
    }

    public bool HasType(ResourceType type, int amount)
    {
        foreach(KeyValuePair<Resource, int> resources in inventory)
        {
            if(resources.Key.type == type)
            {
                amount -= resources.Value;
            }
        }

        return amount <= 0;
    }

    public override string ToString()
    {
        string s = "";
        foreach (KeyValuePair<Resource, int> resource in inventory)
            s += resource.Key + ":" + resource.Value + "\n";
        return s;
    }
}

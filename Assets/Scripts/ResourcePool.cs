using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ResourcePool {

    public static Inventory inventory = new Inventory(true);

    static Action<ResourceStack[], Vector3> onResourceAddedByBuilding;

    public static void Add(Vector3 location, params ResourceStack[] stacks)
    {
        if (onResourceAddedByBuilding != null)
            onResourceAddedByBuilding(stacks, location);
        inventory.Add(stacks);
    }

    public static GameData.ResourceData[] Save()
    {
        List<GameData.ResourceData> stacks = new List<GameData.ResourceData>();
        foreach (Resource resource in inventory.inventory.Keys)
        {
            stacks.Add(new GameData.ResourceData(resource.name, inventory.inventory[resource]));
        }
        return stacks.ToArray();
    }

    public static void Load(GameData.ResourceData[] resourceData)
    {
        foreach (GameData.ResourceData data in resourceData)
            inventory.Add(new ResourceStack(Resource.GetResource(data.name), data.amount));
    }

    public static void RegisterOnResourceAdded(Action<Resource> action)
    {
        inventory.onResourceAdded += action;
    }

    public static void UnregisterOnResourceAdded(Action<Resource> action)
    {
        inventory.onResourceAdded -= action;
    }

    public static void RegisterOnResourceRemoved(Action<Resource> action)
    {
        inventory.onResourceRemoved += action;
    }

    public static void UnregisterOnResourceRemoved(Action<Resource> action)
    {
        inventory.onResourceRemoved -= action;
    }

    public static void RegisterOnResourceSet(Action<Resource, int> action)
    {
        inventory.onResourceSet += action;
    }

    public static void UnregisterOnResourceSet(Action<Resource, int> action)
    {
        inventory.onResourceSet -= action;
    }

    public static void RegisterOnResourceAddedByBuilding(Action<ResourceStack[], Vector3> callback)
    {
        onResourceAddedByBuilding += callback;
    }

    public static void UnregisterOnResourceAddedByBuilding(Action<ResourceStack[], Vector3> callback)
    {
        onResourceAddedByBuilding -= callback;
    }

}

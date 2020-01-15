using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Food,
    Production,
    Research,
    Misc
}

public class Resource
{
    public static List<Resource> resources = new List<Resource>();

    public static Resource berries = new Resource("Berries", ResourceType.Food);
    public static Resource bow = new Resource("Bow", ResourceType.Production);
    public static Resource bowstring = new Resource("Bowstring", ResourceType.Production);
    public static Resource bread = new Resource("Bread", ResourceType.Food);
    public static Resource bronze = new Resource("Bronze", ResourceType.Production);
    public static Resource clay = new Resource("Clay", ResourceType.Production);
    public static Resource clothing = new Resource("Clothing", ResourceType.Production);
    public static Resource copper = new Resource("Copper", ResourceType.Production);
    public static Resource copperOre = new Resource("Copper Ore", ResourceType.Production);
    public static Tool crudeTool = new Tool("Crude Tool", 10, 0);
    public static Resource fish = new Resource("Fish", ResourceType.Food);
    public static Resource gold = new Resource("Gold", ResourceType.Production);
    public static Resource goldOre = new Resource("Gold Ore", ResourceType.Production);
    public static Resource hemp = new Resource("Hemp", ResourceType.Production);
    public static Resource hide = new Resource("Hide", ResourceType.Production);
    public static Resource influence = new Resource("Influence", ResourceType.Misc);
    public static Resource iron = new Resource("Iron", ResourceType.Production);
    public static Resource ironOre = new Resource("Iron Ore", ResourceType.Production);
    public static Resource jewelry = new Resource("Jewelry", ResourceType.Production);
    public static Resource leather = new Resource("Leather", ResourceType.Production);
    public static Resource textile = new Resource("Textile", ResourceType.Production);
    public static Resource meat = new Resource("Meat", ResourceType.Food);
    public static Resource pottery = new Resource("Pottery", ResourceType.Production);
    public static Resource research = new Resource("Research", ResourceType.Research);
    public static Resource salt = new Resource("Salt", ResourceType.Production);
    public static Resource stone = new Resource("Stone", ResourceType.Production);
    public static Tool stoneTool = new Tool("Stone Tool", 25, 1);
    public static Resource tin = new Resource("Tin", ResourceType.Production);
    public static Resource tinOre = new Resource("Tin Ore", ResourceType.Production);
    public static Resource twine = new Resource("Twine", ResourceType.Production);
    public static Resource unfiredPottery = new Resource("Unfired Pottery", ResourceType.Production);
    public static Resource water = new Resource("Water", ResourceType.Production);
    public static Resource wheat = new Resource("Wheat", ResourceType.Production);
    public static Resource wheatFlour = new Resource("Wheat Flour", ResourceType.Production);
    public static Resource wood = new Resource("Wood", ResourceType.Production);
    public static Resource yarn = new Resource("Yarn", ResourceType.Production);


    public string name;
    public ResourceType type;

    public Resource(string name, ResourceType type)
    {
        this.name = name;
        this.type = type;

        resources.Add(this);
    }

    public override string ToString()
    {
        return name;
    }

    public static Resource GetResource(string name)
    {
        foreach (Resource resource in resources)
        {
            if (resource.name == name)
                return resource;
        }

        return null;
    }
}

public class Tool : Resource
{
    public int durability;
    public int tier;

    public static Dictionary<int, Tool> tools = new Dictionary<int, Tool>();

    public Tool(string name, int durability, int tier) : base(name, ResourceType.Production)
    {
        this.durability = durability;
        this.tier = tier;
        tools.Add(tier, this);
    }
}

public struct ResourceStack
{
    public Resource resource;
    public int amount;

    public ResourceStack(Resource resource, int amount)
    {
        this.resource = resource;
        this.amount = amount;
    }

    public override string ToString()
    {
        return amount + " " + resource;
    }

    public static ResourceStack[] Add(params ResourceStack[][] stacks)
    {
        Dictionary<Resource, int> resources = new Dictionary<Resource, int>();
        foreach(ResourceStack[] stack in stacks)
        {
            foreach(ResourceStack resource in stack)
            {
                if (resources.ContainsKey(resource.resource))
                {
                    resources[resource.resource] += resource.amount;
                }
                else
                {
                    resources.Add(resource.resource, resource.amount);
                }
            }
        }
        List<ResourceStack> final = new List<ResourceStack>();
        foreach (Resource resource in resources.Keys)
            final.Add(new ResourceStack(resource, resources[resource]));
        return final.ToArray();
    }

    public static ResourceStack[] operator+(ResourceStack A, ResourceStack B)
    {
        return new ResourceStack[] { A, B };
    }

    public static ResourceStack[] operator +(ResourceStack[] A, ResourceStack B)
    {
        ResourceStack[] stacks = new ResourceStack[A.Length + 1];
        A.CopyTo(stacks, 0);
        stacks[stacks.Length - 1] = B;
        return stacks;
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Research : WorkOrder {

    public int amount;

    public Research(string description, int time, int amount) : base(description, time)
    {
        this.amount = amount;
    }

    public override void OnCompletion(Building building)
    {
        if (Technology.currentTech != null)
            Technology.currentTech.Progress(amount);
        base.OnCompletion(building);
    }

    public override bool CanStart(Building building)
    {
        return Technology.currentTech != null;
    }

    public override WorkOrder Copy()
    {
        return new Research(description, time, amount);
    }

    public override string GetDescription()
    {
        return "Generates " + amount + " research per " + time + " minutes.";
    }
}

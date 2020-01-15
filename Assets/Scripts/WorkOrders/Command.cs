using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command : Gather{

    int amount;
    Army army;

	public Command(string description, int time, int amount) : base(description, time, new ResourceStack[] { new ResourceStack(Resource.influence, amount) }, -1, true)
    {
        this.amount = amount;
    }

    public override void OnAssign(Building building, Worker worker)
    {
        army = new Army(worker, building as MilitaryBase);
    }

    public override void OnUnassign(Building building, Worker worker)
    {
        Army.DeleteArmy(army);
        army = null;
    }

    public override WorkOrder Copy()
    {
        return new Command(description, time, amount);
    }

}

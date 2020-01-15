using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gather : Produce {

	public Gather(string description, int time, ResourceStack[] produce, int toolTier = -1, bool outputToResourcePool = false) : base(description, time, new ResourceStack[] { }, produce, toolTier, outputToResourcePool)
    {

    }

    public override WorkOrder Copy()
    {
        return new Gather(description, time, produce, toolTier, outputToResourcePool);
    }
}

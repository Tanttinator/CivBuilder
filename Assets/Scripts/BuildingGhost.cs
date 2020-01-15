using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGhost : MonoBehaviour {

    BuildingHandler bh;
    List<Collider> colliders;

	void Start () {
        bh = FindObjectOfType<BuildingHandler>();
        colliders = new List<Collider>();
	}

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag != "Building")
            return;
        if (!colliders.Contains(collider))
        {
            colliders.Add(collider);
            bh.isOverlapping = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag != "Building")
            return;
        colliders.Remove(collider);
        if (colliders.Count == 0)
            bh.isOverlapping = false;
    }

}

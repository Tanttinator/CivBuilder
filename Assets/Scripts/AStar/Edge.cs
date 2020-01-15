using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge {

    public Node destination;
    public float cost;

	public Edge(Node destination, float cost)
    {
        this.destination = destination;
        this.cost = cost;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

    public Vector2 position;
    public List<Edge> edges;

	public Node(Vector2 position)
    {
        this.position = position;
        edges = new List<Edge>();
    }
}

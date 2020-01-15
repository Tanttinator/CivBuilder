using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Graph {

    public Dictionary<Vector2, Node> nodes;

	public Graph(Dictionary<Vector2, Node> nodes)
    {
        this.nodes = nodes;
    }

    public Node[] GetNodes()
    {
        return nodes.Values.ToArray();
    }
}

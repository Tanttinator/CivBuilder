using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Threading;

public static class AStar {

    public static void GeneratePathThreaded(Graph graph, Node start, Node end, Action<Queue<Node>> callback, Queue<PathThreadInfo> callbackQueue)
    {
        Debug.Log("Starting Path Generation");
        ThreadStart thread = delegate
        {
            PathGenerationThread(graph, start, end, callback, callbackQueue);
        };
        new Thread(thread).Start();
    }

    static void PathGenerationThread(Graph graph, Node start, Node end, Action<Queue<Node>> callback, Queue<PathThreadInfo> callbackQueue)
    {
       if(callback != null)
        {
            Queue<Node> path = GeneratePath(graph, start, end);
            callbackQueue.Enqueue(new PathThreadInfo(callback, path));
            Debug.Log("Path Generated");
        }
    }

	public static Queue<Node> GeneratePath(Graph graph, Node start, Node end)
    {
        List<Node> closedSet = new List<Node>();
        SortedList<float, Node> openSet = new SortedList<float, Node>(new DuplicateKeyComparer<float>());
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, float> gScore = new Dictionary<Node, float>();
        Dictionary<Node, float> fScore = new Dictionary<Node, float>();
        Queue<Node> path = new Queue<Node>();

        foreach (Node node in graph.GetNodes())
        {
            gScore.Add(node, Mathf.Infinity);
            fScore.Add(node, Mathf.Infinity);
        }

        gScore[start] = 0;
        fScore[start] = CostEstimate(start, end);

        openSet.Add(fScore[start], start);
        
        
        while(openSet.Count > 0)
        {
            Node current = openSet.First().Value;

            if(current == end)
            {
                Node step = end;
                path.Enqueue(step);
                while (cameFrom.ContainsKey(step))
                {
                    step = cameFrom[step];
                    path.Enqueue(step);
                }
                path.Reverse();
                break;
            }

            openSet.RemoveAt(openSet.IndexOfValue(current));
            closedSet.Add(current);

            foreach(Edge edge in current.edges)
            {
                if (closedSet.Contains(edge.destination))
                    continue;
                Node node = edge.destination;

                float tentativeGScroe = gScore[current] + edge.cost;

                if (openSet.ContainsValue(node) && tentativeGScroe >= gScore[node])
                    continue;

                cameFrom[node] = current;
                gScore[node] = tentativeGScroe;
                fScore[node] = gScore[node] + CostEstimate(node, end);

                if (!openSet.ContainsValue(node))
                    openSet.Add(fScore[node], node);
            }
        }
        return path;
    }

    static float CostEstimate(Node node, Node end)
    {
        return Vector2.Distance(node.position, end.position);
    }

    class DuplicateKeyComparer<TKey>
                :
             IComparer<TKey> where TKey : IComparable
    {

        public int Compare(TKey x, TKey y)
        {
            int result = x.CompareTo(y);

            if (result == 0)
                return 1;
            else
                return result;
        }
    }
}


public struct PathThreadInfo
{
    public Action<Queue<Node>> callback;
    public Queue<Node> path;

    public PathThreadInfo(Action<Queue<Node>> callback, Queue<Node> path)
    {
        this.callback = callback;
        this.path = path;
    }
}

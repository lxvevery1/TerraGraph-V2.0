using System.Collections.Generic;
using UnityEngine;

public static class IncidenceGenerator
{
    // Generate incidence lists from adjacency matrix
    public static Incidences GenerateIncidenceLists(int[,] adjacencyMatrix)
    {
        Incidences incidences = new()
        {
            right = RightIncidence(adjacencyMatrix),
            left = LeftIncidence(adjacencyMatrix)
        };

        return incidences;
    }


    private static Dictionary<int, List<int>> RightIncidence(int[,] adjacencyMatrix)
    {
        Debug.Log("==<b>Right</b> Incidence==");

        int numNodes = adjacencyMatrix.GetLength(0);

        Dictionary<int, List<int>> NEmap = new(numNodes);

        for (int i = 0; i < numNodes; i++)
        {
            Debug.Log($"Node {i + 1}:");
            List<int> edges = new();
            for (int j = 0; j < numNodes; j++)
            {
                if (adjacencyMatrix[i, j] >= 1)
                {
                    edges.Add(j + 1);
                    Debug.Log($"Edge {j + 1}");
                }
            }
            int node = i + 1;
            NEmap.Add(node, edges);
        }

        return NEmap;
    }

    private static Dictionary<int, List<int>> LeftIncidence(int[,] adjacencyMatrix)
    {
        Debug.Log("==<b>Left</b> Incidence==");

        int numNodes = adjacencyMatrix.GetLength(0);

        Dictionary<int, List<int>> NEmap = new(numNodes);

        for (int i = 0; i < numNodes; i++)
        {
            Debug.Log($"Node {i + 1}:");
            List<int> edges = new();
            for (int j = 0; j < numNodes; j++)
            {
                if (adjacencyMatrix[j, i] >= 1)
                {
                    edges.Add(j + 1);
                    Debug.Log($"Edge {j + 1}");
                }
            }
            int node = i + 1;
            NEmap.Add(node, edges);
        }

        return NEmap;
    }

    public struct Incidences
    {
        public Dictionary<int, List<int>> right;
        public Dictionary<int, List<int>> left;
    }
}

using UnityEngine;
using System.Collections.Generic;
using TMPro;
using static AlgoManager;
using System.Linq;

/// Adjacency matrix #1
/*
0 1 0 0 0 0 0 
0 0 0 0 0 1 0 
1 0 0 0 1 0 1 
0 0 0 0 0 1 0 
0 1 0 0 0 0 0 
0 0 0 0 0 0 0 
0 0 0 1 0 0 0 
*/


public class UIManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_InputField outputText;

    public void ProcessInput()
    {
        string[] lines = inputField.text.Split('\n');

        // Parse input matrix
        int[,] adjacencyMatrix = new int[lines.Length, lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            string[] values = lines[i].Trim().Split(' ');
            for (int j = 0; j < values.Length; j++)
            {
                adjacencyMatrix[i, j] = int.Parse(values[j]);
            }
        }

        // Convert adjacency matrix to list of edges
        List<Edge> edges = new();
        for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < adjacencyMatrix.GetLength(1); j++)
            {
                if (adjacencyMatrix[i, j] != 0)
                {
                    edges.Add(new Edge(i, j));
                }
            }
        }
        Debug.Log(string.Join(", ", edges.Select(edge => $"({edge.v1+1}, {edge.v2+1})")));
        // Create hierarchical levels
        var levels = new List<HierarchicalLevel>();
        CreateHLevel(levels, lines.Length, edges);

        // Convert to ordered adjacency matrix
        int[,] orderedAdjacencyMatrix = new int[lines.Length, lines.Length];
        List<HierarchicalLevel> normalizedLevels = new();
        List<HierarchicalLevel> normalizedSortedLevels = new();
        CreateHLevel(normalizedLevels, lines.Length, edges);
        CreateHLevel(normalizedSortedLevels, lines.Length, edges);
        for (int i = 0; i < levels.Count; i++)
        {
            normalizedLevels[i].level = levels[i].level.Select(x => x+=1).ToList();
            Debug.Log($"normalizedLevels[{i}].level = " + string.Join(',', normalizedLevels[i].level));
            // now we have {normalizedLevels} -> go to the sorted adjacency matrix







            for (int j = 0; j < lines.Length; j++)
            {
                var k = i + j;
                normalizedSortedLevels[i].level[j] = k;

                //orderedAdjacencyMatrix[vertex, j] = adjacencyMatrix[vertex, j];
            }
            

            Debug.Log($"normalizedSortedLevels[{i}].level = " + string.Join(',', normalizedSortedLevels[i].level));
        }

        // Output the ordered adjacency matrix
        string output = "";
        for (int i = 0; i < orderedAdjacencyMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < orderedAdjacencyMatrix.GetLength(1); j++)
            {
                output += orderedAdjacencyMatrix[i, j] + " ";
            }
            output += "\n";
        }

        outputText.text = output;
    }
}

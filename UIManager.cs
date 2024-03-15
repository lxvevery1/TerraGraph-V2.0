using UnityEngine;
using System.Collections.Generic;
using TMPro;
using static AlgoManager;
using System.Linq;
using UnityEditor;
using System.Text;

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
    public TMP_InputField leftIncidences;

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

        LogEdges(edges);

        // Create hierarchical levels
        var levels = new List<HierarchicalLevel>();
        CreateHLevel(levels, lines.Length, edges);


        // Convert to ordered adjacency matrix
        int[,] orderedAdjacencyMatrix = new int[lines.Length, lines.Length];


        // normalizedLevels
        List<HierarchicalLevel> normalizedLevels = new();
        CreateHLevel(normalizedLevels, lines.Length, edges);
        for (int i = 0; i < levels.Count; i++)
        {
            normalizedLevels[i].Level = levels[i].Level.Select(x => x+=1).ToList();
            Debug.Log($"normalizedLevels[{i}].level = " + string.Join(',', normalizedLevels[i].Level));
        }

        // normalizedSortedLevels
        List<HierarchicalLevel> normalizedSortedLevels = new();
        CreateHLevel(normalizedSortedLevels, lines.Length, edges);
        int ordereredVert = 1;
        for (int i = 0; i < levels.Count; i++)
        {
            for (int j = 0; j < normalizedSortedLevels[i].Level.Count; j++)
            {
                normalizedSortedLevels[i].Level[j] = ordereredVert++;
            }

            Debug.Log($"normalizedSortedLevels[{i}].level = " + string.Join(',', normalizedSortedLevels[i].Level));
        }

        List<List<VertsTransformation>> vertsTransformations = new List<List<VertsTransformation>>();

        int n = normalizedLevels.Count;
        while (n != 0)
        {
            vertsTransformations.Add(new List<VertsTransformation>());
            --n;
        }

        for (int i = 0; i < levels.Count; i++)
        {
            for (int j = 0; j < normalizedSortedLevels[i].Level.Count; j++)
            {
                vertsTransformations[i].Add(new VertsTransformation() { Old = normalizedLevels[i].Level[j], New = normalizedSortedLevels[i].Level[j] });
            }
        }


        List<VertsTransformation> vtransformations = new();
        for (int i = 0; i < levels.Count; i++)
        {
            for (int j = 0; j < normalizedSortedLevels[i].Level.Count; j++)
            {
                vtransformations.Add(vertsTransformations[i][j]);
            }
        }
        LogVertsTransformation(vtransformations);

        var sortedEdges = GenerateSortedEdges(edges, vtransformations);
        LogEdges(sortedEdges);


        // lets calc the numb of links for each vert
        List<int> nLinks = new();

        foreach (HierarchicalLevel level in normalizedLevels)
        {
            int linksCount = level.Level.Count;

            nLinks.Add(linksCount);
        }

        Debug.Log("nLinks = " + string.Join(", ", nLinks));



        // now we have {normalizedSortedLevels} -> go to the sorted adjacency matrix
        // ok, let's go)

        Dictionary<int, List<int>> vertLinks = LeftIncidence(adjacencyMatrix);
        foreach (var entry in vertLinks)
        {
            Debug.Log(entry.Key + " -> " + string.Join(", ", entry.Value));
        }

        // orderedAdjacencyMatrix = ConvertToOrderedAdjacencyMatrix(vertsTransformations, vertLinks);
        orderedAdjacencyMatrix = GenerateAdjacencyMatrix(sortedEdges);



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

        Dictionary<int, List<int>> sortedVertLinks = LeftIncidence(orderedAdjacencyMatrix);

        SetIncidenceToText(sortedVertLinks, ref leftIncidences);

        outputText.text += "\n";
        for (int i = 0; i < vtransformations.Count; i++)
        {
            outputText.text += ($"new {vtransformations[i].New}(old {vtransformations[i].Old})\n");
        }
    }

    private static Dictionary<int, List<int>> LeftIncidence(int[,] adjacencyMatrix)
    {
        Debug.Log("==<b>Left</b> Incidence==");

        int numNodes = adjacencyMatrix.GetLength(0);

        Dictionary<int, List<int>> NEmap = new(numNodes);

        for (int i = 0; i < numNodes; i++)
        {
            List<int> edges = new();
            for (int j = 0; j < numNodes; j++)
            {
                if (adjacencyMatrix[j, i] == 1)
                {
                    edges.Add(j + 1);
                }
            }
            int node = i + 1;
            NEmap.Add(node, edges);
        }

        return NEmap;
    }

    private List<Edge> GenerateSortedEdges(List<Edge> oldEdges, List<VertsTransformation> vertsTransformations)
    {
        List<Edge> newEdges = new();

        // ѕроходимс€ по всем старым ребрам
        foreach (Edge oldEdge in oldEdges)
        {
            int oldV1 = oldEdge.v1;
            int oldV2 = oldEdge.v2;

            // Ќаходим новый узел дл€ первой вершины ребра
            int newV1 = FindNewVertex(oldV1+1, vertsTransformations);

            // Ќаходим новый узел дл€ второй вершины ребра
            int newV2 = FindNewVertex(oldV2+1, vertsTransformations);

            // —оздаем новое ребро и добавл€ем его в список новых ребер
            newEdges.Add(new Edge(newV1, newV2));
        }

        return newEdges;
    }

    // ћетод дл€ нахождени€ нового узла на основе старого и списка vertsTransformations
    private int FindNewVertex(int oldVertex, List<VertsTransformation> vertsTransformations)
    {
        // ѕроходимс€ по всем уровн€м
        foreach (VertsTransformation oldNewVerts in vertsTransformations)
        {
            if (oldNewVerts.Old == oldVertex)
            {
                // Debug.Log($"return oldNewVerts.New = {oldNewVerts.New-1}");
                // ¬озвращаем новый узел
                return oldNewVerts.New-1;
            }
        }

        Debug.Log($"return oldVertex = {oldVertex}");
        // ≈сли новый узел не найден, возвращаем старый
        return oldVertex;
    }

    // ћетод дл€ генерации матрицы смежности на основе списка ребер
    public static int[,] GenerateAdjacencyMatrix(List<Edge> edges)
    {
        int maxVertex = 0;
        foreach (Edge edge in edges)
        {
            maxVertex = Mathf.Max(maxVertex, Mathf.Max(edge.v1+1, edge.v2+1));
        }

        Debug.Log($"Edges maxVertex = " + maxVertex);
        int[,] adjacencyMatrix = new int[maxVertex, maxVertex];

        foreach (Edge edge in edges)
        {
            Debug.Log($"Edge [edge.v1+1, edge.v2+1] = [{edge.v1+1}, {edge.v2+1}]");
            adjacencyMatrix[edge.v1, edge.v2] = 1;
            // adjacencyMatrix[edge.v2 - 1, edge.v1 - 1] = 1; // ≈сли граф неориентированный
        }

        return adjacencyMatrix;
    }




    public struct VertsTransformation
    {
        public int Old;
        public int New;
    }

    private static void LogVertsTransformation(List<List<VertsTransformation>> vertsTransformations)
    {
        for (int i = 0; i < vertsTransformations.Count; i++)
        {
            for (int j = 0; j < vertsTransformations[i].Count; j++)
            {
                Debug.Log($"new {vertsTransformations[i][j].New}(old {vertsTransformations[i][j].Old})");
            }
        }
    }

    private static void LogVertsTransformation(List<VertsTransformation> vertsTransformations)
    {
        for (int i = 0; i < vertsTransformations.Count; i++)
        {
            Debug.Log($"new {vertsTransformations[i].New}(old {vertsTransformations[i].Old})");
        }
    }

    private static void LogEdges(List<Edge> edges)
    {
        Debug.Log(string.Join(", ", edges.Select(edge => $"({edge.v1 + 1}, {edge.v2 + 1})")));
    }

    private void SetIncidenceToText(Dictionary<int, List<int>> incidence, ref TMP_InputField inputField)
    {
        StringBuilder stringBuilder = new StringBuilder();

        foreach (var kvp in incidence)
        {
            stringBuilder.Append($"G({kvp.Key}) = {{");
            for (int i = 0; i < kvp.Value.Count; i++)
            {
                stringBuilder.Append(kvp.Value[i]);
                if (i < kvp.Value.Count - 1)
                {
                    stringBuilder.Append("; ");
                }
            }
            stringBuilder.Append("}\n");
        }

        inputField.text = stringBuilder.ToString();
    }

}

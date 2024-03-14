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
                var k = i + j;
                normalizedSortedLevels[i].Level[j] = ordereredVert++;
            }

            Debug.Log($"normalizedSortedLevels[{i}].level = " + string.Join(',', normalizedSortedLevels[i].Level));
        }

        // orderedAdjacencyMatrix = GetAdjacencyMatrixFromLevels(normalizedLevels);

        // now we have {normalizedSortedLevels} -> go to the sorted adjacency matrix
        // ok, let's go)
        // —оздаем пустую матрицу смежности
        // «аполн€ем матрицу смежности на основе информации из списка иерархических уровней
        for (int i = 0; i < normalizedLevels.Count; i++)
        {
            foreach (var edge in normalizedLevels[i].Edges)
            {
                int j = normalizedLevels.FindIndex(level => level.Level[i] == edge.TargetLevel);
                orderedAdjacencyMatrix[i, j] = 1; // ”станавливаем 1, если есть св€зь между уровн€ми i и j
            }
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

    // ‘ункци€ дл€ получени€ матрицы смежности из списка уровней
    public int[,] GetAdjacencyMatrixFromLevels(List<HierarchicalLevel> normalizedLevels)
    {
        // ќпредел€ем количество вершин на основе количества уровней
        int vertexCount = 0;
        foreach (var level in normalizedLevels)
        {
            vertexCount += level.Level.Count;
        }
        Debug.Log(vertexCount);
        // —оздаем пустую матрицу смежности
        int[,] adjacencyMatrix = new int[vertexCount, vertexCount];

        // «аполн€ем матрицу смежности на основе св€зей между вершинами на разных уровн€х
        int offset = 0; // —мещение дл€ правильной индексации вершин на разных уровн€х
        for (int i = 0; i < normalizedLevels.Count - 1; i++)
        {
            foreach (var vertex1 in normalizedLevels[i].Level)
            {
                foreach (var vertex2 in normalizedLevels[i + 1].Level)
                {
                    Debug.Log(vertex1 + " : " + vertex2);
                    adjacencyMatrix[vertex1 + offset, vertex2 + offset] = 1;

                    // adjacencyMatrix[vertex2 + offset, vertex1 + offset] = 1; // ≈сли граф неориентированный, то нужно установить и дл€ обратного ребра
                }
            }
            Debug.Log("offset = " + offset);
            offset += normalizedLevels[i].Level.Count; // ќбновл€ем смещение дл€ следующего уровн€
        }

        return adjacencyMatrix;
    }



}

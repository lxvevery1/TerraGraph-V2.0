using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static HierarchialLVLScript;
using System;
using System.Text;

public class OldAlgoScript : MonoBehaviour
{
    [SerializeField] private TMP_InputField adjacencyMatrixInputField;
    [SerializeField] private TMP_InputField updatedAdjacencyMatrixInputField;
    [SerializeField] private TMP_InputField leftIncidenceSetInputField;
    private int[,] adjacencyMatrix;


    private void Awake()
    {
        adjacencyMatrixInputField.GetComponentInChildren<TMP_Text>().enableWordWrapping = false;
    }


    // Called when the "Generate" button is clicked
    public void OnGenerateButtonClick()
    {
        string matrixText = adjacencyMatrixInputField.text;
        adjacencyMatrix = ParseAdjacencyMatrix(matrixText);

        var hLVLs = CreateHLevels(new List<HierarchicalLevel>(), adjacencyMatrix.Rank, new List<Edge>());

        Debug.Log(hLVLs.Count);

        int[,] updatedAdjacencyMatrix = ConvertHLVLsToAdjMatr(hLVLs);

        updatedAdjacencyMatrixInputField.text = updatedAdjacencyMatrix.ToString();
        // leftIncidenceSetInputField.text = LeftIncidentEdgesToString(newGraph);
    }

    // Метод для конвертации списка уровней иерархии в матрицу смежности
    public static int[,] ConvertHLVLsToAdjMatr(List<HierarchicalLevel> HL)
    {
        // Подсчитываем общее количество вершин
        int numVertices = 0;
        foreach (var level in HL)
        {
            numVertices += level.level.Count;
        }

        // Создаем пустую матрицу смежности
        int[,] adjacencyMatrix = new int[numVertices, numVertices];

        // Заполняем матрицу смежности на основе уровней иерархии
        int currentVertexIndex = 0;
        foreach (var level in HL)
        {
            foreach (var vertex in level.level)
            {
                // Перебираем все рёбра, связанные с текущей вершиной
                foreach (var edge in levelEdges(level, vertex))
                {
                    // Изменяем соответствующий элемент матрицы смежности
                    adjacencyMatrix[edge.v1 - 1, currentVertexIndex] = 1;
                }

                currentVertexIndex++;
            }
        }

        return adjacencyMatrix;
    }

    // Вспомогательный метод для получения рёбер, связанных с вершиной на данном уровне
    private static List<Edge> levelEdges(HierarchicalLevel level, int vertex)
    {
        var edges = new List<Edge>();

        foreach (var v in level.level)
        {
            if (v != vertex)
            {
                edges.Add(new Edge(v, vertex));
            }
        }

        return edges;
    }

    // Parse the adjacency matrix from the input field text
    public int[,] ParseAdjacencyMatrix(string text)
    {
        string[] lines = text.Trim().Split('\n');
        int numRows = lines.Length;
        int numCols = lines[0].Trim().Split(' ').Length;
        int[,] matrix = new int[numRows, numCols];

        Debug.Log(numRows + ", " + numCols);

        if (numRows != numCols)
        {
            ErrorMessage.Instance.ThrowError("Adjacency matrix must be square (nxn)");
            return null;
        }

        for (int i = 0; i < numRows; i++)
        {
            string[] rowValues = lines[i].Trim().Split(' ');
            if (rowValues.Length != numCols)
            {
                ErrorMessage.Instance.ThrowError("Invalid number of elements in row " + (i + 1));
                return null;
            }

            for (int j = 0; j < numCols; j++)
            {
                if (!int.TryParse(rowValues[j], out matrix[i, j]))
                {
                    ErrorMessage.Instance.ThrowError("Invalid element at row " + (i + 1) + ", column " + (j + 1));
                    return null;
                }
            }
        }

        return matrix;
    }

    public static string LeftIncidentEdgesToString(List<Tuple<int, int>> leftIncidentEdges)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var tuple in leftIncidentEdges)
        {
            sb.Append($"[(NEW){tuple.Item1}]->[(OLD){tuple.Item2}]\n");
        }

        return sb.ToString();
    }
}

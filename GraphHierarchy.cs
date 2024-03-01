using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GraphHierarchy : MonoBehaviour
{
    [SerializeField] private AlgoManager uiManager;
    [SerializeField] private TMP_InputField adjacencyMatrixInputField;
    private readonly int[,] adjacencyMatrix;


    private void Start()
    {
        // Выполнение топологической сортировки и присвоение порядковых номеров
        Dictionary<int, int> vertexOrder = AssignOrder(uiManager.ParseAdjacencyMatrix(adjacencyMatrixInputField.text));

        // Вывод новых и старых номеров вершин
        Debug.Log("Старый номер -> Новый номер");
        foreach (var entry in vertexOrder)
        {
            Debug.Log(entry.Key + " -> " + entry.Value);
        }

        // Представление нового графа с новыми номерами вершин
        Debug.Log("Новый граф:");
        for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
        {
            List<int> neighbors = GetNeighbors(adjacencyMatrix, i);
            List<int> newNeighbors = new List<int>();
            foreach (int neighbor in neighbors)
            {
                newNeighbors.Add(vertexOrder[neighbor]);
            }
            Debug.Log(vertexOrder[i] + " -> " + string.Join(", ", newNeighbors));
        }
    }


    // Функция присвоения порядковых номеров вершинам
    private Dictionary<int, int> AssignOrder(int[,] matrix)
    {
        List<int> topologicalOrder = TopologicalSort(matrix);
        Dictionary<int, int> vertexOrder = new Dictionary<int, int>();
        for (int i = 0; i < topologicalOrder.Count; i++)
        {
            vertexOrder[topologicalOrder[i]] = i + 1; // Нумерация начинается с 1
        }
        return vertexOrder;
    }

    // Функция топологической сортировки
    private List<int> TopologicalSort(int[,] matrix)
    {
        List<int> result = new List<int>();
        HashSet<int> visited = new HashSet<int>();

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            if (!visited.Contains(i))
            {
                DFS(matrix, i, visited, result);
            }
        }

        result.Reverse(); // Результат обратного обхода - топологически упорядоченный список
        return result;
    }

    // Обход в глубину для топологической сортировки
    private void DFS(int[,] matrix, int vertex, HashSet<int> visited, List<int> result)
    {
        visited.Add(vertex);
        for (int i = 0; i < matrix.GetLength(1); i++)
        {
            if (matrix[vertex, i] == 1 && !visited.Contains(i))
            {
                DFS(matrix, i, visited, result);
            }
        }
        result.Add(vertex);
    }

    // Получение списка соседей вершины
    private List<int> GetNeighbors(int[,] matrix, int vertex)
    {
        List<int> neighbors = new List<int>();
        for (int i = 0; i < matrix.GetLength(1); i++)
        {
            if (matrix[vertex, i] == 1)
            {
                neighbors.Add(i);
            }
        }
        return neighbors;
    }

}

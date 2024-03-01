using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using static IncidenceGenerator;

public class AlgoManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField adjacencyMatrixInputField;
    [SerializeField] private TMP_InputField result;
    [SerializeField] private TMP_InputField sortedGraphText;
    [SerializeField] private ErrorMessage errorMessage;
    
    private int[,] adjacencyMatrix;
    private Incidences incidences;


    private void Awake()
    {
        adjacencyMatrixInputField.GetComponentInChildren<TMP_Text>().enableWordWrapping = false;
    }

    private void Start()
    {
        // ���������� �������������� ���������� � ���������� ���������� �������
        Dictionary<int, int> vertexOrder = AssignOrder(ParseAdjacencyMatrix(adjacencyMatrixInputField.text));

        // ����� ����� � ������ ������� ������
        Debug.Log("������ ����� -> ����� �����");
        foreach (var entry in vertexOrder)
        {
            Debug.Log(entry.Key + " -> " + entry.Value);
        }

        // ������������� ������ ����� � ������ �������� ������
        Debug.Log("����� ����:");
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
            errorMessage.ThrowError("Adjacency matrix must be square (nxn)");
            return null;
        }

        for (int i = 0; i < numRows; i++)
        {
            string[] rowValues = lines[i].Trim().Split(' ');
            if (rowValues.Length != numCols)
            {
                errorMessage.ThrowError("Invalid number of elements in row " + (i + 1));
                return null;
            }

            for (int j = 0; j < numCols; j++)
            {
                if (!int.TryParse(rowValues[j], out matrix[i, j]))
                {
                    errorMessage.ThrowError("Invalid element at row " + (i + 1) + ", column " + (j + 1));
                    return null;
                }
            }
        }

        return matrix;
    }


    // Called when the "Generate" button is clicked
    public void OnGenerateButtonClick()
    {
        string matrixText = adjacencyMatrixInputField.text;
        adjacencyMatrix = ParseAdjacencyMatrix(matrixText);
        if (adjacencyMatrix != null)
        {
            incidences = IncidenceGenerator.GenerateIncidenceLists(adjacencyMatrix);

            SetIncidenceToText(incidences.right, ref result);

            // ���������� ��������� ����� �������������� ����������
            SetSortedGraphText();
        }
    }


    private void SetIncidenceToText(Dictionary<int, List<int>> incidence, ref TMP_InputField inputField)
    {
        StringBuilder stringBuilder = new();

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

    // ����������� ����� ��� ����������� ���������� �������������� ����������
    private void SetSortedGraphText()
    {
        // ���������� �������������� ���������� � ���������� ���������� �������
        Dictionary<int, int> vertexOrder = AssignOrder(adjacencyMatrix);

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("����� ����:");
        for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
        {
            List<int> neighbors = GetNeighbors(adjacencyMatrix, i);
            List<int> newNeighbors = new List<int>();
            foreach (int neighbor in neighbors)
            {
                newNeighbors.Add(vertexOrder[neighbor]);
            }
            stringBuilder.AppendLine($"{vertexOrder[i]} -> {string.Join(", ", newNeighbors)}");
        }

        sortedGraphText.text = stringBuilder.ToString(); // ���������� ��������� � ����� TMP_Text
    }


    // ������� ���������� ���������� ������� ��������
    private Dictionary<int, int> AssignOrder(int[,] matrix)
    {
        List<int> topologicalOrder = TopologicalSort(matrix);
        Dictionary<int, int> vertexOrder = new Dictionary<int, int>();
        for (int i = 0; i < topologicalOrder.Count; i++)
        {
            vertexOrder[topologicalOrder[i]] = i + 1; // ��������� ���������� � 1
        }
        return vertexOrder;
    }

    // ������� �������������� ����������
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

        result.Reverse(); // ��������� ��������� ������ - ������������� ������������� ������
        return result;
    }

    // ����� � ������� ��� �������������� ����������
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

    // ��������� ������ ������� �������
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

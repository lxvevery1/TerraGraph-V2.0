using System.Collections.Generic;

public static class AlgoManager
{
    // Define the Edge class
    public class Edge
    {
        public int v1, v2;
        public int TargetLevel { get; set; }

        public Edge(int targetLevel)
        {
            TargetLevel = targetLevel;
        }
        public Edge(int v1, int v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }
    }

    // Define the HierarchicalLevel class
    public class HierarchicalLevel
    {
        public List<int> Level { get; set; }
        public List<Edge> Edges { get; set; }

        public HierarchicalLevel()
        {
            Level = new List<int>();
            Edges = new List<Edge>();
        }
    }

    // Method to create hierarchical levels
    public static void CreateHLevel(List<HierarchicalLevel> HL, int numberV, List<Edge> E) //numberV - количество вершин
    {
        List<int> usedV = new(); //список вершин, уже использованных в порядковой функции

        List<int> notUsedV = new(); //список вершин, еще не использованных в порядковой функции

        for (int i = 0; i < numberV; i++)
            notUsedV.Add(i);

        while (notUsedV.Count > 0)
        {
            HL.Add(new HierarchicalLevel());
            for (int i = 0; i < notUsedV.Count; i++)
            {
                int k = 0;
                for (int j = 0; j < E.Count; j++)
                    if (E[j].v2 == notUsedV[i])
                        k++; //считаем полустепень захода вершины
                for (int m = 0; m < usedV.Count; m++)
                    for (int j = 0; j < E.Count; j++)
                    {
                        if (E[j].v1 == usedV[m] && E[j].v2 == notUsedV[i])
                            k--; //вычитаем дуги, иходящие из вершин предыдущих уровней и входящие в вершину i
                    }
                if (k == 0)
                {
                    HL[HL.Count - 1].Level.Add(notUsedV[i]);
                    notUsedV.RemoveAt(i);
                    i--;
                }
            }

            for (int j = 0; j < HL[HL.Count - 1].Level.Count; j++)
            {
                usedV.Add(HL[HL.Count - 1].Level[j]);
            }
        }
    }

    // Method to convert adjacency matrix to ordered adjacency matrix
    public static int[,] ConvertToOrderedAdjacencyMatrix(int[,] adjacencyMatrix, List<HierarchicalLevel> HL)
    {
        int[,] orderedAdjacencyMatrix = new int[adjacencyMatrix.GetLength(0), adjacencyMatrix.GetLength(1)];

        for (int i = 0; i < HL.Count; i++)
        {
            for (int j = 0; j < HL[i].Level.Count; j++)
            {
                int vertex = HL[i].Level[j];

                for (int k = 0; k < adjacencyMatrix.GetLength(1); k++)
                {
                    orderedAdjacencyMatrix[vertex, k] = adjacencyMatrix[vertex, k];
                }
            }
        }

        return orderedAdjacencyMatrix;
    }
}

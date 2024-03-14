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
using System;
using System.Collections.Generic;

public static class HierarchialLVLScript
{
    public static List<HierarchicalLevel> CreateHLevels(List<HierarchicalLevel> HL, int numberV, List<Edge> E) //numberV - количество вершин
    {
        List<int> usedV = new List<int>(); //список вершин, уже использованных в порядковой функции

        List<int> notUsedV = new List<int>(); //список вершин, еще не использованных в порядковой функции

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
                {
                    for (int j = 0; j < E.Count; j++)
                    {
                        if (E[j].v1 == usedV[m] && E[j].v2 == notUsedV[i])
                            k--; //вычитаем дуги, иходящие из вершин предыдущих уровней и входящие в вершину i
                    }
                }

                if (k == 0)
                {
                    HL[HL.Count - 1].level.Add(notUsedV[i]);
                    notUsedV.RemoveAt(i);
                    i--;
                }
            }

            for (int j = 0; j < HL[HL.Count - 1].level.Count; j++)
            {
                usedV.Add(HL[HL.Count - 1].level[j]);
            }
        }

        return HL;
    }

    public class HierarchicalLevel
    {
        public List<int> level;

        public HierarchicalLevel()
        {
            level = new List<int>();
        }
    }

    public class Edge
    {
        public int v1, v2;

        public Edge(int v1, int v2)
        {
            this.v1 = v1;

            this.v2 = v2;
        }
    }
}

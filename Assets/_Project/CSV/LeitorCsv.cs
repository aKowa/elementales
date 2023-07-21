using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LeitorCsv
{
    /// <summary>
    /// funçao faz tals
    /// </summary>
    /// <param name="textAsset">Arquivo Csv do excel</param>
    /// <param name="skipFirstRow">Pular primeira linha da tabela</param>
    /// <param name="skipFirstColumn">Pular primeira coluna da tabel</param>
    /// <returns>Retorna uma list de string</returns>
    public static List<string> ReadCsv(TextAsset textAsset,bool skipFirstRow,bool skipFirstColumn)
    {
        List<string> list = new List<string>();

        string[] data = textAsset.text.Split(new string[] { ",", "\n" }, StringSplitOptions.None);
        list = data.ToList();
        RemoveEmpty(ref list);

        if (skipFirstRow)
        {
            RemoveFirstRow(ref list);
        }
        if (skipFirstColumn)
        {
            RemoveFirstColumn(ref list);
        }

        return list;

    }
    static void RemoveEmpty(ref List<string> list)
    {
        List<int> indicesToRemove = new List<int>();
        for (int i = 0; i < list.Count; i++) // Remove EmpyLists
        {
            if (list[i] == "")
            {
                indicesToRemove.Add(i);
            }
        }
        list.RemoveRange(indicesToRemove[0], indicesToRemove.Count);
    }
    static void RemoveFirstRow(ref List<string> list)
    {
        for (int i = 0; i < list.Count - 1; i++) // Skip first Row of excel
        {
            list[i] = list[i + 1];
        }
    }
    static void RemoveFirstColumn(ref List<string> list)
    {
        for (int i = 0; i < list.Count - 1; i++) // Skip first Row of excel
        {
            var x = list[i].Split(new char[] { ';' });
            list[i] = "";
            for (int j = 1; j < x.Length; j++)
            {
                list[i] += x[j];
                if (j + 1 < x.Length)
                    list[i]+=";";
            }
        }
        list[list.Count - 1] = "";
        RemoveEmpty(ref list);
    }
}

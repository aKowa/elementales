using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CsvToMonsterType : MonoBehaviour
{
    [SerializeField] private TextAsset csv;
    [SerializeField] private bool pularPrimeiraLinha, pularPrimeiraColuna;
    [SerializeField] private List<MonsterType> tipos;
    private void Awake()
    {
        List<string> tabela = new List<string>();
        tabela = LeitorCsv.ReadCsv(csv, pularPrimeiraLinha, pularPrimeiraColuna);
        foreach (var item in tipos)
        {
            item.VantagemContra.Clear();
        }

        for (int i = 0; i < tipos.Count; i++)
        {
            var x = tabela[i].Split(new char[] { ';' });

            for (int j = 0; j < x.Length; j++)
            {
                tipos[i].VantagemContra.Add(new TypeRelation(tipos[j], x[j]));
            }
        }
    }
}

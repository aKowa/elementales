using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterBook
{
    //Variaveis
    [SerializeField] private List<MonsterEntry> monsterEntries;

    //Getters
    public List<MonsterEntry> MonsterEntries => monsterEntries;

    public int MonstrosEncontrados()
    {
        int monstrosEncontrados = 0;

        for(int i = 0; i < MonsterEntries.Count; i++)
        {
            if (monsterEntries[i].WasFound == true)
            {
                monstrosEncontrados++;
            }
        }

        return monstrosEncontrados;
    }

    public int MonstrosCapturados()
    {
        int monstrosCapturados = 0;

        for (int i = 0; i < MonsterEntries.Count; i++)
        {
            if (monsterEntries[i].WasCaptured == true)
            {
                monstrosCapturados++;
            }
        }

        return monstrosCapturados;
    }

    public bool CapturouMonstroDoTipo(MonsterType tipo)
    {
        for (int i = 0; i < monsterEntries.Count; i++)
        {
            if (monsterEntries[i].WasCaptured == true)
            {
                MonsterData monstro = GlobalSettings.Instance.Listas.ListaDeMonsterData.GetData(i);

                for (int y = 0; y < monstro.GetMonsterTypes.Count; y++)
                {
                    if (monstro.GetMonsterTypes[y] == tipo)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool CapturouMonstroDoTipo(List<MonsterType> tipos)
    {
        for (int i = 0; i < monsterEntries.Count; i++)
        {
            if (monsterEntries[i].WasCaptured == true)
            {
                MonsterData monstro = GlobalSettings.Instance.Listas.ListaDeMonsterData.GetData(i);

                for (int y = 0; y < monstro.GetMonsterTypes.Count; y++)
                {
                    for(int z = 0; z < tipos.Count; z++)
                    {
                        if (monstro.GetMonsterTypes[y] == tipos[z])
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    public MonsterBook(ListaDeMonsterData listaDeMonsterData)
    {
        monsterEntries = new List<MonsterEntry>();

        for(int i = 0; i < listaDeMonsterData.GetListaDataLenght; i++)
        {
            monsterEntries.Add(new MonsterEntry());
        }
    }

    public MonsterBook(ListaDeMonsterData listaDeMonsterData, MonsterBookSave monsterBookSave)
    {
        monsterEntries = new List<MonsterEntry>();

        for (int i = 0; i < listaDeMonsterData.GetListaDataLenght; i++)
        {
            if(i < monsterBookSave.monsterEntries.Count)
            {
                monsterEntries.Add(new MonsterEntry(monsterBookSave.monsterEntries[i]));
            }
            else
            {
                monsterEntries.Add(new MonsterEntry());
            }
        }
    }
}

[System.Serializable]
public class MonsterEntry
{
    //Variaveis
    [SerializeField] private bool wasFound;
    [SerializeField] private bool wasCaptured;

    //Getters

    public bool WasFound
    {
        get => wasFound;
        set => wasFound = value;
    }

    public bool WasCaptured
    {
        get => wasCaptured;
        set => wasCaptured = value;
    }

    public MonsterEntry()
    {
        wasFound = false;
        wasCaptured = false;
    }

    public MonsterEntry(MonsterEntrySave monsterEntrySave)
    {
        wasFound = monsterEntrySave.wasFound;
        wasCaptured = monsterEntrySave.wasCaptured;
    }
}

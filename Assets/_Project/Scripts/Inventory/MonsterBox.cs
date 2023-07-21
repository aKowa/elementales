using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterBox
{
    //Constantes
    public const int numeroMonstrosMax = 15;

    //Variaveis
    [SerializeField] private string boxName;
    private Monster[] monsters; //Nao serialize este array! Ou os itens dele deixarao de ser nulos e causara um erro no jogo

    //Getters
    public Monster[] Monsters => monsters;

    public string BoxName
    {
        get => boxName;
        set => boxName = value;
    }

    public MonsterBox(string boxName)
    {
        this.boxName = boxName;
        monsters = new Monster[numeroMonstrosMax];
    }

    public MonsterBox(MonsterBoxSave monsterBoxSave)
    {
        boxName = monsterBoxSave.boxName;
        monsters = new Monster[monsterBoxSave.monsters.Length];

        for (int i = 0; i < monsterBoxSave.monsters.Length; i++)
        {
            if (monsterBoxSave.monsters[i].monsterDataID >= 0)
            {
                monsters[i] = new Monster(monsterBoxSave.monsters[i]);
            }
            else
            {
                monsters[i] = null;
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackHolder
{
    //Variaveis
    [SerializeField] private ComandoDeAtaque attack;
    [SerializeField] private int pp;

    //Getters
    public ComandoDeAtaque Attack => attack;

    public int PP
    {
        get => pp;
        set => pp = value;
    }

    //Construtor
    public AttackHolder(ComandoDeAtaque ataque)
    {
        this.attack = ataque;
        pp = this.attack.MaxPP;
    }

    public AttackHolder(AttackHolderSave attackHolderSave)
    {
        Comando comando = GlobalSettings.Instance.Listas.ListaDeComandos.GetData(attackHolderSave.attackID);

        if (comando is ComandoDeAtaque)
        {
            attack = (ComandoDeAtaque) comando;
        }
        else
        {
            Debug.LogWarning("O comando " + comando.name + " nao e um comando de ataque!");
        }

        this.pp = attackHolderSave.pp;
    }
}

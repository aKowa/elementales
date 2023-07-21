using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemHolder
{
    //Variaveis
    [SerializeField] private Item item;
    [SerializeField] private int quantidade;

    private DateTime dataGot;

    //Getters
    public Item Item => item;
    public DateTime DataGot => dataGot;

    public int Quantidade
    {
        get => quantidade;
        set => quantidade = value;
    }

    //Construtor
    public ItemHolder(Item item, int quantidade)
    {
        this.item = item;
        this.quantidade = quantidade;
        dataGot = DateTime.Now;
    }

    public ItemHolder(ItemHolderSave itemHolderSave)
    {
        this.item = GlobalSettings.Instance.Listas.ListaDeItens.GetData(itemHolderSave.itemID);
        this.quantidade = itemHolderSave.quantidade;
        this.dataGot = new DateTime(itemHolderSave.dataGot.year, itemHolderSave.dataGot.month, itemHolderSave.dataGot.day, itemHolderSave.dataGot.hour, itemHolderSave.dataGot.minute, itemHolderSave.dataGot.second);
    }
}

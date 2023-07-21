using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class InventarioInfo : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] protected GameObject itemSlotBase;
    [SerializeField] protected RectTransform itemSlotsHolder;
    [SerializeField] protected TMP_Text textoSemItens;

    //Variaveis
    [Header("Variaveis")]
    [SerializeField] private string nomeDaGuia;

    private UnityEvent<ItemSlot> eventoItemSelecionado = new UnityEvent<ItemSlot>();

    protected List<ItemSlot> itemSlots = new List<ItemSlot>();

    //Getters
    public UnityEvent<ItemSlot> EventoItemSelecionado => eventoItemSelecionado;

    public string NomeDaGuia => nomeDaGuia;

    public abstract void AtualizarInformacoes(Inventario inventario);

    protected void AtualizarItens(List<ItemHolder> listaDeItens)
    {
        float boxHeight = 0;
        float itemSlotHeight = itemSlotBase.GetComponent<RectTransform>().sizeDelta.y;
        float spacing = itemSlotsHolder.GetComponent<VerticalLayoutGroup>().spacing;

        ResetarItemSlots();

        for(int i = 0; i < listaDeItens.Count; i++)
        {
            ItemSlot itemSlot = Instantiate(itemSlotBase, itemSlotsHolder).GetComponent<ItemSlot>();
            itemSlot.gameObject.SetActive(true);

            itemSlot.ItemHolder = listaDeItens[i];
            itemSlot.EventoItemSelecionado.AddListener(ItemSelecionado);

            itemSlot.AtualizarInformacoes();

            itemSlots.Add(itemSlot);

            boxHeight += itemSlotHeight;
        }

        boxHeight += (spacing * (itemSlots.Count - 1));

        if(boxHeight < 0)
        {
            boxHeight = 0;
        }

        itemSlotsHolder.sizeDelta = new Vector2(itemSlotsHolder.sizeDelta.x, boxHeight);

        textoSemItens.gameObject.SetActive(listaDeItens.Count <= 0);
    }

    public void ResetarItemSlots()
    {
        foreach (ItemSlot itemSlot in itemSlots)
        {
            Destroy(itemSlot.gameObject);
        }

        itemSlots.Clear();

        itemSlotsHolder.anchoredPosition = Vector2.zero;
    }

    protected void ItemSelecionado(ItemSlot itemSlot)
    {
        eventoItemSelecionado?.Invoke(itemSlot);
    }
}

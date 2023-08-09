using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LojaIAPInfo : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] protected GameObject itemSlotLojaIAPBase;

    [Space(10)]

    [SerializeField] protected RectTransform itemSlotsHolder;
    [SerializeField] protected TMP_Text nomeDaGuia;

    protected InventarioLojaIAP inventarioLoja;

    //Variaveis
    protected UnityEvent<ItemSlotLojaIAP> eventoItemSelecionado = new UnityEvent<ItemSlotLojaIAP>();

    protected List<ItemSlotLojaIAP> itemSlots = new List<ItemSlotLojaIAP>();

    //Getters
    public TMP_Text NomeDaGuia => nomeDaGuia;
    public UnityEvent<ItemSlotLojaIAP> EventoItemSelecionado => eventoItemSelecionado;

    public InventarioLojaIAP InventarioLoja { get => inventarioLoja; set => inventarioLoja = value; }

    public void AtualizarInformacoes()
    {
        AtualizarItens(inventarioLoja);
        
        GetComponent<IAPShop>().AssignButtonBehaviour(itemSlots);
    }

    protected void AtualizarItens(InventarioLojaIAP inventarioLoja)
    {
        ResetarItemSlots();

        //Debug.Log($"Loja: {gameObject.name}, Scriptable: {inventarioLoja.name}", gameObject);

        for (int i = 0; i < inventarioLoja.ItensDaLoja.Length; i++)
        {
            InventarioLojaIAP.ItemLojaIAP itemLoja = inventarioLoja.ItensDaLoja[i];

            //Debug.Log($"Produto: {itemLoja.ProductID}, Indice: {i}.");

            ItemSlotLojaIAP itemSlot = Instantiate(itemSlotLojaIAPBase, itemSlotsHolder).GetComponent<ItemSlotLojaIAP>();
            itemSlot.gameObject.SetActive(true);

            itemSlot.EventoItemSelecionado.AddListener(ItemSelecionado);

            itemSlot.AtualizarImagem(itemLoja.ProductImage);
            itemSlot.IAPButton.productId = itemLoja.ProductID;
            itemSlot.IAPButton.gameObject.SetActive(true);

            itemSlots.Add(itemSlot);
        }
    }

    public void ResetarItemSlots()
    {
        foreach (ItemSlotLojaIAP itemSlot in itemSlots)
        {
            Destroy(itemSlot.gameObject);
        }

        itemSlots.Clear();

        itemSlotsHolder.anchoredPosition = Vector2.zero;
    }

    protected void ItemSelecionado(ItemSlotLojaIAP itemSlot)
    {
        eventoItemSelecionado?.Invoke(itemSlot);
    }
}

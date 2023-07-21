using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LojaIAPInfo : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] protected GameObject itemSlotLojaIAPBase;
    [SerializeField] protected RectTransform itemSlotsHolder;

    //Variaveis
    protected List<ItemSlotLojaIAP> itemSlots = new List<ItemSlotLojaIAP>();

    public void AtualizarInformacoes(List<InventarioLojaIAP.ItemLojaIAP> itensDaLoja)
    {
        AtualizarItens(itensDaLoja);
        
        GetComponent<IAPShop>().AssignButtonBehaviour(itemSlots);
    }

    protected void AtualizarItens(List<InventarioLojaIAP.ItemLojaIAP> listaDeItens)
    {
        float boxHeight = 0;
        float itemSlotHeight = itemSlotLojaIAPBase.GetComponent<RectTransform>().sizeDelta.y;
        float spacing = itemSlotsHolder.GetComponent<VerticalLayoutGroup>().spacing;

        ResetarItemSlots();

        for (int i = 0; i < listaDeItens.Count; i++)
        {
            ItemSlotLojaIAP itemSlot = Instantiate(itemSlotLojaIAPBase, itemSlotsHolder).GetComponent<ItemSlotLojaIAP>();
            itemSlot.gameObject.SetActive(true);

            itemSlot.IAPButton.productId = listaDeItens[i].ProductID;
            itemSlot.IAPButton.gameObject.SetActive(true);

            itemSlots.Add(itemSlot);

            boxHeight += itemSlotHeight;
        }

        boxHeight += (spacing * (itemSlots.Count - 1));

        if (boxHeight < 0)
        {
            boxHeight = 0;
        }

        itemSlotsHolder.sizeDelta = new Vector2(itemSlotsHolder.sizeDelta.x, boxHeight);
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
}

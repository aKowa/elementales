using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LojaInfo : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] protected GameObject itemSlotLojaBase;
    [SerializeField] protected RectTransform itemSlotsHolder;

    //Enums
    public enum ListaDeItens { ListaDaLoja, Itens, MonsterBalls, Skills }

    //Variaveis

    [Header("Variaveis")]
    [SerializeField] private ListaDeItens listaDeItensDaGuia;

    [SerializeField] private bool itensParaVender;

    private UnityEvent<ItemSlotLoja> eventoItemSelecionado = new UnityEvent<ItemSlotLoja>();

    protected List<ItemSlotLoja> itemSlots = new List<ItemSlotLoja>();


    //Getters
    public UnityEvent<ItemSlotLoja> EventoItemSelecionado => eventoItemSelecionado;


    public void AtualizarInformacoes(List<ItemHolder> itensDaLoja, Inventario inventario)
    {
        switch(listaDeItensDaGuia)
        {
            case ListaDeItens.ListaDaLoja:
                AtualizarItens(itensDaLoja);
                break;

            case ListaDeItens.Itens:
                AtualizarItens(inventario.Itens);
                break;

            case ListaDeItens.MonsterBalls:
                AtualizarItens(inventario.MonsterBalls);
                break;
            
            case ListaDeItens.Skills:
                AtualizarItens(inventario.Habilidades);
                break;
        }
    }

    protected void AtualizarItens(List<ItemHolder> listaDeItens)
    {
        float boxHeight = 0;
        float itemSlotHeight = itemSlotLojaBase.GetComponent<RectTransform>().sizeDelta.y;
        float spacing = itemSlotsHolder.GetComponent<VerticalLayoutGroup>().spacing;

        ResetarItemSlots();

        for (int i = 0; i < listaDeItens.Count; i++)
        {
            ItemSlotLoja itemSlot = Instantiate(itemSlotLojaBase, itemSlotsHolder).GetComponent<ItemSlotLoja>();
            itemSlot.gameObject.SetActive(true);

            itemSlot.Iniciar(listaDeItens[i], itensParaVender);

            itemSlot.EventoItemSelecionado.AddListener(ItemSelecionado);

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
        foreach (ItemSlotLoja itemSlot in itemSlots)
        {
            Destroy(itemSlot.gameObject);
        }

        itemSlots.Clear();

        itemSlotsHolder.anchoredPosition = Vector2.zero;
    }

    protected void ItemSelecionado(ItemSlotLoja itemSlot)
    {
        eventoItemSelecionado?.Invoke(itemSlot);
    }
}

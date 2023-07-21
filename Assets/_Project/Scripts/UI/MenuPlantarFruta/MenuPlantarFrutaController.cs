using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuPlantarFrutaController : ViewController
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] protected GameObject itemSlotBase;
    [SerializeField] protected RectTransform itemSlotsHolder;
    [SerializeField] protected TMP_Text textoSemItens;
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;

    //Variaveis
    private List<ItemSlot> itemSlots = new List<ItemSlot>();

    private VasoPlanta vasoAtual;

    protected override void OnAwake()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);
    }

    public override void OnOpen()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(true);

        BergamotaLibrary.PauseManager.Pausar(true);
    }

    protected override void OnClose()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        BergamotaLibrary.PauseManager.Pausar(false);

        ResetarInformacoes();
    }

    public void IniciarMenu(VasoPlanta vasoPlanta)
    {
        ResetarInformacoes();

        vasoAtual = vasoPlanta;

        AtualizarInformacoes();

        OpenView();
    }

    private void AtualizarInformacoes()
    {
        AtualizarItens(PlayerData.Instance.Inventario.Itens);
    }

    private void AtualizarItens(List<ItemHolder> listaDeItens)
    {
        float boxHeight = 0;
        float itemSlotHeight = itemSlotBase.GetComponent<RectTransform>().sizeDelta.y;
        float spacing = itemSlotsHolder.GetComponent<VerticalLayoutGroup>().spacing;

        for (int i = 0; i < listaDeItens.Count; i++)
        {
            StructPlantavel planta = vasoAtual.GetPlantaData(listaDeItens[i].Item);

            if(planta.Item == null)
            {
                continue;
            }

            ItemSlot itemSlot = Instantiate(itemSlotBase, itemSlotsHolder).GetComponent<ItemSlot>();
            itemSlot.gameObject.SetActive(true);

            itemSlot.ItemHolder = listaDeItens[i];
            itemSlot.EventoItemSelecionado.AddListener(ItemSelecionado);

            itemSlot.AtualizarInformacoes();

            itemSlots.Add(itemSlot);

            boxHeight += itemSlotHeight;
        }

        boxHeight += (spacing * (itemSlots.Count - 1));

        if (boxHeight < 0)
        {
            boxHeight = 0;
        }

        itemSlotsHolder.sizeDelta = new Vector2(itemSlotsHolder.sizeDelta.x, boxHeight);

        textoSemItens.gameObject.SetActive(this.itemSlots.Count <= 0);
    }

    private void ResetarInformacoes()
    {
        foreach (ItemSlot itemSlot in itemSlots)
        {
            Destroy(itemSlot.gameObject);
        }

        vasoAtual = null;

        itemSlots.Clear();

        itemSlotsHolder.anchoredPosition = Vector2.zero;
    }

    private void ItemSelecionado(ItemSlot itemSlot)
    {
        vasoAtual.PlantarItem(itemSlot.ItemHolder.Item);

        CloseView();
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlotLoja : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private Image imagem;
    [SerializeField] private TMP_Text nomeItem;
    [SerializeField] private TMP_Text precoItem;

    [Header("Variaveis Padroes")]
    [SerializeField] private Color corSelecionado;

    private ScrollRect scrollRect;

    //Variaveis
    private UnityEvent<ItemSlotLoja> eventoItemSelecionado = new UnityEvent<ItemSlotLoja>();

    private ItemHolder itemHolder;

    private bool itemParaVender;

    private bool apertado;

    //Getters
    public UnityEvent<ItemSlotLoja> EventoItemSelecionado => eventoItemSelecionado;
    public ItemHolder ItemHolder => itemHolder;

    private void Awake()
    {
        //Componentes
        DragAndDropButton dragAndDropButton = GetComponent<DragAndDropButton>();
        HoldButton holdButton = GetComponent<HoldButton>();

        scrollRect = GetComponentInParent<ScrollRect>();

        //Variaveis
        itemParaVender = false;
        apertado = false;

        //Eventos
        holdButton.OnPointerDownEvent.AddListener(OnPointerDown);
        holdButton.OnPointerUpEvent.AddListener(OnPointerUp);
        dragAndDropButton.OnBeginDragEvent.AddListener(OnBeginDrag);
        dragAndDropButton.OnDragEvent.AddListener(OnDrag);
        dragAndDropButton.OnEndDragEvent.AddListener(OnEndDrag);
    }

    public void Iniciar(ItemHolder itemHolder, bool itemParaVender)
    {
        this.itemHolder = itemHolder;
        this.itemParaVender = itemParaVender;

        AtualizarInformacoes();
    }

    public void AtualizarInformacoes()
    {
        nomeItem.text = itemHolder.Item.Nome;

        if(itemParaVender == false)
        {
            precoItem.text = "$ " + itemHolder.Item.Preco.ToString();
        }
        else
        {
            precoItem.text = "$ " + ((int)(itemHolder.Item.Preco * MenuDaLojaController.modificadorItemParaVenda)).ToString();
        }
    }

    public void Selecionado(bool selecionado)
    {
        if (selecionado == true)
        {
            imagem.color = corSelecionado;
        }
        else
        {
            imagem.color = Color.white;
        }
    }

    private void OnPointerDown(PointerEventData eventData)
    {
        apertado = true;
    }

    private void OnPointerUp(PointerEventData eventData)
    {
        if (apertado == true)
        {
            apertado = false;

            eventoItemSelecionado?.Invoke(this);
        }
    }

    private void OnBeginDrag(PointerEventData eventData)
    {
        apertado = false;

        if (scrollRect != null)
        {
            scrollRect.OnBeginDrag(eventData);
        }
    }

    private void OnDrag(PointerEventData eventData)
    {
        if (scrollRect != null)
        {
            scrollRect.OnDrag(eventData);
        }
    }

    private void OnEndDrag(PointerEventData eventData)
    {
        if (scrollRect != null)
        {
            scrollRect.OnEndDrag(eventData);
        }
    }
}

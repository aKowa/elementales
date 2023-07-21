using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private Image imagem;
    [SerializeField] private TMP_Text nomeItem;
    [SerializeField] private TMP_Text quantidadeItem;

    [Header("Variaveis Padroes")]
    [SerializeField] private Color corSelecionado;

    private ScrollRect scrollRect;

    //Variaveis
    private UnityEvent<ItemSlot> eventoItemSelecionado = new UnityEvent<ItemSlot>();

    private ItemHolder itemHolder;

    private bool apertado;

    //Getters
    public UnityEvent<ItemSlot> EventoItemSelecionado => eventoItemSelecionado;

    public ItemHolder ItemHolder
    {
        get => itemHolder;
        set => itemHolder = value;
    }

    private void Awake()
    {
        //Componentes
        DragAndDropButton dragAndDropButton = GetComponent<DragAndDropButton>();
        HoldButton holdButton = GetComponent<HoldButton>();

        scrollRect = GetComponentInParent<ScrollRect>();

        //Variaveis
        apertado = false;

        //Eventos
        holdButton.OnPointerDownEvent.AddListener(OnPointerDown);
        holdButton.OnPointerUpEvent.AddListener(OnPointerUp);
        dragAndDropButton.OnBeginDragEvent.AddListener(OnBeginDrag);
        dragAndDropButton.OnDragEvent.AddListener(OnDrag);
        dragAndDropButton.OnEndDragEvent.AddListener(OnEndDrag);
    }

    public void AtualizarInformacoes()
    {
        nomeItem.text = itemHolder.Item.Nome;
        quantidadeItem.text = itemHolder.Quantidade.ToString();
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
        if(scrollRect != null)
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

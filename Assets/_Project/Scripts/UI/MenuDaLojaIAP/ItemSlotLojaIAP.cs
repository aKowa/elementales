using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ItemSlotLojaIAP : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private IAPButton iapButton;

    [Space(10)]

    [SerializeField] private TMP_Text textoPreco;
    [SerializeField] private Image imagem;

    private ScrollRect scrollRect;

    //Variaveis
    private UnityEvent<ItemSlotLojaIAP> eventoItemSelecionado = new UnityEvent<ItemSlotLojaIAP>();

    private string tituloProduto;
    private string descricaoProduto;
    private string precoProduto;
    private Sprite imagemProduto;

    //Getters
    public IAPButton IAPButton => iapButton;
    public UnityEvent<ItemSlotLojaIAP> EventoItemSelecionado => eventoItemSelecionado;
    public string TituloProduto => tituloProduto;
    public string DescricaoProduto => descricaoProduto;
    public string PrecoProduto => precoProduto;
    public Sprite ImagemProduto => imagemProduto;

    private void Awake()
    {
        //Componentes
        HoldButton holdButton = GetComponent<HoldButton>();
        DragAndDropButton dragAndDropButton = GetComponent<DragAndDropButton>();

        scrollRect = GetComponentInParent<ScrollRect>();

        //Eventos
        holdButton.OnPointerUpEvent.AddListener(ItemSelecionado);

        dragAndDropButton.OnBeginDragEvent.AddListener(OnBeginDrag);
        dragAndDropButton.OnDragEvent.AddListener(OnDrag);
        dragAndDropButton.OnEndDragEvent.AddListener(OnEndDrag);
    }

    public void Iniciar(string tituloProduto, string descricaoProduto, string precoProduto)
    {
        this.tituloProduto = tituloProduto;
        this.descricaoProduto = descricaoProduto;
        this.precoProduto = precoProduto;

        AtualizarInformacoes();
    }

    public void AtualizarImagem(Sprite imagemProduto)
    {
        this.imagemProduto = imagemProduto;
    }

    private void AtualizarInformacoes()
    {
        textoPreco.text = precoProduto;

        imagem.sprite = imagemProduto;
    }

    private void ItemSelecionado(PointerEventData eventData)
    {
        eventoItemSelecionado?.Invoke(this);
    }

    private void OnBeginDrag(PointerEventData eventData)
    {
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

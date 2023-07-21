using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiceSkinSlot : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private Image imagem;
    [SerializeField] private RawImage diceTexture;
    [SerializeField] private TMP_Text nomeSkin;

    [Header("Variaveis Padroes")]
    [SerializeField] private Color corSelecionado;

    private ScrollRect scrollRect;

    //Variaveis
    private UnityEvent<string> eventoSkinSelecionada = new UnityEvent<string>();

    private string chaveDaSkin;

    private bool apertado;

    //Getters
    public UnityEvent<string> EventoSkinSelecionada => eventoSkinSelecionada;

    public string ChaveDaSkin => chaveDaSkin;

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

    public void Iniciar(string chaveDaSkin, RenderTexture imagemDado)
    {
        this.chaveDaSkin = chaveDaSkin;
        this.diceTexture.texture = imagemDado;

        AtualizarInformacoes();
    }

    public void AtualizarInformacoes()
    {
        nomeSkin.text = chaveDaSkin;
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

            eventoSkinSelecionada?.Invoke(chaveDaSkin);
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

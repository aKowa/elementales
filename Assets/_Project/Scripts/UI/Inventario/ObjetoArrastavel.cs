using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(DragAndDropButton), typeof(CanvasGroup))]
public class ObjetoArrastavel : MonoBehaviour
{
    //Componentes
    private CanvasGroup canvasGroup;

    private Canvas canvas;
    private RectTransform canvasTransform;

    private RectTransform rectTransform;

    //Enums
    private enum TipoArrasto { CentralizarNoMouse, Livre }

    //Variaveis
    private static ObjetoArrastavel objetoSendoArrastado;

    private Vector3 posicaoInicial;
    private bool trocouDePosicao;

    private bool seMovendo;
    private readonly float tempoMovimento = 0.2f;

    [Header("Opcoes")]
    [SerializeField] private bool ativado = true;

    [SerializeField]
    [Tooltip("Faz o objeto voltar a posicao inicial quando for solto e nao for pego por nenhum slot de objeto.")]
    private bool voltarAPosicaoInicialQuandoSolto;

    [SerializeField]
    [Tooltip("Define a maneira como o objeto sera arrastado.")]
    private TipoArrasto tipoDeArrasto;

    [Header("Eventos")]

    [SerializeField] private UnityEvent<PointerEventData> onBeginDragEvent = new UnityEvent<PointerEventData>();
    [SerializeField] private UnityEvent<PointerEventData> onDragEvent = new UnityEvent<PointerEventData>();
    [SerializeField] private UnityEvent<PointerEventData> onEndDragEvent = new UnityEvent<PointerEventData>();

    //Getters
    public static ObjetoArrastavel ObjetoSendoArrastado => objetoSendoArrastado;

    public bool TrocouDePosicao
    {
        get => trocouDePosicao;
        set => trocouDePosicao = value;
    }

    public bool Ativado
    {
        get => ativado;
        set => ativado = value;
    }

    public UnityEvent<PointerEventData> OnBeginDragEvent => onBeginDragEvent;
    public UnityEvent<PointerEventData> OnDragEvent => onDragEvent;
    public UnityEvent<PointerEventData> OnEndDragEvent => onEndDragEvent;

    private void Awake()
    {
        //Componentes
        DragAndDropButton dragAndDropButton = GetComponent<DragAndDropButton>();

        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
        canvasTransform = canvas.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();

        //Variaveis
        posicaoInicial = Vector3.zero;
        trocouDePosicao = false;

        seMovendo = false;

        dragAndDropButton.OnBeginDragEvent.AddListener(OnBeginDrag);
        dragAndDropButton.OnDragEvent.AddListener(OnDrag);
        dragAndDropButton.OnEndDragEvent.AddListener(OnEndDrag);
    }

    private void OnBeginDrag(PointerEventData eventData)
    {
        if (ativado == false || seMovendo == true)
        {
            return;
        }

        objetoSendoArrastado = this;
        canvasGroup.blocksRaycasts = false;
        trocouDePosicao = false;

        posicaoInicial = rectTransform.anchoredPosition;

        onBeginDragEvent?.Invoke(eventData);
    }

    private void OnDrag(PointerEventData eventData)
    {
        if (objetoSendoArrastado == null)
        {
            return;
        }

        switch (tipoDeArrasto)
        {
            case TipoArrasto.CentralizarNoMouse:
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, Input.mousePosition, canvas.worldCamera, out Vector2 localPoint);
                transform.localPosition = localPoint;
                break;

            case TipoArrasto.Livre:
                rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
                break;

        }

        onDragEvent?.Invoke(eventData);
    }

    private void OnEndDrag(PointerEventData eventData)
    {
        if (objetoSendoArrastado == null)
        {
            return;
        }

        canvasGroup.blocksRaycasts = true;

        if (trocouDePosicao == false && voltarAPosicaoInicialQuandoSolto == true)
        {
            seMovendo = true;

            Sequence sequencia = DOTween.Sequence();
            sequencia.SetUpdate(true);

            sequencia.Append(rectTransform.DOAnchorPos(posicaoInicial, tempoMovimento));
            sequencia.AppendCallback(FinalizarMovimento);
        }

        trocouDePosicao = false;

        onEndDragEvent?.Invoke(eventData);

        objetoSendoArrastado = null;
    }

    private void FinalizarMovimento()
    {
        seMovendo = false;
    }
}

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MonstroSlot : MonoBehaviour
{
    //Componentes
    [SerializeField] private MonstroSlotInfo monstroSlotInfo;
    private MenuMonstrosController menuMonstrosController;
    private CanvasGroup canvasGroup;

    //Variaveis
    private int indice;
    private Monster monstro;

    private bool apertado;

    private readonly float tempoMovimento = 0.35f;

    //Getters
    public CanvasGroup CanvasGroup => canvasGroup;

    public MenuMonstrosController MenuMonstrosController
    {
        get => menuMonstrosController;
        set => menuMonstrosController = value;
    }

    public int Indice
    {
        get => indice;
        set => indice = value;
    }

    public Monster Monstro
    {
        get => monstro;
        set => monstro = value;
    }

    private void Awake()
    {
        //Componentes
        ObjetoArrastavel objetoArrastavel = GetComponent<ObjetoArrastavel>();
        HoldButton holdButton = GetComponent<HoldButton>();

        canvasGroup = GetComponent<CanvasGroup>();

        //Variaveis
        apertado = false;

        //Eventos
        holdButton.OnPointerDownEvent.AddListener(OnPointerDown);
        holdButton.OnPointerUpEvent.AddListener(OnPointerUp);
        objetoArrastavel.OnBeginDragEvent.AddListener(OnBeginDrag);
        objetoArrastavel.OnEndDragEvent.AddListener(OnEndDrag);
    }

    public void AtualizarInformacoes()
    {
        monstroSlotInfo.Monstro = monstro;
        monstroSlotInfo.AtualizarInformacoes();
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

            menuMonstrosController.AbrirMenuOpcoes(indice);
        }
    }

    private void OnBeginDrag(PointerEventData eventData)
    {
        apertado = false;

        transform.SetAsLastSibling();

        menuMonstrosController.MonstroSlotsRaycast(false);
    }

    private void OnEndDrag(PointerEventData eventData)
    {
        menuMonstrosController.MonstroSlotsRaycast(true);
    }

    public void MoverAte(Vector3 novaPosicao)
    {
        menuMonstrosController.AdicionarObjetoSeMovendo();

        Sequence sequencia = DOTween.Sequence();
        sequencia.SetUpdate(true);

        sequencia.Append(transform.DOMove(novaPosicao, tempoMovimento));
        sequencia.AppendCallback(FinalizarMovimento);
    }

    private void FinalizarMovimento()
    {
        menuMonstrosController.SubtrairObjetoSeMovendo();
    }
}

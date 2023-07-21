using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CombatLessonSlot : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private TMP_Text nomeCombatLesson;
    [SerializeField] private Image imagem;
    [SerializeField] private Image imagemSelecionado;
    [SerializeField] private Image botaoInfo;

    private ScrollRect scrollRect;

    [Header("Variaveis Padroes")]
    [SerializeField] private Color corSelecionado;
    [SerializeField] private string nomeSlotVazio;

    //Variaveis
    private UnityEvent<CombatLesson> eventoSlotSelecionado = new UnityEvent<CombatLesson>();
    private UnityEvent<CombatLesson> botaoInfoSelecionado = new UnityEvent<CombatLesson>();

    private CombatLesson combatLesson;

    private bool apertado;

    //Getters
    public UnityEvent<CombatLesson> EventoSlotSelecionado => eventoSlotSelecionado;
    public UnityEvent<CombatLesson> BotaoInfoSelecionado => botaoInfoSelecionado;
    public CombatLesson CombatLesson => combatLesson;

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

        if(dragAndDropButton != null)
        {
            dragAndDropButton.OnBeginDragEvent.AddListener(OnBeginDrag);
            dragAndDropButton.OnDragEvent.AddListener(OnDrag);
            dragAndDropButton.OnEndDragEvent.AddListener(OnEndDrag);
        }

        Selecionado(false);
    }

    public void Iniciar(CombatLesson combatLesson)
    {
        this.combatLesson = combatLesson;

        botaoInfo.gameObject.SetActive(true);

        AtualizarInformacoes();
    }

    public void AtualizarInformacoes()
    {
        nomeCombatLesson.text = combatLesson.Nome;
    }

    public void ResetarInformacoes()
    {
        combatLesson = null;

        botaoInfo.gameObject.SetActive(false);

        nomeCombatLesson.text = nomeSlotVazio;
    }

    public void Selecionado(bool selecionado)
    {
        imagemSelecionado.gameObject.SetActive(selecionado);

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

            eventoSlotSelecionado?.Invoke(combatLesson);
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

    public void BotaoInfo()
    {
        botaoInfoSelecionado?.Invoke(combatLesson);
    }
}

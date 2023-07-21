using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MonsterEntrySlot : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private Image miniatura;
    [SerializeField] private Image iconeMonstroCapturado;
    [SerializeField] private TMP_Text nome;
    [SerializeField] private TMP_Text id;

    [Header("Variaveis Padroes")]
    [SerializeField] private Sprite monstroNaoEncontrado;
    [SerializeField] private Sprite monstroCapturado;
    [SerializeField] private Sprite monstroNaoCapturado;
    private string textoMonstroNaoEncontrado = "???";

    private ScrollRect scrollRect;

    //Variaveis
    private UnityEvent<MonsterEntrySlot> eventoSlotSelecionado = new UnityEvent<MonsterEntrySlot>();

    private int monstroID;

    private bool ativo;
    private bool apertado;

    //Getters
    public UnityEvent<MonsterEntrySlot> EventoSlotSelecionado => eventoSlotSelecionado;

    public int MonstroID => monstroID;
    public bool Ativo => ativo;

    private void Awake()
    {
        //Componentes
        DragAndDropButton dragAndDropButton = GetComponent<DragAndDropButton>();
        HoldButton holdButton = GetComponent<HoldButton>();

        scrollRect = GetComponentInParent<ScrollRect>();

        //Variaveis
        ativo = false;
        apertado = false;

        //Eventos
        holdButton.OnPointerDownEvent.AddListener(OnPointerDown);
        holdButton.OnPointerUpEvent.AddListener(OnPointerUp);
        dragAndDropButton.OnBeginDragEvent.AddListener(OnBeginDrag);
        dragAndDropButton.OnDragEvent.AddListener(OnDrag);
        dragAndDropButton.OnEndDragEvent.AddListener(OnEndDrag);
    }

    public void AtualizarInformacoes(MonsterData monsterData)
    {
        monstroID = monsterData.ID;
        id.text = monsterData.MonsterBookID.ToString("D2");

        if (PlayerData.MonsterBook.MonsterEntries[monstroID].WasFound == true)
        {
            ativo = true;

            miniatura.sprite = monsterData.Miniatura;
            nome.text = monsterData.GetName;
        }
        else
        {
            ativo = false;

            miniatura.sprite = monstroNaoEncontrado;
            nome.text = textoMonstroNaoEncontrado;
        }

        SetarImagemMonstroCapturado();
    }

    private void SetarImagemMonstroCapturado()
    {
        if (PlayerData.MonsterBook.MonsterEntries[monstroID].WasCaptured == true)
        {
            iconeMonstroCapturado.sprite = monstroCapturado;
        }
        else
        {
            iconeMonstroCapturado.sprite = monstroNaoCapturado;
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

            eventoSlotSelecionado?.Invoke(this);
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

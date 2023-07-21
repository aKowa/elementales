using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MonstroSlotBag : MonoBehaviour
{
    //Componentes
    [SerializeField] private MonstroSlotInfo monstroSlotInfo;

    private HoldButton holdButton;
    private ButtonSelectionEffect buttonSelectionEffect;

    //Variaveis
    private UnityEvent<MonstroSlotBag> eventoSelecionado = new UnityEvent<MonstroSlotBag>();

    private bool ativado;
    private Monster monstro;

    //Getters
    public UnityEvent<MonstroSlotBag> EventoSelecionado => eventoSelecionado;

    public Monster Monstro
    {
        get => monstro;
        set => monstro = value;
    }

    private void Awake()
    {
        //Componentes
        holdButton = GetComponent<HoldButton>();
        buttonSelectionEffect = GetComponent<ButtonSelectionEffect>();

        //Variaveis
        ativado = false;

        //Eventos
        holdButton.OnPointerUpEvent.AddListener(OnPointerUp);
    }

    public void Ativado(bool novoAtivado)
    {
        ativado = novoAtivado;

        buttonSelectionEffect.interactable = novoAtivado;
    }

    public void AtualizarInformacoes()
    {
        monstroSlotInfo.Monstro = monstro;
        monstroSlotInfo.AtualizarInformacoes();
    }

    private void OnPointerUp(PointerEventData eventData)
    {
        if(ativado == false)
        {
            return;
        }

        eventoSelecionado?.Invoke(this);
    }
}

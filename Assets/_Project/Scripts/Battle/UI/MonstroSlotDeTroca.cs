using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MonstroSlotDeTroca : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private MonstroSlotInfo monstroSlotInfo;
    [SerializeField] private Image imagem;

    [Header("Variaveis Padroes")]
    [SerializeField] private Color corMonstroNoTime;

    private ButtonSelectionEffect buttonSelectionEffect;

    //Enums
    public enum CorSlot { Normal, MonstroNoTime }

    //Variaveis
    private UnityEvent<int> slotSelecionado = new UnityEvent<int>();

    private bool ativado;
    private int indice;

    private Monster monstro;

    //Getters
    public UnityEvent<int> SlotSelecionado => slotSelecionado;
    public bool Ativo => ativado;
    public Monster Monstro => monstro;

    private void Awake()
    {
        //Componentes
        HoldButton holdButton = GetComponent<HoldButton>();

        buttonSelectionEffect = GetComponent<ButtonSelectionEffect>();

        //Eventos
        holdButton.OnPointerUpEvent.AddListener(OnPointerUp);

        SetCor(CorSlot.Normal);
        Ativado(false);
    }

    public void Iniciar(int indice, Monster monstro)
    {
        this.indice = indice;
        this.monstro = monstro;

        AtualizarInformacoes();
    }

    public void SetCor(CorSlot cor)
    {
        switch(cor)
        {
            case CorSlot.Normal:
                imagem.color = Color.white;
                break;

            case CorSlot.MonstroNoTime:
                imagem.color = corMonstroNoTime;
                break;
        }
    }

    public void Ativado(bool ativado)
    {
        this.ativado = ativado;
        buttonSelectionEffect.interactable = ativado;
    }

    public void AtualizarInformacoes()
    {
        monstroSlotInfo.Monstro = monstro;
        monstroSlotInfo.AtualizarInformacoes();
    }

    private void OnPointerUp(PointerEventData eventData)
    {
        if (ativado == true)
        {
            slotSelecionado?.Invoke(indice);
        }
        else
        {
            Debug.Log("Voce nao pode trocar para este monstro!");
        }
    }
}

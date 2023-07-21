using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AtaqueSlotBatalha : MonoBehaviour
{
    //Componentes
    [SerializeField] private TMP_Text nomeAtaque;
    [SerializeField] private TMP_Text textoPP;
    [SerializeField] private TMP_Text textoMaxPP;
    [SerializeField] private TMP_Text textoMana;
    [SerializeField] private TipoLogo tipoAtaque;

    private ButtonSelectionEffect buttonSelectionEffect;

    //Variaveis
    private UnityEvent<int> slotSelecionado = new UnityEvent<int>();
    private UnityEvent<int> botaoInfoSelecionado = new UnityEvent<int>();

    private bool ativado;
    private int indice;
    private AttackHolder attackHolder;

    //Getters
    public UnityEvent<int> SlotSelecionado => slotSelecionado;
    public UnityEvent<int> BotaoInfoSelecionado => botaoInfoSelecionado;
    public AttackHolder AttackHolder => attackHolder;

    private void Awake()
    {
        //Componentes
        HoldButton holdButton = GetComponent<HoldButton>();

        buttonSelectionEffect = GetComponent<ButtonSelectionEffect>();

        //Eventos
        holdButton.OnPointerUpEvent.AddListener(OnPointerUp);

        Ativado(true);
    }

    public void Ativado(bool ativado)
    {
        this.ativado = ativado;
        buttonSelectionEffect.interactable = ativado;
    }

    public void AtualizarInformacoes(int novoIndice, Monster monstro,AttackHolder novoAttackHolder)
    {
        indice = novoIndice;
        attackHolder = novoAttackHolder;

        nomeAtaque.text = attackHolder.Attack.Nome;
        textoPP.text = attackHolder.PP.ToString();
        textoMaxPP.text = attackHolder.Attack.MaxPP.ToString();

        textoMana.text = attackHolder.Attack.CustoMana.ToString();

        tipoAtaque.SetTipo(attackHolder.Attack.AttackData.TipoAtaque);

        if (attackHolder.Attack.ConsomePP)
            Ativado(attackHolder.PP > 0);
        else
        {
            Ativado(monstro.AtributosAtuais.Mana >= attackHolder.Attack.CustoMana);
        }
    }

    public void ResetarInformacoes()
    {
        nomeAtaque.text = string.Empty;
        textoPP.text = string.Empty;
        textoMaxPP.text = string.Empty;
        textoMana.text = string.Empty;
    }

    private void OnPointerUp(PointerEventData eventData)
    {
        if(ativado == true)
        {
            slotSelecionado?.Invoke(indice);
        }
        else
        {
            Debug.Log("Voce nao tem PP/Mana o suficiente para usar este ataque!");
        }
    }

    public void BotaoInfo()
    {
        botaoInfoSelecionado?.Invoke(indice);
    }
}

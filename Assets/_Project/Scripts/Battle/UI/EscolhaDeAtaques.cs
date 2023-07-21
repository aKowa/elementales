using System.Collections.Generic;
using UnityEngine;

public class EscolhaDeAtaques : MonoBehaviour
{
    //Componentes
    [SerializeField] private RectTransform menuInicial;
    [SerializeField] private RectTransform menuAlvos;
    [SerializeField] private RectTransform ataquesHolder;
    [SerializeField] private AtaqueInfo ataqueInfo;

    private BattleUI battleUI;

    //Enums
    public enum Menu { Nenhum, Escolha, Alvo }

    //Variaveis
    private Monster monstroAtual;
    private AttackHolder attackHolderAtual;

    private List<AtaqueSlotBatalha> ataqueSlots = new List<AtaqueSlotBatalha>();

    private bool temAtaqueComPP;

    //Getters
    public AttackHolder AttackHolderAtual => attackHolderAtual;
    public bool TemAtaqueComPP => temAtaqueComPP;

    //Setters

    public void SetMenu(Menu menu)
    {
        switch (menu)
        {
            case Menu.Nenhum:
                menuInicial.gameObject.SetActive(false);
                menuAlvos.gameObject.SetActive(false);
                break;

            case Menu.Escolha:
                menuInicial.gameObject.SetActive(true);
                menuAlvos.gameObject.SetActive(false);
                break;

            case Menu.Alvo:
                menuInicial.gameObject.SetActive(false);
                menuAlvos.gameObject.SetActive(true);
                break;
        }
    }

    private void Awake()
    {
        battleUI = GetComponentInParent<BattleUI>();

        ataqueInfo.gameObject.SetActive(false);

        temAtaqueComPP = false;

        foreach (AtaqueSlotBatalha ataqueSlot in ataquesHolder.GetComponentsInChildren<AtaqueSlotBatalha>(true))
        {
            ataqueSlot.SlotSelecionado.AddListener(AbrirEscolhaDeAlvo);
            ataqueSlot.BotaoInfoSelecionado.AddListener(AbrirAtaqueInfo);

            ataqueSlots.Add(ataqueSlot);
        }

        SetMenu(Menu.Nenhum);
    }

    public void IniciarMenu(Monster monstro)
    {
        monstroAtual = monstro;

        IniciarAtaqueSlots();
    }

    public void FecharMenu()
    {
        SetMenu(Menu.Nenhum);
        ResetarAtaqueSlots();

        battleUI.SetMenu(BattleUI.Menu.Escolha);
    }

    private void IniciarAtaqueSlots()
    {
        temAtaqueComPP = false;

        for (int i = 0; i < ataqueSlots.Count; i++)
        {
            if (i < monstroAtual.Attacks.Count)
            {
                ataqueSlots[i].gameObject.SetActive(true);
                ataqueSlots[i].AtualizarInformacoes(i, monstroAtual, monstroAtual.Attacks[i]);

                if (monstroAtual.Attacks[i].Attack.ConsomePP)
                {
                    if (monstroAtual.Attacks[i].PP > 0)
                    {
                        temAtaqueComPP = true;
                    }
                }
                else
                {
                    if (monstroAtual.AtributosAtuais.Mana >= monstroAtual.Attacks[i].Attack.CustoMana)
                    {
                        temAtaqueComPP = true;
                    }
                }

            }
            else
            {
                ataqueSlots[i].gameObject.SetActive(false);
                ataqueSlots[i].ResetarInformacoes();
            }
        }

        if (temAtaqueComPP == false)
        {
            AbrirEscolhaDeAlvoComStruggle();
        }
    }

    private void ResetarAtaqueSlots()
    {
        for (int i = 0; i < ataqueSlots.Count; i++)
        {
            ataqueSlots[i].gameObject.SetActive(false);
            ataqueSlots[i].ResetarInformacoes();
        }
    }

    private void AbrirAtaqueInfo(int indice)
    {
        ataqueInfo.gameObject.SetActive(true);
        ataqueInfo.AtualizarInformacoes(monstroAtual.Attacks[indice].Attack);
    }

    public void FecharAtaqueInfo()
    {
        ataqueInfo.gameObject.SetActive(false);
    }

    private void AbrirEscolhaDeAlvo(int indice)
    {
        attackHolderAtual = monstroAtual.Attacks[indice];

        int indiceTemp = 0;
        for (int i = 0; i < battleUI.IntegranteAtual.MonstrosAtuais.Count; i++)
        {
            if (battleUI.IntegranteAtual.MonstrosAtuais[i].GetMonstro == monstroAtual)
            {
                indiceTemp = i;
                break;
            }
        }

        DeterminarTipoTarget(indiceTemp, indice);
    }

    public void AbrirEscolhaDeAlvoComStruggle()
    {
        attackHolderAtual = new AttackHolder(BattleManager.Instance.Struggle);

        int indiceMonstroAtual = 0;
        for (int i = 0; i < battleUI.IntegranteAtual.MonstrosAtuais.Count; i++)
        {
            if (battleUI.IntegranteAtual.MonstrosAtuais[i].GetMonstro == monstroAtual)
            {
                indiceMonstroAtual = i;
                break;
            }
        }

        DeterminarTipoTarget(indiceMonstroAtual, 0);
    }

    private void IniciarAlvos()
    {
        foreach (Integrante integrante in BattleManager.Instance.Integrantes)
        {
            foreach (Integrante.MonstroAtual monstroAtual in integrante.MonstrosAtuais)
            {
                monstroAtual.MonstroSlotBattle.Ativar();
            }
        }
    }

    public void FecharMenuDosAlvos()
    {
        battleUI.DesativarMonstroSlots();

        if (temAtaqueComPP == true)
        {
            SetMenu(Menu.Escolha);
        }
        else
        {
            FecharMenu();
        }
    }

    void DeterminarTipoTarget(int indiceMonstroAtual, int indiceAtaque)
    {
        if (BattleManager.Instance.QuantidadeDeMonstrosPorTime < 2)
        {
            battleUI.PassarComandoAtaque(EscolherTargetAtaque.DeterminarTargerSingular(indiceMonstroAtual, attackHolderAtual, battleUI.IntegranteAtual, monstroAtual));
        }
        else
        {
            switch (attackHolderAtual.Attack.Target)
            {
                case Comando.TipoTarget.Aliado:
                    foreach (var integrante in BattleManager.Instance.Integrantes)
                    {
                        if (battleUI.IntegranteAtual == integrante)
                        {
                            foreach (var monstros in integrante.MonstrosAtuais)
                            {
                                monstros.MonstroSlotBattle.Ativar();
                            }
                            SetMenu(Menu.Alvo);
                        }
                    }
                    break;

                case Comando.TipoTarget.Inimigo:
                    foreach (var integrante in BattleManager.Instance.Integrantes)
                    {
                        if (battleUI.IntegranteAtual != integrante)
                        {
                            foreach (var monstros in integrante.MonstrosAtuais)
                            {
                                monstros.MonstroSlotBattle.Ativar();
                            }
                            SetMenu(Menu.Alvo);
                        }
                    }
                    break;
                default:
                    battleUI.PassarComandoAtaque(EscolherTargetAtaque.DeterminarTargetDupla(indiceMonstroAtual, attackHolderAtual, battleUI.IntegranteAtual, monstroAtual));
                    break;
            }
        }
    }
}

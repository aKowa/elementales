using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBagBatalhaController : MenuBagController
{
    //Componentes
    private BattleUI battleUI;

    //Variaveis
    private bool jaFezOComando;
    private ItemHolder itemAtual;
    private int indiceMonstroAtual;

    protected override void OnAwake()
    {
        base.OnAwake();

        battleUI = GetComponentInParent<BattleUI>();

        jaFezOComando = false;
        itemAtual = null;
        indiceMonstroAtual = 0;
    }

    public override void OnOpen()
    {
        base.OnOpen();

        jaFezOComando = false;
        itemAtual = null;
    }

    protected override void OnStartCloseAnimation()
    {
        fundoBloqueadorDeAcoes.gameObject.SetActive(true);
        battleUI.AtualizarInformacoesDosSlots();
    }

    protected override void OnClose()
    {
        base.OnClose();

        if(jaFezOComando == true)
        {
            StartCoroutine(FecharMenu());
        }
    }

    protected override void SetarAtivacaoEscolhaDeMonstros()
    {
        foreach (MonstroSlotBag slot in monstroSlots)
        {
            slot.Ativado(itemSlotAtual.ItemHolder.Item.EfeitoNaBatalha.PodeUsarItemNoMonstro(slot.Monstro));
        }
    }

    public override void UsarItemAtual()
    {
        Item item = itemSlotAtual.ItemHolder.Item;

        if (item.EfeitoNaBatalha == null)
        {
            AbrirDialogo(dialogoNaoPodeUsarItem);
        }
        else
        {
            item.EfeitoNaBatalha.UsarItem(this, item);
            AtualizarItemInfo();

            if (itemSlotAtual == null)
            {
                FecharMenuOpcoes();
            }
            else if (itemSlotAtual.ItemHolder.Quantidade <= 0)
            {
                FecharMenuOpcoes();
            }
        }
    }

    protected override void UsarItemNoMonstro(MonstroSlotBag monstroSlot)
    {
        Item item = itemSlotAtual.ItemHolder.Item;

        monstroSlotAtual = monstroSlot;

        if (item.ComandoNaBatalha != null)
        {
            for (int i = 0; i < battleUI.IntegranteAtual.MonstrosAtuais.Count; i++)
            {
                if (battleUI.IntegranteAtual.MonstrosAtuais[i].GetMonstro == monstroSlot.Monstro)
                {
                    UsarItemNoMonstroNaBatalha();
                    return;
                }
            }
        }

        item.EfeitoNaBatalha.UsarItemNoMonstro(this, monstroSlotAtual.Monstro, item);
        AtualizarItemInfo();

        item.EfeitoNaBatalha.EfeitoMonstroSlot(this);
    }

    protected override void TerminouOEfeitoMonstroSlot()
    {
        jaFezOComando = true;
        itemAtual = null;

        CloseView();
    }

    public void UsarItemNaBatalha()
    {
        jaFezOComando = true;
        itemAtual = itemSlotAtual.ItemHolder;

        indiceMonstroAtual = battleUI.IndiceMonstroAtual;

        if (itemAtual.Item.Tipo == Item.TipoItem.Consumivel || itemAtual.Item.Tipo == Item.TipoItem.MonsterBall)
        {
            RemoveItem(itemAtual.Item);
        }

        CloseView();
    }

    public void UsarItemNoMonstroNaBatalha()
    {
        jaFezOComando = true;
        itemAtual = itemSlotAtual.ItemHolder;

        for (int i = 0; i < battleUI.IntegranteAtual.MonstrosAtuais.Count; i++)
        {
            if (battleUI.IntegranteAtual.MonstrosAtuais[i].GetMonstro == monstroSlotAtual.Monstro)
            {
                indiceMonstroAtual = i;
                break;
            }
        }

        if (itemAtual.Item.Tipo == Item.TipoItem.Consumivel || itemAtual.Item.Tipo == Item.TipoItem.MonsterBall)
        {
            RemoveItem(itemAtual.Item);
        }

        CloseView();
    }

    private IEnumerator FecharMenu()
    {
        battleUI.SetMenu(BattleUI.Menu.Nenhum);

        yield return BattleManager.Instance.TrocarMonstrosAtuaisMortosDoJogador(battleUI.IntegranteAtual);

        battleUI.EscolheuItemParaUsar(itemAtual, indiceMonstroAtual);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AcaoNaBatalha : ScriptableObject
{
    public abstract void Executar(BattleManager battleManager, Comando comando);

    public virtual void IniciarAnimacao(BattleManager battleManager, Comando comando)
    {
        battleManager.SetarAnimacao();
    }

    public virtual void DialogoComando(BattleManager battleManager, Comando comando)
    {
        battleManager.DialogoUsouComando(comando);
    }

    public virtual void DialogoComando(BattleManager battleManager, ComandoDeAtaque comando)
    {
        if (comando.TurnosParaSerExecutado <= 0)
        {
            DialogoComando(battleManager, (Comando)comando);
        }
        else
        {
            battleManager.DialogoUsouAtaqueProxTurno(comando);
        }
    }

    protected void ExcluirItem(BattleManager battleManager, Comando comando)
    {
        if (comando is ComandoDeItem)
        {
            ComandoDeItem comandoDeItem = (ComandoDeItem)comando;

            battleManager.ExcluirItem(comandoDeItem.Origem, comandoDeItem.ItemHolder);
        }
    }
}

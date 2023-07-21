using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Batalha/Outros/Trocar Monstro")]
public class TrocarMonstro : AcaoNaBatalha
{
    public override void Executar(BattleManager battleManager, Comando comando)
    {
        ComandoTrocar comandoTrocar = (ComandoTrocar)comando;

        MonsterInBattle m1 = comandoTrocar.GetMonstroInBattle;
        MonsterInBattle m2 = comandoTrocar.MonstroParaTrocar;

        bool trocaValida = true;
        foreach (StatusEffectSecundario statusSecundario in comandoTrocar.GetMonstro.StatusSecundario)//Verificar caso o monstro origem não possui status de trancado
        {
            if(statusSecundario.GetTipoStatus == StatusEffectSecundario.TipoStatus.Locked)
            {
                DialogoComando(battleManager, comando);
                trocaValida = false;
            }
        }
        if (trocaValida)
        {
            for (int i = 0; i < comando.Origem.ListaMonstros.Count; i++)
            {
                if (comando.Origem.ListaMonstros[i].GetMonstro == comando.GetMonstro)
                {
                    battleManager.TrocarMonstro(m1, m2, comando.Origem, comandoTrocar.OrigemIndice, comandoTrocar.TrocaIndice, i, comando.IndiceMonstro);
                    break;
                }
            }
        }

        comando.PodeMeRetirar = true;
    }

    public override void DialogoComando(BattleManager battleManager, Comando comando)
    {
        ComandoTrocar comandoTrocar = (ComandoTrocar)comando;
        battleManager.DialogoMonstroComStatusTrancado(comandoTrocar);
        //Nada
    }
}

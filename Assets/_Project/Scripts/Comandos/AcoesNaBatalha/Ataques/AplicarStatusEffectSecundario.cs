using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Batalha/Ataques/Aplicar Status Secundario")]
public class AplicarStatusEffectSecundario : AcaoNaBatalha
{
    [SerializeField] private List<StatusEffectSecundarioParaAplicar> status = new List<StatusEffectSecundarioParaAplicar>();
    public override void Executar(BattleManager battleManager, Comando comando)
    {
        ComandoDeAtaque comandoDeAtaque = (ComandoDeAtaque)comando;

        for (int i = 0; i < comandoDeAtaque.AlvoAcao.Count; i++)
        {
            if (comandoDeAtaque.AlvoComAtaquesValidos[i] == false)
            {
                comandoDeAtaque.AlvoAcao[i].Monstro.ForcarMiss(comandoDeAtaque.AlvoAcao[i], true);
            }
            else
            {
                foreach (StatusEffectSecundarioParaAplicar status in status)
                {
                    if (Random.Range(0, 100f) <= status.GetPorcentagem)
                    {
                        comandoDeAtaque.AlvoAcao[i].Monstro.AplicarStatusSecundario(comandoDeAtaque.AlvoAcao[i], status.GetStatus);
                    }
                }
            }
        }
        if (comandoDeAtaque.NumeroRoundsComandoVivo <= 0)
        {
            comandoDeAtaque.PodeMeRetirar = true;
        }
        else
        {
            comandoDeAtaque.NumeroRoundsComandoVivo--;
        }
    }
}

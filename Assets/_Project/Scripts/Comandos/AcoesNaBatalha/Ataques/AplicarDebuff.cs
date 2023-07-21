using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Batalha/Ataques/Debuff")]

public class AplicarDebuff : AcaoNaBatalha
{
    //Variaveis
    [SerializeField] private List<StatusEffectDebufAtributo> atributosDebuff;
    [SerializeField] private bool passaComTempo;
    [SerializeField] private int numeroRounds;

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
                for (int j = 0; j < atributosDebuff.Count; j++)
                {
                    comandoDeAtaque.AlvoAcao[i].Monstro.TomarAtaqueAtributo(comandoDeAtaque.AlvoAcao[i], atributosDebuff[j], passaComTempo, numeroRounds);
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
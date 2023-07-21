using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Batalha/Ataques/Debuff Aleatorio")]
public class AplicarRandomDebuffDaLista : AcaoNaBatalha
{
    [SerializeField] private List<StatusEffectDebufAtributo> atributosDebuff;
    [SerializeField] private bool passaComTempo;
    [SerializeField] private int numeroRounds;

    public override void Executar(BattleManager battleManager, Comando comando)
    {
        ComandoDeAtaque combatLesson = (ComandoDeAtaque)comando;

        for (int i = 0; i < combatLesson.AlvoAcao.Count; i++)
        {
            if (combatLesson.AlvoComAtaquesValidos[i] == false)
            {
                combatLesson.AlvoAcao[i].Monstro.ForcarMiss(combatLesson.AlvoAcao[i], true);
            }
            else
            {
                StatusEffectDebufAtributo randomDebuff = atributosDebuff[Random.Range(0, atributosDebuff.Count)];
                combatLesson.AlvoAcao[i].Monstro.TomarAtaqueAtributo(combatLesson.AlvoAcao[i], randomDebuff, passaComTempo, numeroRounds);
            }
        }
        if (combatLesson.NumeroRoundsComandoVivo <= 0)
        {
            combatLesson.PodeMeRetirar = true;
        }
        else
        {
            combatLesson.NumeroRoundsComandoVivo--;
        }
    }
}
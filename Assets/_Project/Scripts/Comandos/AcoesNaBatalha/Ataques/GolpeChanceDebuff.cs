using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Batalha/Ataques/Golpe Com Chance de Debuff")]
public class GolpeChanceDebuff : AcaoNaBatalha
{
    [SerializeField] private List<StatusEffectDebufAtributo> atributosDebuff;
    [SerializeField] private int porcentagemFixa;
    [SerializeField] private bool passaComTempo;
    [SerializeField] private int numeroRounds;

    public override void Executar(BattleManager battleManager, Comando comando)
    {
        ComandoDeAtaque comandoDeAtaque = (ComandoDeAtaque)comando;
        int atributoAtaque;

        if (comandoDeAtaque.AttackData.Categoria == AttackData.CategoriaEnum.Fisico)
        {
            atributoAtaque = comandoDeAtaque.GetMonstro.AtributosAtuais.AtaqueComModificador;
        }
        else
        {
            atributoAtaque = comandoDeAtaque.GetMonstro.AtributosAtuais.SpAtaqueComModificador;
        }


        for (int i = 0; i < comandoDeAtaque.AlvoAcao.Count; i++)
        {
            if (comandoDeAtaque.AlvoComAtaquesValidos[i] == false)
            {
                comandoDeAtaque.AlvoAcao[i].Monstro.ForcarMiss(comandoDeAtaque.AlvoAcao[i], true);
            }
            else
            {
                (float dano, bool acertou) = comandoDeAtaque.AlvoAcao[i].Monstro.TomarAtaque(atributoAtaque, comandoDeAtaque, comandoDeAtaque.AlvoAcao[i], true, true, true);
                if (acertou && Random.Range(0, 100f) <= porcentagemFixa)
                {
                    for (int j = 0; j < atributosDebuff.Count; j++)
                    {
                        comandoDeAtaque.AlvoAcao[i].Monstro.TomarAtaqueAtributo(comandoDeAtaque.AlvoAcao[i], atributosDebuff[j], passaComTempo, numeroRounds);
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
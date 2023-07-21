using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Batalha/Ataques/Golpe Unico com modificador/modificador para self")]

public class GolpeUnicoComBuffDebuffSelf : AcaoNaBatalha
{
    [SerializeField] private bool aplicarDebuffSomenteAcertandoAtaque;
    [SerializeField] private List<StatusEffectDebufAtributo> atributosDebuff;
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
                if (aplicarDebuffSomenteAcertandoAtaque)
                {
                    if (acertou)
                    {
                        for (int j = 0; j < atributosDebuff.Count; j++)
                        {
                            for (int k = 0; k < comandoDeAtaque.Origem.MonstrosAtuais.Count; k++)
                            {
                                if(comandoDeAtaque.Origem.MonstrosAtuais[k].GetMonstro == comandoDeAtaque.GetMonstro)
                                {
                                    comandoDeAtaque.Origem.MonstrosAtuais[k].Monstro.TomarAtaqueAtributo(comandoDeAtaque.Origem.MonstrosAtuais[k], atributosDebuff[j], passaComTempo, numeroRounds);
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < atributosDebuff.Count; j++)
                    {
                        for (int k = 0; k < comandoDeAtaque.Origem.MonstrosAtuais.Count; k++)
                        {
                            if (comandoDeAtaque.Origem.MonstrosAtuais[k].GetMonstro == comandoDeAtaque.GetMonstro)
                            {
                                comandoDeAtaque.Origem.MonstrosAtuais[k].Monstro.TomarAtaqueAtributo(comandoDeAtaque.Origem.MonstrosAtuais[k], atributosDebuff[j], passaComTempo, numeroRounds);
                            }
                        }
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


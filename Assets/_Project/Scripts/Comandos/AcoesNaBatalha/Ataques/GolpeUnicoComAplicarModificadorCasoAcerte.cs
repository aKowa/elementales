using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Batalha/Ataques/Golpe Unico com modificador/modificador para alvo")]

public class GolpeUnicoComAplicarModificadorCasoAcerte : AcaoNaBatalha
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
                (float dano, bool acertou) = comandoDeAtaque.AlvoAcao[i].Monstro.TomarAtaque(atributoAtaque, comandoDeAtaque, comandoDeAtaque.AlvoAcao[i], true, true, false);
                if (aplicarDebuffSomenteAcertandoAtaque)
                {
                    if (acertou)
                    {
                        for (int j = 0; j < atributosDebuff.Count; j++)
                        {
                            comandoDeAtaque.AlvoAcao[i].Monstro.TomarAtaqueAtributo(comandoDeAtaque.AlvoAcao[i], atributosDebuff[j], passaComTempo, numeroRounds);
                        }
                    }
                }
                else
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


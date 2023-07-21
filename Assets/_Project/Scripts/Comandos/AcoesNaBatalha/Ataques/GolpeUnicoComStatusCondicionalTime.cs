using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Batalha/Ataques/Golpe Unico Com Status Condicional Ao time do alvo")]

public class GolpeUnicoComStatusCondicionalTime : AcaoNaBatalha
{
    [SerializeField] private bool statusEmAliados;
    [SerializeField] private bool statusEmInimigos;
    [SerializeField] private List<StatusEffectParaAplicar> status;

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

                if (statusEmAliados || statusEmInimigos && status.Count > 0)
                {
                    foreach (var integrante in battleManager.Integrantes)
                    {
                        foreach (var monstro in integrante.ListaMonstros)
                        {
                            if (monstro.GetMonstro == comandoDeAtaque.AlvoAcao[i].GetMonstro)
                            {
                                if (integrante.Time == comandoDeAtaque.Origem.Time) // caso time do alvo seja o mesmo que time da ação
                                {
                                    if (statusEmAliados)
                                    {
                                        foreach (StatusEffectParaAplicar status in status)
                                        {
                                            comandoDeAtaque.AlvoAcao[i].Monstro.TomarAtaqueStatusEffect(status, atributoAtaque, comandoDeAtaque, comandoDeAtaque.AlvoAcao[i]);

                                        }
                                    }
                                }
                                else
                                {
                                    if (statusEmInimigos)
                                    {
                                        foreach (StatusEffectParaAplicar status in status)
                                        {
                                            comandoDeAtaque.AlvoAcao[i].Monstro.TomarAtaqueStatusEffect(status, atributoAtaque, comandoDeAtaque, comandoDeAtaque.AlvoAcao[i]);

                                        }
                                    }
                                }
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

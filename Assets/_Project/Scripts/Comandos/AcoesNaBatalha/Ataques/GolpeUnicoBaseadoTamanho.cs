using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Batalha/Ataques/Golpe Unico Tamanho")]

public class GolpeUnicoBaseadoTamanho : AcaoNaBatalha
{
    private enum ComparacaoTamanho { MaiorQue, MenorQue };
    //Variaveis
    [SerializeField] private ComparacaoTamanho comparacaoTamanho;
    [SerializeField] private List<StatusEffectSecundarioParaAplicar> statusEffectParaAplicar;
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
                if (acertou)
                {
                    if (comparacaoTamanho == ComparacaoTamanho.MaiorQue)
                    {
                        if (comandoDeAtaque.GetMonstro.MonsterData.Altura >= comandoDeAtaque.AlvoAcao[i].GetMonstro.MonsterData.Altura)
                        {
                            AplicarStatusDebuffs(comandoDeAtaque.AlvoAcao[i]);
                            foreach (StatusEffectSecundarioParaAplicar status in statusEffectParaAplicar)
                            {
                                if (Random.Range(0, 100f) <= status.GetPorcentagem)
                                {
                                    comandoDeAtaque.AlvoAcao[i].Monstro.AplicarStatusSecundario(comandoDeAtaque.AlvoAcao[i], status.GetStatus);
                                }
                            }
                            Debug.Log("ROle do maior la");
                        }
                    }
                    if (comparacaoTamanho == ComparacaoTamanho.MenorQue)
                    {
                        if (comandoDeAtaque.GetMonstro.MonsterData.Altura <= comandoDeAtaque.AlvoAcao[i].GetMonstro.MonsterData.Altura)
                        {
                            AplicarStatusDebuffs(comandoDeAtaque.AlvoAcao[i]);
                            foreach (StatusEffectSecundarioParaAplicar status in statusEffectParaAplicar)
                            {
                                if (Random.Range(0, 100f) <= status.GetPorcentagem)
                                {
                                    comandoDeAtaque.AlvoAcao[i].Monstro.AplicarStatusSecundario(comandoDeAtaque.AlvoAcao[i], status.GetStatus);
                                }
                            }
                            Debug.Log("ROle do menor la");
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

    void AplicarStatusDebuffs(Integrante.MonstroAtual monstro)
    {
        for (int i = 0; i < atributosDebuff.Count; i++)
        {
            monstro.Monstro.TomarAtaqueAtributo(monstro, atributosDebuff[i], passaComTempo, numeroRounds);
        }
    }
}
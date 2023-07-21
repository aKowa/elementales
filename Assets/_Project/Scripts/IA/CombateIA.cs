using System.Collections.Generic;
using UnityEngine;

public static class CombateIA
{
    public static Comando EscolherTrocarMonstro(ref List<int> indicesTrocasDisponiveis, int indiceIntegranteAtual, int indiceMonstroAtualTrocarMonstro, List<Integrante> integrantes)
    {
        int indiceTrocar = Random.Range(0, indicesTrocasDisponiveis.Count);
        Comando comando = BattleManager.Instance.PlayerEscolheuTroca(integrantes[indiceIntegranteAtual], indicesTrocasDisponiveis[indiceTrocar], indiceMonstroAtualTrocarMonstro);
        indicesTrocasDisponiveis.RemoveAt(indiceTrocar);
        return comando;
    }
    public static Comando EscolherAtaque(List<Integrante> integrantes, int indiceIntegranteAtual, int indiceMonstroAtual, int indiceIntegranteAlvo, int quantidadeMonstros)
    {
        List<int> monstrosTargetVivos = new List<int>();
        for (int i = 0; i < integrantes[indiceIntegranteAlvo].MonstrosAtuais.Count; i++)
        {
            if (integrantes[indiceIntegranteAlvo].MonstrosAtuais[i].GetMonstro.IsFainted == false)
            {
                monstrosTargetVivos.Add(i);
            }
        }
        int temp = Random.Range(0, monstrosTargetVivos.Count);

        int indiceMonstroAtualAlvo = monstrosTargetVivos[temp];


        List<AttackHolder> ataquesValidos = new List<AttackHolder>();
        foreach (AttackHolder ataque in integrantes[indiceIntegranteAtual].MonstrosAtuais[indiceMonstroAtual].GetMonstro.Attacks)
        {
            if (ataque.Attack.ConsomePP)//Usando PP
            {
                if (ataque.PP > 0)
                    ataquesValidos.Add(ataque);
            }
            else //Consome Energia
            {
                if (integrantes[indiceIntegranteAtual].MonstrosAtuais[indiceMonstroAtual].GetMonstro.AtributosAtuais.Mana >= ataque.Attack.CustoMana)
                    ataquesValidos.Add(ataque);
            }
        }
        if (ataquesValidos.Count <= 0) //Caso n�o possua nenhumAtaqueValido
        {
            return BattleManager.Instance.ChooseAttackNoPP(integrantes[indiceIntegranteAtual], indiceIntegranteAlvo, indiceMonstroAtual, indiceMonstroAtualAlvo);
        }

        AttackHolder attackHolder = ataquesValidos[Random.Range(0, ataquesValidos.Count)];
        Comando comando = EscolherTargetAtaque.DeterminarTargetDupla(indiceMonstroAtual, attackHolder,
        integrantes[indiceIntegranteAtual], integrantes[indiceIntegranteAtual].MonstrosAtuais[indiceMonstroAtual].GetMonstro);

        if (attackHolder.Attack.Target == Comando.TipoTarget.Aliado)
        {
            (int indiceIntegranteAtualTemp, int indiceMonstroAtualTemp) = DeterminarTargetAliado(integrantes, indiceIntegranteAtual, indiceMonstroAtual);
            return BattleManager.Instance.PlayerEscolheuAtaque(attackHolder, indiceMonstroAtual, integrantes[indiceIntegranteAtual], indiceIntegranteAtualTemp, indiceMonstroAtualTemp);
        }
        if (comando == null)
        {
            if (integrantes[indiceIntegranteAlvo].MonstrosAtuais[indiceMonstroAtualAlvo].GetMonstro.IsFainted == true)
                return null;

            return BattleManager.Instance.PlayerEscolheuAtaque(attackHolder, indiceMonstroAtual, integrantes[indiceIntegranteAtual], indiceIntegranteAlvo, indiceMonstroAtualAlvo);
        }
        return comando;

    }

    private static (int n1, int n2) DeterminarTargetAliado(List<Integrante> integrantes, int integranteAtual, int indiceMonstroAtual) // Retorna o integrante atual e o monstro alvo
    {
        for (int i = 0; i < integrantes[integranteAtual].MonstrosAtuais.Count; i++)
        {
            if (i != indiceMonstroAtual)
            {
                return (integranteAtual, i);
            }
        }
        return (integranteAtual, indiceMonstroAtual);
    }
}

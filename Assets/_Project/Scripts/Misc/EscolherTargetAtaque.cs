using System.Collections.Generic;
using UnityEngine;

public class EscolherTargetAtaque : MonoBehaviour
{
    public static Comando DeterminarTargetDupla(int _indiceMonstroAtual, AttackHolder _attackHolderAtual, Integrante _IntegranteAtual, Monster _monstroAtual)
    {
        List<int> indIntegrante = new List<int>();
        List<int> indMonstro = new List<int>();
        switch (_attackHolderAtual.Attack.Target)
        {
            case Comando.TipoTarget.Self:
                for (int indiceIntegrante = 0; indiceIntegrante < BattleManager.Instance.Integrantes.Count; indiceIntegrante++)
                {
                    for (int indiceMonstroAtual = 0; indiceMonstroAtual < BattleManager.Instance.Integrantes[indiceIntegrante].MonstrosAtuais.Count; indiceMonstroAtual++)
                    {
                        if (_monstroAtual == BattleManager.Instance.Integrantes[indiceIntegrante].MonstrosAtuais[indiceMonstroAtual].GetMonstro)
                        {
                            if (BattleManager.Instance.Integrantes[indiceIntegrante].MonstrosAtuais[indiceMonstroAtual].GetMonstro.IsFainted == false)
                            {
                                indIntegrante.Add(indiceIntegrante);
                                indMonstro.Add(indiceMonstroAtual);
                            }
                        }
                    }
                }
                break;
            case Comando.TipoTarget.TimeAliado:
                for (int indiceIntegrante = 0; indiceIntegrante < BattleManager.Instance.Integrantes.Count; indiceIntegrante++)
                {
                    for (int indiceMonstroAtual = 0; indiceMonstroAtual < BattleManager.Instance.Integrantes[indiceIntegrante].MonstrosAtuais.Count; indiceMonstroAtual++)
                    {
                        if (_IntegranteAtual == BattleManager.Instance.Integrantes[indiceIntegrante])
                        {
                            if (BattleManager.Instance.Integrantes[indiceIntegrante].MonstrosAtuais[indiceMonstroAtual].GetMonstro.IsFainted == false)
                            {
                                indIntegrante.Add(indiceIntegrante);
                                indMonstro.Add(indiceMonstroAtual);
                            }
                        }
                    }
                }
                break;
            case Comando.TipoTarget.TimeInimigo:
                for (int indiceIntegrante = 0; indiceIntegrante < BattleManager.Instance.Integrantes.Count; indiceIntegrante++)
                {
                    for (int indiceMonstroAtual = 0; indiceMonstroAtual < BattleManager.Instance.Integrantes[indiceIntegrante].MonstrosAtuais.Count; indiceMonstroAtual++)
                    {
                        if (_IntegranteAtual != BattleManager.Instance.Integrantes[indiceIntegrante])
                        {
                            if (BattleManager.Instance.Integrantes[indiceIntegrante].MonstrosAtuais[indiceMonstroAtual].GetMonstro.IsFainted == false)
                            {
                                indIntegrante.Add(indiceIntegrante);
                                indMonstro.Add(indiceMonstroAtual);
                            }
                        }
                    }
                }
                break;
            case Comando.TipoTarget.TodosExcetoSelf:
                for (int indiceIntegrante = 0; indiceIntegrante < BattleManager.Instance.Integrantes.Count; indiceIntegrante++)
                {
                    for (int indiceMonstroAtual = 0; indiceMonstroAtual < BattleManager.Instance.Integrantes[indiceIntegrante].MonstrosAtuais.Count; indiceMonstroAtual++)
                    {
                        if (_monstroAtual != BattleManager.Instance.Integrantes[indiceIntegrante].MonstrosAtuais[indiceMonstroAtual].GetMonstro)
                        {
                            if (BattleManager.Instance.Integrantes[indiceIntegrante].MonstrosAtuais[indiceMonstroAtual].GetMonstro.IsFainted == false)
                            {
                                indIntegrante.Add(indiceIntegrante);
                                indMonstro.Add(indiceMonstroAtual);
                            }
                        }
                    }
                }
                break;
            case Comando.TipoTarget.Aleatorio:
                for (int indiceIntegrante = 0; indiceIntegrante < BattleManager.Instance.Integrantes.Count; indiceIntegrante++)
                {
                    for (int indiceMonstroAtual = 0; indiceMonstroAtual < BattleManager.Instance.Integrantes[indiceIntegrante].MonstrosAtuais.Count; indiceMonstroAtual++)
                    {
                        if (_IntegranteAtual != BattleManager.Instance.Integrantes[indiceIntegrante])
                        {
                            if (BattleManager.Instance.Integrantes[indiceIntegrante].MonstrosAtuais[indiceMonstroAtual].GetMonstro.IsFainted == false)
                            {
                                indIntegrante.Add(indiceIntegrante);
                                indMonstro.Add(indiceMonstroAtual);

                            }
                        }
                    }
                }
                int rnd = Random.Range(0, indMonstro.Count);
                int int1 = indIntegrante[rnd];
                int int2 = indMonstro[rnd];
                indIntegrante.Clear();
                indMonstro.Clear();
                indIntegrante.Add(int1);
                indMonstro.Add(int2);
                break;
            default:
                return null;
        }


        return PassarComandoAtaque(_indiceMonstroAtual, indIntegrante, indMonstro, _attackHolderAtual, _IntegranteAtual);
    }
    public static Comando DeterminarTargerSingular(int _indiceMonstroAtual, AttackHolder _attackHolderAtual, Integrante _IntegranteAtual, Monster _monstroAtual)
    {
        List<int> indIntegrante = new List<int>();
        List<int> indMonstro = new List<int>();

        if (_attackHolderAtual.Attack.Target == Comando.TipoTarget.Self || _attackHolderAtual.Attack.Target == Comando.TipoTarget.Aliado || _attackHolderAtual.Attack.Target == Comando.TipoTarget.TimeAliado)
        {
            for (int i = 0; i < BattleManager.Instance.Integrantes.Count; i++)
            {
                if (BattleManager.Instance.Integrantes[i] == _IntegranteAtual)
                {
                    indIntegrante.Add(i);
                    indMonstro.Add(_indiceMonstroAtual);
                }
            }
        }
        else
        {
            for (int i = 0; i < BattleManager.Instance.Integrantes.Count; i++)
            {
                if (BattleManager.Instance.Integrantes[i] != _IntegranteAtual)
                {
                    indIntegrante.Add(i);
                    indMonstro.Add(_indiceMonstroAtual);
                }
            }
        }
        return PassarComandoAtaque(_indiceMonstroAtual, indIntegrante, indMonstro, _attackHolderAtual, _IntegranteAtual);

    }

    static Comando PassarComandoAtaque(int indiceMonstroAtual, List<int> indiceIntegranteAlvo, List<int> indiceMonstroAlvo, AttackHolder attackHolder, Integrante integranteAtual)
    {
        return BattleManager.Instance.PlayerEscolheuAtaque(attackHolder, indiceMonstroAtual, integranteAtual, indiceIntegranteAlvo, indiceMonstroAlvo);
    }
}

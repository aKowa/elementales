using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Batalha/Ataques/Golpe Unico Com AplicarStatusSecundarioCondicionalAoTipoDoAlvo")]

public class GolpeUnicoComAplicarStatusSecundarioCondicionalAoTipoDoAlvo : AcaoNaBatalha
{
    [SerializeField] private List<MonsterType> tipos = new List<MonsterType>();
    [SerializeField] private List<StatusEffectSecundarioParaAplicar> status=new List<StatusEffectSecundarioParaAplicar>();

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
                    if (tipos.Count > 0)
                    {
                        foreach (var tipoMonstroAlvo in comandoDeAtaque.AlvoAcao[i].Monstro.GetMonstro.MonsterData.GetMonsterTypes)
                        {
                            foreach (var tipoMonstroLista in tipos)
                            {
                                if (tipoMonstroAlvo == tipoMonstroLista)
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

                        }
                    }
                    foreach (StatusEffectSecundarioParaAplicar status in status)
                    {
                        if (Random.Range(0, 100f) <= status.GetPorcentagem)
                        {
                            comandoDeAtaque.AlvoAcao[i].Monstro.AplicarStatusSecundario(comandoDeAtaque.AlvoAcao[i], status.GetStatus);
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

[System.Serializable]
public class StatusEffectSecundarioParaAplicar
{
    [SerializeField] private StatusEffectSecundario status;
    [SerializeField] private bool porcentagemVariavel;
    [HideIf("porcentagemVariavel")]
    [SerializeField] private float porcentagemFixa;
    [ShowIfGroup("porcentagemVariavel")]
    [SerializeField] private float porcentagemAtributo;

    public StatusEffectSecundario GetStatus => status;
    public bool GetPorcentagemVariavel => porcentagemVariavel;
    public float GetPorcentagem
    {
        get
        {
            if (porcentagemVariavel)
                return porcentagemAtributo;
            return porcentagemFixa;
        }
    }
}
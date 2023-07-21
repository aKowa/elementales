using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Batalha/Ataques/Aplicar Status")]

public class AplicarStatusEffect : AcaoNaBatalha
{
    //Variaveis
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
                foreach (StatusEffectParaAplicar status in status)
                {
                    comandoDeAtaque.AlvoAcao[i].Monstro.TomarAtaqueStatusEffect(status, atributoAtaque, comandoDeAtaque, comandoDeAtaque.AlvoAcao[i]);
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
public class StatusEffectParaAplicar
{
    [SerializeField] private StatusEffectBase status;
    [SerializeField] private bool porcentagemVariavel;
    [HideIf("porcentagemVariavel")]
    [SerializeField] private float porcentagemFixa;
    [ShowIfGroup("porcentagemVariavel")]
    [SerializeField] private float porcentagemAtributo;

    public StatusEffectBase GetStatus => status;
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
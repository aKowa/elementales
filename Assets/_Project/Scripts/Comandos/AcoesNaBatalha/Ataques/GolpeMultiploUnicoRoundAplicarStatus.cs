using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Batalha/Ataques/Golpe Multiplo Com AplicarStatus")]

public class GolpeMultiploUnicoRoundAplicarStatus : AcaoNaBatalha
{
    [Tooltip("A cada novo indice na lista representa um novo ataque e sua chance")]
    [SerializeField] private bool novosAtaquesCondicionaisAcertarAtaqueAnterior;
    [SerializeField] private List<float> chanceAcerto = new List<float>();

    [Tooltip("A cada Hit tenta aplicar status ou somente no fim de todos os hits")]
    [SerializeField] private bool tentarAplicarStatusEmCadaHit;
    [SerializeField] private List<StatusEffectParaAplicar> status;

    private int quantidadeHits=0;
    private int quantideHitsMax;
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
                    if (tentarAplicarStatusEmCadaHit)
                    {
                        Debug.Log("Vendo status hit");
                        foreach (StatusEffectParaAplicar statu in status)
                        {
                            if (Random.Range(0, 100f) <= statu.GetPorcentagem)
                            {
                                comandoDeAtaque.AlvoAcao[i].Monstro.TomarAtaqueStatusEffect(statu, atributoAtaque, comandoDeAtaque, comandoDeAtaque.AlvoAcao[i]);
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
        quantidadeHits++;
        if(quantidadeHits >= quantideHitsMax)
        {
            for (int i = 0; i < comandoDeAtaque.AlvoAcao.Count; i++)
            {
                if (tentarAplicarStatusEmCadaHit == false)
                {
                    Debug.Log("Vendo status fim");
                    foreach (StatusEffectParaAplicar statu in status)
                    {
                        if (Random.Range(0, 100f) <= statu.GetPorcentagem)
                        {
                            comandoDeAtaque.AlvoAcao[i].Monstro.TomarAtaqueStatusEffect(statu, atributoAtaque, comandoDeAtaque, comandoDeAtaque.AlvoAcao[i]);
                        }
                    }
                }
            }
        }
    }

    public override void IniciarAnimacao(BattleManager battleManager, Comando comando)
    {
        quantideHitsMax = 0;
        quantidadeHits = 0;
        int quantidadeAtaques = 1;

        foreach (float chance in chanceAcerto)
        {
            if (Random.Range(0, 100f) <= chance)
            {
                quantidadeAtaques++;
            }
            else if (novosAtaquesCondicionaisAcertarAtaqueAnterior)
            {
                break;
            }
        }
        quantideHitsMax = quantidadeAtaques;
        Debug.Log("Quantidade de Ataques " + quantidadeAtaques);

        battleManager.SetarAnimacao(quantidadeAtaques);
    }
}
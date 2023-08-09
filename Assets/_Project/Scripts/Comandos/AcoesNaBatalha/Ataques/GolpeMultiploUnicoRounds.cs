using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Batalha/Ataques/Golpe Multiplo Unico Round")]

public class GolpeMultiploUnicoRounds : AcaoNaBatalha
{
    [Tooltip("A cada novo indice na lista representa um novo ataque e sua chance")]
    [SerializeField] private bool novosAtaquesCondicionaisAcertarAtaqueAnterior;
    [SerializeField] private List<float> chanceAcerto = new List<float>();

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

    public override void IniciarAnimacao(BattleManager battleManager, Comando comando)
    {
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

        //Debug.Log("Quantidade de Ataques " + quantidadeAtaques);

        battleManager.SetarAnimacao(quantidadeAtaques);
    }
}
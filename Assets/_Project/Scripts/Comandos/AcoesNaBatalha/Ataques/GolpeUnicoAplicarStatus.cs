using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Batalha/Ataques/Golpe Unico Com AplicarStatus")]

public class GolpeUnicoAplicarStatus : AcaoNaBatalha
{
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
                if (acertou)
                {
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
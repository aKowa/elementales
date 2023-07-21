using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Batalha/Ataques/Struggle")]

public class Struggle : AcaoNaBatalha
{
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
                    if (dano >= 0)
                    {
                        foreach (Integrante.MonstroAtual monstro in comando.Origem.MonstrosAtuais)
                        {
                            if (monstro.GetMonstro == comandoDeAtaque.GetMonstro)
                            {
                                monstro.Monstro.TomarAtaquePuro(Mathf.CeilToInt(dano / 2), monstro, true, true);
                                monstro.GetMonstro.RecuperarMana(monstro.GetMonstro.AtributosAtuais.CalcManaRegenStruggle());
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

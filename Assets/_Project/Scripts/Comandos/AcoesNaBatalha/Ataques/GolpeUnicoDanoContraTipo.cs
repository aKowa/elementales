using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Batalha/Ataques/Golpe Dano Contra Tipo")]
public class GolpeUnicoDanoContraTipo : AcaoNaBatalha
{
    [SerializeField] private List<MonsterType> types;
    [SerializeField] private int modificadorDeDano;
    
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
                if (comandoDeAtaque.AlvoAcao[i].Monstro.GetMonstro.MonsterData.GetMonsterTypes.Intersect(types).Any())
                {
                    comandoDeAtaque.AttackData.Poder *= modificadorDeDano;
                }
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
}
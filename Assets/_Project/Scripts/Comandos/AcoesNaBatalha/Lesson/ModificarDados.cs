using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Batalha/CombatLessons/Modificador de Dado")]
public class ModificarDados : AcaoNaBatalha
{
    [SerializeField] private int modificador;
    [SerializeField] private List<int> dadosEspecificos = new List<int>(); 
    [SerializeField] private ChecksDeDado checksDeDado;
    public override void Executar(BattleManager battleManager, Comando comando)
    {
        ComandoDeAtaque comandoDeAtaque = (ComandoDeAtaque)comando;

        for (var i = 0; i < comandoDeAtaque.GetMonstroInBattle.DiceResults.Count; i++)
        {
            switch (checksDeDado)
            {
                case ChecksDeDado.Par:
                    if (comandoDeAtaque.GetMonstroInBattle.DiceResults[i] % 2 == 0)
                    {
                        comandoDeAtaque.GetMonstroInBattle.DiceResults[i] += modificador;
                    }
                    break;
                case ChecksDeDado.Impar:
                    if (comandoDeAtaque.GetMonstroInBattle.DiceResults[i] % 2 != 0)
                    {
                        comandoDeAtaque.GetMonstroInBattle.DiceResults[i] += modificador;
                    }
                    break;
                case ChecksDeDado.Especifico:
                    if (dadosEspecificos.Contains(comandoDeAtaque.GetMonstroInBattle.DiceResults[i]))
                    {
                        comandoDeAtaque.GetMonstroInBattle.DiceResults[i] += modificador;
                    }
                    break;
                case ChecksDeDado.None:
                    comandoDeAtaque.GetMonstroInBattle.DiceResults[i] += modificador;
                    break;
                case ChecksDeDado.TodosExceto:
                    if (dadosEspecificos.Contains(comandoDeAtaque.GetMonstroInBattle.DiceResults[i]) == false)
                    {
                        comandoDeAtaque.GetMonstroInBattle.DiceResults[i] += modificador;
                    }
                    break;
            }

                comandoDeAtaque.Origem.MonstrosAtuais.Single(m => m.Monstro == comandoDeAtaque.GetMonstroInBattle)
                    .MonstroSlotBattle.UpdateDiceIcons(comandoDeAtaque.GetMonstroInBattle.DiceResults);
        }
    }

    private enum ChecksDeDado
    {
        Par,
        Impar,
        Especifico,
        None,
        TodosExceto
    }
}
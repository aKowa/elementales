using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Batalha/CombatLessons/Dobrar Valor Dado")]
public class DobrarValorDado: AcaoNaBatalha
{
    public override void Executar(BattleManager battleManager, Comando comando)
    {
        ComandoDeAtaque comandoDeAtaque = (ComandoDeAtaque)comando;

        for (var i = 0; i < comandoDeAtaque.GetMonstroInBattle.DiceResults.Count; i++)
        {
            comandoDeAtaque.GetMonstroInBattle.DiceResults[i] *= 2;
        }
        
        comandoDeAtaque.Origem.MonstrosAtuais.Single(m => m.Monstro == comandoDeAtaque.GetMonstroInBattle)
            .MonstroSlotBattle.UpdateDiceIcons(comandoDeAtaque.GetMonstroInBattle.DiceResults);
    }
}
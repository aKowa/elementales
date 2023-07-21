using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Batalha/CombatLessons/Poder Ao Ataque Igual ao Dado")]
public class AdicionarPoderAoComandoDeAtaque : AcaoNaBatalha
{
    public override void Executar(BattleManager battleManager, Comando comando)
    {
        ComandoDeAtaque combatLesson = (ComandoDeAtaque)comando;
        ComandoDeAtaque comandoDoMonstro = battleManager.Comandos.FilterCast<ComandoDeAtaque>()
            .Last(c => c.GetMonstro == combatLesson.GetMonstro 
                       && c.Origem != null
                       && c.QuantidadeVezesComandoRodou == 0);
        Debug.LogWarning($"O outro comando do {combatLesson.GetMonstro.NickName}: {comandoDoMonstro}");

        if (comandoDoMonstro)
        {
            Debug.Log($"Adicionando poder ao {comandoDoMonstro.Nome} igual a {combatLesson.GetMonstroInBattle.DiceResults.Sum()}");
            comandoDoMonstro.AttackData.Poder += combatLesson.GetMonstroInBattle.DiceResults.Sum();
        }
    }
}
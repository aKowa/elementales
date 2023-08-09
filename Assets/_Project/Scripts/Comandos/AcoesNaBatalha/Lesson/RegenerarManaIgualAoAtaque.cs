using System.Linq;
using BergamotaLibrary;
using Sirenix.Utilities;
using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Batalha/CombatLessons/Regenerar mana igual ao ataque")]
public class RegenerarManaIgualAoAtaque : AcaoNaBatalha
{
    [SerializeField] private int chance;
    [SerializeField, Range(0, 1)] private float manaPercentage;
    public override void Executar(BattleManager battleManager, Comando comando)
    {
        ComandoDeAtaque combatLesson = (ComandoDeAtaque)comando;
        Integrante.MonstroAtual monstroAtual = combatLesson.Origem.MonstrosAtuais.Single(m => m.Monstro == combatLesson.GetMonstroInBattle);

        ComandoDeAtaque comandoDoMonstro = battleManager.Comandos.FilterCast<ComandoDeAtaque>()
            .Last(c => c.GetMonstro == combatLesson.GetMonstro 
                       && c.Origem != null
                       && c.QuantidadeVezesComandoRodou == 0);
        Debug.LogWarning($"O outro comando do {combatLesson.GetMonstro.NickName}: {comandoDoMonstro}");

        if (comandoDoMonstro)
        {
            if (Random.Range(0, 101) < chance)
            {
                monstroAtual.GetMonstro.RecuperarMana(Mathf.CeilToInt(comandoDoMonstro.CustoMana * manaPercentage));
                battleManager.TocarSom("Mana");

            }
        }
    }
}
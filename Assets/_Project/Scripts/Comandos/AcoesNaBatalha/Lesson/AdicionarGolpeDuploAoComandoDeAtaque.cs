using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Batalha/CombatLessons/Golpe Duplo Ao Comando Original")]
public class AdicionarGolpeDuploAoComandoDeAtaque : AcaoNaBatalha
{
    [SerializeField] private int modificadorDeNumeroRoundsVivo;
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
            comandoDoMonstro.NumeroRoundsComandoVivo += modificadorDeNumeroRoundsVivo;
        }
    }
}
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Batalha/CombatLessons/Critico Garantido com Dano De Volta")]
public class CriticoGarantidoComDanoDeVolta : AcaoNaBatalha
{
    [SerializeField] private int min, max;
    
    public override void Executar(BattleManager battleManager, Comando comando)
    {
        ComandoDeAtaque combatLesson = (ComandoDeAtaque)comando;
        Integrante.MonstroAtual monstroAtual = combatLesson.Origem.MonstrosAtuais.Single(m => m.Monstro == combatLesson.GetMonstroInBattle);
        int porcentagem = Random.Range(min, max + 1);
        int danoEmPorcentagem = combatLesson.GetMonstro.AtributosAtuais.VidaMax / porcentagem;
        
        Debug.Log($"{monstroAtual.GetMonstro.NickName} critico garantido e sofrendo {danoEmPorcentagem} de dano.");
        
        combatLesson.GetMonstroInBattle.CriticoGarantido = true;
        combatLesson.GetMonstroInBattle.TomarAtaquePuro(danoEmPorcentagem, monstroAtual, true, true);

    }
}
public class IntoxicatedStatus : StatusEffectBase
{
    public override bool VerificarExcecao(MonsterType tipoAtaque)
    {
        return base.VerificarExcecao(tipoAtaque);
    }

    public override bool ExecutarInBattle(Integrante.MonstroAtual monstroAtual)
    {
        return base.ExecutarInBattle(monstroAtual);
    }

    public override bool ExecutarOutBattle(Monster monstro)
    {
        return base.ExecutarOutBattle(monstro);
    }
}
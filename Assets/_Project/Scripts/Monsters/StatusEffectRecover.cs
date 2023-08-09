using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Monster/Status/Recover")]

public class StatusEffectRecover : StatusEffectBase
{
    [SerializeField] private float porcentagemRecuperarVida, porcentagemRecuperarMana;
    [SerializeField] private AudioClip somRecuperarVida;
    [SerializeField] private AudioClip somRecuperarMana;
    public override bool ExecutarInBattle(Integrante.MonstroAtual monstroAtual)
    {
        Monster monstro = monstroAtual.GetMonstro;

        monstro.ReceberCura((int)(monstro.AtributosAtuais.VidaMax * (porcentagemRecuperarVida / 100)));
        monstro.RecuperarManaPorcentagem(porcentagemRecuperarMana);
        statusEffectOpcoesDentroCombate.QuantidadeTurnosAtuais++;

        SoundManager.instance.TocarSomIgnorandoPause(somRecuperarVida);
        SoundManager.instance.TocarSomIgnorandoPause(somRecuperarMana);


        if (statusEffectOpcoesDentroCombate.GetEfeitoPassaComTempo)
        {
            if (statusEffectOpcoesDentroCombate.QuantidadeTurnosAtuais >= statusEffectOpcoesDentroCombate.GetquantidadeMaximaTurnos)
            {
                seRemover = true;
                Debug.Log("Cessou efeito de " + nome);
            }
        }
        else
        {
            if (Random.Range(0f, 100f) <= statusEffectOpcoesDentroCombate.GetchancePorcentagemEfeitoPassar)
            {
                seRemover = true;
                Debug.Log("Cessou efeito de " + nome);
            }
        }

        if (seRemover)
        {
            RemoverModificador(monstro);
            BattleManager.Instance.RodarEfeito(monstroAtual, this, 0, true, false);
        }

        return seRemover;
    }
}

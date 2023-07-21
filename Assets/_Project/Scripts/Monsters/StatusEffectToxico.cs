using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Monster/Status/Entoxicado ")]

public class StatusEffectToxico : StatusEffectBase
{
    private int indiceListDano = 0;
    [SerializeField] private List<int> progressaoDano = new List<int>();
    public override bool ExecutarInBattle(Integrante.MonstroAtual monstroAtual)
    {
        Monster monstro = monstroAtual.GetMonstro;

        statusEffectOpcoesDentroCombate.QuantidadeTurnosAtuais++;
        if (statusEffectOpcoesDentroCombate.GetdanoNoCombate)
        {
            if (statusEffectOpcoesDentroCombate.GetdanoDentroCombate.GetTemDanoFixo)
            {
                float x = (progressaoDano[indiceListDano]);
                monstro.TomarDanoPuro(Mathf.CeilToInt(x));

                BattleManager.Instance.RodarEfeito(monstroAtual, this, Mathf.CeilToInt(statusEffectOpcoesDentroCombate.GetdanoDentroCombate.Dano), false, false);
            }
            else
            {
                float x = ((float)progressaoDano[indiceListDano] / 100) * monstro.AtributosAtuais.VidaMax;
                monstro.TomarDanoPuro(Mathf.CeilToInt(x));
                Debug.Log("Causei isso de dano " + progressaoDano[indiceListDano]);
                BattleManager.Instance.RodarEfeito(monstroAtual, this, (int)x, false, false);
            }
            indiceListDano++;
            if (indiceListDano + 1 > progressaoDano.Count)
                indiceListDano = progressaoDano.Count-1;
            Debug.Log("Tomei dano de " + nome + " dentro do combate");
        }

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

using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Monster/Status/Confusion ")]

public class StatusEffectConfusion : StatusEffectBase
{
    [Header("Confusion")]
    [SerializeField] private float taxaConfusionSeAcertar;
    [SerializeField] private int quantidadeTurnosGarantidos;
    public Comando Executar(Comando comandoAntigo)
    {
        if (Random.Range(0, 100) <= taxaConfusionSeAcertar)
        {
            if (comandoAntigo.AlvoAcao.Count > 0)
            {
                for (int i = 0; i < BattleManager.Instance.Integrantes.Count; i++)
                {
                    for (int j = 0; j < BattleManager.Instance.Integrantes[i].MonstrosAtuais.Count; j++)
                    {
                        if (BattleManager.Instance.Integrantes[i].MonstrosAtuais[j].GetMonstro == comandoAntigo.GetMonstro)
                        {
                            ComandoDeAtaque ataque = BattleManager.Instance.AtaqueConfusion;
                            ComandoDeAtaque comandoNovo = ScriptableObject.Instantiate(ataque);
                            comandoNovo.ReceberVariaves(BattleManager.Instance.Integrantes[i], BattleManager.Instance.Integrantes[i].MonstrosAtuais[j], j);
                            comandoNovo.AlvoComAtaquesValidos = new List<bool>();
                            comandoNovo.AlvoComAtaquesValidos.Add(true);
                            comandoNovo.name = ataque.name;
                            return comandoNovo;

                        }
                    }
                }
            }
        }
        return null;
    }
    public override bool ExecutarInBattle(Integrante.MonstroAtual monstroAtual)
    {
        Monster monstro = monstroAtual.GetMonstro;

        if (statusEffectOpcoesDentroCombate.QuantidadeTurnosAtuais < quantidadeTurnosGarantidos)
        {
            Debug.Log("Fiz o garantido");
            statusEffectOpcoesDentroCombate.QuantidadeTurnosAtuais++;
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

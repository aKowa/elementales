using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraMana : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private Image barraMana;

    //Variaveis
    [Header("Variaveis")]
    [SerializeField] private float tempo;

    public void AtualizarBarra(Monster monstro)
    {
        barraMana.fillAmount = FillAmountAtual(monstro);
    }

    private float FillAmountAtual(Monster monstro)
    {
        float amount = (float)monstro.AtributosAtuais.Mana / (float)monstro.AtributosAtuais.ManaMax;

        if(monstro.AtributosAtuais.Mana > 0 && amount < 0.035)
        {
            amount = 0.035f;
        }

        return amount;
    }

    public IEnumerator AumentarMana(Monster monstro)
    {
        float fillAmountDestino = FillAmountAtual(monstro);

        float vel = Mathf.Abs(fillAmountDestino - barraMana.fillAmount);
        vel = vel / tempo;

        while (barraMana.fillAmount < fillAmountDestino)
        {
            barraMana.fillAmount += vel * Time.unscaledDeltaTime;
            yield return null;
        }

        barraMana.fillAmount = fillAmountDestino;
    }

    public IEnumerator DiminuirMana(Monster monstro)
    {
        float fillAmountDestino = FillAmountAtual(monstro);

        float vel = Mathf.Abs(fillAmountDestino - barraMana.fillAmount);
        vel = vel / tempo;

        while (barraMana.fillAmount > fillAmountDestino)
        {
            barraMana.fillAmount -= vel * Time.unscaledDeltaTime;
            yield return null;
        }

        barraMana.fillAmount = fillAmountDestino;
    }
}

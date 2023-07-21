using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraHP : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private Image barraHP;

    //Variaveis
    [Header("Variaveis")]
    [SerializeField] private float tempo;

    public void AtualizarBarra(Monster monstro)
    {
        barraHP.fillAmount = FillAmountAtual(monstro);
    }

    private float FillAmountAtual(Monster monstro)
    {
        float amount = (float)monstro.AtributosAtuais.Vida / (float)monstro.AtributosAtuais.VidaMax;

        if(monstro.AtributosAtuais.Vida > 0 && amount < 0.035)
        {
            amount = 0.035f;
        }

        return amount;
    }

    public IEnumerator AumentarHP(Monster monstro)
    {
        float fillAmountDestino = FillAmountAtual(monstro);

        float vel = Mathf.Abs(fillAmountDestino - barraHP.fillAmount);
        vel = vel / tempo;

        while (barraHP.fillAmount < fillAmountDestino)
        {
            barraHP.fillAmount += vel * Time.unscaledDeltaTime;
            yield return null;
        }

        barraHP.fillAmount = fillAmountDestino;
    }

    public IEnumerator DiminuirHP(Monster monstro)
    {
        float fillAmountDestino = FillAmountAtual(monstro);

        float vel = Mathf.Abs(fillAmountDestino - barraHP.fillAmount);
        vel = vel / tempo;

        while (barraHP.fillAmount > fillAmountDestino)
        {
            barraHP.fillAmount -= vel * Time.unscaledDeltaTime;
            yield return null;
        }

        barraHP.fillAmount = fillAmountDestino;
    }
}

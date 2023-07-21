using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraExp : MonoBehaviour
{
    //Componentes
    [SerializeField] private Image barraExp;

    //Variaveis
    [Header("Variaveis")]
    [SerializeField] private float tempo;

    public void AtualizarBarra(Monster monstro)
    {
        barraExp.fillAmount = FillAmountAtual(monstro);
    }

    public void AtualizarBarra(float amount)
    {
        barraExp.fillAmount = amount;
    }

    private float FillAmountAtual(Monster monstro)
    {
        float amount = (float)monstro.AtributosAtuais.ExpEmRelacaoAoNivelAtual() / ((float)monstro.AtributosAtuais.ExpParaOProxNivelRaw() - (float)monstro.AtributosAtuais.ExpParaONivelAtual());

        if (monstro.AtributosAtuais.ExpEmRelacaoAoNivelAtual() > 0 && amount < 0.035)
        {
            amount = 0.035f;
        }

        if(monstro.AtributosAtuais.Nivel >= MonsterAttributes.nivelMax)
        {
            amount = 1;
        }

        return amount;
    }

    public IEnumerator AumentarExp(Monster monstro)
    {
        float fillAmountDestino = FillAmountAtual(monstro);

        float vel = Mathf.Abs(fillAmountDestino - barraExp.fillAmount);
        vel = vel / tempo;

        while (barraExp.fillAmount < fillAmountDestino)
        {
            barraExp.fillAmount += vel * Time.unscaledDeltaTime;
            yield return null;
        }

        barraExp.fillAmount = fillAmountDestino;
    }

    public IEnumerator DiminuirExp(Monster monstro)
    {
        float fillAmountDestino = FillAmountAtual(monstro);

        float vel = Mathf.Abs(fillAmountDestino - barraExp.fillAmount);
        vel = vel / tempo;

        while (barraExp.fillAmount > fillAmountDestino)
        {
            barraExp.fillAmount -= vel * Time.unscaledDeltaTime;
            yield return null;
        }

        barraExp.fillAmount = fillAmountDestino;
    }

    public IEnumerator EncherABarra()
    {
        float fillAmountDestino = 1;

        float vel = Mathf.Abs(fillAmountDestino - barraExp.fillAmount);
        vel = vel / tempo;

        while (barraExp.fillAmount < fillAmountDestino)
        {
            barraExp.fillAmount += vel * Time.unscaledDeltaTime;
            yield return null;
        }

        barraExp.fillAmount = fillAmountDestino;
    }
}

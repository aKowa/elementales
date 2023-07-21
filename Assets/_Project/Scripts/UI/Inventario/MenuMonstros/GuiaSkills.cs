using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GuiaSkills : MonstroInfo
{
    //Componentes
    [Header("Componentes")]

    [SerializeField] private TMP_Text textoHP;
    [SerializeField] private TMP_Text textoMaxHP;
    [SerializeField] private TMP_Text textoMana;
    [SerializeField] private TMP_Text textoMaxMana;
    [SerializeField] private TMP_Text textoAtk;
    [SerializeField] private TMP_Text textoSpAtk;
    [SerializeField] private TMP_Text textoDef;
    [SerializeField] private TMP_Text textoSpDef;
    [SerializeField] private TMP_Text textoSpeed;
    [SerializeField] private TMP_Text textoExp;
    [SerializeField] private TMP_Text textoToNextLevel;

    [SerializeField] private BarraHP barraHP;
    [SerializeField] private BarraMana barraMana;
    [SerializeField] private BarraExp barraExp;

    public override void AtualizarInformacoes(Monster monstro)
    {
        textoHP.text = monstro.AtributosAtuais.Vida.ToString();
        textoMaxHP.text = monstro.AtributosAtuais.VidaMax.ToString();
        textoMana.text = monstro.AtributosAtuais.Mana.ToString();
        textoMaxMana.text = monstro.AtributosAtuais.ManaMax.ToString();
        textoAtk.text = monstro.AtributosAtuais.Ataque.ToString();
        textoSpAtk.text = monstro.AtributosAtuais.SpAtaque.ToString();
        textoDef.text = monstro.AtributosAtuais.Defesa.ToString();
        textoSpDef.text = monstro.AtributosAtuais.SpDefesa.ToString();
        textoSpeed.text = monstro.AtributosAtuais.Velocidade.ToString();
        textoExp.text = monstro.AtributosAtuais.Exp.ToString();
        textoToNextLevel.text = monstro.AtributosAtuais.ExpParaOProxNivel().ToString();

        barraHP.AtualizarBarra(monstro);
        barraMana.AtualizarBarra(monstro);
        barraExp.AtualizarBarra(monstro);
    }

    public override void ResetarInformacoes()
    {
        textoHP.text = string.Empty;
        textoMaxHP.text = string.Empty;
        textoMana.text = string.Empty;
        textoMaxMana.text = string.Empty;
        textoAtk.text = string.Empty;
        textoSpAtk.text = string.Empty;
        textoDef.text = string.Empty;
        textoSpDef.text = string.Empty;
        textoSpeed.text = string.Empty;
        textoExp.text = string.Empty;
        textoToNextLevel.text = string.Empty;
    }
}

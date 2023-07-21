using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JanelaDeAtributosDoLevelUp : MonoBehaviour
{
    //Componentes
    [SerializeField] private RectTransform janela;
    [SerializeField] private TMP_Text textoHP;
    [SerializeField] private TMP_Text textoMana;
    [SerializeField] private TMP_Text textoAtk;
    [SerializeField] private TMP_Text textoAtkSp;
    [SerializeField] private TMP_Text textoDef;
    [SerializeField] private TMP_Text textoDefSp;
    [SerializeField] private TMP_Text textoVel;
    [SerializeField] private TMP_Text diferencaHP;
    [SerializeField] private TMP_Text diferencaMana;
    [SerializeField] private TMP_Text diferencaAtk;
    [SerializeField] private TMP_Text diferencaAtkSp;
    [SerializeField] private TMP_Text diferencaDef;
    [SerializeField] private TMP_Text diferencaDefSp;
    [SerializeField] private TMP_Text diferencaVel;

    //Variaveis
    private MonsterAttributesSave atributosIniciais;

    public void IniciarAtributos(MonsterAttributes atributos)
    {
        atributosIniciais = new MonsterAttributesSave(atributos);
    }

    public void AbrirJanela(MonsterAttributes atributos)
    {
        janela.gameObject.SetActive(true);

        AtualizarInformacoes(atributos);
    }

    public void FecharJanela()
    {
        janela.gameObject.SetActive(false);

        ResetarInformacoes();
    }

    private void AtualizarInformacoes(MonsterAttributes atributos)
    {
        int diferenca;

        //---------------------HP--------------------------
        diferenca = atributos.VidaMax - atributosIniciais.vidaMax;
        textoHP.text = atributos.VidaMax.ToString();
        
        if(diferenca > 0)
        {
            diferencaHP.text = "+" + diferenca.ToString();
        }
        else
        {
            diferencaHP.text = string.Empty;
        }

        //---------------------Mana--------------------------
        diferenca = atributos.ManaMax - atributosIniciais.manaMax;
        textoMana.text = atributos.ManaMax.ToString();

        if (diferenca > 0)
        {
            diferencaMana.text = "+" + diferenca.ToString();
        }
        else
        {
            diferencaMana.text = string.Empty;
        }

        //---------------------Atk--------------------------
        diferenca = atributos.Ataque - atributosIniciais.ataque;
        textoAtk.text = atributos.Ataque.ToString();

        if (diferenca > 0)
        {
            diferencaAtk.text = "+" + diferenca.ToString();
        }
        else
        {
            diferencaAtk.text = string.Empty;
        }

        //---------------------AtkSp--------------------------
        diferenca = atributos.SpAtaque - atributosIniciais.spAtaque;
        textoAtkSp.text = atributos.SpAtaque.ToString();

        if (diferenca > 0)
        {
            diferencaAtkSp.text = "+" + diferenca.ToString();
        }
        else
        {
            diferencaAtkSp.text = string.Empty;
        }

        //---------------------Def--------------------------
        diferenca = atributos.Defesa - atributosIniciais.defesa;
        textoDef.text = atributos.Defesa.ToString();

        if (diferenca > 0)
        {
            diferencaDef.text = "+" + diferenca.ToString();
        }
        else
        {
            diferencaDef.text = string.Empty;
        }

        //---------------------DefSp--------------------------
        diferenca = atributos.SpDefesa - atributosIniciais.spDefesa;
        textoDefSp.text = atributos.SpDefesa.ToString();

        if (diferenca > 0)
        {
            diferencaDefSp.text = "+" + diferenca.ToString();
        }
        else
        {
            diferencaDefSp.text = string.Empty;
        }

        //---------------------Vel--------------------------
        diferenca = atributos.Velocidade - atributosIniciais.velocidade;
        textoVel.text = atributos.Velocidade.ToString();

        if (diferenca > 0)
        {
            diferencaVel.text = "+" + diferenca.ToString();
        }
        else
        {
            diferencaVel.text = string.Empty;
        }
    }

    private void ResetarInformacoes()
    {
        atributosIniciais = null;

        textoHP.text = string.Empty;
        textoAtk.text = string.Empty;
        textoAtkSp.text = string.Empty;
        textoDef.text = string.Empty;
        textoDefSp.text = string.Empty;
        textoVel.text = string.Empty;
        diferencaHP.text = string.Empty;
        diferencaAtk.text = string.Empty;
        diferencaAtkSp.text = string.Empty;
        diferencaDef.text = string.Empty;
        diferencaDefSp.text = string.Empty;
        diferencaVel.text = string.Empty;
    }
}

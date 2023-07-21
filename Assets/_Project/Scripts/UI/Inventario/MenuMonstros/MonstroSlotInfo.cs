using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonstroSlotInfo : MonoBehaviour
{
    //Componentes
    [SerializeField] private Image miniatura;
    [SerializeField] private TMP_Text textoLevel;
    [SerializeField] private TMP_Text textoNome;
    [SerializeField] private TMP_Text textoHP;
    [SerializeField] private TMP_Text textoMaxHP;
    [SerializeField] private TMP_Text textoMana;
    [SerializeField] private TMP_Text textoMaxMana;
    [SerializeField] private BarraHP barraHP;
    [SerializeField] private BarraMana barraMana;
    [SerializeField] private GameObject statusLogoBase;
    [SerializeField] private Transform statusHolder;

    //Variaveis
    private Monster monstro;
    private List<StatusLogo> status = new List<StatusLogo>();

    //Getters
    public BarraHP BarraHP => barraHP;
    public BarraMana BarraMana => barraMana;

    public Monster Monstro
    {
        get => monstro;
        set => monstro = value;
    }

    public void AtualizarInformacoes()
    {
        if(miniatura != null)
        {
            miniatura.sprite = monstro.MonsterData.Miniatura;
        }

        textoLevel.text = monstro.Nivel.ToString();
        textoNome.text = monstro.NickName;
        textoHP.text = monstro.AtributosAtuais.Vida.ToString();
        textoMaxHP.text = monstro.AtributosAtuais.VidaMax.ToString();
        textoMana.text = monstro.AtributosAtuais.Mana.ToString();
        textoMaxMana.text = monstro.AtributosAtuais.ManaMax.ToString();

        barraHP.AtualizarBarra(monstro);
        barraMana.AtualizarBarra(monstro);

        AtualizarStatus();
    }

    private void AtualizarStatus()
    {
        foreach (StatusLogo tipo in status)
        {
            Destroy(tipo.gameObject);
        }

        status.Clear();

        foreach(StatusEffectBase statusEffect in monstro.Status)
        {
            StatusLogo logo = Instantiate(statusLogoBase, statusHolder).GetComponent<StatusLogo>();
            logo.SetStatus(statusEffect);

            status.Add(logo);
        }
    }
}

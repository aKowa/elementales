using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GuiaInfo : MonstroInfo
{
    //Componentes
    [Header("Componentes")]

    [SerializeField] private GameObject tipoLogoBase;

    [SerializeField] private TMP_Text infoNumero;
    [SerializeField] private TMP_Text infoNome;
    [SerializeField] private Transform infoTiposHolder;
    [SerializeField] private TMP_Text infoFT;
    [SerializeField] private TMP_Text infoDataMet;
    [SerializeField] private TMP_Text infoWhere;
    [SerializeField] private TMP_Text infoLevelMet;

    //Variaveis
    private List<TipoLogo> tipoLogos = new List<TipoLogo>();

    public override void AtualizarInformacoes(Monster monstro)
    {
        
        infoNumero.text = monstro.MonsterData.MonsterBookID.ToString("D2");
        infoNome.text = monstro.MonsterData.GetName;
        infoFT.text = monstro.MonsterInfo.FirstTrainer;
        infoDataMet.text = monstro.MonsterInfo.DataMet.ToString("MM/dd/yyyy");
        infoWhere.text = monstro.MonsterInfo.Where;
        infoLevelMet.text = $"Lvl. {monstro.MonsterInfo.LevelMet.ToString()}";

        AtualizarTipo(monstro);
    }

    public override void ResetarInformacoes()
    {
        infoNumero.text = string.Empty;
        infoNome.text = string.Empty;
        infoFT.text = string.Empty;
        infoDataMet.text = string.Empty;
        infoWhere.text = string.Empty;
        infoLevelMet.text = string.Empty;

        ResetarTipoLogos();
    }

    private void AtualizarTipo(Monster monstro)
    {
        ResetarTipoLogos();

        for (int i = 0; i < monstro.MonsterData.GetMonsterTypes.Count; i++)
        {
            TipoLogo tipoLogo = Instantiate(tipoLogoBase, infoTiposHolder).GetComponent<TipoLogo>();
            tipoLogo.gameObject.SetActive(true);

            tipoLogo.SetTipo(monstro.MonsterData.GetMonsterTypes[i]);

            tipoLogos.Add(tipoLogo);
        }
    }

    private void ResetarTipoLogos()
    {
        foreach (TipoLogo tipoLogo in tipoLogos)
        {
            Destroy(tipoLogo.gameObject);
        }

        tipoLogos.Clear();
    }
}

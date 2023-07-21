using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventosCutscene : MonoBehaviour
{
    //Componentes
    [Header("Flags")]
    [SerializeField] protected ListaDeFlags flagsGerais;
    [SerializeField] protected string nomeDaFlag;
    [SerializeField] protected bool rodarCutsceneQuandoIniciarACena;

    protected Cutscene cutscene;

    private void Awake()
    {
        cutscene = GetComponent<Cutscene>();

        OnAwake();
    }

    protected virtual void OnAwake()
    {
        //Feito para ser substituido
    }

    private void Start()
    {
        if(rodarCutsceneQuandoIniciarACena == true)
        {
            if (Flags.GetFlag(flagsGerais.name, nomeDaFlag) == false)
            {
                RodarCustcene();
            }
        }
    }

    public virtual void RodarCustcene()
    {
        BergamotaDialogueSystem.DialogueUI.Instance.SetPlaceholderDeTexto("%player", PlayerData.Instance.GetPlayerName);
        cutscene.IniciarCutscene(0, OnCutsceneEnd);

        OnCutsceneStart();
    }

    protected virtual void OnCutsceneStart()
    {
        //Feito para ser substituido
    }

    protected virtual void OnCutsceneEnd()
    {
        if(flagsGerais != null && string.IsNullOrEmpty(nomeDaFlag) == false)
        {
            Flags.SetFlag(flagsGerais.name, nomeDaFlag, true);
        }
    }
}

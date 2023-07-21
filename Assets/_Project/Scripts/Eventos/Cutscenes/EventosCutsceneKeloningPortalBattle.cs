using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventosCutsceneKeloningPortalBattle : EventosCutscene
{
    //Componentes
    [Space(10)]

    [SerializeField] private NPCBatalha npcBatalhaKeloning;

    public void IniciarBatalha()
    {
        NPCManager.IniciandoBatalha = true;

        npcBatalhaKeloning.IniciarBatalha();

        cutscene.PausarCutscene();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(Flags.GetFlag(flagsGerais.name, nomeDaFlag) == false)
            {
                RodarCustcene();
            }
        }
    }

    protected override void OnCutsceneEnd()
    {
        //Nada
    }
}

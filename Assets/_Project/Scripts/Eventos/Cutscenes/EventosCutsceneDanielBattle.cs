using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventosCutsceneDanielBattle : EventosCutscene
{
    //Componentes
    [SerializeField] private NPCBatalha npcBatalhaDaniel;
    [SerializeField] private Transform npcsNormais;

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    public void IniciarBatalha()
    {
        NPCManager.IniciandoBatalha = true;

        npcBatalhaDaniel.IniciarBatalha();

        cutscene.PausarCutscene();
    }

    protected override void OnCutsceneStart()
    {
        npcsNormais.gameObject.SetActive(false);
    }
}

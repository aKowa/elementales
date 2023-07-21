using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BergamotaLibrary;
using BergamotaDialogueSystem;
public class ObjetoComDialogo : Interagivel
{
    private DialogueActivator dialogueActivator;

    private void Awake()
    {
        dialogueActivator = GetComponent<DialogueActivator>();
    }

    public override void Interagir(Player player)
    {
        dialogueActivator.ShowDialogue(player.DialogueUI);
    }
}

using BergamotaDialogueSystem;
using BergamotaLibrary;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : Interagivel
{
    //Componentes
    protected NPC npc;
    protected DialogueActivator dialogueActivator;
    protected ConditionalDialoguesCaller conditionalDialoguesCaller;

    //Enums
    public enum TipoDialogo { Unico, Condicional }

    //Variaveis
    [SerializeField] protected TipoDialogo tipoDialogo;

    [SerializeField]
    [ShowIf("tipoDialogo", TipoDialogo.Unico)]
    protected DialogueObject dialogo;

    protected virtual void Awake()
    {
        npc = GetComponentInParent<NPC>();
        dialogueActivator = GetComponent<DialogueActivator>();
        conditionalDialoguesCaller = GetComponent<ConditionalDialoguesCaller>();
    }

    public override void Interagir(Player player)
    {
        npc.VirarNaDirecao(player.transform.position);

        switch(tipoDialogo)
        {
            case TipoDialogo.Unico:
                MostrarDialogo(dialogo, player);
                break;

            case TipoDialogo.Condicional:
                MostrarDialogo(conditionalDialoguesCaller.GetDialogue(), player);
                break;
        }
    }

    protected void MostrarDialogo(DialogueObject dialogo, Player player)
    {
        dialogueActivator.ShowDialogue(dialogo, player.DialogueUI);
    }
}

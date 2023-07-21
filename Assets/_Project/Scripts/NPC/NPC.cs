using BergamotaDialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class NPC : EntityModel
{
    //Managers
    private DialogueUI dialogueUI;

    //Componentes
    [SerializeField] private NPCAnimation animacao;
    private NPCMovement npcMovement;

    //Getters
    public DialogueUI DialogueUI => dialogueUI;
    public NPCAnimation Animacao => animacao;
    public NPCMovement NPCMovement => npcMovement;

    private void Awake()
    {
        //Managers
        dialogueUI = DialogueUI.Instance;

        //Componentes
        npcMovement = GetComponent<NPCMovement>();
    }

    public override void SetDirection(Direction direcao)
    {
        base.SetDirection(direcao);
        animacao.SetDirection(direcao);
    }
}

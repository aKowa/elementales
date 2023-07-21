using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BergamotaDialogueSystem;

public class ItemOnGround : Interagivel
{
    //Componentes
    private DialogueUI dialogueUI;
    private DialogueActivator dialogueActivator;

    //Variaveis
    [SerializeField] private Item item;
    [SerializeField] private int quantidade;

    private void Awake()
    {
        dialogueActivator = GetComponent<DialogueActivator>();
        dialogueUI = FindObjectOfType<DialogueUI>();
    }

    public override void Interagir(Player player)
    {
        dialogueActivator.ShowDialogue(dialogueUI);
        player.PlayerData.Inventario.AddItem(item, quantidade);
        gameObject.SetActive(false);
    }
}

using BergamotaDialogueSystem;
using BergamotaLibrary;
using LumenSection.LevelLinker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueActivator))]
public class Porta : Interagivel
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private Animator animacaoDeInteracao;

    private Gateway gateway;
    private DialogueActivator dialogueActivator;

    //Variaveis
    [Header("Variaveis")]
    [SerializeField] private Item itemParaAbrir;

    [Header("Dialogos")]
    [SerializeField] private DialogueObject dialogoPortaTrancada;

    private void Awake()
    {
        gateway = GetComponent<Gateway>();
        dialogueActivator = GetComponent<DialogueActivator>();

        NaAreaDeInteracao(false);
    }

    public override void Interagir(Player player)
    {
        if(itemParaAbrir != null)
        {
            if (PlayerData.Instance.Inventario.GetQuantidadeDoItem(itemParaAbrir) > 0)
            {
                gateway.FazerTransicao(player);
            }
            else
            {
                dialogueActivator.ShowDialogue(dialogoPortaTrancada, DialogueUI.Instance);
            }
        }
        else
        {
            gateway.FazerTransicao(player);
        }
    }

    public override void NaAreaDeInteracao(bool estaNaArea)
    {
        animacaoDeInteracao.gameObject.SetActive(estaNaArea);
    }
}

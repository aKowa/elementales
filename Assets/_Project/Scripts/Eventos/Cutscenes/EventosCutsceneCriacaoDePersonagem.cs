using BergamotaDialogueSystem;
using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EventosCutsceneCriacaoDePersonagem : EventosCutscene
{
    //Componentes
    private MenuCriacaoDePersonagemController menuCriacaoDePersonagemController;
    private Player player;

    protected override void OnAwake()
    {
        menuCriacaoDePersonagemController = FindObjectOfType<MenuCriacaoDePersonagemController>();
        player = FindObjectOfType<Player>();

        menuCriacaoDePersonagemController.EventoInformacoesAtualizadas.AddListener(InformacoesSetadas);
    }

    public void AbrirMenuDeCriacao()
    {
        cutscene.PausarCutscene();

        menuCriacaoDePersonagemController.IniciarMenu();
    }

    private void InformacoesSetadas()
    {
        cutscene.ResumirCutscene();
        player.AtualizarSprite();

        DialogueUI.Instance.SetPlaceholderDeTexto("%player", PlayerData.Instance.GetPlayerName);

        player.SetDirection(EntityModel.Direction.Right);
    }
}

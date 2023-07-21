using BergamotaDialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventosCutsceneEscolhaMonstroInicial : EventosCutscene
{
    //Componentes
    private MenuDeEscolhaDoElemonInicialController menuDeEscolhaDoElemonInicial;
    private Player player;

    //Variaveis
    [Header("Item pra Dar pro Jogador")] 
    [SerializeField] private List<ItemEQuantidade> itens;

    protected override void OnAwake()
    {
        menuDeEscolhaDoElemonInicial = FindObjectOfType<MenuDeEscolhaDoElemonInicialController>();
        player = FindObjectOfType<Player>();

        menuDeEscolhaDoElemonInicial.EventoMonstroEscolhido.AddListener(MonstroEscolhido);
    }

    public void AbrirMenuDeEscolha()
    {
        menuDeEscolhaDoElemonInicial.IniciarMenu();
        menuDeEscolhaDoElemonInicial.BotoesAtivos(false);
    }

    public void AtivarBotoesDoMenu()
    {
        cutscene.PausarCutscene();

        menuDeEscolhaDoElemonInicial.BotoesAtivos(true);
    }

    public void MonstroEscolhido(Monster monstro)
    {
        cutscene.ResumirCutscene();

        DialogueUI.Instance.SetPlaceholderDeTexto("%monstro", monstro.MonsterData.GetName);

        player.SetDirection(EntityModel.Direction.Left);

        itens.ForEach(itemEQuantidade => PlayerData.Instance.Inventario.AddItem(itemEQuantidade.item, itemEQuantidade.quantidade));
    }

    [System.Serializable]
    private struct ItemEQuantidade
    {
        public Item item;
        public int quantidade;
    }
}
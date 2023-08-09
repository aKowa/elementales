using BergamotaDialogueSystem;
using BergamotaLibrary;
using System.Collections.Generic;
using UnityEngine;
public class ObjetoInteragivelComItem : Interagivel
{
    //Componentes
    protected DialogueActivator dialogueActivator;
    protected BoxCollider2D boxCollider2D;
    protected Animator animator;

    [Header("Dialogos")]
    [SerializeField] protected DialogueObject dialogoPossuiItem, dialogoNaoPossuiItem;

    //Variaveis
    [Header("Variaveis")]
    [SerializeField] protected List<Item> itemNecessario = new List<Item>();
    [Header("Som")]
    [SerializeField] protected AudioClip somParaTocarAoInteragir;

    [Space(10)]

    [SerializeField] protected string nomeAnimacao;
    [SerializeField] protected bool rodarAnimacao;


    protected void Awake()
    {
        Iniciar();
    }

    protected virtual void Iniciar()
    {
        dialogueActivator = GetComponent<DialogueActivator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();

        boxCollider2D.enabled = true;
    }

    public override void Interagir(Player player)
    {
        InteragirComItem(player, VerificarSePossuiItem(player));
    }

    public bool VerificarSePossuiItem(Player player)
    {
        int quantidadeItensNecessariosPlayerPossui = 0;
        foreach (var itemPlayer in player.PlayerData.Inventario.ItensChave)
        {
            foreach (Item itenChave in itemNecessario)
            {
                if (itenChave == null)
                    continue;

                if (itemPlayer.Item.ID == itenChave.ID)
                {
                    quantidadeItensNecessariosPlayerPossui ++;
                }
            }
        }
        return quantidadeItensNecessariosPlayerPossui >= itemNecessario.Count;
    }

    public virtual void InteragirComItem(Player player, bool possuiItem)
    {
        if (!possuiItem)
        {
            dialogueActivator.ShowDialogue(dialogoNaoPossuiItem, player.DialogueUI);
            return;
        }

        dialogueActivator.ShowDialogue(dialogoPossuiItem, player.DialogueUI);
        boxCollider2D.enabled = false;
        tocarSom();

        if (rodarAnimacao == true)
        {
            animator.Play(nomeAnimacao);
        }
    }
    protected void tocarSom()
    {
        SoundManager.instance.TocarSom(somParaTocarAoInteragir);
    }
}

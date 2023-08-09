using BergamotaLibrary;
using System.Collections;
using BergamotaDialogueSystem;
using UnityEngine;

public class Bau : Interagivel
{
    //Componentes
    private Animator animacao;
    private EntregarItem entregarItem;

    //Variaveis
    [SerializeField] private ListaDeFlags listaDeFlags;
    [SerializeField] private string nomeDaFlag;

    //Para baus fechados
    [Header("Baus Trancados")]
    [SerializeField] private Item chaveQueAbre;
    [SerializeField] private DialogueObject dialogoNaoTemChave;

    //Para baus com Ads
    [Header("Baus de ADS")] 
    [SerializeField] private EntregarItem stashSecundario;
    private DialogueActivator dialogueActivator;

    [Space(10)]
    [SerializeField] private bool rodarAnimacao = true;
    [SerializeField] private bool desaparecerAposPego = false;

    [Header("Som")]
    [SerializeField] private AudioClip somAbrirBau;
    private bool foiPego;

    private void Awake()
    {
        //Componentes
        animacao = GetComponent<Animator>();
        entregarItem = GetComponent<EntregarItem>();
        dialogueActivator = stashSecundario?.GetComponent<DialogueActivator>();
        animacao.enabled = rodarAnimacao;

        ConferirSeFoiPego();
    }

    private void OnEnable()
    {
        StartCoroutine(AtualizarParametrosNoOnEnable());
    }

    public override void Interagir(Player player)
    {
        if (foiPego) return;
        
        if (chaveQueAbre)
        {
            if (VerificarSeTemChave(player) == false)
            {
                DialogueUI.Instance.ShowDialogue(dialogoNaoTemChave);
                return;
            }
            else
            {
                GastarChave(player);
            }
        }
        SoundManager.instance.TocarSomIgnorandoPause(somAbrirBau);

        entregarItem.EntregarItens();

        Flags.SetFlag(listaDeFlags.name, nomeDaFlag, true);

        ConferirSeFoiPego();

        if(stashSecundario)
        {
            DarRecompensaSecundaria();
        }
    }

    private void GastarChave(Player player)
    {
        Inventario inventario = player.PlayerData.Inventario;

        inventario.RemoverItem(chaveQueAbre, 1);
    }
    
    private IEnumerator AbrirDialogoCorrotina(DialogueObject dialogo)
    {
        yield return new WaitUntil(() => DialogueUI.Instance.IsOpen == false);

        dialogueActivator.ShowDialogue(dialogo, DialogueUI.Instance);
    }

    private bool VerificarSeTemChave(Player player)
    {
        foreach (ItemHolder itemPlayer in player.PlayerData.Inventario.ItensChave)
        {
            if (itemPlayer.Item.ID == chaveQueAbre.ID)
            {
                return true;
            }
        }

        return false;
    }   
    private void DarRecompensaSecundaria()
    {
        //stashSecundario.EntregarItens();
    }

    private void ConferirSeFoiPego()
    {
        foiPego = Flags.GetFlag(listaDeFlags.name, nomeDaFlag);

        if(rodarAnimacao == true)
        {
            if (foiPego == false)
            {
                animacao.Play("Fechado");
            }
            else
            {
                animacao.Play("Aberto");
            }
        }

        if (desaparecerAposPego)
        {
            if (foiPego == true)
            {
                gameObject.SetActive(false);
            }
        }
    }

    protected virtual IEnumerator AtualizarParametrosNoOnEnable()
    {
        //Esperar o fim da frame pra nao acontecer junto com o Awake
        yield return null;

        ConferirSeFoiPego();
    }
}

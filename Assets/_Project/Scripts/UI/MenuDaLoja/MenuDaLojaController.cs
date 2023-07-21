using BergamotaDialogueSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuDaLojaController : ViewController
{
    //Constantes
    public const float modificadorItemParaVenda = 0.5f;

    //Componentes
    [Header("Componentes")]
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;
    [SerializeField] private RectTransform fundoDialogo;
    [SerializeField] private TMP_Text nomeDaLoja;
    [SerializeField] private LojaInfo guiaDaLoja;
    [SerializeField] private LojaInfo[] guiasDoInventario;
    [SerializeField] private Transform botoesGuiaHolder;
    [SerializeField] private RectTransform menuOpcoes;

    [Header("Dinheiro")]
    [SerializeField] private TMP_Text dinheiroTexto;
    [SerializeField] private TMP_Text dinheiroOpcoesTexto;

    [Header("Item Info")]
    [SerializeField] private TMP_Text nomeItem;
    [SerializeField] private TMP_Text descricaoItem;
    [SerializeField] private Image imagemItem;

    [Header("Compra/Venda Info")]
    [SerializeField] private TMP_Text precoTotalTexto;
    [SerializeField] private TMP_Text quantidadeItemTexto;
    [SerializeField] private TMP_Text quantidadeItensNaBagTexto;
    [SerializeField] private TMP_Text textoBotaoComprarOuVender;
    [SerializeField] private Image imagemItemOpcoes;
    [SerializeField] private ButtonSelectionEffect botaoMenos;
    [SerializeField] private ButtonSelectionEffect botaoMais;

    [Header("Variaveis Padroes")]
    [SerializeField] private string nomeDaLojaDeCompra;
    [SerializeField] private string nomeDaLojaDeVenda;
    [SerializeField] private string textoComprar;
    [SerializeField] private string textoVender;

    [Header("Dialogos")]
    [SerializeField] private DialogueObject dialogoNaoTemDinheiroSuficiente;
    [SerializeField] private DialogueObject dialogoNaoTemEspacoNoInventario;

    private Inventario inventario;
    private DialogueActivator dialogueActivator;
    private DialogueUI dialogueUI;

    //Enums
    public enum TipoLoja { Compra, Venda }

    //Variaveis
    protected List<BotaoGuia> botoesGuia = new List<BotaoGuia>();
    protected ItemSlotLoja itemSlotAtual;

    private InventarioLoja inventarioLojaAtual;
    private TipoLoja tipoLojaAtual;

    private List<ItemHolder> itensDaLoja = new List<ItemHolder>();

    private int precoItemBase;
    private int precoTotal;
    private int quantidadeItem;
    private int quantidadeItensNaBag;

    protected override void OnAwake()
    {
        //Componentes
        dialogueActivator = GetComponent<DialogueActivator>();
        dialogueUI = FindObjectOfType<DialogueUI>();

        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);
        fundoDialogo.gameObject.SetActive(false);
        menuOpcoes.gameObject.SetActive(false);

        //Variaveis
        precoItemBase = 0;
        precoTotal = 0;
        quantidadeItem = 0;
        quantidadeItensNaBag = 0;

        int i = 0;
        foreach (BotaoGuia botaoGuia in botoesGuiaHolder.GetComponentsInChildren<BotaoGuia>(true))
        {
            botaoGuia.Indice = i;
            botaoGuia.EventoAbrirGuia.AddListener(TrocarGuia);

            botoesGuia.Add(botaoGuia);

            i++;
        }

        guiaDaLoja.EventoItemSelecionado.AddListener(AbrirMenuItem);

        foreach (LojaInfo guia in guiasDoInventario)
        {
            guia.EventoItemSelecionado.AddListener(AbrirMenuItem);
        }
    }

    protected override void OnStart()
    {
        //Componentes
        inventario = PlayerData.Instance.Inventario;
    }

    public override void OnOpen()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(true);

        BergamotaLibrary.PauseManager.Pausar(true);
    }

    protected override void OnClose()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        ResetarInformacoes();

        guiaDaLoja.ResetarItemSlots();

        foreach (LojaInfo inventarioInfo in guiasDoInventario)
        {
            inventarioInfo.ResetarItemSlots();
        }

        BergamotaLibrary.PauseManager.Pausar(false);
    }

    public void IniciarMenu(InventarioLoja inventarioLoja, TipoLoja tipoLoja)
    {
        OpenView();

        inventarioLojaAtual = inventarioLoja;
        tipoLojaAtual = tipoLoja;

        CriarItensDaLoja();

        TrocarGuia(0);
        AtualizarInformacoes();

        switch(tipoLojaAtual)
        {
            case TipoLoja.Compra:
                nomeDaLoja.text = nomeDaLojaDeCompra;
                break;

            case TipoLoja.Venda:
                nomeDaLoja.text = nomeDaLojaDeVenda;
                break;
        }

        fundoDialogo.gameObject.SetActive(false);
    }

    private void ResetarInformacoes()
    {
        dinheiroTexto.text = string.Empty;
        dinheiroOpcoesTexto.text = string.Empty;

        nomeItem.text = string.Empty;
        descricaoItem.text = string.Empty;
        imagemItem.sprite = null;

        itensDaLoja.Clear();
    }

    private void CriarItensDaLoja()
    {
        itensDaLoja.Clear();

        for(int i = 0; i < inventarioLojaAtual.ItensDaLoja.Length; i++)
        {
            itensDaLoja.Add(new ItemHolder(inventarioLojaAtual.ItensDaLoja[i], 1));
        }

        itensDaLoja = itensDaLoja.OrderBy(item => item.Item.Efeito).ToList();
    }

    private void AtualizarInformacoes()
    {
        bool itensParaVender = (tipoLojaAtual == TipoLoja.Venda);

        switch(tipoLojaAtual)
        {
            case TipoLoja.Compra:
                guiaDaLoja.AtualizarInformacoes(itensDaLoja, inventario, itensParaVender);
                break;

            case TipoLoja.Venda:

                for (int i = 0; i < guiasDoInventario.Length; i++)
                {
                    guiasDoInventario[i].AtualizarInformacoes(itensDaLoja, inventario, itensParaVender);
                }

                break;
        }

        AtualizarDinheiro();
    }

    private void AtualizarDinheiro()
    {
        dinheiroTexto.text = "$ " + inventario.Dinheiro.ToString();
        dinheiroOpcoesTexto.text = "$ " + inventario.Dinheiro.ToString();
    }

    public void TrocarGuia(int indice)
    {
        switch(tipoLojaAtual)
        {
            case TipoLoja.Compra:

                guiaDaLoja.gameObject.SetActive(true);

                botoesGuiaHolder.gameObject.SetActive(false);
                for (int i = 0; i < guiasDoInventario.Length; i++)
                {
                    guiasDoInventario[i].gameObject.SetActive(false);
                    botoesGuia[i].Selecionado(false);
                }

                break;

            case TipoLoja.Venda:

                guiaDaLoja.gameObject.SetActive(false);

                botoesGuiaHolder.gameObject.SetActive(true);
                for (int i = 0; i < guiasDoInventario.Length; i++)
                {
                    if (i == indice)
                    {
                        guiasDoInventario[i].gameObject.SetActive(true);
                        botoesGuia[i].Selecionado(true);
                    }
                    else
                    {
                        guiasDoInventario[i].gameObject.SetActive(false);
                        botoesGuia[i].Selecionado(false);
                    }
                }

                break;
        }
    }

    private void AbrirMenuItem(ItemSlotLoja itemSlot)
    {
        itemSlotAtual = itemSlot;

        AbrirMenuOpcoes();
    }

    private void AbrirMenuOpcoes()
    {
        menuOpcoes.gameObject.SetActive(true);

        if (itemSlotAtual != null)
        {
            itemSlotAtual.Selecionado(true);

            AtualizarItemInfo();

            IniciarOpcoesCompraOuVenda();
        }
        else
        {
            Debug.LogWarning("O menu de opcoes tentou ser aberto sem um ItemSlotAtual!");
        }
    }

    public void FecharMenuOpcoes()
    {
        menuOpcoes.gameObject.SetActive(false);

        itemSlotAtual?.Selecionado(false);
    }

    protected void AtualizarItemInfo()
    {
        if (itemSlotAtual == null)
        {
            return;
        }

        nomeItem.text = itemSlotAtual.ItemHolder.Item.Nome;
        descricaoItem.text = itemSlotAtual.ItemHolder.Item.Descricao;

        imagemItem.sprite = itemSlotAtual.ItemHolder.Item.Imagem;
        imagemItemOpcoes.sprite = itemSlotAtual.ItemHolder.Item.Imagem;

        itemSlotAtual.AtualizarInformacoes();
    }

    private void IniciarOpcoesCompraOuVenda()
    {
        switch(tipoLojaAtual)
        {
            case TipoLoja.Compra:
                IniciarOpcoesCompra();
                break;

            case TipoLoja.Venda:
                IniciarOpcoesVenda();
                break;
        }
    }

    private void IniciarOpcoesCompra()
    {
        precoItemBase = itemSlotAtual.ItemHolder.Item.Preco;
        quantidadeItem = 1;

        textoBotaoComprarOuVender.text = textoComprar;

        AtualizarQuantidadeItensNaBag();
        AtualizarInformacoesMenuOpcoes();
    }

    private void IniciarOpcoesVenda()
    {
        precoItemBase = (int)(itemSlotAtual.ItemHolder.Item.Preco * modificadorItemParaVenda);
        quantidadeItem = 1;

        textoBotaoComprarOuVender.text = textoVender;

        AtualizarQuantidadeItensNaBag();
        AtualizarInformacoesMenuOpcoes();
    }

    private void AtualizarQuantidadeItensNaBag()
    {
        quantidadeItensNaBag = inventario.GetQuantidadeDoItem(itemSlotAtual.ItemHolder.Item);

        quantidadeItensNaBagTexto.text = quantidadeItensNaBag.ToString();
    }

    private void AtualizarInformacoesMenuOpcoes()
    {
        precoTotal = precoItemBase * quantidadeItem;

        quantidadeItemTexto.text = quantidadeItem.ToString();
        precoTotalTexto.text = "$ " + precoTotal.ToString();

        switch (tipoLojaAtual)
        {
            case TipoLoja.Compra:
                botaoMais.interactable = (quantidadeItem < Inventario.quantidadeMaxItem - quantidadeItensNaBag);
                break;

            case TipoLoja.Venda:
                botaoMais.interactable = (quantidadeItem < quantidadeItensNaBag);
                break;
        }

        botaoMenos.interactable = (quantidadeItem > 1);
    }

    public void SomarQuantidadeItem()
    {
        switch(tipoLojaAtual)
        {
            case TipoLoja.Compra:

                if(quantidadeItem < Inventario.quantidadeMaxItem - quantidadeItensNaBag)
                {
                    quantidadeItem++;

                    AtualizarInformacoesMenuOpcoes();
                }

                break;

            case TipoLoja.Venda:

                if(quantidadeItem < quantidadeItensNaBag)
                {
                    quantidadeItem++;

                    AtualizarInformacoesMenuOpcoes();
                }

                break;
        }
    }

    public void SubtrairQuantidadeItem()
    {
        if(quantidadeItem > 1)
        {
            quantidadeItem--;

            AtualizarInformacoesMenuOpcoes();
        }
    }

    public void ComprarOuVenderItem()
    {
        switch(tipoLojaAtual)
        {
            case TipoLoja.Compra:
                ComprarItem();
                break;

            case TipoLoja.Venda:
                VenderItem();
                break;
        }
    }

    private void ComprarItem()
    {
        if(quantidadeItensNaBag >= Inventario.quantidadeMaxItem)
        {
            AbrirDialogo(dialogoNaoTemEspacoNoInventario);
            return;
        }

        if(inventario.Dinheiro >= precoTotal)
        {
            inventario.AddItem(itemSlotAtual.ItemHolder.Item, quantidadeItem);
            inventario.Dinheiro -= precoTotal;

            AtualizarDinheiro();
            FecharMenuOpcoes();
        }
        else
        {
            AbrirDialogo(dialogoNaoTemDinheiroSuficiente);
        }
    }

    private void VenderItem()
    {
        inventario.Dinheiro += precoTotal;

        RemoveItem(itemSlotAtual.ItemHolder.Item, quantidadeItem);

        AtualizarDinheiro();
        FecharMenuOpcoes();
    }

    public void AbrirDialogo(DialogueObject dialogo)
    {
        dialogueUI.SetPlaceholderDeTexto("%player", PlayerData.Instance.GetPlayerName);
        dialogueUI.SetPlaceholderDeTexto("%item", itemSlotAtual.ItemHolder.Item.Nome);

        dialogueActivator.ShowDialogue(dialogo, dialogueUI);

        fundoDialogo.gameObject.SetActive(true);

        StartCoroutine(DialogoAberto(dialogueUI));
    }

    private void FecharDialogo()
    {
        fundoDialogo.gameObject.SetActive(false);
    }

    public void RemoveItem(Item item, int quantidade)
    {
        inventario.RemoverItem(item, quantidade);

        if (itemSlotAtual.ItemHolder.Quantidade <= 0)
        {
            AtualizarInformacoes();
            itemSlotAtual = null;
        }
    }

    private IEnumerator DialogoAberto(DialogueUI dialogueUI)
    {
        while (dialogueUI.IsOpen == true)
        {
            yield return null;
        }

        FecharDialogo();
    }
}

using BergamotaDialogueSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuBagController : ViewController
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private TMP_Text nomeGuia;
    [SerializeField] private InventarioInfo[] guias;
    [SerializeField] private GameObject monstroSlotBase;
    [SerializeField] private Transform botoesGuiaHolder;
    [SerializeField] private Transform monstroSlotsHolder;
    [SerializeField] private RectTransform menuOpcoes;
    [SerializeField] private RectTransform menuOpcoesSorting;
    [SerializeField] private RectTransform fundoEscolhaMonstros;
    [SerializeField] private RectTransform fundoDialogo;
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;
    [SerializeField] protected RectTransform fundoBloqueadorDeAcoes;

    [Header("Item Info")]
    [SerializeField] private TMP_Text nomeItem;
    [SerializeField] private TMP_Text descricaoItem;
    [SerializeField] private TMP_Text quantidadeItem;
    [SerializeField] private Image imagemItem;

    [Header("Menus")]
    [SerializeField] private MenuDeTipos menuDeTipos;

    private Inventario inventario;
    private DialogueActivator dialogueActivator;
    private DialogueUI dialogueUI;
    private MonsterLearnAttack telaAprenderAtaque;

    //Variaveis
    [Header("Dialogos")]
    [SerializeField] protected DialogueObject dialogoNaoPodeUsarItem;

    protected List<BotaoGuia> botoesGuia = new List<BotaoGuia>();
    protected List<MonstroSlotBag> monstroSlots = new List<MonstroSlotBag>();

    protected ItemSlot itemSlotAtual;
    protected MonstroSlotBag monstroSlotAtual;

    //Getters
    public MonstroSlotBag MonstroSlotAtual => monstroSlotAtual;
    public DialogueUI DialogueUI => dialogueUI;
    public MonsterLearnAttack TelaAprenderAtaque => telaAprenderAtaque;

    protected override void OnAwake()
    {
        dialogueActivator = GetComponent<DialogueActivator>();
        dialogueUI = DialogueUI.Instance;
        telaAprenderAtaque = FindObjectOfType<MonsterLearnAttack>();

        monstroSlotBase.SetActive(false);
        menuOpcoes.gameObject.SetActive(false);
        menuOpcoesSorting.gameObject.SetActive(false);
        fundoEscolhaMonstros.gameObject.SetActive(false);
        fundoDialogo.gameObject.SetActive(false);
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);
        fundoBloqueadorDeAcoes.gameObject.SetActive(false);

        onOpen += IniciarMenu;

        int i = 0;
        foreach (BotaoGuia botaoGuia in botoesGuiaHolder.GetComponentsInChildren<BotaoGuia>(true))
        {
            botaoGuia.Indice = i;
            botaoGuia.EventoAbrirGuia.AddListener(TrocarGuia);

            botoesGuia.Add(botaoGuia);

            i++;
        }

        foreach (InventarioInfo guia in guias)
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
    }

    protected override void OnClose()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        FecharEscolhaDeMonstros();
        FecharMenuOpcoes();
        FecharMenuOpcoesSorting();

        ResetarInformacoes();

        ResetarMonstroSlots();

        foreach(InventarioInfo inventarioInfo in guias)
        {
            inventarioInfo.ResetarItemSlots();
        }
    }

    private void IniciarMenu()
    {
        ResetarMonstroSlots();

        for (int i = 0; i < inventario.MonsterBag.Count; i++)
        {
            MonstroSlotBag monstroSlot = Instantiate(monstroSlotBase).GetComponent<MonstroSlotBag>();
            monstroSlot.GetComponent<RectTransform>().SetParent(monstroSlotsHolder.transform, false);
            monstroSlot.gameObject.SetActive(true);

            monstroSlot.EventoSelecionado.AddListener(UsarItemNoMonstro);

            monstroSlot.Ativado(false);
            monstroSlot.Monstro = inventario.MonsterBag[i];

            monstroSlot.AtualizarInformacoes();

            monstroSlots.Add(monstroSlot);
        }

        TrocarGuia(0);
        AtualizarInformacoes();

        fundoDialogo.gameObject.SetActive(false);
        fundoBloqueadorDeAcoes.gameObject.SetActive(false);
    }

    private void ResetarMonstroSlots()
    {
        foreach (MonstroSlotBag slot in monstroSlots)
        {
            Destroy(slot.gameObject);
        }

        monstroSlots.Clear();
    }

    private void ResetarInformacoes()
    {
        nomeItem.text = string.Empty;
        descricaoItem.text = string.Empty;
        quantidadeItem.text = string.Empty;
        imagemItem.sprite = null;
    }

    private void AtualizarInformacoes()
    {
        for (int i = 0; i < guias.Length; i++)
        {
            guias[i].AtualizarInformacoes(inventario);
        }
    }

    private void TrocarGuia(int indice)
    {
        for (int i = 0; i < guias.Length; i++)
        {
            if (i == indice)
            {
                guias[i].gameObject.SetActive(true);
                nomeGuia.text = guias[i].NomeDaGuia;
                botoesGuia[i].Selecionado(true);
            }
            else
            {
                guias[i].gameObject.SetActive(false);
                botoesGuia[i].Selecionado(false);
            }
        }
    }

    private void AbrirMenuItem(ItemSlot itemSlot)
    {
        itemSlotAtual = itemSlot;

        AbrirMenuOpcoes();
    }

    private void AbrirMenuOpcoes()
    {
        menuOpcoes.gameObject.SetActive(true);

        if(itemSlotAtual != null)
        {
            itemSlotAtual.Selecionado(true);

            AtualizarItemInfo();
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
        quantidadeItem.text = itemSlotAtual.ItemHolder.Quantidade.ToString();
        imagemItem.sprite = itemSlotAtual.ItemHolder.Item.Imagem;

        itemSlotAtual.AtualizarInformacoes();
    }

    public void AbrirMenuOpcoesSorting()
    {
        menuOpcoesSorting.gameObject.SetActive(true);
    }

    public void FecharMenuOpcoesSorting()
    {
        menuOpcoesSorting.gameObject.SetActive(false);
    }

    public void SortItensByName()
    {
        inventario.SortItensByName();
        AtualizarInformacoes();
    }

    public void SortItensByType()
    {
        inventario.SortItensByType();
        AtualizarInformacoes();
    }

    public void SortItensNewestLast()
    {
        inventario.SortItensNewestLast();
        AtualizarInformacoes();
    }

    public void AbrirEscolhaDeMonstros()
    {
        menuOpcoes.gameObject.SetActive(false);

        fundoEscolhaMonstros.gameObject.SetActive(true);

        SetarAtivacaoEscolhaDeMonstros();
    }

    public void FecharEscolhaDeMonstros()
    {
        menuOpcoes.gameObject.SetActive(true);

        fundoEscolhaMonstros.gameObject.SetActive(false);

        foreach (MonstroSlotBag slot in monstroSlots)
        {
            slot.Ativado(false);
        }
    }

    protected virtual void SetarAtivacaoEscolhaDeMonstros()
    {
        foreach (MonstroSlotBag slot in monstroSlots)
        {
            slot.Ativado(itemSlotAtual.ItemHolder.Item.EfeitoForaDaBatalha.PodeUsarItemNoMonstro(slot.Monstro));
        }
    }

    public virtual void UsarItemAtual()
    {
        Item item = itemSlotAtual.ItemHolder.Item;

        if (item.EfeitoForaDaBatalha == null)
        {
            AbrirDialogo(dialogoNaoPodeUsarItem);
        }
        else
        {
            item.EfeitoForaDaBatalha.UsarItem(this, item);
            AtualizarItemInfo();

            if(itemSlotAtual == null)
            {
                FecharMenuOpcoes();
            }
            else if(itemSlotAtual.ItemHolder.Quantidade <= 0)
            {
                FecharMenuOpcoes();
            }
        }
    }

    protected virtual void UsarItemNoMonstro(MonstroSlotBag monstroSlot)
    {
        Item item = itemSlotAtual.ItemHolder.Item;

        monstroSlotAtual = monstroSlot;

        item.EfeitoForaDaBatalha.UsarItemNoMonstro(this, monstroSlotAtual.Monstro, item);
        AtualizarItemInfo();

        item.EfeitoForaDaBatalha.EfeitoMonstroSlot(this);
    }

    public void AtualizarEscolhaDeMonstros()
    {
        if (itemSlotAtual == null)
        {
            FecharEscolhaDeMonstros();
            FecharMenuOpcoes();
            return;
        }
        else if (itemSlotAtual.ItemHolder.Quantidade <= 0)
        {
            FecharEscolhaDeMonstros();
            FecharMenuOpcoes();
            return;
        }

        SetarAtivacaoEscolhaDeMonstros();
    }

    public void AtualizarInformacoesMonstros()
    {
        MonstroSlotAtual.AtualizarInformacoes();

        AtualizarEscolhaDeMonstros();

        TerminouOEfeitoMonstroSlot();
    }

    public void AumentarBarraDeHPMonstroSlot()
    {
        StartCoroutine(AumentarBarraDeHPMonstroSlotCorrotina());
    }

    public void AumentarBarraDeManaMonstroSlot()
    {
        StartCoroutine(AumentarBarraDeManaMonstroSlotCorrotina());
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

    public void RemoveItem(Item item)
    {
        inventario.RemoverItem(item, 1);

        if (itemSlotAtual.ItemHolder.Quantidade <= 0)
        {
            AtualizarInformacoes();
            itemSlotAtual = null;
        }
    }

    public void AbrirTelaAprenderAtaque()
    {
        EnsinarHabilidade acaoEnsinarHabilidade = (EnsinarHabilidade) itemSlotAtual.ItemHolder.Item.EfeitoForaDaBatalha;

        telaAprenderAtaque.AbrirTelaAprenderAtaque(acaoEnsinarHabilidade.Ataque);
    }

    public void AbrirMenuDeTipos()
    {
        menuDeTipos.OpenView();
    }

    protected virtual void TerminouOEfeitoMonstroSlot()
    {
        //Usado pelo MenuBagBatalhaController
    }

    private IEnumerator DialogoAberto(DialogueUI dialogueUI)
    {
        while (dialogueUI.IsOpen == true)
        {
            yield return null;
        }

        FecharDialogo();
    }

    private IEnumerator AumentarBarraDeHPMonstroSlotCorrotina()
    {
        fundoBloqueadorDeAcoes.gameObject.SetActive(true);

        yield return monstroSlotAtual.GetComponent<MonstroSlotInfo>().BarraHP.AumentarHP(monstroSlotAtual.Monstro);

        fundoBloqueadorDeAcoes.gameObject.SetActive(false);

        AtualizarEscolhaDeMonstros();

        TerminouOEfeitoMonstroSlot();
    }

    private IEnumerator AumentarBarraDeManaMonstroSlotCorrotina()
    {
        fundoBloqueadorDeAcoes.gameObject.SetActive(true);

        yield return monstroSlotAtual.GetComponent<MonstroSlotInfo>().BarraMana.AumentarMana(monstroSlotAtual.Monstro);

        fundoBloqueadorDeAcoes.gameObject.SetActive(false);

        AtualizarEscolhaDeMonstros();

        TerminouOEfeitoMonstroSlot();
    }
}
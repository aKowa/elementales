using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuDaLojaIAPController : ViewController
{
    //Constantes
    public const float modificadorItemParaVenda = 0.5f;

    //Componentes
    [Header("Componentes")]
    [SerializeField] private GameObject botaoGuiaBase;
    [SerializeField] private GameObject guiaIAPBase;

    [Space(10)]

    [SerializeField] private Transform botoesGuiaHolder;
    [SerializeField] private Transform guiasHolder;
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;

    private List<LojaIAPInfo> guias = new List<LojaIAPInfo>();

    [Header("Menus")]
    [SerializeField] private MenuDeInformacoesDeItemIAP menuDeInformacoesDeItemIAP;

    //Variaveis
    [Header("Guias")]
    [SerializeField] private GuiaLojaIAP[] guiasLojaIAP;

    protected List<BotaoGuia> botoesGuia = new List<BotaoGuia>();

    //Getters
    public GuiaLojaIAP[] GuiasLojaIAP => guiasLojaIAP;

    protected override void OnAwake()
    {
        //Componentes
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        botaoGuiaBase.gameObject.SetActive(false);
        guiaIAPBase.gameObject.SetActive(false);

        CriarGuias();
    }

    public override void OnOpen()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(true);

        BergamotaLibrary.PauseManager.Pausar(true);
    }

    protected override void OnClose()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        BergamotaLibrary.PauseManager.Pausar(false);

        foreach (LojaIAPInfo guia in guias)
        {
            guia.ResetarItemSlots();
        }
    }

    public void IniciarMenu()
    {
        OpenView();

        foreach (LojaIAPInfo guia in guias)
        {
            guia.gameObject.SetActive(false);
        }

        AtualizarBotoesGuia();
        AtualizarInformacoes();

        int guiaDisponivel = 0;

        for (int i = 0; i < guiasLojaIAP.Length; i++)
        {
            if(guiasLojaIAP[i].ExibirGuia == true)
            {
                guiaDisponivel = i;
                break;
            }
        }

        TrocarGuia(guiaDisponivel);
    }

    private void CriarGuias()
    {
        int i = 0;

        foreach(GuiaLojaIAP guiaLojaIAP in guiasLojaIAP)
        {
            string nomeDaGuia = guiaLojaIAP.NomeDaGuia;

            //Botao
            BotaoGuia botaoGuia = Instantiate(botaoGuiaBase, botoesGuiaHolder).GetComponent<BotaoGuia>();
            botaoGuia.Indice = i;
            botaoGuia.GetComponentInChildren<TMP_Text>().text = nomeDaGuia;
            botaoGuia.EventoAbrirGuia.AddListener(TrocarGuia);

            botoesGuia.Add(botaoGuia);

            botaoGuia.gameObject.SetActive(true);

            //Guia
            LojaIAPInfo guia = Instantiate(guiaIAPBase, guiasHolder).GetComponent<LojaIAPInfo>();
            guia.InventarioLoja = guiaLojaIAP.InventarioLoja;
            guia.NomeDaGuia.text = nomeDaGuia;
            guia.EventoItemSelecionado.AddListener(AbrirMenuDeInformacoesDoItemIAP);

            guias.Add(guia);

            i++;
        }
    }

    private void AtualizarBotoesGuia()
    {
        for(int i = 0; i < guiasLojaIAP.Length; i++)
        {
            botoesGuia[i].gameObject.SetActive(guiasLojaIAP[i].ExibirGuia);
        }
    }

    private void AtualizarInformacoes()
    {
        foreach(LojaIAPInfo guia in guias)
        {
            guia.AtualizarInformacoes();
        }
    }

    private void TrocarGuia(int indice)
    {
        for (int i = 0; i < guias.Count; i++)
        {
            if (i == indice)
            {
                guias[i].gameObject.SetActive(true);
                botoesGuia[i].Selecionado(true);
            }
            else
            {
                guias[i].gameObject.SetActive(false);
                botoesGuia[i].Selecionado(false);
            }
        }
    }

    private void AbrirMenuDeInformacoesDoItemIAP(ItemSlotLojaIAP itemSlot)
    {
        menuDeInformacoesDeItemIAP.IniciarMenu(itemSlot.IAPButton, itemSlot.TituloProduto, itemSlot.DescricaoProduto, itemSlot.PrecoProduto, itemSlot.ImagemProduto);
    }

    [System.Serializable]
    public class GuiaLojaIAP
    {
        //Variaveis
        [SerializeField] private bool exibirGuia;

        [Space(10)]

        [SerializeField] private string nomeDaGuia;
        [SerializeField] private InventarioLojaIAP inventarioLoja;

        //Getters
        public bool ExibirGuia { get => exibirGuia; set => exibirGuia = value; }
        public string NomeDaGuia => nomeDaGuia;
        public InventarioLojaIAP InventarioLoja => inventarioLoja;
    }
}

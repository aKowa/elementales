using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuSummaryController : ViewController
{
    //Componentes
    [Header("Menus")]
    [SerializeField] private MonstroInfo[] guias;
    [SerializeField] private GameObject botaoMonstroBase;
    [SerializeField] private Transform botoesGuiaHolder;
    [SerializeField] private Transform botoesMonstroHolder;
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;

    [Header("Geral")]
    [SerializeField] private TMP_Text nomeGuia;
    [SerializeField] private TMP_Text nomeMonstro;
    [SerializeField] private TMP_Text levelMontro;
    [SerializeField] private Animator animatorMonstro;
    [SerializeField] private BergamotaLibrary.Animacao animacaoMonstro;

    private Inventario inventario;

    //Variaveis
    private List<BotaoGuia> botoesGuia = new List<BotaoGuia>();
    private List<BotaoMonstro> botoesMonstro = new List<BotaoMonstro>();

    private Monster monstroAtual;
    private int indiceMonstroAtual;
    private int contadorAnimacao;

    //Getters
    public Monster MonstroAtual => monstroAtual;

    protected override void OnAwake()
    {
        botaoMonstroBase.SetActive(false);
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        indiceMonstroAtual = 0;
        contadorAnimacao = 0;

        int i = 0;
        foreach (BotaoGuia botaoGuia in botoesGuiaHolder.GetComponentsInChildren<BotaoGuia>(true))
        {
            botaoGuia.Indice = i;
            botaoGuia.EventoAbrirGuia.AddListener(TrocarGuia);

            botoesGuia.Add(botaoGuia);

            i++;
        }

        onLateOpen += AtualizarInformacoes;
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

        ResetarInformacoes();

        ResetarBotoesMonstro();

        foreach(MonstroInfo monstroInfo in guias)
        {
            monstroInfo.ResetarInformacoes();
        }
    }

    public void IniciarMenu(int indiceMonstroAtual)
    {
        ResetarBotoesMonstro();

        OpenView();

        for (int i = 0; i < inventario.MonsterBag.Count; i++)
        {
            BotaoMonstro botaoMonstro = Instantiate(botaoMonstroBase).GetComponent<BotaoMonstro>();
            botaoMonstro.GetComponent<RectTransform>().SetParent(botoesMonstroHolder.transform, false);
            botaoMonstro.gameObject.SetActive(true);

            botaoMonstro.MenuSummaryController = this;
            botaoMonstro.Indice = i;

            botoesMonstro.Add(botaoMonstro);
        }

        TrocarMonstro(indiceMonstroAtual);
        TrocarGuia(0);
    }

    public void IniciarMenuSemBotoesMonstro(Monster monstro)
    {
        ResetarBotoesMonstro();

        OpenView();

        TrocarMonstro(monstro);
        TrocarGuia(0);
    }

    private void ResetarBotoesMonstro()
    {
        foreach (BotaoMonstro botao in botoesMonstro)
        {
            Destroy(botao.gameObject);
        }

        botoesMonstro.Clear();
    }

    private void ResetarInformacoes()
    {
        nomeMonstro.text = string.Empty;
        levelMontro.text = string.Empty;
        animatorMonstro.runtimeAnimatorController = null;
    }

    public void TrocarMonstro(int indiceMonstro)
    {
        indiceMonstroAtual = indiceMonstro;
        monstroAtual = inventario.MonsterBag[indiceMonstroAtual];

        nomeMonstro.text = monstroAtual.NickName;
        levelMontro.text = monstroAtual.Nivel.ToString();

        if (monstroAtual.MonsterData.Animator != null)
        {
            animatorMonstro.runtimeAnimatorController = monstroAtual.MonsterData.Animator;
        }
        else
        {
            Debug.LogWarning("O monstro " + monstroAtual.MonsterData.GetName + " nao possui um Animator Override Controller para ser usado!");
        }

        for (int i = 0; i < botoesMonstro.Count; i++)
        {
            if (i == indiceMonstroAtual)
            {
                botoesMonstro[i].Selecionado(true);
            }
            else
            {
                botoesMonstro[i].Selecionado(false);
            }
        }

        animacaoMonstro.TrocarAnimacao("Idle");
        contadorAnimacao = 0;

        AtualizarInformacoes();
    }

    public void TrocarMonstro(Monster monstro)
    {
        monstroAtual = monstro;

        nomeMonstro.text = monstroAtual.NickName;
        levelMontro.text = monstroAtual.Nivel.ToString();

        if (monstroAtual.MonsterData.Animator != null)
        {
            animatorMonstro.runtimeAnimatorController = monstroAtual.MonsterData.Animator;
        }
        else
        {
            Debug.LogWarning("O monstro " + monstroAtual.MonsterData.GetName + " nao possui um Animator Override Controller para ser usado!");
        }

        animacaoMonstro.TrocarAnimacao("Idle");
        contadorAnimacao = 0;

        AtualizarInformacoes();
    }

    private void AtualizarInformacoes()
    {
        for (int i = 0; i < guias.Length; i++)
        {
            guias[i].AtualizarInformacoes(monstroAtual);
        }
    }

    public void TrocarGuia(int indice)
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

    public void TrocarAnimacaoMonstro()
    {
        if (animacaoMonstro.AnimacaoAtual != "Idle")
        {
            return;
        }

        switch (contadorAnimacao)
        {
            case 0:
                contadorAnimacao = 1;
                animacaoMonstro.TrocarAnimacao("Atk");
                animacaoMonstro.ExecutarUmMetodoAposOFimDaAnimacao(MonstroAnimacaoIdle);
                break;

            case 1:
                contadorAnimacao = 0;
                animacaoMonstro.TrocarAnimacao("SpAtk");
                animacaoMonstro.ExecutarUmMetodoAposOFimDaAnimacao(MonstroAnimacaoIdle);
                break;

            default:
                contadorAnimacao = 0;
                animacaoMonstro.TrocarAnimacao("Idle");
                Debug.LogWarning("O contador de animacao estava com um valor incorreto!");
                break;
        }
    }

    private void MonstroAnimacaoIdle()
    {
        animacaoMonstro.TrocarAnimacao("Idle");
    }
}
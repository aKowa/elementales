using BergamotaLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelaAvaliarJogo : ViewController
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;

    [Space(10)]

    [SerializeField] private RectTransform botoesEstrelaHolder;

    //Variaveis
    [Header("Variaveis")]
    [SerializeField] [Range(1, 5)] private int valorInicialDaAvaliacao;

    [Header("Tempos")]
    [SerializeField] private int diasParaAparecerOPopupInicialmente = 1;
    [SerializeField] private int diasParaAparecerOPopupMaisTarde = 2;
    [SerializeField] private float horasParaAparecerOPopupInicialmente = 2;

    private List<BotaoGuia> botoesEstrela = new List<BotaoGuia>();

    private int avaliacao;

    protected override void OnAwake()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        int i = 0;
        foreach (BotaoGuia botaoEstrela in botoesEstrelaHolder.GetComponentsInChildren<BotaoGuia>(true))
        {
            botaoEstrela.Indice = i + 1;
            botaoEstrela.EventoAbrirGuia.AddListener(TrocarAvaliacao);

            botoesEstrela.Add(botaoEstrela);

            i++;
        }
    }

    public override void OnOpen()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(true);

        BergamotaLibrary.PauseManager.Pausar(true);

        IniciarMenu();
    }

    protected override void OnClose()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        BergamotaLibrary.PauseManager.Pausar(false);

        ResetarInformacoes();
    }

    public void MostrarTelaAposUmaBatalha()
    {
        GameManager.Instance.StartCoroutine(MostrarTelaQuandoAcabarTransicaoDeBatalhaCorrotina());
    }

    public void AvaliarSeVaiMostrarATela()
    {
        ConfiguracoesSave configuracoesSave = SaveManager.ConfiguracoesSaveAtual;

        DateTime dataTelaAvaliarJogo = SerializableDateTime.NewDateTime(configuracoesSave.dataTelaAvaliarJogo);

        switch(configuracoesSave.estadoTelaAvaliarJogo)
        {
            case ConfiguracoesSave.EstadoTelaAvaliarJogo.NaoViuPopup:

                if (DateTime.Now - dataTelaAvaliarJogo >= TimeSpan.FromDays(diasParaAparecerOPopupInicialmente) || TimeSpan.FromSeconds(PlayerData.TimePlayed) >= TimeSpan.FromHours(horasParaAparecerOPopupInicialmente))
                {
                    OpenView();
                }

                break;

            case ConfiguracoesSave.EstadoTelaAvaliarJogo.VerMaisTarde:

                if (DateTime.Now - dataTelaAvaliarJogo >= TimeSpan.FromDays(diasParaAparecerOPopupMaisTarde))
                {
                    OpenView();
                }

                break;
        }
    }

    private void IniciarMenu()
    {
        TrocarAvaliacao(valorInicialDaAvaliacao);
        SaveManager.ConfiguracoesSaveAtual.dataTelaAvaliarJogo = new SerializableDateTime(DateTime.Now);
    }

    private void ResetarInformacoes()
    {
        TrocarAvaliacao(valorInicialDaAvaliacao);
    }

    private void TrocarAvaliacao(int indice)
    {
        avaliacao = indice;

        for (int i = 0; i < botoesEstrela.Count; i++)
        {
            botoesEstrela[i].Selecionado(botoesEstrela[i].Indice > avaliacao);
        }
    }

    public void EnviarAvaliacao()
    {
        AbrirPaginaDaPlayStore();

        SaveManager.ConfiguracoesSaveAtual.estadoTelaAvaliarJogo = ConfiguracoesSave.EstadoTelaAvaliarJogo.NuncaMaisVer;

        CloseView();
    }

    public void PerguntarMaisTarde()
    {
        SaveManager.ConfiguracoesSaveAtual.estadoTelaAvaliarJogo = ConfiguracoesSave.EstadoTelaAvaliarJogo.VerMaisTarde;

        CloseView();
    }

    public void NaoPerguntarDeNovo()
    {
        SaveManager.ConfiguracoesSaveAtual.estadoTelaAvaliarJogo = ConfiguracoesSave.EstadoTelaAvaliarJogo.NuncaMaisVer;

        CloseView();
    }

    private void AbrirPaginaDaPlayStore()
    {
        Application.OpenURL("market://details?id=" + Application.identifier);
    }

    private IEnumerator MostrarTelaQuandoAcabarTransicaoDeBatalhaCorrotina()
    {
        yield return new WaitUntil(() => Transition.GetInstance().FazendoTransicao == true);
        yield return new WaitUntil(() => Transition.GetInstance().FazendoTransicao == false);

        AvaliarSeVaiMostrarATela();
    }
}

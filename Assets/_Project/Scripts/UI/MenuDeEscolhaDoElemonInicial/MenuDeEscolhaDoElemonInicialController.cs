using BergamotaDialogueSystem;
using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuDeEscolhaDoElemonInicialController : ViewController
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private TMP_Text descricaoMonstro;
    [SerializeField] private Transform botoesMonstroHolder;
    [SerializeField] private Image botaoConfirmar;
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;
    [SerializeField] private RectTransform fundoDialogo;

    private DialogueActivator dialogueActivator;

    [Header("Menus")]
    [SerializeField] private JanelaTrocarNickname janelaTrocarNickname;

    [Header("Variaveis Padroes")]
    [SerializeField] private DialogueObject dialogoEscolhaUmMonstro;
    [SerializeField] private DialogueObject dialogoConfirmarMonstro;
    [SerializeField] private DialogueObject dialogoQuerDarUmNicknameProMonstro;

    //Variaveis
    [Header("Flags do Monstro Inicial")]
    [SerializeField] protected ListaDeFlags flagsMonstroInicial;
    [SerializeField] protected string nomeDaFlagMonstro1;
    [SerializeField] protected string nomeDaFlagMonstro2;
    [SerializeField] protected string nomeDaFlagMonstro3;

    [Header("Variaveis")]
    [SerializeField] private int nivel;
    [SerializeField] private MonstroInicial[] monstros;

    private UnityEvent<Monster> eventoMonstroEscolhido = new UnityEvent<Monster>();

    private List<BotaoGuia> botoesMonstro = new List<BotaoGuia>();

    private MonsterData monstroAtual;
    private List<ComandoDeAtaque> ataquesAtuais;
    private Monster monstroCriado;

    //Getters
    public UnityEvent<Monster> EventoMonstroEscolhido => eventoMonstroEscolhido;

    protected override void OnAwake()
    {
        dialogueActivator = GetComponent<DialogueActivator>();

        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        int i = 0;
        foreach (BotaoGuia botaoMonstro in botoesMonstroHolder.GetComponentsInChildren<BotaoGuia>(true))
        {
            botaoMonstro.Indice = i;
            botaoMonstro.EventoAbrirGuia.AddListener(TrocarMonstro);

            botoesMonstro.Add(botaoMonstro);

            i++;
        }
    }

    public override void OnOpen()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(true);
    }

    protected override void OnClose()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        ResetarInformacoes();
    }

    public void IniciarMenu()
    {
        OpenView();

        ResetarInformacoes();

        BotoesAtivos(true);
    }

    public void FecharMenu()
    {
        CloseView();
    }

    private void ResetarInformacoes()
    {
        TrocarMonstro(-1);

        descricaoMonstro.text = string.Empty;
        monstroCriado = null;
    }

    public void BotoesAtivos(bool ativo)
    {
        for (int i = 0; i < botoesMonstro.Count; i++)
        {
            botoesMonstro[i].GetComponent<Image>().raycastTarget = ativo;
        }

        botaoConfirmar.raycastTarget = ativo;
    }

    private void TrocarMonstro(int indice)
    {
        if(indice >= 0)
        {
            monstroAtual = monstros[indice].Monstro;
            ataquesAtuais = monstros[indice].Ataques;
            descricaoMonstro.text = monstros[indice].Descricao;
        }
        else
        {
            monstroAtual = null;
            ataquesAtuais = null;
            descricaoMonstro.text = string.Empty;
        }

        for (int i = 0; i < botoesMonstro.Count; i++)
        {
            if (i == indice)
            {
                botoesMonstro[i].Selecionado(true);
            }
            else
            {
                botoesMonstro[i].Selecionado(false);
            }
        }
    }

    public void EscolherMonstro()
    {
        if(monstroAtual == null)
        {
            AbrirDialogo(dialogoEscolhaUmMonstro);
            return;
        }

        DialogueUI.Instance.SetPlaceholderDeTexto("%monstro", monstroAtual.GetName);
        AbrirDialogo(dialogoConfirmarMonstro);
    }

    public void ConfirmarEscolhaDoMonstro()
    {
        BotoesAtivos(false);

        StartCoroutine(SequenciaEscolheuMonstro());
    }

    public void AbrirTelaNickname()
    {
        janelaTrocarNickname.IniciarMenu(monstroCriado);
    }

    private void AbrirDialogo(DialogueObject dialogo)
    {
        dialogueActivator.ShowDialogue(dialogo, DialogueUI.Instance);

        fundoDialogo.gameObject.SetActive(true);

        StartCoroutine(DialogoAberto(DialogueUI.Instance));
    }

    private void FecharDialogo()
    {
        fundoDialogo.gameObject.SetActive(false);
    }

    private IEnumerator DialogoAberto(DialogueUI dialogueUI)
    {
        yield return new WaitUntil(() => dialogueUI.IsOpen == false);

        FecharDialogo();
    }

    private IEnumerator SequenciaEscolheuMonstro()
    {
        monstroCriado = new Monster(monstroAtual, nivel, ataquesAtuais);


        for(int i = 0; i < monstros.Length; i++)
        {
            if(monstroCriado.MonsterData.ID == monstros[i].Monstro.ID)
            {
                if(i == 0)
                {
                    Flags.SetFlag(flagsMonstroInicial.name, nomeDaFlagMonstro1, true);
                    Flags.SetFlag(flagsMonstroInicial.name, nomeDaFlagMonstro2, false);
                    Flags.SetFlag(flagsMonstroInicial.name, nomeDaFlagMonstro3, false);
                }
                else if(i == 1)
                {
                    Flags.SetFlag(flagsMonstroInicial.name, nomeDaFlagMonstro1, false);
                    Flags.SetFlag(flagsMonstroInicial.name, nomeDaFlagMonstro2, true);
                    Flags.SetFlag(flagsMonstroInicial.name, nomeDaFlagMonstro3, false);
                }
                else if(i == 2)
                {
                    Flags.SetFlag(flagsMonstroInicial.name, nomeDaFlagMonstro1, false);
                    Flags.SetFlag(flagsMonstroInicial.name, nomeDaFlagMonstro2, false);
                    Flags.SetFlag(flagsMonstroInicial.name, nomeDaFlagMonstro3, true);
                }

                break;
            }
        }

        yield return new WaitUntil(() => DialogueUI.Instance.IsOpen == false);

        DialogueUI.Instance.SetPlaceholderDeTexto("%monstro", monstroCriado.NickName);
        AbrirDialogo(dialogoQuerDarUmNicknameProMonstro);

        yield return new WaitUntil(() => DialogueUI.Instance.IsOpen == false);
        yield return new WaitUntil(() => janelaTrocarNickname.IsViewOpenned() == false);

        PlayerData.Instance.Inventario.AddMonsterToBag(monstroCriado, true);

        eventoMonstroEscolhido?.Invoke(monstroCriado);

        CloseView();
    }

    [System.Serializable]
    public struct MonstroInicial
    {
        //Variaveis
        [SerializeField] private MonsterData monstro;
        [SerializeField] private List<ComandoDeAtaque> ataques;
        [SerializeField][TextArea(3, 6)] private string descricao;

        //Getters
        public MonsterData Monstro => monstro;
        public List<ComandoDeAtaque> Ataques => ataques;
        public string Descricao => descricao;
    }
}

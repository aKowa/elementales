using BergamotaDialogueSystem;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class JanelaTrocarNickname : ViewController
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private TMP_InputField textoNomeNovo;
    [SerializeField] private Image imagemMonsto;
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;
    [SerializeField] private RectTransform fundoDialogo;

    private DialogueActivator dialogueActivator;

    [Header("Variaveis Padroes")]
    [SerializeField] private DialogueObject dialogoNomeInvalido;

    //Variaveis
    private UnityEvent eventoNomeTrocado = new UnityEvent();

    private Monster monstroAtual;

    //Getters
    public UnityEvent EventoNomeTrocado => eventoNomeTrocado;

    protected override void OnAwake()
    {
        dialogueActivator = GetComponent<DialogueActivator>();
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

    public void IniciarMenu(Monster monstro)
    {
        OpenView();

        ResetarInformacoes();

        monstroAtual = monstro;
        imagemMonsto.sprite = monstro.MonsterData.Miniatura;
    }

    public void FecharMenu()
    {
        CloseView();
    }

    private void ResetarInformacoes()
    {
        monstroAtual = null;
        textoNomeNovo.text = string.Empty;
        imagemMonsto.sprite = null;
    }

    public void SetarNome()
    {
        string novoNome = textoNomeNovo.text;

        if (VerificarNomeInvalido(novoNome) == false)
        {
            monstroAtual.NickName = novoNome;

            eventoNomeTrocado?.Invoke();
            FecharMenu();
        }
        else
        {
            AbrirDialogo(dialogoNomeInvalido);
        }
    }

    private bool VerificarNomeInvalido(string novoNome)
    {
        return string.IsNullOrWhiteSpace(novoNome);
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
}

using BergamotaDialogueSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MenuCriacaoDePersonagemController : ViewController
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private PlayerSO playerSO;
    [SerializeField] private TMP_InputField textoNomeNovo;
    [SerializeField] private Transform botoesSexoHolder;
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;
    [SerializeField] private RectTransform fundoDialogo;

    private DialogueActivator dialogueActivator;

    [Header("Variaveis Padroes")]
    [SerializeField] private DialogueObject dialogoNomeInvalido;

    //Variaveis
    private UnityEvent eventoInformacoesAtualizadas = new UnityEvent();

    private List<BotaoGuia> botoesSexo = new List<BotaoGuia>();

    private PlayerSO.Sexo novoSexo;

    //Getters
    public UnityEvent EventoInformacoesAtualizadas => eventoInformacoesAtualizadas;

    protected override void OnAwake()
    {
        dialogueActivator = GetComponent<DialogueActivator>();
        fundoDialogo.gameObject.SetActive(false);
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        int i = 0;
        foreach (BotaoGuia botaoSexo in botoesSexoHolder.GetComponentsInChildren<BotaoGuia>(true))
        {
            botaoSexo.Indice = i;
            botaoSexo.EventoAbrirGuia.AddListener(TrocarSexo);

            botoesSexo.Add(botaoSexo);

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
    }

    public void FecharMenu()
    {
        CloseView();
    }

    private void ResetarInformacoes()
    {
        textoNomeNovo.text = string.Empty;

        TrocarSexo(0);
    }

    public void SetarInformacoes()
    {
        string novoNome = textoNomeNovo.text;

        if (VerificarNomeInvalido(novoNome) == false)
        {
            playerSO.SetarInformacoes(novoNome, novoSexo);

            eventoInformacoesAtualizadas?.Invoke();
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

    public void TrocarSexo(int indice)
    {
        novoSexo = (PlayerSO.Sexo)indice;

        for (int i = 0; i < botoesSexo.Count; i++)
        {
            if (i == indice)
            {
                botoesSexo[i].Selecionado(true);
            }
            else
            {
                botoesSexo[i].Selecionado(false);
            }
        }
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

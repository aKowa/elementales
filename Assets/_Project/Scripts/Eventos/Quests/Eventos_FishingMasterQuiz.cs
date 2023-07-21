using BergamotaDialogueSystem;
using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eventos_FishingMasterQuiz : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private DialogueActivator dialogueActivator;

    [Header("Dialogos")]
    [SerializeField] private DialogueObject dialogoQuizCompleto;

    private NPC npc;
    private FlagSetter flagSetter;

    //Variaveis
    [Header("Variaveis")]
    [SerializeField] private string nomeDaFlagPodeIniciarQuiz;

    [Space(10)]

    [SerializeField] private MonsterType tipoAgua;

    [Space(10)]

    [SerializeField] private List<DialogueObject> perguntas;

    private List<DialogueObject> perguntasParaResponder = new List<DialogueObject>();

    private void Awake()
    {
        npc = GetComponent<NPC>();
        flagSetter = GetComponent<FlagSetter>();
    }

    private void OnEnable()
    {
        StartCoroutine(AtualizarFlagPodeIniciarQuizNoOnEnable());
    }

    private void AtualizarFlagPodeIniciarQuiz()
    {
        switch(PlayerData.MonsterBook.CapturouMonstroDoTipo(tipoAgua))
        {
            case true:
                flagSetter.SetFlagAsTrue(nomeDaFlagPodeIniciarQuiz);
                break;

            case false:
                flagSetter.SetFlagAsFalse(nomeDaFlagPodeIniciarQuiz);
                break;
        }
    }

    public void IniciarQuiz()
    {
        perguntasParaResponder.Clear();

        foreach(DialogueObject pergunta in perguntas)
        {
            perguntasParaResponder.Add(pergunta);
        }

        MostrarPergunta();
    }

    public void MostrarPergunta()
    {
        if(perguntasParaResponder.Count > 0)
        {
            int indice = UnityEngine.Random.Range(0, perguntasParaResponder.Count);

            DialogueObject pergunta = perguntasParaResponder[indice];

            perguntasParaResponder.Remove(pergunta);

            AbrirDialogo(pergunta);
        }
        else
        {
            AbrirDialogo(dialogoQuizCompleto);
        }
    }

    private IEnumerator AtualizarFlagPodeIniciarQuizNoOnEnable()
    {
        yield return null;

        AtualizarFlagPodeIniciarQuiz();
    }

    private void AbrirDialogo(DialogueObject dialogo)
    {
        StartCoroutine(AbrirDialogoCorrotina(dialogo));
    }

    private IEnumerator AbrirDialogoCorrotina(DialogueObject dialogo)
    {
        yield return new WaitUntil(() => DialogueUI.Instance.IsOpen == false);

        npc.VirarNaDirecao(PlayerData.Instance.transform.position);
        dialogueActivator.ShowDialogue(dialogo, DialogueUI.Instance);
    }
}

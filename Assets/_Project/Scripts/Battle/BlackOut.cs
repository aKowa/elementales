using BergamotaDialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackOut : MonoBehaviour
{
    //Instancia do singleton
    private static BlackOut instance = null;

    //Variaveis
    [SerializeField] private DialogueObject dialogoBlackOut;
    [SerializeField] private DialogueObject dialogoBlackOutComNPC;
    [SerializeField] private DialogueObject dialogoMonstrosCurados;

    private DialogueActivator dialogueActivator;

    //Getters
    public static BlackOut Instance => instance;

    private void Awake()
    {
        //Faz do script um singleton
        if (instance == null) //Confere se a instancia nao e nula
        {
            instance = this;
        }
        else if (instance != this) //Caso a instancia nao seja nula e nao seja este objeto, ele se destroi
        {
            Destroy(gameObject);
            return;
        }

        //Caso o objeto esteja sendo criado pela primeira vez e esteja no root da cena, marca ela para nao ser destruido em mudancas de cenas
        if (transform.parent == null)
        {
            DontDestroyOnLoad(transform.gameObject);
        }

        //Componentes
        dialogueActivator = GetComponent<DialogueActivator>();
    }

    public void IniciarBlackOut()
    {
        BergamotaLibrary.PauseManager.PermitirInputGeral = false;
        BergamotaLibrary.PauseManager.PermitirInput = true;

        MusicController.Instance.PararMusicaNoBlackout(50);

        Transition.GetInstance().DoTransition("FadeIn", 0, () =>
        {
            BergamotaLibrary.PauseManager.Pausar(true);

            BattleManager.Instance.FinalizarBatalha();
            StartCoroutine(DialogoBlackOut(dialogoBlackOut));
        });
    }

    public void IniciarBlackOutComNPC(string nomeDoNPC)
    {
        BergamotaLibrary.PauseManager.PermitirInputGeral = false;
        BergamotaLibrary.PauseManager.PermitirInput = true;

        MusicController.Instance.PararMusicaNoBlackout(50);

        DialogueUI.Instance.SetPlaceholderDeTexto("%integrante", nomeDoNPC);

        Transition.GetInstance().DoTransition("FadeIn", 0, () =>
        {
            BergamotaLibrary.PauseManager.Pausar(true);

            BattleManager.Instance.FinalizarBatalha();

            StartCoroutine(DialogoBlackOut(dialogoBlackOutComNPC));
        });
    }

    private int TirarDinheiroDoPlayer()
    {
        int dinheiroPerdido = PlayerData.Instance.Inventario.Dinheiro / 3;

        PlayerData.Instance.Inventario.Dinheiro -= dinheiroPerdido;

        return dinheiroPerdido;
    }

    private IEnumerator DialogoBlackOut(DialogueObject dialogoBlackOut)
    {
        int dinheiroPerdido = TirarDinheiroDoPlayer();

        DialogueUI.Instance.SetPlaceholderDeTexto("%player", PlayerData.Instance.GetPlayerName);
        DialogueUI.Instance.SetPlaceholderDeTexto("%quantidade", dinheiroPerdido.ToString());

        dialogueActivator.ShowDialogue(dialogoBlackOut, DialogueUI.Instance);

        yield return new WaitUntil(() => DialogueUI.Instance.IsOpen == false);

        if(BergamotaLibrary.MusicManager.instance.MusicaTocando == true)
        {
            BergamotaLibrary.MusicManager.instance.PararMusica();
        }

        MapsManager.GetInstance().LoadScene(SceneSpawnManager.LastMonsterCenterGatewayID, () =>
        {
            BergamotaLibrary.PauseManager.Pausar(false);

            Transition.GetInstance().DoTransition("FadeOut", 0.5f, () =>
            {
                FindObjectOfType<JanelaMapaAtual>(true).MostrarNomeDoMapa();

                StartCoroutine(DialogoMonstrosCurados());
            });
        });
    }
    
    private IEnumerator DialogoMonstrosCurados()
    {
        PlayerData.Instance.Inventario.RestaurarTodosOsMonstros();

        dialogueActivator.ShowDialogue(dialogoMonstrosCurados, DialogueUI.Instance);

        yield return new WaitUntil(() => DialogueUI.Instance.IsOpen == false);

        BergamotaLibrary.PauseManager.PermitirInputGeral = true;
    }
}

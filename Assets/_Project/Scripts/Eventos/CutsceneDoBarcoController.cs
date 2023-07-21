using BergamotaDialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneDoBarcoController : MonoBehaviour
{
    //Componentes
    private Eventos_MostrarDialogo mostrarDialogo;

    //Variaveis
    private static string currentDoorGuid;
    private static DialogueObject dialogo;

    private void Awake()
    {
        mostrarDialogo = GetComponent<Eventos_MostrarDialogo>();
    }

    private void Start()
    {
        mostrarDialogo.MostrarDialogoQuandoAcabarTransicao(dialogo);

        StartCoroutine(EsperarDialogoFecharParaTrocarCena());
    }

    public static void Iniciar(GatewayDeBarco gatewayDeBarco)
    {
        currentDoorGuid = gatewayDeBarco.Gateway.Guid;
        dialogo = gatewayDeBarco.DialogoNoBarco;
    }

    public void FazerTransicao()
    {
        BergamotaLibrary.PauseManager.PermitirInput = false;

        Transition.GetInstance().DoTransition("FadeIn", 0, 1, () =>
        {
            MapsManager.GetInstance().TakeDoorWithGuid(currentDoorGuid, () =>
            {
                Transition.GetInstance().DoTransition("FadeOut", 0.5f, 1, () =>
                {
                    FindObjectOfType<JanelaMapaAtual>(true).MostrarNomeDoMapa();
                    BergamotaLibrary.PauseManager.PermitirInput = true;
                });
            });
        });
    }

    private IEnumerator EsperarDialogoFecharParaTrocarCena()
    {
        yield return new WaitUntil(() => DialogueUI.Instance.IsOpen == true);
        yield return new WaitUntil(() => DialogueUI.Instance.IsOpen == false);

        FazerTransicao();
    }
}

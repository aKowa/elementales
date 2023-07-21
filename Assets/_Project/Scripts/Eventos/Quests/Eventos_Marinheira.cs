using BergamotaDialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eventos_Marinheira : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private DialogueActivator dialogueActivator;

    private FlagSetter flagSetter;

    //Variaveis
    [Header("Variaveis")]
    [SerializeField] private string nomeDaFlagTemTicket;

    [Space(10)]

    [SerializeField] private Item itemDoTicket;

    [Header("Cenas")]

    [SerializeField] private SceneReference cenaDoBarco;

    [Space(10)]

    [SerializeField] private GatewayDeBarco[] gatewaysDeBarco;

    private void Awake()
    {
        flagSetter = GetComponent<FlagSetter>();
    }

    private void OnEnable()
    {
        StartCoroutine(AtualizarFlagTemTicketNoOnEnable());
    }

    public void IrParaMapaUsandoGatewayDeBarco(int indice)
    {
        GatewayDeBarco gatewayDeBarco = gatewaysDeBarco[indice];

        CutsceneDoBarcoController.Iniciar(gatewayDeBarco);

        StartCoroutine(EsperarDialogoFecharParaTrocarCena());
    }

    public void FazerTransicaoParaCenaDoBarco()
    {
        BergamotaLibrary.PauseManager.PermitirInput = false;

        Transition.GetInstance().DoTransition("FadeIn", 0, 1, () =>
        {
            MapsManager.GetInstance().LoadSceneByName(cenaDoBarco, () =>
            {
                Transition.GetInstance().DoTransition("FadeOut", 0.5f, 1);
            });
        });
    }

    private void AtualizarFlagTemTicket()
    {
        if(PlayerData.Instance.Inventario.GetQuantidadeDoItem(itemDoTicket) > 0)
        {
            flagSetter.SetFlagAsTrue(nomeDaFlagTemTicket);
        }
        else
        {
            flagSetter.SetFlagAsFalse(nomeDaFlagTemTicket);
        }
    }

    private IEnumerator AtualizarFlagTemTicketNoOnEnable()
    {
        yield return null;

        AtualizarFlagTemTicket();
    }

    private IEnumerator EsperarDialogoFecharParaTrocarCena()
    {
        yield return new WaitUntil(() => DialogueUI.Instance.IsOpen == false);

        FazerTransicaoParaCenaDoBarco();
    }
}

[System.Serializable]
public struct GatewayDeBarco
{
    //Variaveis
    [SerializeField] private LumenSection.LevelLinker.Gateway gateway;
    [SerializeField] private DialogueObject dialogoNoBarco;

    //Getters
    public LumenSection.LevelLinker.Gateway Gateway => gateway;
    public DialogueObject DialogoNoBarco => dialogoNoBarco;
}

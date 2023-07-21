using BergamotaDialogueSystem;
using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eventos_ChigugoNeedsHelp : MonoBehaviour
{
    [SerializeField] private DialogueObject dialogoChigugoSalvo;

    [Space(10)]

    [SerializeField] private ListaDeFlags listaDeFlags;

    [Space(10)]

    [SerializeField] private ConditionalDialogues.CondicaoDeFlag[] condicoesParaFinalizarAQuest;

    public void ConferirQuest()
    {
        if (ConditionalDialogues.CondicoesVerdadeiras(listaDeFlags.name, condicoesParaFinalizarAQuest) == true)
        {
            Flags.SetFlag(listaDeFlags.name, "ChigugoHelp_ChigugoSaved", true);

            DialogueUI.Instance.StartCoroutine(MostrarDialogo());
        }
    }

    private IEnumerator MostrarDialogo()
    {
        yield return new WaitUntil(() => Transition.GetInstance().FazendoTransicao == true);
        yield return new WaitUntil(() => Transition.GetInstance().FazendoTransicao == false);

        DialogueUI.Instance.ShowDialogue(dialogoChigugoSalvo);
    }
}

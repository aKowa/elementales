using BergamotaDialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eventos_MostrarDialogo : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private DialogueActivator dialogueActivator;

    //Variaveis
    [Header("Variaveis")]
    [SerializeField] private DialogueObject dialogo;

    public void MostrarDialogo()
    {
        dialogueActivator.ShowDialogue(dialogo, null);
    }

    public void MostrarDialogo(DialogueObject dialogo)
    {
        dialogueActivator.ShowDialogue(dialogo, null);
    }

    public void MostrarDialogoQuandoAcabarTransicao()
    {
        DialogueUI.Instance.StartCoroutine(MostrarDialogoQuandoAcabarTransicaoCorrotina(dialogo));
    }

    public void MostrarDialogoQuandoAcabarTransicao(DialogueObject dialogo)
    {
        DialogueUI.Instance.StartCoroutine(MostrarDialogoQuandoAcabarTransicaoCorrotina(dialogo));
    }

    private IEnumerator MostrarDialogoQuandoAcabarTransicaoCorrotina(DialogueObject dialogo)
    {
        yield return new WaitUntil(() => Transition.GetInstance().FazendoTransicao == true);
        yield return new WaitUntil(() => Transition.GetInstance().FazendoTransicao == false);

        dialogueActivator.ShowDialogue(dialogo, null);
    }
}

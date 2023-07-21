using BergamotaDialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eventos_SailorJoseph : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private DialogueActivator dialogueActivator;
    [SerializeField] private NPCBatalha npcBatalha;

    [Header("Dialogos")]
    [SerializeField] private DialogueObject dialogoNaoTemDinheiro;
    [SerializeField] private DialogueObject dialogoComprouItem;
    [SerializeField] private DialogueObject dialogoPerdeuABatalha;

    private NPC npc;
    private FlagSetter flagSetter;
    private EntregarItem entregarItem;

    //Variaveis
    [Header("Variaveis")]
    [SerializeField] private string nomeDaFlagItemVendido;

    [Space(10)]

    [SerializeField] private int precoDoItem;

    private void Awake()
    {
        npc = GetComponent<NPC>();
        flagSetter = GetComponent<FlagSetter>();
        entregarItem = GetComponent<EntregarItem>();
    }

    public void IniciarBatalha()
    {
        NPCManager.IniciandoBatalha = true;

        npcBatalha.IniciarBatalha();
    }

    public void DialogoPerdeuABatalha()
    {
        DialogueUI.Instance.StartCoroutine(MostrarDialogoDepoisDaBatalha(dialogoPerdeuABatalha));
    }

    public void VenderItem()
    {
        if (PlayerData.Instance.Inventario.Dinheiro >= precoDoItem)
        {
            PlayerData.Instance.Inventario.Dinheiro -= precoDoItem;

            AbrirDialogo(dialogoComprouItem);

            flagSetter.SetFlagAsTrue(nomeDaFlagItemVendido);

            entregarItem.EntregarItens();
        }
        else
        {
            AbrirDialogo(dialogoNaoTemDinheiro);
        }
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

    private IEnumerator MostrarDialogoDepoisDaBatalha(DialogueObject dialogo)
    {
        yield return new WaitUntil(() => Transition.GetInstance().FazendoTransicao == true);
        yield return new WaitUntil(() => Transition.GetInstance().FazendoTransicao == false);

        dialogueActivator.ShowDialogue(dialogo, DialogueUI.Instance);
    }
}

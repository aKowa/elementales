using BergamotaDialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogosDoPlayer : MonoBehaviour
{
    //Componentes
    [Header("Dialogos")]
    [SerializeField] private DialogueObject dialogoRepelenteAcabou;
    [SerializeField] private DialogueObject dialogoQuerUsarOutroRepelente;

    [Header("Repelentes")]
    [SerializeField] private List<Item> repelentes;

    private DialogueActivator dialogueActivator;

    //Variaveis
    private Item repelenteAtual;

    private void Awake()
    {
        dialogueActivator = GetComponent<DialogueActivator>();
    }

    private void Start()
    {
        Player player = GetComponent<Player>();

        player.EventoRepelenteTerminou.AddListener(AbrirDialogoRepelente);
    }

    private void AbrirDialogoRepelente()
    {
        StartCoroutine(DialogoRepelente());
    }

    private Item ProcurarRepelenteNoInventario()
    {
        List<ItemHolder> listaDeItens = PlayerData.Instance.Inventario.Itens;

        foreach(Item repelente in repelentes)
        {
            for (int i = 0; i < listaDeItens.Count; i++)
            {
                if (listaDeItens[i].Item.ID == repelente.ID)
                {
                    return listaDeItens[i].Item;
                }
            }
        }

        return null;
    }

    public void UsarRepelente()
    {
        UsarRepelente acaoDoRepelente = (UsarRepelente)repelenteAtual.EfeitoForaDaBatalha;

        PlayerData.Repelente = acaoDoRepelente.QuantidadeDePassosDoRepelente;

        PlayerData.Instance.Inventario.RemoverItem(repelenteAtual, 1);
    }

    private IEnumerator DialogoRepelente()
    {
        repelenteAtual = ProcurarRepelenteNoInventario();

        dialogueActivator.ShowDialogue(dialogoRepelenteAcabou, DialogueUI.Instance);

        yield return new WaitUntil(() => DialogueUI.Instance.IsOpen == false);

        if(repelenteAtual != null)
        {
            dialogueActivator.ShowDialogue(dialogoQuerUsarOutroRepelente, DialogueUI.Instance);

            yield return new WaitUntil(() => DialogueUI.Instance.IsOpen == false);
        }

        repelenteAtual = null;
    }
}

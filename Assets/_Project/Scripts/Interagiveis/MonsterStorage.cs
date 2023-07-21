using BergamotaDialogueSystem;
using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStorage : Interagivel
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private Animator simboloDeInteracao;

    [Header("Dialogos")]
    [SerializeField] private DialogueObject dialogoAbrirMonsterStorage;

    private MonsterBoxController monsterBoxController;
    private DialogueActivator dialogueActivator;

    private void Awake()
    {
        monsterBoxController = FindObjectOfType<MonsterBoxController>();
        dialogueActivator = GetComponent<DialogueActivator>();

        NaAreaDeInteracao(false);
    }

    public override void Interagir(Player player)
    {
        StartCoroutine(AbrirMonsterBox());
    }

    public override void NaAreaDeInteracao(bool estaNaArea)
    {
        simboloDeInteracao.gameObject.SetActive(estaNaArea);
    }

    private IEnumerator AbrirMonsterBox()
    {
        DialogueUI.Instance.SetPlaceholderDeTexto("%player", PlayerData.Instance.GetPlayerName);
        dialogueActivator.ShowDialogue(dialogoAbrirMonsterStorage, DialogueUI.Instance);

        yield return new WaitUntil(() => DialogueUI.Instance.IsOpen == false);

        monsterBoxController.OpenView();
    }
}

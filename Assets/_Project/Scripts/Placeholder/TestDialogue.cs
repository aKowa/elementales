using BergamotaDialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDialogue : MonoBehaviour
{
    private DialogueUI dialogueUI;
    private DialogueActivator dialogueActivator;

    private void Start()
    {
        dialogueUI = FindObjectOfType<DialogueUI>();
        dialogueActivator = GetComponent<DialogueActivator>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            dialogueActivator.ShowDialogue(dialogueUI);
        }
    }
}

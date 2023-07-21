using UnityEngine;
using System;

namespace BergamotaDialogueSystem
{
    [RequireComponent(typeof(DialogueActivator))]

    public class DialogueEndEvents : MonoBehaviour
    {
        //Variavais
        [SerializeField] private DialogueEndEventContainer[] dialogueEndEventsContainer;

        //Getters
        public DialogueEndEventContainer[] DialogueEndEventsContainer => dialogueEndEventsContainer;

        [System.Serializable]
        public class DialogueEndEventContainer
        {
            //Variaveis
            [SerializeField] private DialogueObject dialogueObject;
            [SerializeField] private ResponseEvent events;

            //Getters
            public DialogueObject DialogueObject => dialogueObject;
            public ResponseEvent Events => events;
        }
    }
}
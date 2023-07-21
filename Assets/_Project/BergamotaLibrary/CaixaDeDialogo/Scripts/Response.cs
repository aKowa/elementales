using UnityEngine;

namespace BergamotaDialogueSystem
{
    [System.Serializable]
    public class Response
    {
        [Header("Response")]

        [SerializeField] private string responseText;
        [SerializeField] private DialogueObject dialogueObject;

        //Getters
        public string ResponseText => responseText;
        public DialogueObject DialogueObject => dialogueObject;
    }
}
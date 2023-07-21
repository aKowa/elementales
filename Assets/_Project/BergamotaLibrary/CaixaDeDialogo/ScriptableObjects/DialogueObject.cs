using UnityEngine;

namespace BergamotaDialogueSystem
{
    [CreateAssetMenu(menuName = "Dialogue/Dialogue Object")]

    public class DialogueObject : ScriptableObject
    {
        [SerializeField] private DialogueStruct[] dialogue;
        [SerializeField] private Response[] responses;

        //Getters
        public DialogueStruct[] Dialogue => dialogue;
        public bool HasResponses => Responses != null && Responses.Length > 0;
        public Response[] Responses => responses;

        [System.Serializable]
        public struct DialogueStruct
        {
            [Header("Dialogue")]

            //Variaveis
            [SerializeField] private Sprite portrait;
            [SerializeField] private TypingSound typingSound;
            [SerializeField] [TextArea] private string text;

            //Getters
            public Sprite Portrait => portrait;
            public TypingSound TypingSound => typingSound;
            public string Text => text;
        }
    }
}

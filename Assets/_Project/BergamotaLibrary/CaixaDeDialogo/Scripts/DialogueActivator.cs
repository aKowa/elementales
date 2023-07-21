using UnityEngine;

namespace BergamotaDialogueSystem
{
    public class DialogueActivator : MonoBehaviour
    {
        //Variaveis
        [SerializeField] private DialogueObject dialogueObject;

        /// <summary>
        /// Abre a caixa de dialogo com o dialogo atual deste Dialogue Activator.
        /// </summary>
        /// <param name="dialogueUI">Referencia para a Dialogue UI</param>
        public void ShowDialogue(DialogueUI dialogueUI)
        {
            dialogueUI = DialogueUI.Instance;

            if (dialogueUI.IsOpen == false)
            {
                UpdateResponseEvents(dialogueUI, this.dialogueObject);
                dialogueUI.UpdateDialogueActivator(this);
                dialogueUI.ShowDialogue(dialogueObject);
            }
        }

        /// <summary>
        /// Atualiza o dialogo atual e abre a caixa de dialogo.
        /// </summary>
        /// <param name="dialogueObject">Novo dialogo</param>
        /// <param name="dialogueUI">Referencia para a Dialogue UI</param>
        public void ShowDialogue(DialogueObject dialogueObject, DialogueUI dialogueUI)
        {
            this.dialogueObject = dialogueObject;

            ShowDialogue(dialogueUI);
        }

        public void UpdateResponseEvents(DialogueUI dialogueUI, DialogueObject dialogueObject)
        {
            DialogueResponseEvents dialogueResponseEvents = GetComponent<DialogueResponseEvents>();
            DialogueEndEvents dialogueEndEvents = GetComponent<DialogueEndEvents>();

            if (dialogueResponseEvents != null)
            {
                for (int i = 0; i < dialogueResponseEvents.ResponseEventsContainer.Length; i++)
                {
                    if (dialogueResponseEvents.ResponseEventsContainer[i].DialogueObject == dialogueObject)
                    {
                        dialogueUI.AddResponseEvents(dialogueResponseEvents.ResponseEventsContainer[i].events);
                        break;
                    }
                }
            }

            if (dialogueEndEvents != null)
            {
                for (int i = 0; i < dialogueEndEvents.DialogueEndEventsContainer.Length; i++)
                {
                    if (dialogueEndEvents.DialogueEndEventsContainer[i].DialogueObject == dialogueObject)
                    {
                        dialogueUI.AddDialogueEndEvents(dialogueEndEvents.DialogueEndEventsContainer[i].Events);
                        break;
                    }
                }
            }
        }
    }
}

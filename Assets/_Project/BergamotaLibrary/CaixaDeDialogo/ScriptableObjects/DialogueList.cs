using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BergamotaDialogueSystem
{
    [CreateAssetMenu(menuName = "Dialogue/Dialogue List")]

    public class DialogueList : ScriptableObject
    {
        //Variaveis
        [SerializeField] private DialogueObject[] dialogueList;

        //Getters

        /// <summary>
        /// Retorna o array da dialogue list.
        /// </summary>
        public DialogueObject[] GetDialogueListArray => dialogueList;

        /// <summary>
        /// Retorna um Dialogue Object.
        /// </summary>
        /// <param name="nome">Nome do dialogo</param>
        /// <returns>Um scriptable object do tipo DialogueObject</returns>
        public DialogueObject GetDialogue(string nome)
        {
            for (int i = 0; i < dialogueList.Length; i++)
            {
                if (dialogueList[i].name == nome)
                {
                    return dialogueList[i];
                }
            }

            Debug.LogWarning("Nao foi possivel achar o dialogo \"" + nome + "\" nesta lista. Complicado.");
            return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BergamotaDialogueSystem
{
    [CreateAssetMenu(menuName = "Dialogue/Dialogue Book")]

    public class DialogueBook : ScriptableObject
    {
        //Variaveis
        [SerializeField] private DialogueList[] dialogueBook;

        //Getters

        /// <summary>
        /// Retorna o array do dialogue book.
        /// </summary>
        public DialogueList[] GetDialogueBookArray => dialogueBook;

        /// <summary>
        /// Retorna uma Dialogue List.
        /// </summary>
        /// <param name="nome">Nome da lista</param>
        /// <returns>Um scriptable object do tipo DialogueList</returns>
        public DialogueList GetDialogueList(string nome)
        {
            for (int i = 0; i < dialogueBook.Length; i++)
            {
                if (dialogueBook[i].name == nome)
                {
                    return dialogueBook[i];
                }
            }

            Debug.LogWarning("Nao foi possivel achar a lista de dialogos \"" + nome + "\" nesta lista. Complicado.");
            return null;
        }
    }
}

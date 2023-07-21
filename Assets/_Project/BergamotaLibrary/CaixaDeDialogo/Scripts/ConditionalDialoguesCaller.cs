using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BergamotaDialogueSystem
{
    public class ConditionalDialoguesCaller : MonoBehaviour
    {
        //Variaveis
        [SerializeField] private ConditionalDialogues dialogosCondicionais;

        private DialogueList dialogueListAtual;
        private int indicieDialogueList;

        private void Awake()
        {
            dialogueListAtual = null;
            indicieDialogueList = 0;
        }

        /// <summary>
        /// Tenta pegar um dialogo na lista de dialogos condicionais, se nao conseguir, tenta pegar uma lista de dialogos e retorna um dialogo dela.
        /// </summary>
        /// <returns>Um scriptable object do tipo DialogueObject</returns>
        public DialogueObject GetDialogue()
        {
            DialogueObject dialogueObject = null;
            DialogueList dialogueList = null;

            dialogueObject = dialogosCondicionais.GetDialogo();

            //Se conseguir um dialogo na lista de dialogos condicionais, retorna ele
            if(dialogueObject != null)
            {
                return dialogueObject;
            }

            dialogueList = dialogosCondicionais.GetListaDeDialogos();

            //Se conseguir uma lista de dialogos na lista de listas de dialogos condicionais, retorna um dialogo dela.
            //Na primeira vez que uma lista ser pega, retorna o dialogo do indice 0, e se a mesma lista for pega novamente, retorna o indice seguinte, ate chegar ao fim e voltar para o primeiro. 
            if (dialogueList != null)
            {
                AtualizarDialogueListAtual(dialogueList);

                return dialogueList.GetDialogueListArray[indicieDialogueList];
            }

            //Se nao conseguir achar nenhum dialogo ou lista, retorna nulo
            Debug.LogWarning("As condicoes de nenhum dialogo ou lista de dialogos foram atendidas! \nConditional Dialogue: " + dialogosCondicionais.name);
            return null;
        }

        private void AtualizarDialogueListAtual(DialogueList novaDialogueList)
        {
            if(dialogueListAtual != novaDialogueList)
            {
                dialogueListAtual = novaDialogueList;
                indicieDialogueList = 0;
            }
            else
            {
                indicieDialogueList++;

                if(indicieDialogueList >= dialogueListAtual.GetDialogueListArray.Length)
                {
                    indicieDialogueList = 0;
                }
            }
        }
    }
}

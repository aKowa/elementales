using UnityEngine;
using System;

namespace BergamotaDialogueSystem
{
    [RequireComponent(typeof(DialogueActivator))]

    public class DialogueResponseEvents : MonoBehaviour
    {
        //Variavais
        [SerializeField] private ResponseEventContainer[] responseEventsContainer = new ResponseEventContainer[0];

        //Getters
        public ResponseEventContainer[] ResponseEventsContainer => responseEventsContainer;

        //OnValidate roda sempre que esse script e carregado ou um valor dele e mudado no inspetor
        public void OnValidate()
        {
            for (int i = 0; i < responseEventsContainer.Length; i++)
            {
                //Confere se e necessario atualizar a lista de eventos
                if (responseEventsContainer[i].DialogueObject == null) continue;
                if (responseEventsContainer[i].DialogueObject.Responses == null) continue;
                if (responseEventsContainer[i].events != null && responseEventsContainer[i].events.Length == responseEventsContainer[i].DialogueObject.Responses.Length) continue;

                //Cria a lista de eventos caso ela seja nula, e so muda o tamanho caso ela ja exista
                if (responseEventsContainer[i].events == null)
                {
                    responseEventsContainer[i].events = new ResponseEvent[responseEventsContainer[i].DialogueObject.Responses.Length];
                }
                else
                {
                    Array.Resize(ref responseEventsContainer[i].events, responseEventsContainer[i].DialogueObject.Responses.Length);
                }

                for (int y = 0; y < responseEventsContainer[i].DialogueObject.Responses.Length; y++)
                {
                    Response response = responseEventsContainer[i].DialogueObject.Responses[y];

                    //Muda o nome do item na lista caso ele nao seja nulo
                    if (responseEventsContainer[i].events[y] != null)
                    {
                        responseEventsContainer[i].events[y].name = response.ResponseText;
                        continue; //Ignora as linha de codigo abaixo e pula para a proxima iteracao do for
                    }

                    //Cria um novo item na lista de eventos
                    responseEventsContainer[i].events[y] = new ResponseEvent() { name = response.ResponseText };
                }
            }
        }

        [System.Serializable]
        public class ResponseEventContainer
        {
            //Variaveis
            [SerializeField] private DialogueObject dialogueObject;
            [SerializeField] public ResponseEvent[] events;

            //Getters
            public DialogueObject DialogueObject => dialogueObject;
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace BergamotaDialogueSystem
{
    public class ResponseHandler : MonoBehaviour
    {
        //Variaveis de configuracao

        [Header("Opcoes")]

        [SerializeField] private bool responderPorClique;
        [SerializeField] private bool responderPorInput;

        //Componentes

        [Header("Componentes")]

        [SerializeField] private RectTransform responseBox;
        [SerializeField] private RectTransform responseButtonTemplate;
        [SerializeField] private RectTransform responseContainer;

        private DialogueUI dialogueUI;
        private ResponseEvent[] responseEvents;

        private List<GameObject> tempResponseButtons = new List<GameObject>();

        private Coroutine waitingForResponse;

        private void Start()
        {
            dialogueUI = GetComponent<DialogueUI>();
        }

        public void Stop()
        {
            responseBox.gameObject.SetActive(false);
            ClearResponseButtons();
            responseEvents = null;

            if (waitingForResponse != null)
            {
                StopCoroutine(waitingForResponse);
            }
        }

        public void AddResponseEvents(ResponseEvent[] responseEvents)
        {
            this.responseEvents = responseEvents;
        }

        public void ShowResponses(Response[] responses, DialogueJSONData dialogueData, bool usarOJSONReader, bool achouArquivoDeTexto)
        {
            float responseBoxHeight = 0;
            float spacing = 0;

            VerticalLayoutGroup verticalLayoutGroup = responseBox.GetComponentInChildren<VerticalLayoutGroup>();

            if (verticalLayoutGroup != null)
            {
                spacing = verticalLayoutGroup.spacing;
            }

            for (int i = 0; i < responses.Length; i++)
            {
                Response response = responses[i];
                int responseIndex = i;

                string responseText;

                GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);
                responseButton.gameObject.SetActive(true);

                if (usarOJSONReader == true)
                {
                    //Confere se um arquivo de texto com o texto da resposta foi encontrado, e usa o texto dele. Caso nao tenha sido encontrado, usa o texto que esta no objeto da resposta
                    if (achouArquivoDeTexto == true && dialogueData.respostas.Length > i)
                    {
                        responseText = dialogueData.respostas[i].texto;
                    }
                    else
                    {
                        if (achouArquivoDeTexto == true)
                        {
                            Debug.LogWarning("Ha menos respostas no arquivo do que no objeto de respostas!");
                        }

                        responseText = response.ResponseText;
                    }
                }
                else
                {
                    responseText = response.ResponseText;
                }

                //Substitui os placeholders no texto
                for (int y = 0; y < dialogueUI.PlaceholdersDeTexto.Length; y++)
                {
                    if (responseText.Contains(dialogueUI.PlaceholdersDeTexto[y].Chave))
                    {
                        responseText = responseText.Replace(dialogueUI.PlaceholdersDeTexto[y].Chave, dialogueUI.PlaceholdersDeTexto[y].Valor);
                    }
                }

                responseButton.GetComponentInChildren<TMP_Text>().text = responseText;

                if (responderPorClique == true)
                {
                    responseButton.GetComponent<Button>().onClick.AddListener(() => OnPickResponse(response, responseIndex)); //Adiciona um metodo ao botao, a mesma coisa que se faz atraves do editor do Unity, mas por codigo
                }

                tempResponseButtons.Add(responseButton);

                responseBoxHeight += responseButtonTemplate.sizeDelta.y;
            }

            responseBoxHeight += (spacing * (tempResponseButtons.Count - 1));

            responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight); //Seta o tamanho do objeto
            responseBox.gameObject.SetActive(true);

            if (responderPorInput == true)
            {
                if (waitingForResponse != null)
                {
                    StopCoroutine(waitingForResponse);
                }

                waitingForResponse = StartCoroutine(ChooseResponse(responses));
            }
        }

        private IEnumerator ChooseResponse(Response[] responses)
        {
            bool responsePicked = false;
            int selection = 0;
            UpdateButtonSelectionEffect(selection);
            while (responsePicked == false)
            {
                yield return null;

                if (dialogueUI.Cima()) //Mover para cima
                {
                    if (selection > 0)
                    {
                        selection--;
                        UpdateButtonSelectionEffect(selection);

                        dialogueUI.TocarSomOneShot(dialogueUI.ListaDeSons.GetSom("MoverCima"));
                    }
                }
                else if (dialogueUI.Baixo()) //Mover para baixo
                {
                    if (selection < responses.Length - 1)
                    {
                        selection++;
                        UpdateButtonSelectionEffect(selection);
                        dialogueUI.TocarSomOneShot(dialogueUI.ListaDeSons.GetSom("MoverBaixo"));
                    }
                }
                else if (dialogueUI.AvancarDialogo()) //Confirmar
                {
                    responsePicked = true;
                }
            }

            OnPickResponse(responses[selection], selection);
        }

        private void OnPickResponse(Response response, int responseIndex)
        {
            if (waitingForResponse != null)
            {
                StopCoroutine(waitingForResponse);
            }

            dialogueUI.TocarSomOneShot(dialogueUI.ListaDeSons.GetSom("Confirmar"));

            responseBox.gameObject.SetActive(false);

            ClearResponseButtons();

            if (responseEvents != null && responseIndex <= responseEvents.Length)
            {
                responseEvents[responseIndex].OnPickedResponse?.Invoke();
            }

            responseEvents = null;

            if (response.DialogueObject)
            {
                dialogueUI.CallUpdateResponseEvents(response.DialogueObject);
                dialogueUI.ShowDialogue(response.DialogueObject);
            }
            else
            {
                dialogueUI.CloseDialogueBox();
            }
        }

        private void UpdateButtonSelectionEffect(int selection)
        {
            for (int i = 0; i < tempResponseButtons.Count; i++)
            {
                if (i == selection)
                {
                    tempResponseButtons[i].GetComponent<Animator>().SetBool("Selecionado", true); //Alterar para a maneira como o jogo destaca itens selecionados
                }
                else
                {
                    tempResponseButtons[i].GetComponent<Animator>().SetBool("Selecionado", false); //Alterar para a maneira como o jogo nao destaca itens selecionados
                }
            }
        }

        private void ClearResponseButtons()
        {
            foreach (GameObject button in tempResponseButtons)
            {
                Destroy(button);
            }
            tempResponseButtons.Clear();
        }
    }
}

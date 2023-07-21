using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using BergamotaLibrary;

namespace BergamotaDialogueSystem
{
    [RequireComponent(typeof(TypewriterEffect))]
    [RequireComponent(typeof(ResponseHandler))]
    [RequireComponent(typeof(DialogueJSONReader))]
    [RequireComponent(typeof(TypingSoundsPlayer))]

    public class DialogueUI : MonoBehaviour
    {
        //Instancia do singleton
        private static DialogueUI instance = null;

        //Variaveis de configuracao

        [Header("Debug")]

        [SerializeField] private bool printarNomeDosDialogos;

        [Header("Opcoes")]

        [SerializeField] private bool avancarDialogoPorBotao;
        [SerializeField] private bool avancarDialogoPorInput;
        [SerializeField] private bool usarOJSONReader;

        [Header("Modificadores")]
        [SerializeField] private PlaceHolderDeTexto[] placeholdersDeTexto;

        //Componentes

        [Header("Componentes")]

        [SerializeField] private GameObject dialogueBox; //Guarda toda a caixa de dialogo
        [SerializeField] private TMP_Text textLabel; //Guarda a caixa de texto
        [SerializeField] private Image portrait; //Guarda o retrato da caixa de dialogo 
        [SerializeField] private RectTransform botaoAvancarDialogo; //Guarda o botao de avancar dialogo

        [SerializeField] private RectTransform fundoAvancarDialogo;

        private ResponseHandler responseHandler;
        private TypewriterEffect typewriterEffect;

        private DialogueJSONReader dialogueJSONReader;

        private TypingSoundsPlayer typingSoundsPlayer;

        [SerializeField] private ListaDeSons listaDeSons;

        //Variaveis
        public bool IsOpen { get; private set; }

        private DialogueActivator dialogueActivator;

        private ResponseEvent dialogueEndEvents;

        private bool avancarDialogo;

        //Getters
        public static DialogueUI Instance => instance;
        public PlaceHolderDeTexto[] PlaceholdersDeTexto => placeholdersDeTexto;
        public bool SomTocando => typingSoundsPlayer.SomTocando;
        public ListaDeSons ListaDeSons => listaDeSons;

        //Setters

        /// <summary>
        /// Altera o valor de um dos placeholders de texto da DialogueUI.
        /// </summary>
        /// <param name="chave">Chave do placeholder.</param>
        /// <param name="valor">Novo valor para o placeholder.</param>
        public void SetPlaceholderDeTexto(string chave, string valor)
        {
            for(int i =0; i < placeholdersDeTexto.Length; i++)
            {
                if (placeholdersDeTexto[i].Chave == chave)
                {
                    placeholdersDeTexto[i].Valor = valor;
                    return;
                }
            }

            Debug.LogWarning("A chave " + chave + " nao foi encontrada nos plaheholders de texto!");
        }

        private void Awake()
        {
            //Faz do script um singleton
            if (instance == null) //Confere se a instancia nao e nula
            {
                instance = this;
            }
            else if (instance != this) //Caso a instancia nao seja nula e nao seja este objeto, ele se destroi
            {
                Destroy(gameObject);
                return;
            }

            //Caso o objeto esteja sendo criado pela primeira vez e esteja no root da cena, marca ela para nao ser destruido em mudancas de cenas
            if (transform.parent == null)
            {
                DontDestroyOnLoad(transform.gameObject);
            }

            //Componentes
            typewriterEffect = GetComponent<TypewriterEffect>();
            responseHandler = GetComponent<ResponseHandler>();
            dialogueJSONReader = GetComponent<DialogueJSONReader>();
            typingSoundsPlayer = GetComponent<TypingSoundsPlayer>();

            portrait.gameObject.SetActive(false);
            dialogueBox.SetActive(false);
            fundoAvancarDialogo.gameObject.SetActive(false);
        }

        private void Start()
        {
            textLabel.text = string.Empty;
            IsOpen = false;
            avancarDialogo = false;
        }

        /// <summary>
        /// Abre a caixa de dialogo e exibe um dialogo.
        /// </summary>
        /// <param name="dialogueObject">Dialogo a ser exibido.</param>
        public void ShowDialogue(DialogueObject dialogueObject)
        {
            if(printarNomeDosDialogos == true)
            {
                print("Dialogo: " + dialogueObject.name);
            }

            typingSoundsPlayer.AtualizarAudioSources();

            IsOpen = true;
            dialogueBox.SetActive(true);
            fundoAvancarDialogo.gameObject.SetActive(true);

            botaoAvancarDialogo.gameObject.SetActive(avancarDialogoPorBotao);

            StartCoroutine(StepThroughDialogue(dialogueObject));
        }

        public void CloseDialogueBox()
        {
            dialogueEndEvents = null;

            dialogueActivator = null;

            IsOpen = false;
            portrait.gameObject.SetActive(false);
            dialogueBox.SetActive(false);
            fundoAvancarDialogo.gameObject.SetActive(false);
            textLabel.text = string.Empty;
        }

        /// <summary>
        /// Fecha a caixa de dialogo forcadamente, interrompendo tudo que estiver sendo executado por ela.
        /// </summary>
        public void ForceCloseDialogueBox()
        {
            StopAllCoroutines();
            typewriterEffect.Stop();
            responseHandler.Stop();

            CloseDialogueBox();
        }

        public void AddResponseEvents(ResponseEvent[] responseEvents)
        {
            responseHandler.AddResponseEvents(responseEvents);
        }

        public void AddDialogueEndEvents(ResponseEvent responseEvents)
        {
            this.dialogueEndEvents = responseEvents;
        }

        public void UpdateDialogueActivator(DialogueActivator dialogueActivator)
        {
            this.dialogueActivator = dialogueActivator;
        }

        public void CallUpdateResponseEvents(DialogueObject dialogueObject)
        {
            if(dialogueActivator != null)
            {
                dialogueActivator.UpdateResponseEvents(this, dialogueObject);
            }
        }

        //Atualiza a imagem do retrato e a borda esquerda da caixa de texto
        private void UpdateImage(DialogueObject.DialogueStruct dialogue)
        {
            if (dialogue.Portrait != null)
            {
                textLabel.margin = new Vector4(portrait.GetComponent<RectTransform>().sizeDelta.x + 5, 0, 0, 0);
                portrait.gameObject.SetActive(true);
                portrait.sprite = dialogue.Portrait;
            }
            else
            {
                textLabel.margin = new Vector4(0, 0, 0, 0);
                portrait.gameObject.SetActive(false);
            }
        }

        private bool AguardarAvancarDialogo()
        {
            if (avancarDialogoPorInput == true)
            {
                if (AvancarDialogo())
                {
                    avancarDialogo = true;
                }
            }

            return avancarDialogo;
        }

        /// <summary>
        /// Seta a variavel de avancar dialogo como verdadeira.
        /// </summary>
        public void SetAvancarDialogo()
        {
            avancarDialogo = true;
        }

        /// <summary>
        /// Retorna se o jogador fez o comando para avancar o dialogo.
        /// </summary>
        /// <returns>Uma booleana.</returns>
        public bool AvancarDialogo()
        {
            return Input.GetKeyDown(KeyCode.Space);
        }

        /// <summary>
        /// Retorna se o jogador fez o comando para mover para cima na UI.
        /// </summary>
        /// <returns>Uma booleana.</returns>
        public bool Cima()
        {
            return Input.GetKeyDown(KeyCode.UpArrow);
        }

        /// <summary>
        /// Retorna se o jogador fez o comando para mover para baixo na UI.
        /// </summary>
        /// <returns>Uma booleana.</returns>
        public bool Baixo()
        {
            return Input.GetKeyDown(KeyCode.DownArrow);
        }

        /// <summary>
        /// Toca algum som no modo normal.
        /// </summary>
        /// <param name="som">Som para tocar.</param>
        public void TocarSom(AudioClip som)
        {
            typingSoundsPlayer.TocarSom(som);
        }

        /// <summary>
        /// Toca algum som no modo OneShot.
        /// </summary>
        /// <param name="som">Som para tocar.</param>
        public void TocarSomOneShot(AudioClip som)
        {
            typingSoundsPlayer.TocarSomOneShot(som);
        }

        //Passa por cada um dos arrays de texto do dialogueObject e os mostra na tela
        private IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
        {
            bool achouArquivoDeTexto = false;

            if(usarOJSONReader == true)
            {
                achouArquivoDeTexto = dialogueJSONReader.CarregarDialogo(dialogueObject);
            }

            for (int i = 0; i < dialogueObject.Dialogue.Length; i++)
            {
                string dialogue;
                TypingSound typingSound;

                UpdateImage(dialogueObject.Dialogue[i]);

                if(usarOJSONReader)
                {
                    //Confere se um arquivo de texto com o dialogo foi encontrado, e usa o texto dele. Caso nao tenha sido encontrado, usa o texto que esta no DialogueObject
                    if (achouArquivoDeTexto == true && dialogueJSONReader.dialogueData.dialogos.Length > i)
                    {
                        dialogue = dialogueJSONReader.dialogueData.dialogos[i].texto;
                    }
                    else
                    {
                        if (achouArquivoDeTexto == true)
                        {
                            Debug.LogWarning("Ha menos dialogos no arquivo do que no objeto de dialogo!");
                        }

                        dialogue = dialogueObject.Dialogue[i].Text;
                    }
                }
                else
                {
                    dialogue = dialogueObject.Dialogue[i].Text;
                }

                //Substitui os placeholders no texto
                for (int y = 0; y < placeholdersDeTexto.Length; y++)
                {
                    if(dialogue.Contains(placeholdersDeTexto[y].Chave))
                    {
                        dialogue = dialogue.Replace(placeholdersDeTexto[y].Chave, placeholdersDeTexto[y].Valor);
                    }
                }

                typingSound = dialogueObject.Dialogue[i].TypingSound;

                avancarDialogo = false;
                yield return RunTypingEffect(dialogue, typingSound);

                textLabel.maxVisibleCharacters = dialogue.Length;

                if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.HasResponses) break;

                yield return null;

                avancarDialogo = false;
                yield return new WaitUntil(() => AguardarAvancarDialogo());
            }

            if (dialogueObject.HasResponses)
            {
                dialogueEndEvents = null;

                botaoAvancarDialogo.gameObject.SetActive(false);

                responseHandler.ShowResponses(dialogueObject.Responses, dialogueJSONReader.dialogueData, usarOJSONReader, achouArquivoDeTexto);
            }
            else
            {
                dialogueEndEvents?.OnPickedResponse?.Invoke();

                CloseDialogueBox();
            }
        }

        private IEnumerator RunTypingEffect(string dialogue, TypingSound typingSound)
        {
            typewriterEffect.Run(dialogue, textLabel, typingSound, this);

            while (typewriterEffect.IsRunning)
            {
                yield return null;

                if (AguardarAvancarDialogo() == true)
                {
                    typewriterEffect.Stop();
                }
            }
        }
    }

    [System.Serializable]
    public class PlaceHolderDeTexto
    {
        //Variaveis
        [SerializeField] private string chave;
        private string valor;

        //Getters
        public string Chave => chave;

        public string Valor
        {
            get
            {
                return valor;
            }

            set
            {
                valor = value;
            }
        }
    }
}

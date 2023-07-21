using System.IO;
using UnityEngine;

namespace BergamotaDialogueSystem
{
    public class DialogueJSONReader : MonoBehaviour
    {
        //A instancia da classe DataDeDialogo
        public DialogueJSONData dialogueData = new DialogueJSONData();

        public bool CarregarDialogo(DialogueObject dialogueObject)
        {
            string caminhoDoArquivo = Path.Combine("Textos", "Dialogos", "Idioma", dialogueObject.name);

            TextAsset texto = (TextAsset)Resources.Load(caminhoDoArquivo);

            if (texto != null)
            {
                dialogueData = JsonUtility.FromJson<DialogueJSONData>(texto.text);
                return true;
            }

            Debug.LogWarning("O arquivo de texto nao foi encontrado!\nCaminho: " + caminhoDoArquivo);
            return false;
        }
    }
}
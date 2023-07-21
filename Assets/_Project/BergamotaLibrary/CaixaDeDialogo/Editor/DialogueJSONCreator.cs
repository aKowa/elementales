using System.IO;
using UnityEngine;
using UnityEditor;

namespace BergamotaDialogueSystem
{
    public class DialogueJSONCreator : EditorWindow
    {
        //Variaveis
        private string caminhoDoAplicativo;
        private string caminhoDosArquivos = string.Empty;
        public DialogueObject[] dialogueObjects;

        //Adiciona um item na janela Window
        [MenuItem("Window/Dialogue JSON Creator")]
        public static void ShowWindow()
        {
            //Mostra a instancia atual da janela. Se ela nao existir, cria uma.
            EditorWindow.GetWindow(typeof(DialogueJSONCreator));
        }

        private void OnEnable()
        {
            caminhoDoAplicativo = Application.dataPath;
        }

        private void OnGUI()
        {
            GUILayout.Label("Caminho da Pasta para Gerar os Arquivos Dentro da Pasta Assets", EditorStyles.boldLabel);
            GUILayout.Space(5);

            caminhoDosArquivos = EditorGUILayout.TextField("Caminho dos Arquivos", caminhoDosArquivos);

            GUILayout.Space(10);

            //Mostra o array de Dialogue Objects
            ScriptableObject scriptableObj = this;
            SerializedObject serialObj = new SerializedObject(scriptableObj);
            SerializedProperty serialProp = serialObj.FindProperty("dialogueObjects");

            EditorGUILayout.PropertyField(serialProp, true);
            serialObj.ApplyModifiedProperties();

            GUILayout.Space(10);

            //Botao para criar os arquivos
            if (GUILayout.Button("Criar Arquivos"))
            {
                CriarArquivos();
            }
        }

        private void CriarArquivos()
        {
            for(int i = 0; i < dialogueObjects.Length; i++)
            {
                DialogueJSONData dialogueData = new DialogueJSONData();

                //Inicia o array
                dialogueData.dialogos = new DialogueJSONData.Dialogo[dialogueObjects[i].Dialogue.Length];

                //Inicia a classe de cada item do array
                for (int y = 0; y < dialogueData.dialogos.Length; y++)
                {
                    dialogueData.dialogos[y] = new DialogueJSONData.Dialogo();
                }

                //Copia os textos dos dialogos
                for (int y = 0; y < dialogueData.dialogos.Length; y++)
                {
                    dialogueData.dialogos[y].texto = dialogueObjects[i].Dialogue[y].Text;
                }

                //Inicia o array
                dialogueData.respostas = new DialogueJSONData.Dialogo[dialogueObjects[i].Responses.Length];

                //Inicia a classe de cada item do array
                for (int y = 0; y < dialogueData.respostas.Length; y++)
                {
                    dialogueData.respostas[y] = new DialogueJSONData.Dialogo();
                }

                //Copia os textos das respostas
                for (int y = 0; y < dialogueData.respostas.Length; y++)
                {
                    dialogueData.respostas[y].texto = dialogueObjects[i].Responses[y].ResponseText;
                }

                SalvarArquivo(dialogueData, dialogueObjects[i].name);
            }

            AssetDatabase.Refresh();
        }

        private void SalvarArquivo(DialogueJSONData dialogueData, string nomeDoDialogo)
        {
            string textoDoArquivo = JsonUtility.ToJson(dialogueData, true);

            string nomeDoArquivo = string.Concat(nomeDoDialogo, ".txt");

            string caminho = Path.Combine(caminhoDoAplicativo, caminhoDosArquivos, nomeDoArquivo);

            File.WriteAllText(caminho, textoDoArquivo);
        }
    }

}
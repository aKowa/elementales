using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BergamotaLibrary
{
    [CreateAssetMenu(menuName = "Listas/Lista de Sons")]

    public class ListaDeSons : ScriptableObject, ISerializationCallbackReceiver
    {
        //Variaveis
        [SerializeField] private Som[] listaDeSons;

        private Dictionary<string, Som> GetSomStruct = new Dictionary<string, Som>();

        //Getters

        /// <summary>
        /// Retorna um som da lista de sons.
        /// </summary>
        /// <param name="nome">Nome do som</param>
        /// <returns>Um AudioClip</returns>
        public AudioClip GetSom(string nome)
        {
            return GetSomStruct[nome].Audio;
        }

        //Cria e preenche o dicionario depois que o Unity desserizaliza o scriptable object
        public void OnAfterDeserialize()
        {
            GetSomStruct = new Dictionary<string, Som>();

            for (int i = 0; i < listaDeSons.Length; i++)
            {
                GetSomStruct.Add(listaDeSons[i].Nome, listaDeSons[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            //Nada
        }

        [System.Serializable]
        private struct Som
        {
            //Variaveis
            [SerializeField] private string nome;
            [SerializeField] private AudioClip[] audio;

            //Getters
            public string Nome => nome;
            public AudioClip Audio => audio[Random.Range(0, audio.Length)];
        }
    }
}

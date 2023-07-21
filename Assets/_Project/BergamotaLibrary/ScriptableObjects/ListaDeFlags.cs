using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BergamotaLibrary
{
    [CreateAssetMenu(menuName = "Listas/Lista de Flags")]
    public class ListaDeFlags : ScriptableObject, ISerializationCallbackReceiver
    {
        //Variaveis
        [SerializeField] private Flag[] listaDeFlags;

        private Dictionary<string, Flag> GetFlagStruct = new Dictionary<string, Flag>();

        //Getters

        /// <summary>
        /// Retorna o array da lista de flags.
        /// </summary>
        public Flag[] GetListaDeFlags => listaDeFlags;

        /// <summary>
        /// Retorna o valor de uma flag da lista de flags.
        /// </summary>
        /// <param name="nome">Nome da flag</param>
        /// <returns>Uma booleana</returns>
        public bool GetFlag(string nome)
        {
            return GetFlagStruct[nome].Valor;
        }

        /// <summary>
        /// Altera o valor de uma flag da lista de flags.
        /// </summary>
        /// <param name="nome">Nome da flag</param>
        /// <param name="valor">Novo valor da flag</param>
        public void SetFlag(string nome, bool valor)
        {
            GetFlagStruct[nome].Valor = valor;
        }

        /// <summary>
        /// Carrega a lista de flags com os valores na classe de save.
        /// </summary>
        /// <param name="listaDeFlagsSave">Classe de save da lista de flags.</param>
        public void CarregarFlags(ListaDeFlagsSave listaDeFlagsSave)
        {
            for (int i = 0; i < listaDeFlagsSave.valorDasFlags.Length; i++)
            {
                listaDeFlags[i].Valor = listaDeFlagsSave.valorDasFlags[i];
            }
        }

        //Cria e preenche o dicionario depois que o Unity desserizaliza o scriptable object
        public void OnAfterDeserialize()
        {
            GetFlagStruct = new Dictionary<string, Flag>();

            for (int i = 0; i < listaDeFlags.Length; i++)
            {
                GetFlagStruct.Add(listaDeFlags[i].Nome, listaDeFlags[i]);
            }
        }

        public void OnBeforeSerialize()
        {
            //Nada
        }

        [System.Serializable]
        public class Flag
        {
            //Variaveis
            [SerializeField] private string nome;
            [SerializeField] private bool valor;

            //Getters
            public string Nome => nome;
            
            public bool Valor
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
}

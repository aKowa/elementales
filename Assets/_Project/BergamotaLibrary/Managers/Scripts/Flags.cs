using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BergamotaLibrary
{
    public class Flags : MonoBehaviour
    {
        //Instancia do singleton
        private static Flags instance = null;

        //Variaveis
        [SerializeField] private ListaDeFlags[] listasDeFlags;

        private static Dictionary<string, ListaDeFlags> GetFlagList = new Dictionary<string, ListaDeFlags>();

        //Getters
        public static Flags Instance => instance;

        /// <summary>
        /// Retorma o dicionario da lista de listas de flags.
        /// </summary>
        public static Dictionary<string, ListaDeFlags> GetFlagListDictionary => GetFlagList;

        /// <summary>
        /// Retona uma lista de flags.
        /// </summary>
        /// <param name="nome">O nome da lista de flags.</param>
        /// <returns>Um scriptable object do tipo ListaDeFlags.</returns>
        public static ListaDeFlags GetListaDeFlags(string nome)
        {
            return GetFlagList[nome];
        }

        /// <summary>
        /// Retorna o valor de uma flag na lista de flags especificada.
        /// </summary>
        /// <param name="nomeDaListaDeFlags">Nome da lista de flags.</param>
        /// <param name="nomeDaFlag">Nome da flag.</param>
        /// <returns>Uma booleana.</returns>
        public static bool GetFlag(string nomeDaListaDeFlags, string nomeDaFlag)
        {
            return GetFlagList[nomeDaListaDeFlags].GetFlag(nomeDaFlag);
        }

        /// <summary>
        /// Altera o valor de uma flag da lista de flags especificada.
        /// </summary>
        /// <param name="nomeDaListaDeFlags">Nome da lista de flags.</param>
        /// <param name="nomeDaFlag">Nome da flag.</param>
        /// <param name="valor">Novo valor da flag.</param>
        public static void SetFlag(string nomeDaListaDeFlags, string nomeDaFlag, bool valor)
        {
            GetFlagList[nomeDaListaDeFlags].SetFlag(nomeDaFlag, valor);
        }

        /// <summary>
        /// Carrega as listas de flags com os valores na classe de save.
        /// </summary>
        /// <param name="flagsSave">Classe de save das flags.</param>
        public static void CarregarFlags(FlagsSave flagsSave)
        {
            for (int i = 0; i < flagsSave.listasDeFlags.Count; i++)
            {
                GetFlagList[flagsSave.listasDeFlags[i].chave].CarregarFlags(flagsSave.listasDeFlags[i]);
            }
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

            //Inicia a lista de flags
            if (GetFlagList.Count <= 0)
            {
                ResetarFlags();
            }
        }

        /// <summary>
        /// Reseta as listas de flags para os valores padroes dos scriptable objects.
        /// </summary>
        public void ResetarFlags()
        {
            //Cria um novo dicionario
            GetFlagList = new Dictionary<string, ListaDeFlags>();

            //Preenche o dicionario das listas de flags, adicionando as listas como copias do scriptable original
            for (int i = 0; i < listasDeFlags.Length; i++)
            {
                GetFlagList.Add(listasDeFlags[i].name, ScriptableObject.Instantiate(listasDeFlags[i]));
            }
        }
    }
}

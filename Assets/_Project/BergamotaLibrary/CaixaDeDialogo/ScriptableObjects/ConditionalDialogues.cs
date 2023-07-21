using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BergamotaDialogueSystem
{
    [CreateAssetMenu(menuName = "Dialogue/Conditional Dialogues")]

    public class ConditionalDialogues : ScriptableObject
    {
        //Variaveis

        [Header("Lista de Flags")]
        [SerializeField] private ListaDeFlags listaDeFlags;

        [Header("Dialogos Condicionais")]
        [SerializeField] private DialogoCondicional[] dialogosCondicionais;

        [Header("Listas de Dialogos Condicionais")]
        [SerializeField] private ListaDeDialogosCondicional[] listasDeDialogosCondicionais;

        /// <summary>
        /// Retorna um dialogo da lista de dialogos condicionais, caso todas as condicoes de um deles sejam verdadeiras.
        /// </summary>
        /// <returns>Um scriptable object do tipo DialogueObject</returns>
        public DialogueObject GetDialogo()
        {
            for (int i = 0; i < dialogosCondicionais.Length; i++)
            {
                if (dialogosCondicionais[i].CondicoesVerdadeiras(listaDeFlags.name) == true)
                {
                    return dialogosCondicionais[i].Dialogo;
                }
            }

            return null;
        }

        /// <summary>
        /// Retorna uma lista de dialogos da lista de listas de dialogos condicionais, caso todas as condicoes de uma delas sejam verdadeiras.
        /// </summary>
        /// <returns>Um scriptable object do tipo DialogueList</returns>
        public DialogueList GetListaDeDialogos()
        {
            for (int i = 0; i < listasDeDialogosCondicionais.Length; i++)
            {
                if (listasDeDialogosCondicionais[i].CondicoesVerdadeiras(listaDeFlags.name) == true)
                {
                    return listasDeDialogosCondicionais[i].ListaDeDialogos;
                }
            }

            return null;
        }

        [System.Serializable]
        private struct DialogoCondicional
        {
            [Header("Dialogo Condicional")]

            //Variaveis
            [SerializeField] private CondicaoDeFlag[] condicoes;
            [SerializeField] private DialogueObject dialogo;

            //Getters
            public DialogueObject Dialogo => dialogo;

            public bool CondicoesVerdadeiras(string nomeDaListaDeFlags)
            {
                for (int i = 0; i < condicoes.Length; i++)
                {
                    if (condicoes[i].Valor != Flags.GetListaDeFlags(nomeDaListaDeFlags).GetFlag(condicoes[i].Nome))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        [System.Serializable]
        private struct ListaDeDialogosCondicional
        {
            [Header("Lista de Dialogos Condicial")]

            //Variaveis
            [SerializeField] private CondicaoDeFlag[] condicoes;
            [SerializeField] private DialogueList listaDeDialogos;

            //Getters
            public DialogueList ListaDeDialogos => listaDeDialogos;

            public bool CondicoesVerdadeiras(string nomeDaListaDeFlags)
            {
                for (int i = 0; i < condicoes.Length; i++)
                {
                    if (condicoes[i].Valor != Flags.GetListaDeFlags(nomeDaListaDeFlags).GetFlag(condicoes[i].Nome))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public static bool CondicoesVerdadeiras(string nomeDaListaDeFlags, CondicaoDeFlag[] condicoes)
        {
            for (int i = 0; i < condicoes.Length; i++)
            {
                if (condicoes[i].Valor != Flags.GetListaDeFlags(nomeDaListaDeFlags).GetFlag(condicoes[i].Nome))
                {
                    return false;
                }
            }

            return true;
        }

        [System.Serializable]
        public struct CondicaoDeFlag
        {
            //Variaveis
            [SerializeField] private string nomeDaFlag;
            [SerializeField] private bool valor;

            //Getters
            public string Nome => nomeDaFlag;
            public bool Valor => valor;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Versoes serializaveis da Lista de Flags e das Flags

namespace BergamotaLibrary
{
    [System.Serializable]
    public class ListaDeFlagsSave
    {
        public string chave;
        public bool[] valorDasFlags;

        public ListaDeFlagsSave(string chave, ListaDeFlags listaDeFlags)
        {
            this.chave = chave;

            valorDasFlags = new bool[listaDeFlags.GetListaDeFlags.Length];

            for(int i = 0; i < listaDeFlags.GetListaDeFlags.Length; i++)
            {
                valorDasFlags[i] = listaDeFlags.GetListaDeFlags[i].Valor;
            }
        }
    }

    [System.Serializable]
    public class FlagsSave
    {
        public List<ListaDeFlagsSave> listasDeFlags;

        public FlagsSave()
        {
            listasDeFlags = new List<ListaDeFlagsSave>();

            foreach (KeyValuePair<string, ListaDeFlags> pair in Flags.GetFlagListDictionary)
            {
                listasDeFlags.Add(new ListaDeFlagsSave(pair.Key, pair.Value));
            }
        }
    }
}

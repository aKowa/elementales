using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesteFlags : MonoBehaviour
{
    //Variaveis
    [SerializeField] private ListaDeFlags listaDeFlags;
    [SerializeField] private string flag;
    [SerializeField] private bool valor;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            Debug.Log(listaDeFlags + " - Flag: " + flag + " - " + Flags.GetListaDeFlags(listaDeFlags.name).GetFlag(flag));
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Flags.GetListaDeFlags(listaDeFlags.name).SetFlag(flag, valor);
        }
    }
}

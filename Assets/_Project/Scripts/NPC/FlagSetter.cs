using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagSetter : MonoBehaviour
{
    //Componentes
    [SerializeField] private ListaDeFlags listaDeFlags;

    public void SetFlagAsTrue(string nomeDaFlag)
    {
        Flags.SetFlag(listaDeFlags.name, nomeDaFlag, true);
    }

    public void SetFlagAsFalse(string nomeDaFlag)
    {
        Flags.SetFlag(listaDeFlags.name, nomeDaFlag, false);
    }
}

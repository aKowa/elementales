using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    //Variaveis
    private static bool iniciandoBatalha = false;

    //Getters
    public static bool IniciandoBatalha
    {
        get
        {
            return iniciandoBatalha;
        }

        set
        {
            iniciandoBatalha = value;
        }
    }
}

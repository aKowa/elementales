using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TocadorDeSons : MonoBehaviour
{
    public void TocarSom(AudioClip audioClip)
    {
        SoundManager.instance.TocarSom(audioClip);
    }

    public void TocarSomIgnorandoPause(AudioClip audioClip)
    {
        SoundManager.instance.TocarSomIgnorandoPause(audioClip);
    }
}

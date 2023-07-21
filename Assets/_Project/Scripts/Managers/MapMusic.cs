using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMusic : MonoBehaviour
{
    //Variaveis
    [SerializeField] private MusicaDoMapa musicaDoMapa;

    [Space(10)]

    [SerializeField] private bool tocarMusicaNoAwake = true;

    private static AudioClip musicaAtual;
    private static float tempoDaMusica;

    private void Awake()
    {
        if(musicaDoMapa.Musica != musicaAtual || MusicManager.instance.Musica != musicaDoMapa.Musica)
        {
            musicaAtual = musicaDoMapa.Musica;

            if(tocarMusicaNoAwake == true)
            {
                TocarMusica();
            }
        }

        if(MusicManager.instance.MusicaTocando == false && tocarMusicaNoAwake == true)
        {
            TocarMusica();
        }
    }

    public static void TocarMusica()
    {
        if(musicaAtual != null)
        {
            MusicController.Instance.TrocarMusicaDoMapa(musicaAtual, 100);
        }
        else
        {
            Debug.LogWarning("A musica atual e nula!");
        }
    }

    public static void SalvarTempoDaMusicaDoMapaETocarOutra(AudioClip musicaParaTocar)
    {
        tempoDaMusica = MusicManager.instance.TempoDaMusica;

        MusicManager.instance.SetIntensidade(100);
        MusicManager.instance.TocarMusica(musicaParaTocar);
    }

    public static void ResumirMusicaDoMapa()
    {
        if (musicaAtual != null)
        {
            MusicController.Instance.ResumirMusicaDoMapa(musicaAtual, tempoDaMusica, 100);
        }
        else
        {
            Debug.LogWarning("A musica atual e nula!");
        }
    }
}

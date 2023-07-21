using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    //Instancia do singleton
    private static MusicController instance = null;

    //Variaveis
    private Coroutine corrotinaMusica;

    private bool esperandoFade;

    //Getters
    public static MusicController Instance => instance;

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

        corrotinaMusica = null;
        esperandoFade = false;
    }

    public void TrocarMusicaDoMapa(AudioClip musica, float velocidadeDosFades)
    {
        if(corrotinaMusica != null)
        {
            StopCoroutine(corrotinaMusica);
        }

        corrotinaMusica = StartCoroutine(TrocarMusicaDoMapaCorrotina(musica, velocidadeDosFades));
    }

    public void PararMusicaNoBlackout(float velocidadeDosFades)
    {
        if (corrotinaMusica != null)
        {
            StopCoroutine(corrotinaMusica);
        }

        corrotinaMusica = StartCoroutine(PararMusicaNoBlackoutCorrotina(velocidadeDosFades));
    }

    public void ResumirMusicaDoMapa(AudioClip musicaDoMapa, float tempoDaMusica, float velocidadeDosFades)
    {
        if (corrotinaMusica != null)
        {
            StopCoroutine(corrotinaMusica);
        }

        corrotinaMusica = StartCoroutine(ResumirMusicaDoMapaCorrotina(musicaDoMapa, tempoDaMusica, velocidadeDosFades));
    }

    private IEnumerator TrocarMusicaDoMapaCorrotina(AudioClip musica, float velocidadeDosFades)
    {
        esperandoFade = true;

        MusicManager.instance.FadeOut(0, velocidadeDosFades, () => esperandoFade = false);

        yield return new WaitUntil(() => esperandoFade == false);

        MusicManager.instance.SetIntensidade(100);

        MusicManager.instance.Looping = true;
        MusicManager.instance.TocarMusica(musica);

        corrotinaMusica = null;
    }

    private IEnumerator PararMusicaNoBlackoutCorrotina(float velocidadeDosFades)
    {
        esperandoFade = true;

        MusicManager.instance.FadeOut(0, velocidadeDosFades, () => esperandoFade = false);

        yield return new WaitUntil(() => esperandoFade == false);

        MusicManager.instance.PararMusica();
    }

    private IEnumerator ResumirMusicaDoMapaCorrotina(AudioClip musicaDoMapa, float tempoDaMusica, float velocidadeDosFades)
    {
        esperandoFade = true;

        MusicManager.instance.FadeOut(0, velocidadeDosFades, () => esperandoFade = false);

        yield return new WaitUntil(() => esperandoFade == false);

        MusicManager.instance.Looping = true;
        MusicManager.instance.TocarMusica(musicaDoMapa, tempoDaMusica);

        MusicManager.instance.FadeIn(100, velocidadeDosFades);

        corrotinaMusica = null;
    }
}

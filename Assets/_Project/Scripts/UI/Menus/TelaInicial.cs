using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TelaInicial : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private RectTransform fundoBloqueadorDeAcoes;

    [Header("Sons")]
    [SerializeField] private AudioClip musicaDaTela;

    //Variaveis
    [Header("Variaveis Iniciais")]
    [SerializeField] private SceneReference menuPrincipal;

    private void Awake()
    {
        fundoBloqueadorDeAcoes.gameObject.SetActive(false);
    }

    private void Start()
    {
        Transition.GetInstance().DoTransition("FadeOutWhite", 0);
        Transition.GetInstance().PauseTransitionForATime(0.5f);

        if (MusicManager.instance.MusicaTocando == true)
        {
            if (MusicManager.instance.Musica == musicaDaTela)
            {
                return;
            }

            MusicManager.instance.FadeOut(0, 100, () =>
            {
                MusicManager.instance.PararMusica();

                MusicManager.instance.SetIntensidade(100);
                MusicManager.instance.TocarMusica(musicaDaTela);
            });
        }
        else
        {
            MusicManager.instance.SetIntensidade(100);
            MusicManager.instance.TocarMusica(musicaDaTela);
        }
    }

    public void IrParaOMenuPrincipal()
    {
        fundoBloqueadorDeAcoes.gameObject.SetActive(false);

        FazerTransicaoProMenuPrincipal();
    }

    private void FazerTransicaoProMenuPrincipal()
    {
        Transition.GetInstance().DoTransition("FadeIn", 0, () =>
        {
            MapsManager.GetInstance().LoadSceneByName(menuPrincipal);
        });
    }
}

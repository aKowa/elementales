using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPrincipal : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private PlayerSO playerSO;
    [SerializeField] private RectTransform fundoBloqueadorDeAcoes;

    [Header("Sons")]
    [SerializeField] private AudioClip musicaDoMenu;

    //Variaveis
    [Header("Variaveis Iniciais")]
    [SerializeField] private SceneReference cenaInicial;
    [SerializeField] private string doorGuidInicial;
    [SerializeField] private string monsterCenterDoorGuidInicial;

    private void Awake()
    {
        PauseManager.Pausar(false);
        PauseManager.PermitirInput = true;
        PauseManager.PermitirInputGeral = true;

        NPCManager.IniciandoBatalha = false;
    }

    private void Start()
    {
        Transition.GetInstance().DoTransition("FadeOut", 0);
        Transition.GetInstance().PauseTransitionForATime(0.5f);

        if(MusicManager.instance.MusicaTocando == true)
        {
            if(MusicManager.instance.Musica == musicaDoMenu)
            {
                return;
            }

            MusicManager.instance.FadeOut(0, 100, () =>
            {
                MusicManager.instance.PararMusica();

                MusicManager.instance.SetIntensidade(100);
                MusicManager.instance.TocarMusica(musicaDoMenu);
            });
        }
        else
        {
            MusicManager.instance.SetIntensidade(100);
            MusicManager.instance.TocarMusica(musicaDoMenu);
        }
    }

    public void CriarNovoJogo()
    {
        fundoBloqueadorDeAcoes.gameObject.SetActive(true);

        playerSO.ResetarInformacoes(string.Empty);
        BergamotaLibrary.Flags.Instance.ResetarFlags();

        SceneInfoSave sceneInfoInicial = new SceneInfoSave();
        sceneInfoInicial.gateway.currentDoorGuid = doorGuidInicial;
        sceneInfoInicial.monsterCenterGateway.currentDoorGuid = monsterCenterDoorGuidInicial;

        SceneSpawnManager.CarregarInformacoes(sceneInfoInicial);

        FazerTransicaoProMapaDoInicioDoJogo();
    }

    public void SairDoJogo()
    {
        fundoBloqueadorDeAcoes.gameObject.SetActive(true);

        MusicManager.instance.FadeOut(0, 100, () =>
        {
            MusicManager.instance.PararMusica();
        });

        Transition.GetInstance().DoTransition("FadeIn", 0, () => Application.Quit());
    }

    private void FazerTransicaoProMapaDoInicioDoJogo()
    {
        BergamotaLibrary.PauseManager.PermitirInput = false;

        Transition.GetInstance().DoTransition("FadeIn", 0, () =>
        {
            MapsManager.GetInstance().LoadSceneByName(cenaInicial.ScenePath, () =>
            {
                Transition.GetInstance().DoTransition("FadeOut", 1f, () =>
                {
                    BergamotaLibrary.PauseManager.PermitirInput = true;
                    FindObjectOfType<JanelaMapaAtual>(true).MostrarNomeDoMapa();
                });
            });
        });
    }
}

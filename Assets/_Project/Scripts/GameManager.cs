using BergamotaDialogueSystem;
using BergamotaLibrary;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    //Instancia do singleton
    private static GameManager instance = null;

    //Variaveis
    [SerializeField] private PlayerSO playerSO;
    [SerializeField] private GameObject cenaBatalhaHolder;

    private GameObject objetoCenaAtual = null;
    private GameObject objetoCenaBatalha = null;

    private PlayableDirector directorAtual;

    private Action onDirectorStop;

    //Getters
    public static GameManager Instance => instance;

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

        PlayerData.SetPlayerData(playerSO);

        StartCoroutine(ContadorDeTempo());

        directorAtual = null;
    }

    private void Start()
    {
        if(SaveManager.SaveConfiguracoesExiste() == true)
        {
            SaveManager.CarregarConfiguracoes();
        }
        else
        {
            BergamotaLibrary.MusicManager.instance.SetVolume(100);
            BergamotaLibrary.SoundManager.instance.SetVolume(100);

            SaveManager.SalvarConfiguracoes();
        }
    }

    #region Battle
    //Batalha Wild
    public void StartBattle(ComandoArena ComandoDaArena, int numeroMonstrosBatalhandoCadaTimeAoMesmoTempo, PlayerData playerData, Sprite backgroundDaBatalha, Monster monstroSelvagem, AudioClip musicaDeBatalha, bool salvarTempoDaMusicaDoMapa, Action eventoVenceuABatalha = null)
    {
        BergamotaLibrary.PauseManager.PermitirInput = false;

        if(salvarTempoDaMusicaDoMapa == true)
        {
            MapMusic.SalvarTempoDaMusicaDoMapaETocarOutra(musicaDeBatalha);
        }
        else
        {
            MusicManager.instance.SetIntensidade(100);
            MusicManager.instance.TocarMusica(musicaDeBatalha);
        }

        Transition.GetInstance().DoTransition("BattleTransitionIn", 0, () =>
        {
            CreateBattle();
            BattleManager.Instance.CreateBattle(ComandoDaArena, numeroMonstrosBatalhandoCadaTimeAoMesmoTempo, playerData, backgroundDaBatalha, monstroSelvagem, eventoVenceuABatalha);

            Transition.GetInstance().DoTransition("BattleTransitionOut", 0.2f);
        });
    }
    //Batalha Npc
    public void StartBattle(ComandoArena ComandoDaArena, int numeroMonstrosBatalhandoCadaTimeAoMesmoTempo, PlayerData playerData, Sprite backgroundDaBatalha, NPCBatalha npcBatalha, AudioClip musicaDeBatalha, bool salvarTempoDaMusicaDoMapa, Action eventoVenceuABatalha = null)
    {
        BergamotaLibrary.PauseManager.PermitirInput = false;

        if (salvarTempoDaMusicaDoMapa == true)
        {
            MapMusic.SalvarTempoDaMusicaDoMapaETocarOutra(musicaDeBatalha);
        }
        else
        {
            MusicManager.instance.SetIntensidade(100);
            MusicManager.instance.TocarMusica(musicaDeBatalha);
        }

        Transition.GetInstance().DoTransition("BattleTransitionIn", 0, () =>
        {
            CreateBattle();
            BattleManager.Instance.CreateBattle(ComandoDaArena, numeroMonstrosBatalhandoCadaTimeAoMesmoTempo, playerData, backgroundDaBatalha, npcBatalha, eventoVenceuABatalha);

            Transition.GetInstance().DoTransition("BattleTransitionOut", 0.2f);
        });
    }

    //Batalha escolher se player vai poder correr
    public void StartBattle(ComandoArena ComandoDaArena, int numeroMonstrosBatalhandoCadaTimeAoMesmoTempo, PlayerData playerData, Sprite backgroundDaBatalha, Monster monstroSelvagem, AudioClip musicaDeBatalha, bool salvarTempoDaMusicaDoMapa, bool podeCorrer, Action eventoVenceuABatalha = null)
    {
        BergamotaLibrary.PauseManager.PermitirInput = false;

        if (salvarTempoDaMusicaDoMapa == true)
        {
            MapMusic.SalvarTempoDaMusicaDoMapaETocarOutra(musicaDeBatalha);
        }
        else
        {
            MusicManager.instance.SetIntensidade(100);
            MusicManager.instance.TocarMusica(musicaDeBatalha);
        }

        Transition.GetInstance().DoTransition("BattleTransitionIn", 0, () =>
        {
            CreateBattle();
            BattleManager.Instance.CreateBattle(ComandoDaArena, numeroMonstrosBatalhandoCadaTimeAoMesmoTempo, playerData, backgroundDaBatalha, monstroSelvagem, eventoVenceuABatalha, podeCorrer);

            Transition.GetInstance().DoTransition("BattleTransitionOut", 0.2f);
        });
    }

    private void CreateBattle()
    {
        if (objetoCenaBatalha == null)
        {
            objetoCenaBatalha = Instantiate(cenaBatalhaHolder);
            DontDestroyOnLoad(objetoCenaBatalha);
        }

        objetoCenaAtual = GameObject.FindGameObjectWithTag("Respawn").gameObject;

        objetoCenaAtual.gameObject.SetActive(false);
        objetoCenaBatalha.gameObject.SetActive(true);
    }

    public void FinishBattle()
    {
        objetoCenaBatalha.gameObject.SetActive(false);
        objetoCenaAtual.gameObject.SetActive(true);
    }

    #endregion

    #region Timeline

    public void IniciarTimeline(PlayableDirector director, float tempo = 0, Action onDirectorStop = null)
    {
        directorAtual = director;

        director.Play();
        director.time = tempo;

        this.onDirectorStop = onDirectorStop;
    }

    public void PararTimeline(PlayableDirector director)
    {
        director.Stop();
    }

    public void PararTimeline()
    {
        directorAtual.Stop();
    }

    public void PausarTimeline(PlayableDirector director)
    {
        director.playableGraph.GetRootPlayable(0).SetSpeed(0d);
    }

    public void PausarTimeline()
    {
        directorAtual.playableGraph.GetRootPlayable(0).SetSpeed(0d);
    }

    public void ResumirTimeline(PlayableDirector director)
    {
        director.playableGraph.GetRootPlayable(0).SetSpeed(1d);
    }

    public void ResumirTimeline()
    {
        directorAtual.playableGraph.GetRootPlayable(0).SetSpeed(1d);
    }

    public void EsperarDialogoParaResumirTimeline(PlayableDirector director)
    {
        StartCoroutine(EsperarDialogoParaResumirTimelineCorrotina(director));
    }

    public void FinalizarTimeline()
    {
        onDirectorStop?.Invoke();
    }

    private IEnumerator ContadorDeTempo()
    {
        yield return new WaitForSecondsRealtime(1);

        while (true)
        {
            playerSO.TempoDeJogo += 1;

            yield return new WaitForSecondsRealtime(1);
        }
    }

    private IEnumerator EsperarDialogoParaResumirTimelineCorrotina(PlayableDirector director)
    {
        yield return new WaitUntil(() => DialogueUI.Instance.IsOpen == false);

        ResumirTimeline(director);
    }

    #endregion
}
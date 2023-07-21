using BergamotaLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Cutscene : MonoBehaviour
{
    //Componentes
    [SerializeField] private PlayableDirector director;

    private PlayerInputManager playerInputManager;

    private void Awake()
    {
        playerInputManager = FindObjectOfType<PlayerInputManager>();
    }

    public void IniciarCutscene()
    {
        IniciarCutscene(0, null);
    }

    public void IniciarCutscene(float tempo = 0, Action onDirectorStop = null)
    {
        BloquearComandos();
        GameManager.Instance.IniciarTimeline(director, tempo, () =>
        {
            LiberarComandos();

            onDirectorStop?.Invoke();
        });
    }

    public void PararCutscene()
    {
        LiberarComandos();
        GameManager.Instance.PararTimeline(director);
    }

    public void ResumirCutscene()
    {
        GameManager.Instance.ResumirTimeline(director);
    }

    public void PausarCutscene()
    {
        GameManager.Instance.PausarTimeline(director);
    }

    private void BloquearComandos()
    {
        PauseManager.PermitirInputGeral = false;

        playerInputManager.gameObject.SetActive(false);
    }

    private void LiberarComandos()
    {
        PauseManager.PermitirInputGeral = true;

        playerInputManager.gameObject.SetActive(true);
    }
}

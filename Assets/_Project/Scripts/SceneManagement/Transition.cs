using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class Transition : Singleton<Transition>
{
    [SerializeField] private Animator transitionAnimator;
    private Action OnTransitionEnded;
    private float velocidadeAtual;
    private bool fazendoTransicao = false;

    public bool FazendoTransicao => fazendoTransicao;

    [Button]
    public void DoTransition(string transitionName, float timeToStartTransition, Action onTransitionEnded = null)
    {
        DoTransition(transitionName, timeToStartTransition, 1, onTransitionEnded);
    }

    [Button]
    public void DoTransition(string transitionName, float timeToStartTransition, float velocidadeDaAnimacao, Action onTransitionEnded = null)
    {
        velocidadeAtual = velocidadeDaAnimacao;

        transitionAnimator.speed = velocidadeAtual;
        OnTransitionEnded = onTransitionEnded;

        StopAllCoroutines();

        StartCoroutine(StartTransition(transitionName, timeToStartTransition));
    }

    public void SetEmptySprite()
    {
        transitionAnimator.Play("Vazio");
    }

    public void ResumeTransition()
    {
        transitionAnimator.speed = velocidadeAtual;
    }

    public void PauseTransition()
    {
        transitionAnimator.speed = 0;
    }

    public void PauseTransitionForATime(float time, bool ignorarTimeScale = false)
    {
        StartCoroutine(PauseTransitionForATimeCorroutine(time, ignorarTimeScale));
    }

    private IEnumerator StartTransition(string transitionName, float timeToStartTransition)
    {
        fazendoTransicao = true;

        yield return new WaitForSecondsRealtime(timeToStartTransition);

        transitionAnimator.Play(transitionName);

        yield return DoingTransition(transitionName);

        fazendoTransicao = false;
    }

    private IEnumerator DoingTransition(string transitionName)
    {
        yield return new WaitUntil(() => transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName(transitionName) == true);

        yield return new WaitUntil(() => transitionAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);

        OnTransitionEnded?.Invoke();
    }

    private IEnumerator PauseTransitionForATimeCorroutine(float time, bool ignorarTimeScale)
    {
        PauseTransition();

        if(ignorarTimeScale == false)
        {
            yield return new WaitForSeconds(time);
        }
        else
        {
            yield return new WaitForSecondsRealtime(time);
        }

        ResumeTransition();
    }
}

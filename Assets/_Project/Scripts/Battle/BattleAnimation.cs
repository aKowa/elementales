using BergamotaLibrary;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[SelectionBase]
public class BattleAnimation : MonoBehaviour
{
    //Componentes
    private SpriteRenderer spriteRenderer;

    [Header("Componentes")]
    [SerializeField] private Animacao animacao;
    [SerializeField] private Animacao animacaoSprite;
    [SerializeField] private EfeitosVisuais efeitosVisuais;
    [SerializeField] private Transform posicaoQuantidadeDano;

    //Enums
    public enum AnimacaoMovimento { Nenhum, TomandoDano, Surgindo, Desaparecendo, Morrendo, Tremer, AvancoRapido, EsticarETremer, Pular }
    public enum AnimacaoSprite { Idle, Atk, SpAtk, TomandoDano, Morrendo, Vazio }

    //Variaveis
    [Header("Variaveis")]
    [SerializeField] private float tempoEfeitoPiscar;
    [SerializeField] private float velocidadeEfeitoPiscar;

    private Coroutine efeito;

    //Getters
    public Transform PosicaoQuantidadeDano => posicaoQuantidadeDano;
    public UnityEvent EfeitoTerminou => efeitosVisuais.EfeitoTerminou;
    public string AnimacaoAtualMovimento => animacao.AnimacaoAtual;
    public string AnimacaoAtualSprite => animacaoSprite.AnimacaoAtual;

    public Vector3 PosicaoCentroDoMonstro()
    {
        Vector3 posicaoDaOrigem = spriteRenderer.transform.position;

        return new Vector3(posicaoDaOrigem.x, posicaoDaOrigem.y + spriteRenderer.bounds.extents.y, posicaoDaOrigem.z);
    }

    private void Awake()
    {
        spriteRenderer = animacaoSprite.GetComponent<SpriteRenderer>();
    }

    public void AtualizarAnimator(AnimatorOverrideController animatorOverrideController)
    {
        animacaoSprite.GetComponent<Animator>().runtimeAnimatorController = animatorOverrideController;
    }

    public void TrocarAnimacao(AnimacaoSprite novaAnimacao, AnimacaoMovimento novaAnimacaoMovimento)
    {
        animacaoSprite.TrocarAnimacao(novaAnimacao.ToString(), 0);
        animacao.TrocarAnimacao(novaAnimacaoMovimento.ToString(), 0);

        if(novaAnimacao == AnimacaoSprite.Atk || novaAnimacao == AnimacaoSprite.SpAtk)
        {
            animacaoSprite.TrocarAnimacaoAposOFimDaAtual(AnimacaoSprite.Idle.ToString());
        }
    }

    public void TrocarAnimacao(AnimacaoSprite novaAnimacao, float tempoAnimacaoSprite, AnimacaoMovimento novaAnimacaoMovimento, float tempoAnimacaoMovimento)
    {
        animacaoSprite.TrocarAnimacao(novaAnimacao.ToString(), tempoAnimacaoSprite);
        animacao.TrocarAnimacao(novaAnimacaoMovimento.ToString(), tempoAnimacaoMovimento);

        if (novaAnimacao == AnimacaoSprite.Atk || novaAnimacao == AnimacaoSprite.SpAtk)
        {
            animacaoSprite.TrocarAnimacaoAposOFimDaAtual(AnimacaoSprite.Idle.ToString());
        }
    }

    public void AnimacaoTomarDano(AudioClip somDano)
    {
        TrocarAnimacao(AnimacaoSprite.TomandoDano, AnimacaoMovimento.TomandoDano);
        animacao.ExecutarUmMetodoAposOFimDaAnimacao(FimAnimacaoDano);

        EfeitoPiscar(tempoEfeitoPiscar, velocidadeEfeitoPiscar);
        
        TocarSom(somDano);
    }
    public void TocarSom(AudioClip audio)
    {
        SoundManager.instance.TocarSom(audio);
    }

    public void FimAnimacaoDano()
    {
        TrocarAnimacao(AnimacaoSprite.Idle, AnimacaoMovimento.Nenhum);
    }

    public void ExecutarUmMetodoAposOFimDaAnimacaoDoMovimento(Action metodo)
    {
        animacao.ExecutarUmMetodoAposOFimDaAnimacao(metodo);
    }

    public void ExecutarUmMetodoAposOFimDaAnimacaoDoSprite(Action metodo)
    {
        animacaoSprite.ExecutarUmMetodoAposOFimDaAnimacao(metodo);
    }

    private void EfeitoPiscar(float tempo, float velocidade)
    {
        if(efeito != null)
        {
            StopCoroutine(efeito);
        }

        efeito = StartCoroutine(EfeitoPiscarCorrotina(tempo, velocidade));
    }



    public void SetTintEffect(Color cor, float velocidadeEfeito)
    {
        efeitosVisuais.SetTintEffect(cor, velocidadeEfeito);
    }

    public void SetTintEffectSlow(Color cor, float velocidadeEfeito)
    {
        efeitosVisuais.SetTintEffectSlow(cor, velocidadeEfeito);
    }

    public void SetTintSolidEffect(Color cor, float velocidadeEfeito)
    {
        efeitosVisuais.SetTintSolidEffect(cor, velocidadeEfeito);
    }

    public void SetTintSolidEffectSlow(Color cor, float velocidadeEfeito)
    {
        efeitosVisuais.SetTintSolidEffectSlow(cor, velocidadeEfeito);
    }

    private IEnumerator EfeitoPiscarCorrotina(float tempo, float velocidade)
    {
        float tempoPiscar = 0;

        while(tempo > 0)
        {
            tempo -= Time.deltaTime;
            tempoPiscar -= Time.deltaTime;

            if(tempoPiscar <= 0)
            {
                tempoPiscar += velocidade;
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }

            yield return null;
        }

        spriteRenderer.enabled = true;
    }
}

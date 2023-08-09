using BergamotaLibrary;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEffectAnimation : MonoBehaviour
{
    //Componentes
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Animacao animacao;

    //Variaveis

    Comando comandoAtual;

    private WaitUntil esperarDialogoFechar;

    private Sprite spriteDoItem;
    private bool trocarParaOSpriteDoItem;

    private int sortingOrderPadrao;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animacao = GetComponent<Animacao>();

        esperarDialogoFechar = new WaitUntil(() => BattleManager.Instance.GetDialogoAberto == false);

        sortingOrderPadrao = spriteRenderer.sortingOrder;

        trocarParaOSpriteDoItem = false;
    }

    private void LateUpdate()
    {
        if(trocarParaOSpriteDoItem == true)
        {
            spriteRenderer.sprite = spriteDoItem;
        }
    }

    public void ResetarEfeito()
    {
        trocarParaOSpriteDoItem = false;

        animator.speed = 1;

        EspelharNaDirecaoNormal();
        IrParaPosicao0();
    }

    public void IniciarEfeito(Comando comando)
    {
        comandoAtual = comando;

        animacao.TrocarAnimacao("Vazio");

        animator.runtimeAnimatorController = comandoAtual.AnimacaoEfeito;

        trocarParaOSpriteDoItem = false;
    }

    public void IniciarAnimacao()
    {
        animator.speed = 1;

        EspelharNaDirecaoNormal();
        IrParaPosicao0();

        SetSpriteSortingOrder(sortingOrderPadrao);

        animacao.TrocarAnimacao("Efeito", 0);
    }

    public void ExecutarAcao()
    {
        BattleManager.Instance.ExecutarAcao();
    }

    public void FinalizarAnimacao()
    {
        trocarParaOSpriteDoItem = false;

        animacao.TrocarAnimacao("Vazio");

        BattleManager.Instance.FinalizarEsperarAnimacao();
    }

    public void EspelharNaDirecaoNormal()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }

    public void EspelharNaDirecaoDaOrigem()
    {
        if(comandoAtual.Origem == null)
        {
            Debug.LogWarning("O comando nao tem origem para se espelhar na direcao!");
            return;
        }

        if (comandoAtual.Origem.MonstrosAtuais[comandoAtual.IndiceMonstro].BattleAnimation.transform.localScale.x >= 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void EspelharNaDirecaoDoAlvo()
    {
        if (comandoAtual.AlvoAcao.Count <= 0)
        {
            Debug.LogWarning("O comando nao tem um alvo para se espelhar na direcao!");
            return;
        }

        if (comandoAtual.AlvoAcao[0].BattleAnimation.transform.localScale.x >= 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void IrParaPosicao0()
    {
        transform.position = Vector3.zero;
    }

    public void IrParaPosicaoDaOrigem()
    {
        if (comandoAtual.Origem == null)
        {
            Debug.LogWarning("O comando nao tem origem para ir ate a posicao!");
            return;
        }

        transform.position = comandoAtual.Origem.MonstrosAtuais[comandoAtual.IndiceMonstro].BattleAnimation.PosicaoCentroDoMonstro();
    }

    public void IrParaPosicaoDoAlvo()
    {
        if(comandoAtual.AlvoAcao.Count <= 0)
        {
            Debug.LogWarning("O comando nao tem alvos para ir ate a posicao!");
            return;
        }

        int numeroDeAlvos = 0;
        Vector3 posicaoDosAlvos = Vector3.zero;

        for(int i = 0; i < comandoAtual.AlvoAcao.Count; i++)
        {
            posicaoDosAlvos += comandoAtual.AlvoAcao[i].BattleAnimation.PosicaoCentroDoMonstro();
            numeroDeAlvos++;
        }

        posicaoDosAlvos = posicaoDosAlvos / numeroDeAlvos;

        transform.position = posicaoDosAlvos;
    }

    public void FazerUmTweenParaPosicaoDaOrigem(float duracao)
    {
        if (comandoAtual.Origem == null)
        {
            Debug.LogWarning("O comando nao tem origem para ir ate a posicao!");
            return;
        }

        transform.DOMove(comandoAtual.Origem.MonstrosAtuais[comandoAtual.IndiceMonstro].BattleAnimation.PosicaoCentroDoMonstro(), duracao).SetEase(Ease.Linear);
    }

    public void FazerUmTweenParaPosicaoDoAlvo(float duracao)
    {
        if (comandoAtual.AlvoAcao.Count <= 0)
        {
            Debug.LogWarning("O comando nao tem alvos para ir ate a posicao!");
            return;
        }

        int numeroDeAlvos = 0;
        Vector3 posicaoDosAlvos = Vector3.zero;

        for (int i = 0; i < comandoAtual.AlvoAcao.Count; i++)
        {
            posicaoDosAlvos += comandoAtual.AlvoAcao[i].BattleAnimation.PosicaoCentroDoMonstro();
            numeroDeAlvos++;
        }

        posicaoDosAlvos = posicaoDosAlvos / numeroDeAlvos;

        transform.DOMove(posicaoDosAlvos, duracao).SetEase(Ease.Linear);
    }

    public void FazerOAlvoSurgir()
    {
        for (int i = 0; i < comandoAtual.AlvoAcao.Count; i++)
        {
            comandoAtual.AlvoAcao[i].BattleAnimation.TrocarAnimacao(BattleAnimation.AnimacaoSprite.Idle, BattleAnimation.AnimacaoMovimento.Surgindo);
        }
    }

    public void FazerOAlvoDesaparecer()
    {
        for (int i = 0; i < comandoAtual.AlvoAcao.Count; i++)
        {
            comandoAtual.AlvoAcao[i].BattleAnimation.TrocarAnimacao(BattleAnimation.AnimacaoSprite.Idle, BattleAnimation.AnimacaoMovimento.Desaparecendo);
        }
    }

    public void SetTrocarParaOSpriteDoItem()
    {
        if (comandoAtual is ComandoDeItem)
        {
            ComandoDeItem comandoDeItem = (ComandoDeItem)comandoAtual;

            spriteDoItem = comandoDeItem.ItemHolder.Item.Imagem;

            trocarParaOSpriteDoItem = true;
        }
        else
        {
            Debug.LogError("O comando nao e um comando de item e nao pode usar o sprite de um item!");
        }
    }

    public void SetTintEffectNaOrigem(ParametrosDeTintEffect efeito)
    {
        if (comandoAtual.Origem == null)
        {
            Debug.LogWarning("O comando nao tem origem para fazer um efeito!");
            return;
        }

        comandoAtual.Origem.MonstrosAtuais[comandoAtual.IndiceMonstro].BattleAnimation.SetTintEffect(efeito.CorDoEfeito, efeito.VelocidadeDoEfeito);
    }

    public void SetTintEffectNoAlvo(ParametrosDeTintEffect efeito)
    {
        for (int i = 0; i < comandoAtual.AlvoAcao.Count; i++)
        {
            comandoAtual.AlvoAcao[i].BattleAnimation.SetTintEffect(efeito.CorDoEfeito, efeito.VelocidadeDoEfeito);
        }
    }

    public void SetTintEffectSlowNaOrigem(ParametrosDeTintEffect efeito)
    {
        if (comandoAtual.Origem == null)
        {
            Debug.LogWarning("O comando nao tem origem para fazer um efeito!");
            return;
        }

        comandoAtual.Origem.MonstrosAtuais[comandoAtual.IndiceMonstro].BattleAnimation.SetTintEffectSlow(efeito.CorDoEfeito, efeito.VelocidadeDoEfeito);
    }

    public void SetTintEffectSlowNoAlvo(ParametrosDeTintEffect efeito)
    {
        for (int i = 0; i < comandoAtual.AlvoAcao.Count; i++)
        {
            comandoAtual.AlvoAcao[i].BattleAnimation.SetTintEffectSlow(efeito.CorDoEfeito, efeito.VelocidadeDoEfeito);
        }
    }

    public void SetSpriteSortingOrder(int ordem)
    {
        spriteRenderer.sortingOrder = ordem;
    }

    public void ConferirSeCapturouMonstro()
    {
        StartCoroutine(ConferirSeCapturouMonstroCorrotina());
    }

    private IEnumerator ConferirSeCapturouMonstroCorrotina()
    {
        animator.speed = 0;

        yield return esperarDialogoFechar;

        ExecutarAcao();

        if(BattleManager.Instance.MonstroCapturado != null)
        {
            BattleManager.Instance.FinalizarEsperarAnimacao();

            yield break;
        }

        animator.speed = 1;
    }
}

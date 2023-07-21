using System;
using System.Collections;
using UnityEngine;

namespace BergamotaLibrary
{
    [RequireComponent(typeof(Animator))]

    public class Animacao : MonoBehaviour
    {
        //Componentes
        private Animator animacao; //Animator

        //Variaveis
        private string animacaoAtual; //Guarda a animacao atual
        private Coroutine corrotinaTrocarAnimacao; //Guarda a corrotina de trocar animacoes

        //Getters
        public string AnimacaoAtual => animacaoAtual;
        public AnimatorStateInfo GetCurrentAnimatorClipInfo => animacao.GetCurrentAnimatorStateInfo(0);

        /// <summary>
        /// Retorna se o Animator deste script de Animacao esta rodando a animacao com o nome passado.
        /// </summary>
        /// <param name="animacao">Nome da animacao.</param>
        /// <returns>Uma booleana.</returns>
        public bool AnimatorEstaRodandoAAnimacao(string animacao)
        {
            return GetCurrentAnimatorClipInfo.IsName(animacao);
        }

        void Awake()
        {
            animacao = GetComponent<Animator>();
            animacaoAtual = string.Empty;
        }

        #region Funcoes com o Animator

        /// <summary>
        /// Troca a animacao atual.
        /// </summary>
        /// <param name="animacao">Nome da animacao.</param>
        public void TrocarAnimacao(string animacao)
        {
            animacaoAtual = animacao;
            this.animacao.Play(animacaoAtual);
        }

        /// <summary>
        /// Troca a animacao atual e roda a nova em um tempo especifico.
        /// </summary>
        /// <param name="animacao">Nome da animacao.</param>
        /// <param name="normalizedTime">Tempo normalizado para o inicio da animacao.</param>
        public void TrocarAnimacao(string animacao, float normalizedTime)
        {
            animacaoAtual = animacao;
            this.animacao.Play(animacaoAtual, 0, normalizedTime);
        }

        /// <summary>
        /// Troca a animacao atual e faz uma transicao entre ela e a nova.
        /// </summary>
        /// <param name="animacao">Nome da animacao.</param>
        /// <param name="duracaoDaTransicao">Duracao da transicao.</param>
        public void TrocarAnimacaoComTransicao(string animacao, float duracaoDaTransicao)
        {
            animacaoAtual = animacao;
            this.animacao.CrossFade(animacao, duracaoDaTransicao);
        }

        /// <summary>
        /// Atualiza o valor de um parametro int no animator.
        /// </summary>
        /// <param name="parametro">Nome do parametro.</param>
        /// <param name="valor">Valor do int.</param>
        public void SetInt(string parametro, int valor)
        {
            animacao.SetInteger(parametro, valor);
        }

        /// <summary>
        /// Atualiza o valor de um parametro float no animator.
        /// </summary>
        /// <param name="parametro">Nome do parametro.</param>
        /// <param name="valor">Valor do float.</param>
        public void SetFloat(string parametro, float valor)
        {
            animacao.SetFloat(parametro, valor);
        }

        /// <summary>
        /// Atualiza o valor de um parametro bool no animator.
        /// </summary>
        /// <param name="parametro">Nome do parametro.</param>
        /// <param name="valor">Valor do bool.</param>
        public void SetBool(string parametro, bool valor)
        {
            animacao.SetBool(parametro, valor);
        }

        /// <summary>
        /// Atualiza o valor de um parametro trigger no animator.
        /// </summary>
        /// <param name="parametro">Nome do parametro.</param>
        public void SetTrigger(string parametro)
        {
            animacao.SetTrigger(parametro);
        }

        #endregion

        #region Funcoes com Corrotinas

        /// <summary>
        /// Troca a animacao quando a atual chegar ao fim. Se a animacao atual for um looping, ela sera trocada apos o fim do primeiro loop.
        /// </summary>
        /// <param name="animacao">Nome da animacao.</param>
        public void TrocarAnimacaoAposOFimDaAtual(string animacao)
        {
            if (corrotinaTrocarAnimacao != null)
            {
                StopCoroutine(corrotinaTrocarAnimacao);
            }

            corrotinaTrocarAnimacao = StartCoroutine(TrocarAnimacaoAposUmPontoCorrotina(animacao, 1));
        }

        /// <summary>
        /// Troca a animacao e a roda em um tempo especifico quando a atual chegar ao fim. Se a animacao atual for um looping, ela sera trocada apos o fim do primeiro loop.
        /// </summary>
        /// <param name="animacao">Nome da animacao.</param>
        /// <param name="normalizedTime">Tempo normalizado para o inicio da animacao.</param>
        public void TrocarAnimacaoAposOFimDaAtual(string animacao, float normalizedTime)
        {
            if (corrotinaTrocarAnimacao != null)
            {
                StopCoroutine(corrotinaTrocarAnimacao);
            }

            corrotinaTrocarAnimacao = StartCoroutine(TrocarAnimacaoAposUmPontoCorrotina(animacao, normalizedTime, 1));
        }

        /// <summary>
        /// Troca a animacao e faz uma transicao entre ela e a nova quando a atual chegar ao fim. Se a animacao atual for um looping, ela sera trocada apos o fim do primeiro loop.
        /// </summary>
        /// <param name="animacao">Nome da animacao.</param>
        /// <param name="duracaoDaTransicao">Duracao da transicao.</param>
        public void TrocarAnimacaoComTransicaoAposOFimDaAtual(string animacao, float duracaoDaTransicao)
        {
            if (corrotinaTrocarAnimacao != null)
            {
                StopCoroutine(corrotinaTrocarAnimacao);
            }

            corrotinaTrocarAnimacao = StartCoroutine(TrocarAnimacaoComTransicaoAposUmPontoCorrotina(animacao, duracaoDaTransicao, 1));
        }

        /// <summary>
        /// Troca a animacao quando a atual chegar em um tempo especifico.
        /// </summary>
        /// <param name="animacao">Nome da animacao.</param>
        /// <param name="tempo">Tempo normalizado para trocar a animacao.</param>
        public void TrocarAnimacaoAposUmPontoDaAtual(string animacao, float tempo)
        {
            if (corrotinaTrocarAnimacao != null)
            {
                StopCoroutine(corrotinaTrocarAnimacao);
            }

            corrotinaTrocarAnimacao = StartCoroutine(TrocarAnimacaoAposUmPontoCorrotina(animacao, tempo));
        }

        /// <summary>
        /// Troca a animacao e a roda em um tempo especifico quando a atual chegar em um tempo especifico.
        /// </summary>
        /// <param name="animacao">Nome da animacao.</param>
        /// <param name="tempo">Tempo normalizado para trocar a animacao.</param>
        /// <param name="normalizedTime">Tempo normalizado para o inicio da animacao.</param>
        public void TrocarAnimacaoAposUmPontoDaAtual(string animacao, float tempo, float normalizedTime)
        {
            if (corrotinaTrocarAnimacao != null)
            {
                StopCoroutine(corrotinaTrocarAnimacao);
            }

            corrotinaTrocarAnimacao = StartCoroutine(TrocarAnimacaoAposUmPontoCorrotina(animacao, normalizedTime, tempo));
        }

        /// <summary>
        /// Troca a animacao e faz uma transicao entre ela e a nova quando a atual chegar em um tempo especifico.
        /// </summary>
        /// <param name="animacao">Nome da animacao.</param>
        /// <param name="duracaoDaTransicao">Duracao da transicao.</param>
        /// <param name="tempo">Tempo normalizado para trocar a animacao.</param>
        public void TrocarAnimacaoComTransicaoAposUmPontoDaAtual(string animacao, float duracaoDaTransicao, float tempo)
        {
            if (corrotinaTrocarAnimacao != null)
            {
                StopCoroutine(corrotinaTrocarAnimacao);
            }

            corrotinaTrocarAnimacao = StartCoroutine(TrocarAnimacaoComTransicaoAposUmPontoCorrotina(animacao, duracaoDaTransicao, tempo));
        }

        /// <summary>
        /// Executa um metodo sem retorno e sem parametros apos o fim da animacao atual.
        /// </summary>
        /// <param name="Metodo">O metodo a ser executado.</param>
        public void ExecutarUmMetodoAposOFimDaAnimacao(Action Metodo)
        {
            corrotinaTrocarAnimacao = StartCoroutine(ExecutarUmMetodoAposUmPontoCorrotina(Metodo, 1));
        }

        /// <summary>
        /// Executa um metodo sem retorno e sem parametros quando a animacao atual chegar em um ponto especifico.
        /// </summary>
        /// <param name="Metodo">O metodo a ser executado.</param>
        /// <param name="tempo">Tempo normalizado para o metodo ser executado.</param>
        public void ExecutarUmMetodoAposUmPontoDaAnimacao(Action Metodo, float tempo)
        {
            corrotinaTrocarAnimacao = StartCoroutine(ExecutarUmMetodoAposUmPontoCorrotina(Metodo, tempo));
        }

        /// <summary>
        /// Para todas as corrotinas que estejam rodando no script.
        /// </summary>
        public void PararCorrotinas()
        {
            StopAllCoroutines();
        }

        #endregion

        #region Corrotinas

        private IEnumerator TrocarAnimacaoAposUmPontoCorrotina(string animacao, float tempo)
        {
            string animacaoAtiva = animacaoAtual;

            yield return null; //Aguarda duas frames para garantir que a animacao foi trocado
            yield return null;

            //Aguarda ate que o Unity comece a tocar a animacao atual, caso esteja no meio de alguma transicao
            while (this.animacao.GetCurrentAnimatorStateInfo(0).IsName(animacaoAtiva) == false)
            {
                //Se a animacao atual for diferente da animacao na qual esta corrotina foi iniciada, a corrotina sera interrompida
                if (animacaoAtiva != animacaoAtual)
                {
                    yield break;
                }

                yield return null;
            }

            while (this.animacao.GetCurrentAnimatorStateInfo(0).normalizedTime < tempo)
            {
                //Se a animacao atual for diferente da animacao na qual esta corrotina foi iniciada, a corrotina sera interrompida
                if (animacaoAtiva != animacaoAtual)
                {
                    yield break;
                }

                yield return null;
            }

            TrocarAnimacao(animacao);
        }

        private IEnumerator TrocarAnimacaoAposUmPontoCorrotina(string animacao, float normalizedTime, float tempo)
        {
            string animacaoAtiva = animacaoAtual;

            yield return null; //Aguarda duas frames para garantir que a animacao foi trocado
            yield return null;

            //Aguarda ate que o Unity comece a tocar a animacao atual, caso esteja no meio de alguma transicao
            while (this.animacao.GetCurrentAnimatorStateInfo(0).IsName(animacaoAtiva) == false)
            {
                //Se a animacao atual for diferente da animacao na qual esta corrotina foi iniciada, a corrotina sera interrompida
                if (animacaoAtiva != animacaoAtual)
                {
                    yield break;
                }

                yield return null;
            }

            while (this.animacao.GetCurrentAnimatorStateInfo(0).normalizedTime < tempo)
            {
                //Se a animacao atual for diferente da animacao na qual esta corrotina foi iniciada, a corrotina sera interrompida
                if (animacaoAtiva != animacaoAtual)
                {
                    yield break;
                }

                yield return null;
            }

            TrocarAnimacao(animacao, normalizedTime);
        }

        private IEnumerator TrocarAnimacaoComTransicaoAposUmPontoCorrotina(string animacao, float duracaoDaTransicao, float tempo)
        {
            string animacaoAtiva = animacaoAtual;

            yield return null; //Aguarda duas frames para garantir que a animacao foi trocado
            yield return null;

            //Aguarda ate que o Unity comece a tocar a animacao atual, caso esteja no meio de alguma transicao
            while (this.animacao.GetCurrentAnimatorStateInfo(0).IsName(animacaoAtiva) == false)
            {
                //Se a animacao atual for diferente da animacao na qual esta corrotina foi iniciada, a corrotina sera interrompida
                if (animacaoAtiva != animacaoAtual)
                {
                    yield break;
                }

                yield return null;
            }

            while (this.animacao.GetCurrentAnimatorStateInfo(0).normalizedTime < tempo)
            {
                //Se a animacao atual for diferente da animacao na qual esta corrotina foi iniciada, a corrotina sera interrompida
                if (animacaoAtiva != animacaoAtual)
                {
                    yield break;
                }

                yield return null;
            }

            TrocarAnimacaoComTransicao(animacao, duracaoDaTransicao);
        }

        private IEnumerator ExecutarUmMetodoAposUmPontoCorrotina(Action Metodo, float tempo)
        {
            string animacaoAtiva = animacaoAtual;

            yield return null; //Aguarda duas frames para garantir que a animacao foi trocado
            yield return null;

            //Aguarda ate que o Unity comece a tocar a animacao atual, caso esteja no meio de alguma transicao
            while (this.animacao.GetCurrentAnimatorStateInfo(0).IsName(animacaoAtiva) == false)
            {
                //Se a animacao atual for diferente da animacao na qual esta corrotina foi iniciada, a corrotina sera interrompida
                if (animacaoAtiva != animacaoAtual)
                {
                    yield break;
                }

                yield return null;
            }

            while (this.animacao.GetCurrentAnimatorStateInfo(0).normalizedTime < tempo)
            {
                //Se a animacao atual for diferente da animacao na qual esta corrotina foi iniciada, a corrotina sera interrompida
                if (animacaoAtiva != animacaoAtual)
                {
                    yield break;
                }

                yield return null;
            }

            Metodo?.Invoke();
        }

        #endregion
    }
}

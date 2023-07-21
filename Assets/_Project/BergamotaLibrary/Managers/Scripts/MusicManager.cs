using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BergamotaLibrary
{
    [RequireComponent(typeof(AudioSource))]

    public class MusicManager : MonoBehaviour
    {
        //Instancia do singleton
        public static MusicManager instance = null;

        //Componentes
        private AudioSource audioSource;

        //Variaveis
        private float volume;
        private float intensidade;

        private Coroutine fadeEffect;
        private Coroutine executarMetodoQuandoAMusicaParar;

        //Getters
        public float Volume => volume;
        public float Intensidade => intensidade;
        public AudioClip Musica => audioSource.clip;
        public bool MusicaTocando => audioSource.isPlaying;

        public float TempoDaMusica
        {
            get => audioSource.time;
            set => audioSource.time = value;
        }

        public bool Looping
        {
            get => audioSource.loop;
            set => audioSource.loop = value;
        }

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
            if(transform.parent == null)
            {
                DontDestroyOnLoad(transform.gameObject);
            }

            //Componentes
            audioSource = GetComponent<AudioSource>();

            //Faz a musica ignorar as pausas ao AudionListener do Unity, para continuar tocando enquanto o jogo esta pausado.
            audioSource.ignoreListenerPause = true;

            audioSource.Stop();

            //Variaveis
            SetVolume(100);
            SetIntensidade(100);
        }

        private void SetMusica(AudioClip musica)
        {
            PararMusica();

            this.audioSource.clip = musica;
        }

        /// <summary>
        /// Toca ou despausa a musica.
        /// </summary>
        public void TocarMusica()
        {
            TocarMusica(0);
        }

        /// <summary>
        /// Toca ou despausa a musica.
        /// </summary>
        /// <param name="tempo">Tempo em que a musica sera tocada.</param>
        public void TocarMusica(float tempo)
        {
            audioSource.Play();
            audioSource.time = tempo;
        }

        /// <summary>
        /// Troca e toca a musica.
        /// </summary>
        /// <param name="musica">Nova musica para tocar</param>
        public void TocarMusica(AudioClip musica)
        {
            SetMusica(musica);

            TocarMusica();
        }

        /// <summary>
        /// Troca e toca a musica.
        /// </summary>
        /// <param name="musica">Nova musica para tocar</param>
        /// <param name="tempo">Tempo em que a musica sera tocada.</param>
        public void TocarMusica(AudioClip musica, float tempo)
        {
            SetMusica(musica);

            TocarMusica(tempo);
        }

        /// <summary>
        /// Pausa a musica.
        /// </summary>
        public void PausarMusica()
        {
            audioSource.Pause();
        }

        /// <summary>
        /// Para de tocar a musica.
        /// </summary>
        public void PararMusica()
        {
            audioSource.Stop();
        }

        /// <summary>
        /// Altera o volume dos sons.
        /// </summary>
        /// <param name="novoVolume">Volume entre 0 e 100</param>
        public void SetVolume(float novoVolume)
        {
            volume = Mathf.Clamp(novoVolume, 0, 100);

            AtualizarAudioSources();
        }

        /// <summary>
        /// Altera a intensidade dos sons.
        /// </summary>
        /// <param name="novaIntensidade">Intensidade entre 0 e 100</param>
        public void SetIntensidade(float novaIntensidade)
        {
            intensidade = Mathf.Clamp(novaIntensidade, 0, 100);

            AtualizarAudioSources();
        }

        private void AtualizarAudioSources()
        {
            audioSource.volume = (volume / 100) * (intensidade / 100);
        }

        /// <summary>
        /// Aumenta a intensidade do som ate o valor desejado.
        /// </summary>
        /// <param name="intensidadeFinal">O valor no qual a intensidade tem que chegar</param>
        /// <param name="velocidade">O quanto a intensidade vai aumentar por segundo</param>
        public void FadeIn(float intensidadeFinal, float velocidade, Action onFadeEnd = null)
        {
            //Confere se nao ha uma corrotina ativa para iniciar outra, se houver, interrompe ela
            if (fadeEffect != null)
            {
                StopCoroutine(fadeEffect);
            }

            fadeEffect = StartCoroutine(FadeInCorroutine(intensidadeFinal, velocidade, onFadeEnd));
        }

        /// <summary>
        /// Diminui a intensidade do som ate o valor desejado.
        /// </summary>
        /// <param name="intensidadeFinal">O valor no qual a intensidade tem que chegar</param>
        /// <param name="velocidade">O quanto a intensidade vai diminuir por segundo</param>
        public void FadeOut(float intensidadeFinal, float velocidade, Action onFadeEnd = null)
        {
            //Confere se nao ha uma corrotina ativa para iniciar outra, se houver, interrompe ela
            if (fadeEffect != null)
            {
                StopCoroutine(fadeEffect);
            }

            fadeEffect = StartCoroutine(FadeOutCorroutine(intensidadeFinal, velocidade, onFadeEnd));
        }

        public void ExecutarUmMetodoQuandoAMusicaParar(Action onMusicEnd)
        {
            //Confere se nao ha uma corrotina ativa para iniciar outra, se houver, interrompe ela
            if (executarMetodoQuandoAMusicaParar != null)
            {
                StopCoroutine(executarMetodoQuandoAMusicaParar);
            }

            executarMetodoQuandoAMusicaParar = StartCoroutine(ExecutarUmMetodoQuandoAMusicaPararCorrotina(onMusicEnd));
        }

        private IEnumerator FadeInCorroutine(float intensidadeFinal, float velocidade, Action onFadeEnd)
        {
            while (intensidade < intensidadeFinal)
            {
                SetIntensidade(intensidade + velocidade * Time.unscaledDeltaTime);

                yield return null;
            }

            intensidade = intensidadeFinal;

            onFadeEnd?.Invoke();

            fadeEffect = null;
        }

        private IEnumerator FadeOutCorroutine(float intensidadeFinal, float velocidade, Action onFadeEnd)
        {
            while (intensidade > intensidadeFinal)
            {
                SetIntensidade(intensidade - velocidade * Time.unscaledDeltaTime);

                yield return null;
            }

            intensidade = intensidadeFinal;

            onFadeEnd?.Invoke();

            fadeEffect = null;
        }

        private IEnumerator ExecutarUmMetodoQuandoAMusicaPararCorrotina(Action onMusicEnd)
        {
            yield return new WaitUntil(() => MusicaTocando == false && TempoDaMusica == 0);

            onMusicEnd?.Invoke();

            executarMetodoQuandoAMusicaParar = null;
        }
    }
}

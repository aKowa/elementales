using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BergamotaLibrary
{
    [RequireComponent(typeof(AudioSource))]

    public class SoundManager : MonoBehaviour
    {
        //Instancia do singleton
        public static SoundManager instance = null;

        //Componentes
        private AudioSource audioSource;
        private AudioSource audioSourceIgnorandoPause;

        //Variaveis
        private float volume;
        private float intensidade;

        private Coroutine fadeEffect;

        //Getters
        public float Volume => volume;
        public float Intensidade => intensidade;

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

            //Componentes
            audioSource = GetComponent<AudioSource>();

            //Duplicar o Audio Source para fazer a versao que ignora o pause
            audioSourceIgnorandoPause = gameObject.AddComponent<AudioSource>() as AudioSource;
            audioSourceIgnorandoPause.ignoreListenerPause = true;

            CopiarAudioSource(audioSourceIgnorandoPause, audioSource);

            //Variaveis
            SetVolume(100);
            SetIntensidade(100);
        }

        public void TocarSom(AudioClip som)
        {
            audioSource.PlayOneShot(som);
        }

        public void TocarSomIgnorandoPause(AudioClip audio)
        {
            audioSourceIgnorandoPause.PlayOneShot(audio);
        }

        private void CopiarAudioSource(AudioSource audioSource, AudioSource audioSource2)
        {
            audioSource.outputAudioMixerGroup = audioSource2.outputAudioMixerGroup;
            audioSource.mute = audioSource2.mute;
            audioSource.bypassEffects = audioSource2.bypassEffects;
            audioSource.bypassListenerEffects = audioSource2.bypassListenerEffects;
            audioSource.bypassReverbZones = audioSource2.bypassReverbZones;
            audioSource.playOnAwake = audioSource2.playOnAwake;
            audioSource.loop = audioSource2.loop;

            audioSource.priority = audioSource2.priority;
            audioSource.volume = audioSource2.volume;
            audioSource.pitch = audioSource2.pitch;
            audioSource.panStereo = audioSource2.panStereo;
            audioSource.spatialBlend = audioSource2.spatialBlend;
            audioSource.reverbZoneMix = audioSource2.reverbZoneMix;

            audioSource.dopplerLevel = audioSource2.dopplerLevel;
            audioSource.spread = audioSource2.spread;
            audioSource.rolloffMode = audioSource2.rolloffMode;
            audioSource.minDistance = audioSource2.minDistance;
            audioSource.maxDistance = audioSource2.maxDistance;
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
            audioSourceIgnorandoPause.volume = (volume / 100) * (intensidade / 100);
        }

        /// <summary>
        /// Aumenta a intensidade do som ate o valor desejado.
        /// </summary>
        /// <param name="intensidadeFinal">O valor no qual a intensidade tem que chegar</param>
        /// <param name="velocidade">O quanto a intensidade vai aumentar por segundo</param>
        public void FadeIn(float intensidadeFinal, float velocidade)
        {
            //Confere se nao ha uma corrotina ativa para iniciar outra, se houver, interrompe ela
            if (fadeEffect != null)
            {
                StopCoroutine(fadeEffect);
            }

            fadeEffect = StartCoroutine(FadeInCorroutine(intensidadeFinal, velocidade));
        }

        /// <summary>
        /// Diminui a intensidade do som ate o valor desejado.
        /// </summary>
        /// <param name="intensidadeFinal">O valor no qual a intensidade tem que chegar</param>
        /// <param name="velocidade">O quanto a intensidade vai diminuir por segundo</param>
        public void FadeOut(float intensidadeFinal, float velocidade)
        {
            //Confere se nao ha uma corrotina ativa para iniciar outra, se houver, interrompe ela
            if (fadeEffect != null)
            {
                StopCoroutine(fadeEffect);
            }

            fadeEffect = StartCoroutine(FadeOutCorroutine(intensidadeFinal, velocidade));
        }

        private IEnumerator FadeInCorroutine(float intensidadeFinal, float velocidade)
        {
            while (intensidade < intensidadeFinal)
            {
                SetIntensidade(intensidade + velocidade * Time.unscaledDeltaTime);

                yield return null;
            }

            intensidade = intensidadeFinal;
        }

        private IEnumerator FadeOutCorroutine(float intensidadeFinal, float velocidade)
        {
            while (intensidade > intensidadeFinal)
            {
                SetIntensidade(intensidade - velocidade * Time.unscaledDeltaTime);

                yield return null;
            }

            intensidade = intensidadeFinal;
        }
    }
}

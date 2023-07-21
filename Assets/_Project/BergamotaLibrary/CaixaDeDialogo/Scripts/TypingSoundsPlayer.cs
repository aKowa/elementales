using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BergamotaDialogueSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class TypingSoundsPlayer : MonoBehaviour
    {
        //Componentes
        private AudioSource audioSource;

        //Variaveis
        private float volume;
        private float intensidade;

        //Getters
        public bool SomTocando => audioSource.isPlaying;

        private void Awake()
        {
            //Componentes
            audioSource = GetComponent<AudioSource>();

            audioSource.ignoreListenerPause = true;
        }

        private void Start()
        {
            //Variaveis
            AtualizarVolumes();
        }

        public void TocarSom(AudioClip som)
        {
            audioSource.clip = som;
            audioSource.Play();
        }

        public void TocarSomOneShot(AudioClip som)
        {
            audioSource.PlayOneShot(som);
        }

        private void AtualizarVolumes()
        {
            volume = SoundManager.instance.Volume;
            intensidade = SoundManager.instance.Intensidade;
        }

        public void AtualizarAudioSources()
        {
            AtualizarVolumes();

            audioSource.volume = (volume / 100) * (intensidade / 100);
        }
    }
}
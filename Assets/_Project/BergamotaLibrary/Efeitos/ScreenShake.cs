using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BergamotaLibrary
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]

    public class ScreenShake : MonoBehaviour
    {
        //Componentes
        private CinemachineVirtualCamera cinemachineVirtualCamera;
        private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

        //Variaveis
        private float tempo;
        private float tempoMax;
        private float intensidade;

        private Coroutine shakingScreen;

        private void Awake()
        {
            //Componentes
            cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
            cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            //Variaveis
            tempo = 0;
            tempoMax = 0;
        }

        /// <summary>
        /// Faz a camera tremer.
        /// </summary>
        /// <param name="intensidade">A intensidade com que a camera vai tremer</param>
        /// <param name="tempo">O tempo que ela vai ficar tremendo</param>
        public void ShakeCamera(float intensidade, float tempo)
        {
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensidade;

            this.intensidade = intensidade;
            tempoMax = tempo;
            this.tempo = tempo;

            //Confere se nao ha uma corrotina ativa para iniciar outra, se houver, interrompe ela
            if (shakingScreen != null)
            {
                StopCoroutine(shakingScreen);
            }

            shakingScreen = StartCoroutine(ShakingCamera());
        }

        private IEnumerator ShakingCamera()
        {
            while (tempo > 0)
            {
                tempo -= Time.deltaTime;

                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(0f, intensidade, tempo / tempoMax);

                yield return null;
            }
        }
    }
}

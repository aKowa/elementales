using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BergamotaLibrary
{
    public class Piscar : MonoBehaviour
    {
        //Componentes
        private SpriteRenderer spriteRenderer;
        private Image image;
        private TMP_Text texto;

        //Variaveis
        [SerializeField] private float tempo;
        private float tempo2;

        void Awake()
        {
            //Componentes
            spriteRenderer = GetComponent<SpriteRenderer>();
            image = GetComponent<Image>();
            texto = GetComponent<TMP_Text>();

            //Variaveis
            tempo2 = 0;
        }

        void Update()
        {
            tempo2 += Time.deltaTime;

            if (tempo2 >= tempo)
            {
                tempo2 -= tempo;

                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = !spriteRenderer.enabled;
                }

                if (image != null)
                {
                    image.enabled = !image.enabled;
                }

                if (texto != null)
                {
                    texto.enabled = !texto.enabled;
                }
            }
        }
    }
}

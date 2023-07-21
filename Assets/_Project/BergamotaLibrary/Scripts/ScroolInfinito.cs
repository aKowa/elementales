using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BergamotaLibrary
{
    public class ScroolInfinito : MonoBehaviour
    {
        //Componentes
        private SpriteRenderer spriteRenderer;
        private Image image;

        //Variaveis
        [SerializeField] private float velocidadeX;
        [SerializeField] private float velocidadeY;

        private float largura, altura;
        private Vector2 posicaoInicial;

        private void Awake()
        {
            //Componentes
            spriteRenderer = GetComponent<SpriteRenderer>();
            image = GetComponent<Image>();

            //Variaveis
            largura = 0;
            altura = 0;

            if (spriteRenderer != null)
            {
                largura = spriteRenderer.bounds.size.x;
                altura = spriteRenderer.bounds.size.y;
            }
            else if (image != null)
            {
                Rect retanguloGlobal = LiBergamota.GetWorldRect(image.rectTransform);

                largura = retanguloGlobal.size.x;
                altura = retanguloGlobal.size.x;
            }
            else
            {
                Debug.LogWarning("Nao ha nenhum SpriteRenderer ou Image para ser usado neste Scrool Infinito!", this);
                this.enabled = false;
            }

            posicaoInicial = transform.position;
        }

        private void FixedUpdate()
        {
            transform.position = (Vector2)transform.position + (new Vector2(velocidadeX, velocidadeY) * Time.deltaTime);

            if (transform.position.x < posicaoInicial.x - largura)
            {
                transform.position += new Vector3(largura, 0);
            }

            if (transform.position.x > posicaoInicial.x + largura)
            {
                transform.position -= new Vector3(largura, 0);
            }

            if (transform.position.y < posicaoInicial.y - altura)
            {
                transform.position += new Vector3(0, altura);
            }

            if (transform.position.y > posicaoInicial.y + altura)
            {
                transform.position -= new Vector3(0, altura);
            }
        }
    }
}

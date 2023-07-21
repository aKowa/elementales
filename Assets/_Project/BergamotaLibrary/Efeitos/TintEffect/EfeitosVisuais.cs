using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BergamotaLibrary
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(MaterialTintColor))]

    public class EfeitosVisuais : MonoBehaviour
    {
        //Componentes
        private SpriteRenderer spriteRenderer;
        private MaterialTintColor materialTintColor;

        [SerializeField] private Material materialTint;
        [SerializeField] private Material materialTintSolid;

        //Variaveis
        private Color tintColor;
        private float tintFadeSpeed;

        private Coroutine tintEffect;

        private UnityEvent efeitoTerminou = new UnityEvent();

        //Getters
        public UnityEvent EfeitoTerminou => efeitoTerminou;

        private void Start()
        {
            //Componentes
            spriteRenderer = GetComponent<SpriteRenderer>();
            materialTintColor = materialTintColor = GetComponent<MaterialTintColor>();

            materialTint = Instantiate(materialTint);
            materialTintSolid = Instantiate(materialTintSolid);

            //Variaveis
            tintColor = new Color(1, 0, 0, 0);
            tintFadeSpeed = 0;
        }

        /// <summary>
        /// Inicia o efeito de piscar com a cor transparente.
        /// </summary>
        /// <param name="cor">Cor</param>
        /// <param name="velocidadeEfeito">Velocidade do efeito</param>
        public void SetTintEffect(Color cor, float velocidadeEfeito)
        {
            spriteRenderer.material = materialTint;
            materialTintColor.SetMaterial(materialTint);
            tintColor = cor;
            tintFadeSpeed = velocidadeEfeito;

            IniciarCorrotinaTintEffect();
        }

        /// <summary>
        /// Inicia o efeito de piscar com a cor solida.
        /// </summary>
        /// <param name="cor">Cor</param>
        /// <param name="velocidadeEfeito">Velocidade do efeito</param>
        public void SetTintSolidEffect(Color cor, float velocidadeEfeito)
        {
            spriteRenderer.material = materialTintSolid;
            materialTintColor.SetMaterial(materialTintSolid);
            tintColor = cor;
            tintFadeSpeed = velocidadeEfeito;

            IniciarCorrotinaTintEffect();
        }

        /// <summary>
        /// Inicia o efeito de piscar lentamente com a cor transparente.
        /// </summary>
        /// <param name="cor">Cor</param>
        /// <param name="velocidadeEfeito">Velocidade do efeito</param>
        public void SetTintEffectSlow(Color cor, float velocidadeEfeito)
        {
            spriteRenderer.material = materialTint;
            materialTintColor.SetMaterial(materialTint);
            tintColor = cor;
            tintFadeSpeed = velocidadeEfeito;

            IniciarCorrotinaTintEffectSlow();
        }

        /// <summary>
        /// Inicia o efeito de piscar lentamente com a cor solida.
        /// </summary>
        /// <param name="cor">Cor</param>
        /// <param name="velocidadeEfeito">Velocidade do efeito</param>
        public void SetTintSolidEffectSlow(Color cor, float velocidadeEfeito)
        {
            spriteRenderer.material = materialTintSolid;
            materialTintColor.SetMaterial(materialTintSolid);
            tintColor = cor;
            tintFadeSpeed = velocidadeEfeito;

            IniciarCorrotinaTintEffectSlow();
        }

        private void IniciarCorrotinaTintEffect()
        {
            //Confere se nao ha uma corrotina ativa para iniciar outra, se houver, interrompe ela
            if (tintEffect != null)
            {
                InterromperCorrotina();
            }

            tintEffect = StartCoroutine(TintEffect());
        }

        private void IniciarCorrotinaTintEffectSlow()
        {
            //Confere se nao ha uma corrotina ativa para iniciar outra, se houver, interrompe ela
            if (tintEffect != null)
            {
                InterromperCorrotina();
            }

            tintEffect = StartCoroutine(TintEffectSlow());
        }

        private void InterromperCorrotina()
        {
            StopCoroutine(tintEffect);

            materialTintColor.SetTintColor(new Color(1, 0, 0, 0));
        }

        private IEnumerator TintEffect()
        {
            materialTintColor.SetTintColor(tintColor);

            while (tintColor.a > 0)
            {
                tintColor.a = Mathf.Clamp01(tintColor.a - tintFadeSpeed * Time.deltaTime);
                materialTintColor.SetTintColor(tintColor);

                yield return null;
            }

            efeitoTerminou?.Invoke();
        }

        private IEnumerator TintEffectSlow()
        {
            tintColor.a = 0;

            materialTintColor.SetTintColor(tintColor);

            while (tintColor.a < 1)
            {
                tintColor.a = Mathf.Clamp01(tintColor.a + tintFadeSpeed * Time.deltaTime);
                materialTintColor.SetTintColor(tintColor);

                yield return null;
            }

            while (tintColor.a > 0)
            {
                tintColor.a = Mathf.Clamp01(tintColor.a - tintFadeSpeed * Time.deltaTime);
                materialTintColor.SetTintColor(tintColor);

                yield return null;
            }

            efeitoTerminou?.Invoke();
        }
    }
}

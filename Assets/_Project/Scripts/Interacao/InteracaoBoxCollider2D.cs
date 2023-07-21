using System.Collections;
using UnityEngine;

namespace BergamotaLibrary
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class InteracaoBoxCollider2D : MonoBehaviour
    {
        private BoxCollider2D boxCollider2D;
        private Interagivel interagivelAtual;

        [SerializeField] private LayerMask m_LayerMask;

        private void Awake()
        {
            boxCollider2D = GetComponent<BoxCollider2D>();
            interagivelAtual = null;
        }

        private void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(ProcurarInteragivel());
        }

        /// <summary>
        /// Interage com um objeto, caso haja algum para interagir.
        /// </summary>
        public void Interagir(Player player)
        {
            interagivelAtual?.Interagir(player);
        }

        private IEnumerator ProcurarInteragivel()
        {
            while (true)
            {
                Interagivel interagivelAnterior = interagivelAtual;

                interagivelAtual = null;

                Collider2D[] hitColliders = Physics2D.OverlapBoxAll(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, m_LayerMask);

                for (int i = 0; i < hitColliders.Length; i++)
                {
                    Interagivel interagivel = hitColliders[i].GetComponent<Interagivel>();

                    if (interagivel == null)
                    {
                        continue;
                    }

                    if (interagivelAtual == null || LiBergamota.Distancia(this.transform.position, interagivel.transform.position) < LiBergamota.Distancia(this.transform.position, interagivelAtual.transform.position))
                    {
                        interagivelAtual = interagivel;
                    }
                }
                
                if(interagivelAtual != interagivelAnterior)
                {
                    if(interagivelAnterior != null)
                    {
                        interagivelAnterior.NaAreaDeInteracao(false);
                    }

                    if (interagivelAtual != null)
                    {
                        interagivelAtual.NaAreaDeInteracao(true);
                    }
                }

                yield return new WaitForSeconds(0.3f);
            }
        }
    }
}
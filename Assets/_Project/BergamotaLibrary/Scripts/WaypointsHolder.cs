using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BergamotaLibrary
{
    public class WaypointsHolder : MonoBehaviour
    {
        //Variaveis
        [SerializeField] private List<Transform> waypoints;

        [Header("Opcoes")]
        [SerializeField] private bool idaEVolta;

        //Getters
        public List<Transform> Waypoints => waypoints;

        private void Awake()
        {
            if (idaEVolta == true)
            {
                GerarIdaEVolta();
            }
        }

        /// <summary>
        /// Preenche a lista de waypoints com as posicoes na ordem inversa, com excecao do ultimo e primeiro item da lista.
        /// </summary>
        public void GerarIdaEVolta()
        {
            int valor = waypoints.Count;
            for (int i = 1; i < valor - 1; i++)
            {
                waypoints.Add(waypoints[valor - i - 1]);
            }
        }

        /// <summary>
        /// Retorna o indice do Waypoint mais proximo da posicao que for passada.
        /// </summary>
        /// <param name="posicao">Posicao</param>
        /// <returns>O indice do Waypoint na lista Waypoints</returns>
        public int IndiceWaypointMaisProximo(Transform posicao)
        {
            int indice = 0;
            float menorDistancia = LiBergamota.Distancia(posicao.position, waypoints[0].position);

            for (int i = 0; i < waypoints.Count; i++)
            {
                float distancia = LiBergamota.Distancia(posicao.position, waypoints[i].position);

                if (distancia < menorDistancia)
                {
                    indice = i;
                    menorDistancia = distancia;
                }
            }

            return indice;
        }

        /// <summary>
        /// Retorna se uma posicao ja chegou ou passou de um Waypoint.
        /// </summary>
        /// <param name="posicaoOrigem">Posicao em que o movimento foi iniciado</param>
        /// <param name="waypoint">Waypoint</param>
        /// <param name="posicaoAtual">Posicao atual</param>
        /// <returns>Uma booleana</returns>
        public bool ChegouNoWaypoint(Vector3 posicaoOrigem, Vector3 waypoint, Vector3 posicaoAtual)
        {
            return LiBergamota.PassouDoPonto(posicaoOrigem, waypoint, posicaoAtual) == true || LiBergamota.Distancia(posicaoAtual, waypoint) <= 0.01;
        }
    }
}
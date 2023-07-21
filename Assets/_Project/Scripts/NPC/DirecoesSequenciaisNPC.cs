using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirecoesSequenciaisNPC : MonoBehaviour
{
    //Componentes
    private NPCComum npc;

    //Variaveis
    [SerializeField] private List<DirecaoParaOlhar> direcoes;

    [Header("Opcoes")]
    [SerializeField] private bool idaEVolta;

    private int indicePosicao;
    private float tempoPosicao;

    private void Awake()
    {
        //Componentes
        npc = GetComponent<NPCComum>();

        //Variaveis
        indicePosicao = 0;
        tempoPosicao = 0;

        if (idaEVolta == true)
        {
            GerarIdaEVolta();
        }
    }

    /// <summary>
    /// Altera a posicao de uma entidade no objeto, conforme a lista de direcoes e o tempo de cada direcao.
    /// </summary>
    public void AlterarPosicao()
    {
        tempoPosicao += Time.deltaTime;

        if (tempoPosicao > direcoes[indicePosicao].Tempo)
        {
            tempoPosicao -= direcoes[indicePosicao].Tempo;

            indicePosicao++;

            if (indicePosicao >= direcoes.Count)
            {
                indicePosicao = 0;
            }

            AtualizarPosicao();
        }
    }

    public void AtualizarPosicao()
    {
        npc.AtualizarDirecao(direcoes[indicePosicao].Direcao);
    }

    /// <summary>
    /// Preenche a lista de direcoes com as posicoes na ordem inversa, com excecao do ultimo e primeiro item da lista.
    /// </summary>
    public void GerarIdaEVolta()
    {
        int valor = direcoes.Count;
        for (int i = 1; i < valor - 1; i++)
        {
            direcoes.Add(direcoes[valor - i - 1]);
        }
    }

    [System.Serializable]
    private struct DirecaoParaOlhar
    {
        //Variaveis
        [SerializeField] private EntityModel.Direction direcao;
        [SerializeField] private float tempo;

        //Getters
        public EntityModel.Direction Direcao => direcao;
        public float Tempo => tempo;
    }
}

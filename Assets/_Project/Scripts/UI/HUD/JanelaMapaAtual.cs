using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JanelaMapaAtual : MonoBehaviour
{
    //Componentes
    [SerializeField] private TMP_Text nomeDoMapaAtual;

    private Animator animacao;

    private void Awake()
    {
        animacao = GetComponent<Animator>();
    }

    public void MostrarNomeDoMapa()
    {
        gameObject.SetActive(true);

        nomeDoMapaAtual.text = SceneSpawnManager.NomeDoMapaAtual;

        animacao.Play("MostrandoNomeDoMapaAtual");
    }

    public void EsconderJanela()
    {
        gameObject.SetActive(false);
    }
}

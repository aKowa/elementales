using BergamotaLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JanelaCombatLesson : MonoBehaviour
{
    //Componentes
    [SerializeField] private Image miniatura;
    [SerializeField] TMP_Text nomeCombatLesson;

    private Animacao animacao;

    private void Awake()
    {
        animacao = GetComponent<Animacao>();
    }

    private void Start()
    {
        animacao.TrocarAnimacao("Vazio");
    }

    public void MostrarCombatLesson(Sprite miniaturaElemon, string nomeCombatLesson, Action onAnimationEnd = null)
    {
        miniatura.sprite = miniaturaElemon;
        this.nomeCombatLesson.text = nomeCombatLesson;

        animacao.TrocarAnimacao("Mostrando", 0);
        animacao.ExecutarUmMetodoAposOFimDaAnimacao(onAnimationEnd);
    }
}

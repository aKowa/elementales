using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuTrainerController : ViewController
{
    //Componentes
    [Header("Componentes")]

    [SerializeField] private TMP_Text nomeTreinador;
    [SerializeField] private TMP_Text dinheiro;
    [SerializeField] private TMP_Text monstrosEncontrados;
    [SerializeField] private TMP_Text tempoDeJogo;
    [SerializeField] private Image imagemTreinador;
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;

    [Header("Variaveis Padroes")]
    [SerializeField] private Sprite spriteMasculino;
    [SerializeField] private Sprite spriteFeminino;

    protected override void OnAwake()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        onOpen += IniciarMenu;
    }

    public override void OnOpen()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(true);
    }

    protected override void OnClose()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        ResetarInformacoes();
    }

    private void IniciarMenu()
    {
        nomeTreinador.text = PlayerData.Instance.GetPlayerName;
        dinheiro.text = "$ " + PlayerData.Instance.Inventario.Dinheiro.ToString();
        monstrosEncontrados.text = PlayerData.MonsterBook.MonstrosCapturados().ToString();
        tempoDeJogo.text = TimeSpan.FromSeconds(PlayerData.TimePlayed).ToString(@"hh\:mm"); ;

        switch(PlayerData.GetPlayerSexo)
        {
            case PlayerSO.Sexo.Masculino:
                imagemTreinador.sprite = spriteMasculino;
                break;

            case PlayerSO.Sexo.Feminino:
                imagemTreinador.sprite = spriteFeminino;
                break;
        }
    }

    private void ResetarInformacoes()
    {
        nomeTreinador.text = string.Empty;
        dinheiro.text = string.Empty;
        monstrosEncontrados.text = string.Empty;
        tempoDeJogo.text = string.Empty;
    }
}

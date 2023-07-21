using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BotaoMonstro : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private Image imagem;

    [Header("Variaveis Padroes")]
    [SerializeField] private Color corSelecionado;
    [SerializeField] private Color corNaoSelecionado;

    private MenuSummaryController menuSummaryController;

    //Variaveis
    private int indice;

    //Getters
    public int Indice
    {
        get => indice;
        set => indice = value;
    }

    public MenuSummaryController MenuSummaryController
    {
        get => menuSummaryController;
        set => menuSummaryController = value;
    }

    private void Awake()
    {
        HoldButton holdButton = GetComponent<HoldButton>();

        holdButton.OnPointerDownEvent.AddListener(TrocarMonstro);
    }

    private void TrocarMonstro(PointerEventData eventData)
    {
        menuSummaryController.TrocarMonstro(indice);
    }

    public void Selecionado(bool selecionado)
    {
        if(selecionado == true)
        {
            imagem.color = corSelecionado;
        }
        else
        {
            imagem.color = corNaoSelecionado;
        }
    }
}

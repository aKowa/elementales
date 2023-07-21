using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BotaoGuia : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private Image imagem;

    [Header("Variaveis Padroes")]
    [SerializeField] private Color corSelecionado;

    private UnityEvent<int> eventoAbrirGuia = new UnityEvent<int>();

    //Variaveis
    private int indice;

    //Getters
    public UnityEvent<int> EventoAbrirGuia => eventoAbrirGuia;

    public int Indice
    {
        get => indice;
        set => indice = value;
    }

    private void Awake()
    {
        HoldButton holdButton = GetComponent<HoldButton>();

        holdButton.OnPointerDownEvent.AddListener(TrocarGuia);
    }

    private void TrocarGuia(PointerEventData eventData)
    {
        eventoAbrirGuia?.Invoke(indice);
    }

    public void Selecionado(bool selecionado)
    {
        if (selecionado == true)
        {
            imagem.color = corSelecionado;
        }
        else
        {
            imagem.color = Color.white;
        }
    }
}

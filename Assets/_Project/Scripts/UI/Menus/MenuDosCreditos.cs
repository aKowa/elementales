using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuDosCreditos : ViewController
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;

    protected override void OnAwake()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);
    }

    public override void OnOpen()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(true);
    }

    protected override void OnClose()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventarioController : ViewController
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

        BergamotaLibrary.PauseManager.Pausar(true);
    }

    protected override void OnClose()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        BergamotaLibrary.PauseManager.Pausar(false);
    }
}

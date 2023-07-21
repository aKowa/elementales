using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTutorialDeBatalhaController : ViewController
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private Transform telasHolder;
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;

    //Variaves
    private List<Transform> telas = new List<Transform>();

    private int indiceTelaAtual;

    protected override void OnAwake()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        indiceTelaAtual = 0;

        foreach(Transform tela in telasHolder)
        {
            telas.Add(tela);
        }
    }

    public override void OnOpen()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(true);
    }

    protected override void OnClose()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);
    }

    public void IniciarMenu()
    {
        OpenView();

        foreach(Transform tela in telas)
        {
            tela.gameObject.SetActive(false);
        }

        indiceTelaAtual = 0;

        telas[indiceTelaAtual].gameObject.SetActive(true);
    }

    public void AvancarTela()
    {
        indiceTelaAtual++;

        if(indiceTelaAtual < telas.Count)
        {
            telas[indiceTelaAtual - 1].gameObject.SetActive(false);
            telas[indiceTelaAtual].gameObject.SetActive(true);
        }
        else
        {
            CloseView();
        }
    }
}

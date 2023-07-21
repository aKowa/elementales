using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuDaLojaIAPController : ViewController
{
    //Constantes
    public const float modificadorItemParaVenda = 0.5f;

    //Componentes
    [Header("Componentes")]
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;
    [SerializeField] private LojaIAPInfo guiaDaLoja;

    //Variaveis
    private List<InventarioLojaIAP.ItemLojaIAP> itensDaLoja = new List<InventarioLojaIAP.ItemLojaIAP>();

    private InventarioLojaIAP inventarioLojaAtual;

    protected override void OnAwake()
    {
        //Componentes
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

        ResetarInformacoes();

        guiaDaLoja.ResetarItemSlots();

        BergamotaLibrary.PauseManager.Pausar(false);
    }

    public void IniciarMenu(InventarioLojaIAP inventarioLoja)
    {
        OpenView();

        inventarioLojaAtual = inventarioLoja;

        CriarItensDaLoja();

        AtualizarInformacoes();
    }

    private void ResetarInformacoes()
    {
        itensDaLoja.Clear();
    }

    private void CriarItensDaLoja()
    {
        itensDaLoja.Clear();

        for (int i = 0; i < inventarioLojaAtual.ItensDaLoja.Length; i++)
        {
            itensDaLoja.Add(inventarioLojaAtual.ItensDaLoja[i]);
        }
    }

    private void AtualizarInformacoes()
    {
        guiaDaLoja.AtualizarInformacoes(itensDaLoja);
    }
}

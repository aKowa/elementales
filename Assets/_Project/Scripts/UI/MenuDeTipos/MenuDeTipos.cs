using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuDeTipos : ViewController
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private GameObject relacaoDeTipoBase;
    [SerializeField] private RectTransform layout;

    [Space(10)]

    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;

    //Variaveis
    [Header("Variaveis")]
    [SerializeField] private MonsterType[] tipos;

    private List<RelacaoDeTipo> relacoesDeTipo = new List<RelacaoDeTipo>();

    private float spacing;

    protected override void OnAwake()
    {
        relacaoDeTipoBase.SetActive(false);

        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);

        spacing = layout.GetComponent<VerticalLayoutGroup>().spacing;

        CriarRelacoesDeTipos();
    }

    public override void OnOpen()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(true);
    }

    protected override void OnClose()
    {
        fundoBloqueadorDeAcoesDoMenu.gameObject.SetActive(false);
    }

    private void CriarRelacoesDeTipos()
    {
        ResetarInformacoes();

        float larguraMenu = 0;
        float alturaMenu = 0;

        viewContent.gameObject.SetActive(true);

        foreach(MonsterType tipo in tipos)
        {
            RelacaoDeTipo relacaoDeTipo = Instantiate(relacaoDeTipoBase, layout).GetComponent<RelacaoDeTipo>();
            relacaoDeTipo.gameObject.SetActive(true);

            relacaoDeTipo.CriarRelacaoDeTipo(tipo);

            float largura = relacaoDeTipo.GetComponent<RectTransform>().sizeDelta.x;

            if (largura > larguraMenu)
            {
                larguraMenu = largura;
            }

            relacoesDeTipo.Add(relacaoDeTipo);
        }

        alturaMenu = (relacaoDeTipoBase.GetComponent<RectTransform>().sizeDelta.y * (relacoesDeTipo.Count)) + (spacing * (relacoesDeTipo.Count - 1));

        layout.sizeDelta = new Vector2(larguraMenu, alturaMenu);

        viewContent.gameObject.SetActive(false);
    }

    private void ResetarInformacoes()
    {
        foreach(RelacaoDeTipo relacaoDeTipo in relacoesDeTipo)
        {
            Destroy(relacaoDeTipo.gameObject);
        }

        relacoesDeTipo.Clear();
    }
}

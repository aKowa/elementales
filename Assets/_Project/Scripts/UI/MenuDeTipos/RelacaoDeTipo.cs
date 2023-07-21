using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelacaoDeTipo : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private GameObject tipoLogoBase;

    [Space(10)]

    [SerializeField] private RectTransform tipoHolder;
    [SerializeField] private RectTransform tipoLayout;
    [SerializeField] private RectTransform vantagensHolder;
    [SerializeField] private RectTransform vantagensLayout;
    [SerializeField] private RectTransform desvantagensHolder;
    [SerializeField] private RectTransform desvantagensLayout;

    private RectTransform rectTransform;

    //Variaveis
    private float larguraBaseVantagensHolder;
    private float larguraBaseDesvantagensHolder;
    private float spacingVantagensLayout;
    private float spacingDesvantagensLayout;

    private TipoLogo tipoPrincipal;
    private List<TipoLogo> vantagens = new List<TipoLogo>();
    private List<TipoLogo> desvantagens = new List<TipoLogo>();

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        larguraBaseVantagensHolder = vantagensHolder.sizeDelta.x;
        larguraBaseDesvantagensHolder = desvantagensHolder.sizeDelta.x;

        spacingVantagensLayout = vantagensLayout.GetComponent<HorizontalLayoutGroup>().spacing;
        spacingDesvantagensLayout = desvantagensLayout.GetComponent<HorizontalLayoutGroup>().spacing;
    }

    public void CriarRelacaoDeTipo(MonsterType tipo)
    {
        ResetarInformacoes();

        tipoPrincipal = Instantiate(tipoLogoBase, tipoLayout).GetComponent<TipoLogo>();
        tipoPrincipal.gameObject.SetActive(true);

        tipoPrincipal.SetTipo(tipo);

        foreach(TypeRelation relacaoDeTipo in tipo.VantagemContra)
        {
            if(relacaoDeTipo.modifier > 1)
            {
                AdicionarVantagemContra(relacaoDeTipo.GetMonsterType);
            }
            else if(relacaoDeTipo.modifier < 1)
            {
                AdicionarDesvantagemContra(relacaoDeTipo.GetMonsterType);
            }
        }

        float larguraTipoLogo = tipoLogoBase.GetComponent<RectTransform>().sizeDelta.x;

        //Vantagens Holder
        float larguraExtraDosLogos = (larguraTipoLogo * (vantagens.Count - 1)) + (spacingVantagensLayout * (vantagens.Count - 1));

        if(larguraExtraDosLogos < 0)
        {
            larguraExtraDosLogos = 0;
        }

        vantagensHolder.sizeDelta = new Vector2(larguraBaseVantagensHolder + larguraExtraDosLogos, vantagensHolder.sizeDelta.y);

        //Desvantagens Holder
        larguraExtraDosLogos = (larguraTipoLogo * (desvantagens.Count - 1)) + (spacingDesvantagensLayout * (desvantagens.Count - 1));

        if (larguraExtraDosLogos < 0)
        {
            larguraExtraDosLogos = 0;
        }

        desvantagensHolder.sizeDelta = new Vector2(larguraBaseDesvantagensHolder + larguraExtraDosLogos, desvantagensHolder.sizeDelta.y);

        //Objeto Pai
        rectTransform.sizeDelta = new Vector2(tipoHolder.sizeDelta.x + vantagensHolder.sizeDelta.x + desvantagensHolder.sizeDelta.x, rectTransform.sizeDelta.y);
    }

    private void ResetarInformacoes()
    {
        if(tipoPrincipal != null)
        {
            Destroy(tipoPrincipal.gameObject);
        }

        foreach(TipoLogo tipo in vantagens)
        {
            Destroy(tipo.gameObject);
        }

        foreach (TipoLogo tipo in desvantagens)
        {
            Destroy(tipo.gameObject);
        }

        tipoPrincipal = null;
        vantagens.Clear();
        desvantagens.Clear();
    }

    private void AdicionarVantagemContra(MonsterType tipo)
    {
        TipoLogo tipoLogo = Instantiate(tipoLogoBase, vantagensLayout).GetComponent<TipoLogo>();
        tipoLogo.gameObject.SetActive(true);

        tipoLogo.SetTipo(tipo);

        vantagens.Add(tipoLogo);
    }

    private void AdicionarDesvantagemContra(MonsterType tipo)
    {
        TipoLogo tipoLogo = Instantiate(tipoLogoBase, desvantagensLayout).GetComponent<TipoLogo>();
        tipoLogo.gameObject.SetActive(true);

        tipoLogo.SetTipo(tipo);

        desvantagens.Add(tipoLogo);
    }
}

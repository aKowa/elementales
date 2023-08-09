using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class MenuDeInformacoesDeItemIAP : ViewController
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private RectTransform fundoBloqueadorDeAcoesDoMenu;

    [Space(10)]

    [SerializeField] private TMP_Text textoTitulo;
    [SerializeField] private TMP_Text textoDescricao;
    [SerializeField] private TMP_Text textoPreco;
    [SerializeField] private Image imagem;

    //Variaveis
    private IAPButton iapButtonAtual;

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

        ResetarInformacoes();
    }

    public void IniciarMenu(IAPButton iapButton, string titulo, string descricao, string preco, Sprite imagemProduto)
    {
        OpenView();

        iapButtonAtual = iapButton;

        iapButtonAtual.onPurchaseComplete.AddListener(FecharAposACompra);
        iapButtonAtual.onPurchaseFailed.AddListener(LiberarComandos);

        textoTitulo.text = titulo;
        textoDescricao.text = descricao;
        textoPreco.text = preco;
        imagem.sprite = imagemProduto;
    }

    private void ResetarInformacoes()
    {
        iapButtonAtual.onPurchaseComplete.RemoveListener(FecharAposACompra);
        iapButtonAtual.onPurchaseFailed.RemoveListener(LiberarComandos);

        iapButtonAtual = null;

        textoTitulo.text = string.Empty;
        textoDescricao.text = string.Empty;
        textoPreco.text = string.Empty;
        imagem.sprite = null;
    }

    public void ComprarProduto()
    {
        iapButtonAtual.GetComponent<Button>().onClick.Invoke();

        BloquearComandos();
    }

    private void BloquearComandos()
    {
        canvasGroup.blocksRaycasts = false;
    }

    private void LiberarComandos(Product product, PurchaseFailureReason purchaseFailureReason)
    {
        canvasGroup.blocksRaycasts = true;
    }

    private void FecharAposACompra(Product product)
    {
        LiberarComandos(null, default);

        CloseView();
    }
}

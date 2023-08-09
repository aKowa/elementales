using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;

[RequireComponent(typeof(IAPButton))]
public class IAPButtonDescriptionController : MonoBehaviour
{
    [SerializeField] private ItemSlotLojaIAP itemSlotLojaIAP;
 
    private IAPButton attachedButton;
 
    private void Awake()
    {
        attachedButton = GetComponent<IAPButton>();
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        Product product = CodelessIAPStoreListener.Instance.GetProduct(attachedButton.productId);

        string titulo = product.metadata.localizedTitle;
        string descricao = product.metadata.localizedDescription;
        string preco = RemoveNonNumeric(product.metadata.localizedPriceString);

        itemSlotLojaIAP.Iniciar(titulo, descricao, preco);
    }

    private string RemoveNonNumeric(string inputString)
    {
        // Use a regular expression to match any character that is not a digit, comma, or dot
        string pattern = @"[^\d,.]";
        // Replace all non-numeric characters with an empty string
        string result = Regex.Replace(inputString, pattern, "");
        return result;
    }
}
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;

[RequireComponent(typeof(IAPButton))]
public class IAPButtonDescriptionController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
 
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
        var product = CodelessIAPStoreListener.Instance.GetProduct(attachedButton.productId);
        if (priceText != null)
        {
            priceText.SetText(RemoveNonNumeric(product.metadata.localizedPriceString));
        }

        if (titleText != null)
            titleText.SetText(product.metadata.localizedTitle);
 
        if (descriptionText != null)
            descriptionText.SetText(product.metadata.localizedDescription);
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventarioNPC", menuName = "Inventario/Inventario de Loja IAP")]
public class InventarioLojaIAP : ScriptableObject
{
    //Variaveis
    [SerializeField] private ItemLojaIAP[] itensDaLoja;

    //Getters
    public ItemLojaIAP[] ItensDaLoja => itensDaLoja;

    [System.Serializable]
    public struct ItemLojaIAP
    {
        [SerializeField] private string productID;
        [SerializeField] private Sprite productImage;

        public string ProductID => productID;
        public Sprite ProductImage => productImage;
    }
}

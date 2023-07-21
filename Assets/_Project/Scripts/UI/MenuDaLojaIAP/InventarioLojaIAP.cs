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

        public string ProductID => productID;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventarioNPC", menuName = "Inventario/Inventario de Loja")]
public class InventarioLoja : ScriptableObject
{
    //Variaveis
    [SerializeField] private Item[] itensDaLoja;

    //Getters
    public Item[] ItensDaLoja => itensDaLoja;
}
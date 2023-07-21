using BergamotaLibrary;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventario com Peso", menuName = "Inventario/Inventario com Peso")]
public class InventarioComPeso : ScriptableObject
{
    public string displayName;
    [SerializeField] private WeightedRandomList<Item> itensDaLoja;

    public WeightedRandomList<Item> ItensDaLoja => itensDaLoja;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventarioNPC", menuName = "Inventario/Inventario de NPC")]
public class InventarioNPC : ScriptableObject
{
    //Variaveis
    [SerializeField] private List<Monster> monsterBag;
    [SerializeField] private List<ItemHolder> itens;

    //Getters
    public List<Monster> MonsterBag => monsterBag;
    public List<ItemHolder> Itens => itens;
}

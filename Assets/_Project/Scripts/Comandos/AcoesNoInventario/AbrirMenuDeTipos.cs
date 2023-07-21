using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Inventario/Abrir Menu de Tipos")]
public class AbrirMenuDeTipos : AcaoNoInventario
{
    public override void UsarItem(MenuBagController menuBagController, Item item)
    {
        menuBagController.AbrirMenuDeTipos();
    }
}

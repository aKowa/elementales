using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Inventario/Status/Xp extra")]

public class AumentarXpExtraMapa : AcaoNoInventario
{
    public override void UsarItem(MenuBagController menuBagController, Item item)
    {
        menuBagController.AbrirEscolhaDeMonstros();
    }

    public override bool PodeUsarItemNoMonstro(Monster monstro)
    {
        return !BattleManager.Instance.XpExtraComerFruta;
    }

    public override void UsarItemNoMonstro(MenuBagController menuBagController, Monster monstro, Item item)
    {
        BattleManager.Instance.XpExtraComerFruta = true;
        if (item.Tipo == Item.TipoItem.Consumivel)
        {
            menuBagController.RemoveItem(item);
        }
    }
}

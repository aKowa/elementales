using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Inventario/Status/DragonFruit")]

public class AcaoDragonFruitMapa : AcaoNoInventario
{
    //Variaveis
    [SerializeField] private StatusEffectParaAplicar statusMana;
    [SerializeField] private StatusEffectParaAplicar statusHealth;

    public override void UsarItem(MenuBagController menuBagController, Item item)
    {
        menuBagController.AbrirEscolhaDeMonstros();
    }

    public override bool PodeUsarItemNoMonstro(Monster monstro)
    {
        if (monstro.IsFainted)
            return false;
        return true;
    }

    public override void UsarItemNoMonstro(MenuBagController menuBagController, Monster monstro, Item item)
    {
        monstro.AplicarStatus(statusMana.GetStatus);
        monstro.AplicarStatus(statusHealth.GetStatus);


        if (item.Tipo == Item.TipoItem.Consumivel)
        {
            menuBagController.RemoveItem(item);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Inventario/Status/Recuperar Mana")]
public class RecuperarMana_Mapa : AcaoNoInventario
{
    //Variaveis
    [SerializeField] private int quantidaDeMana;

    public override void UsarItem(MenuBagController menuBagController, Item item)
    {
        menuBagController.AbrirEscolhaDeMonstros();
    }

    public override bool PodeUsarItemNoMonstro(Monster monstro)
    {
        return (!monstro.IsFainted) && (monstro.AtributosAtuais.Mana < monstro.AtributosAtuais.ManaMax);
    }

    public override void UsarItemNoMonstro(MenuBagController menuBagController, Monster monstro, Item item)
    {
        monstro.RecuperarMana(quantidaDeMana);

        if (item.Tipo == Item.TipoItem.Consumivel)
        {
            menuBagController.RemoveItem(item);
        }
    }

    public override void EfeitoMonstroSlot(MenuBagController menuBagController)
    {
        menuBagController.AumentarBarraDeManaMonstroSlot();
    }
}

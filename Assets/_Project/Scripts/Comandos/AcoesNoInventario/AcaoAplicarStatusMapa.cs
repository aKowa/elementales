using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Inventario/Status/Aplicar Status")]

public class AcaoAplicarStatusMapa : AcaoNoInventario
{
    //Variaveis
    [SerializeField] private List<StatusEffectParaAplicar> status;

    public override void UsarItem(MenuBagController menuBagController, Item item)
    {
        menuBagController.AbrirEscolhaDeMonstros();
    }

    public override bool PodeUsarItemNoMonstro(Monster monstro)
    {
        if (monstro.IsFainted)
            return false;

        foreach (var statusMonstro in monstro.Status)
        {
            foreach (var statusItem in status)
            {
                if(statusMonstro.Nome == statusItem.GetStatus.Nome)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public override void UsarItemNoMonstro(MenuBagController menuBagController, Monster monstro, Item item)
    {
        foreach (StatusEffectParaAplicar status in status)
        {
            monstro.AplicarStatus(status.GetStatus);
        }

        if (item.Tipo == Item.TipoItem.Consumivel)
        {
            menuBagController.RemoveItem(item);
        }
    }
}

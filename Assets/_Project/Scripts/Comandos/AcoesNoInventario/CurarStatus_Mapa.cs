using System;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Inventario/Status/Curar Status")]
public class CurarStatus_Mapa : AcaoNoInventario
{
    [SerializeField] private StatusEffectBase[] statusParaCurar;
    
    public override void UsarItem(MenuBagController menuBagController, Item item)
    {
        menuBagController.AbrirEscolhaDeMonstros();
    }

    public override bool PodeUsarItemNoMonstro(Monster monstro)
    {
        if (monstro.IsFainted)
            return false;

        return statusParaCurar.ForEach(status => monstro.Status
            .Where(statusDoMonstro => status.name == statusDoMonstro.name)).Any();
    }

    public override void UsarItemNoMonstro(MenuBagController menuBagController, Monster monstro, Item item)
    {
        statusParaCurar.ForEach(monstro.RemoverStatusPorTipo);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Inventario/Status/Modificador")]

public class AumentarModificador_Mapa : AcaoNoInventario
{
    [SerializeField] List<ModificadorExtra> modificadores = new List<ModificadorExtra>();
    public override void UsarItem(MenuBagController menuBagController, Item item)
    {
        menuBagController.AbrirEscolhaDeMonstros();
    }

    public override bool PodeUsarItemNoMonstro(Monster monstro)
    {
        return (!monstro.IsFainted);
    }

    public override void UsarItemNoMonstro(MenuBagController menuBagController, Monster monstro, Item item)
    {
        foreach (var modificador in modificadores)
        {
            monstro.AtributosAtuais.ReceberModificadorStatus(modificador.atributo, modificador.modificador);
            monstro.AtributosAtuais.TocarSomModificador(modificador.modificador.ValorModificador, monstro.MonsterData);

        }
        if (item.Tipo == Item.TipoItem.Consumivel)
        {
            menuBagController.RemoveItem(item);
        }
    }
}
[System.Serializable]
public class ModificadorExtra
{
    public Modificador.Atributo atributo;
    public ModificadorTurno modificador;
}
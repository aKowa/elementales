using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Inventario/Status/Modificador Aleatorio")]

public class AumentarModificarAleatorio_Mapa : AcaoNoInventario
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
            int indiceNovoAtributo = Random.Range(0, (int)Modificador.Atributo.velocidade);
            modificador.atributo = (Modificador.Atributo)indiceNovoAtributo;
            monstro.AtributosAtuais.ReceberModificadorStatus(modificador.atributo, modificador.modificador);
        }
        if (item.Tipo == Item.TipoItem.Consumivel)
        {
            menuBagController.RemoveItem(item);
        }
    }
}

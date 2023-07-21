using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Inventario/Status/Curar HP")]
public class CurarHP_Mapa : AcaoNoInventario
{
    //Variaveis
    [SerializeField] private int quantidaDeCura;

    public override void UsarItem(MenuBagController menuBagController, Item item)
    {
        menuBagController.AbrirEscolhaDeMonstros();
    }

    public override bool PodeUsarItemNoMonstro(Monster monstro)
    {
        return (!monstro.IsFainted) && (monstro.AtributosAtuais.Vida < monstro.AtributosAtuais.VidaMax);
    }

    public override void UsarItemNoMonstro(MenuBagController menuBagController, Monster monstro, Item item)
    {
        monstro.ReceberCura(quantidaDeCura);

        if(item.Tipo == Item.TipoItem.Consumivel)
        {
            menuBagController.RemoveItem(item);
        }
    }

    public override void EfeitoMonstroSlot(MenuBagController menuBagController)
    {
        menuBagController.AumentarBarraDeHPMonstroSlot();
    }
}
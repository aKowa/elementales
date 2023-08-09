using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Inventario/Status/Reviver")]
public class ReviverMonstro_Mapa : AcaoNoInventario
{
    //Variaveis
    [SerializeField][Range(0, 100)] private int porcentagemDeCura;
    [SerializeField] private AudioClip somRecuperarVida;

    public override void UsarItem(MenuBagController menuBagController, Item item)
    {
        menuBagController.AbrirEscolhaDeMonstros();
    }

    public override bool PodeUsarItemNoMonstro(Monster monstro)
    {
        return monstro.IsFainted;
    }

    public override void UsarItemNoMonstro(MenuBagController menuBagController, Monster monstro, Item item)
    {
        int quantidadeDeCura = (int)(((float)porcentagemDeCura / 100) * monstro.AtributosAtuais.VidaMax);

        monstro.ReceberCura(quantidadeDeCura);

        if (item.Tipo == Item.TipoItem.Consumivel)
        {
            menuBagController.RemoveItem(item);
        }
        SoundManager.instance.TocarSomIgnorandoPause(somRecuperarVida);

    }

    public override void EfeitoMonstroSlot(MenuBagController menuBagController)
    {
        menuBagController.AumentarBarraDeHPMonstroSlot();
    }
}

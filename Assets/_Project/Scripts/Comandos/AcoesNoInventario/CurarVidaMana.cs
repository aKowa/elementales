using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Acoes/Inventario/Status/Curar HP-Mana")]

public class CurarVidaMana : AcaoNoInventario
{
    //Variaveis
    [SerializeField] private float porcentagemCura;
    [SerializeField] private float porcentagemMana;
    [SerializeField] private AudioClip somRecuperarVida;
    [SerializeField] private AudioClip somRecuperarMana;

    public override void UsarItem(MenuBagController menuBagController, Item item)
    {
        menuBagController.AbrirEscolhaDeMonstros();
    }

    public override bool PodeUsarItemNoMonstro(Monster monstro)
    {
        if (monstro.IsFainted)
            return false;
        if(porcentagemCura > 0)
        {
            if (monstro.AtributosAtuais.Vida >= monstro.AtributosAtuais.VidaMax)
                return false;
        }
        if (porcentagemMana > 0)
        {
            if (monstro.AtributosAtuais.Mana >= monstro.AtributosAtuais.ManaMax)
                return false;
        }
        return true;
    }

    public override void UsarItemNoMonstro(MenuBagController menuBagController, Monster monstro, Item item)
    {
        monstro.ReceberCura((int)(monstro.AtributosAtuais.VidaMax * (porcentagemCura / 100)));
        monstro.RecuperarManaPorcentagem(porcentagemMana);

        if (item.Tipo == Item.TipoItem.Consumivel)
        {
            menuBagController.RemoveItem(item);
        }
        SoundManager.instance.TocarSomIgnorandoPause(somRecuperarVida);
        SoundManager.instance.TocarSomIgnorandoPause(somRecuperarMana);

    }

    public override void EfeitoMonstroSlot(MenuBagController menuBagController)
    {
        menuBagController.AumentarBarraDeHPMonstroSlot();
        menuBagController.AumentarBarraDeManaMonstroSlot();
    }
}

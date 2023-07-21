using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AcaoNoInventario : ScriptableObject
{
    public abstract void UsarItem(MenuBagController menuBagController, Item item);

    public virtual bool PodeUsarItemNoMonstro(Monster monstro)
    {
        return false;
    }

    public virtual void UsarItemNoMonstro(MenuBagController menuBagController, Monster monstro, Item item)
    {
        Debug.LogWarning("Este metodo nao foi implementado!");
    }

    public virtual void EfeitoMonstroSlot(MenuBagController menuBagController)
    {
        menuBagController.AtualizarInformacoesMonstros();
    }
}

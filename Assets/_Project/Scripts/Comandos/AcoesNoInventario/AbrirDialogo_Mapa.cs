using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Inventario/Abrir Dialogo")]
public class AbrirDialogo_Mapa : AcaoNoInventario
{
    //Variaveis
    [SerializeField] private BergamotaDialogueSystem.DialogueObject dialogo;

    public override void UsarItem(MenuBagController menuBagController, Item item)
    {
        menuBagController.AbrirDialogo(dialogo);
    }
}

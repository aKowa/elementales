using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Inventario/Usar Repelente")]
public class UsarRepelente : AcaoNoInventario
{
    //Variaveis
    [SerializeField] private int quantidadeDePassosDoRepelente;
    [SerializeField] private BergamotaDialogueSystem.DialogueObject dialogoUsouRepelente;
    [SerializeField] private BergamotaDialogueSystem.DialogueObject dialogoJaEstaUsandoRepelente;

    //Getters
    public int QuantidadeDePassosDoRepelente => quantidadeDePassosDoRepelente;

    public override void UsarItem(MenuBagController menuBagController, Item item)
    {
        if(PlayerData.Repelente <= 0)
        {
            PlayerData.Repelente = quantidadeDePassosDoRepelente;

            menuBagController.AbrirDialogo(dialogoUsouRepelente);

            if (item.Tipo == Item.TipoItem.Consumivel)
            {
                menuBagController.RemoveItem(item);
            }
        }
        else
        {
            menuBagController.AbrirDialogo(dialogoJaEstaUsandoRepelente);
        }
    }
}

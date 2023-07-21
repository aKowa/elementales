using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Inventario/Usar Monster Ball na Batalha")]
public class UsarMonsterBallNaBatalha : AcaoNoInventario
{
    //Variaveis
    [SerializeField] private BergamotaDialogueSystem.DialogueObject dialogoNaoPodeUsar;

    public override void UsarItem(MenuBagController menuBagController, Item item)
    {
        MenuBagBatalhaController menuBagBatalhaController = (MenuBagBatalhaController)menuBagController;

        if(BattleManager.Instance.GetTipoBatalha == BattleManager.TipoBatalha.MonstroSelvagem && BattleManager.Instance.QuantidadeDeMonstrosPorTime == 1)
        {
            menuBagBatalhaController.UsarItemNaBatalha();
        }
        else
        {
            menuBagController.AbrirDialogo(dialogoNaoPodeUsar);
        }
    }
}

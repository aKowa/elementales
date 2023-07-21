using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Inventario/Combat Lesson")]
public class EnsinarCombatLesson : AcaoNoInventario
{
    //Variaveis
    [Header("Variaveis")]
    [SerializeField] private CombatLesson combatLesson;

    [Header("Dialogos")]
    [SerializeField] private BergamotaDialogueSystem.DialogueObject dialogoAprendeuCombatLesson;
    [SerializeField] private BergamotaDialogueSystem.DialogueObject dialogoJaAprendeuCombatLesson;

    public override void UsarItem(MenuBagController menuBagController, Item item)
    {
        menuBagController.AbrirEscolhaDeMonstros();
    }

    public override bool PodeUsarItemNoMonstro(Monster monstro)
    {
        return true;
    }

    public override void UsarItemNoMonstro(MenuBagController menuBagController, Monster monstro, Item item)
    {
        menuBagController.DialogueUI.SetPlaceholderDeTexto("%monstro", monstro.NickName);
        menuBagController.DialogueUI.SetPlaceholderDeTexto("%move", combatLesson.Nome);

        if(monstro.VerificarSePossuiCombatLesson(combatLesson) == false)
        {
            monstro.AddCombatLesson(combatLesson);

            menuBagController.AbrirDialogo(dialogoAprendeuCombatLesson);

            if (item.Tipo == Item.TipoItem.Consumivel || item.Tipo == Item.TipoItem.Habilidade)
            {
                menuBagController.RemoveItem(item);
            }
        }
        else
        {
            menuBagController.AbrirDialogo(dialogoJaAprendeuCombatLesson);
        }
    }
}

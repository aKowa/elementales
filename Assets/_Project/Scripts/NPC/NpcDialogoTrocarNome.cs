using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcDialogoTrocarNome : NPCDialogue
{
    public override void Interagir(Player player)
    {
        npc.VirarNaDirecao(player.transform.position);
        MostrarDialogo(conditionalDialoguesCaller.GetDialogue(), player);
    }
}

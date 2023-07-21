using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBatalhaDialogue : NPCDialogue
{
    //Componentes
    private NPCBatalha npcBatalha;

    protected override void Awake()
    {
        base.Awake();

        npcBatalha = GetComponentInParent<NPCBatalha>();
    }

    public override void Interagir(Player player)
    {
        if(npcBatalha.JaFezABatalha() == true)
        {
            npc.VirarNaDirecao(player.transform.position);

            switch (tipoDialogo)
            {
                case TipoDialogo.Unico:
                    MostrarDialogo(dialogo, player);
                    break;

                case TipoDialogo.Condicional:
                    MostrarDialogo(conditionalDialoguesCaller.GetDialogue(), player);
                    break;
            }
        }
        else
        {
            npcBatalha.IniciarEventoDaBatalha(player);
        }
    }
}

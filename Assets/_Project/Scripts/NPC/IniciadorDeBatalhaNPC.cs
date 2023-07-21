using BergamotaDialogueSystem;
using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IniciadorDeBatalhaNPC : MonoBehaviour
{
    //Componentes
    private NPCBatalha npcBatalha;

    //Variaveis
    [SerializeField] private LayerMask layerQueBloqueiaVisao;

    private void Awake()
    {
        npcBatalha = GetComponentInParent<NPCBatalha>();
    }

    public void Rotacionar(Vector2 direcao)
    {
        Quaternion paraRotacionar = Quaternion.LookRotation(Vector3.forward, direcao);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, paraRotacionar, 360);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (PauseManager.JogoPausado == true)
        {
            return;
        }

        if(PauseManager.PermitirInput == false || PauseManager.PermitirInputGeral == false)
        {
            return;
        }

        if (npcBatalha.JaFezABatalha() == false && NPCManager.IniciandoBatalha == false && DialogueUI.Instance.IsOpen == false)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                RaycastHit2D hit = Physics2D.Raycast(npcBatalha.transform.position, collision.transform.position - npcBatalha.transform.position, Vector2.Distance(npcBatalha.transform.position, collision.transform.position), layerQueBloqueiaVisao);

                if (hit == false)
                {
                    Player player = collision.gameObject.GetComponent<Player>();

                    if(player.GetEstadoPlayer == Player.EstadoPlayer.Normal)
                    {
                        npcBatalha.IniciarEventoDaBatalha(player);
                    }
                }
            }
        }
    }
}

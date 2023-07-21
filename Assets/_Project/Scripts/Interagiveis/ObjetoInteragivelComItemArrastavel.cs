using BergamotaDialogueSystem;
using BergamotaLibrary;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetoInteragivelComItemArrastavel : ObjetoInteragivelComItem
{
    [Header("Objeto Arrastavel")]
    [SerializeField] protected DialogueObject dialogoNaoPodeMover;

    [Space(10)]

    [SerializeField] protected LayerMask layerDasParedes;

    Vector3 pos = Vector3.zero;

    public override void InteragirComItem(Player player, bool possuiItem)
    {
        if (!possuiItem)
        {
            dialogueActivator.ShowDialogue(dialogoNaoPossuiItem, player.DialogueUI);
            return;
        }
        else
        {
            Vector2 vetorDirecao = EntityModel.GetDirectionVector(player.GetDirection);
            Vector2 novaPosicao = transform.position + Vector3.Scale((Vector3)boxCollider2D.offset, transform.lossyScale) + (Vector3)vetorDirecao;

            if (VerificarSePodeSeMover(novaPosicao))
            {
                PauseManager.PermitirInput = false;

                Sequence sequencia = DOTween.Sequence();

                sequencia.Append(transform.DOMove(novaPosicao - Vector2.Scale((Vector3)boxCollider2D.offset, transform.lossyScale), 0.4f));
                sequencia.AppendCallback(() => PauseManager.PermitirInput = true);

                if (rodarAnimacao == true)
                {
                    animator.SetFloat("Direcao", (float)player.GetDirection);
                    animator.Play(nomeAnimacao);
                }
            }
            else
            {
                dialogueActivator.ShowDialogue(dialogoNaoPodeMover, player.DialogueUI);
            }
        }
    }

    protected bool VerificarSePodeSeMover(Vector2 novaPosicao)
    {
        pos = novaPosicao;
        return !Physics2D.OverlapBox(novaPosicao, Vector3.Scale(boxCollider2D.size, transform.localScale), 0, layerDasParedes);
    }

    private void OnDrawGizmos()
    {
        if(Application.isPlaying)
        {
            Gizmos.DrawWireCube(pos, Vector3.Scale(boxCollider2D.size, transform.localScale));
        }
    }
}

using BergamotaLibrary;
using UnityEngine;

public class NPCAnimation : MonoBehaviour
{
    //Components
    [SerializeField] private Animacao animacaoSombra;
    
    private NPC npc;
    private Animacao animacao;

    private void Awake()
    {
        npc = GetComponentInParent<NPC>();
        animacao = GetComponent<Animacao>();
    }

    public void Animate()
    {
        if ((npc.NPCMovement.VelVector.x == 0 && npc.NPCMovement.VelVector.y == 0) || !(LiBergamota.VetorDiferente(npc.NPCMovement.LastPos, npc.transform.position)))
        {
            ChangeAnimation("Idle");
        }
        else if ((npc.NPCMovement.VelVector.x != 0 || npc.NPCMovement.VelVector.y != 0) && LiBergamota.VetorDiferente(npc.NPCMovement.LastPos, npc.transform.position))
        {
            ChangeAnimation("Walk");
        }
    }

    private void ChangeAnimation(string animation)
    {
        if (animacao.AnimatorEstaRodandoAAnimacao(animation) == false)
        {
            animacao.TrocarAnimacao(animation);
        }

        if (animacaoSombra.AnimatorEstaRodandoAAnimacao(animation) == false)
        {
            animacaoSombra.TrocarAnimacao(animation);
        }
    }

    public void SetDirection(EntityModel.Direction direction)
    {
        animacao.SetFloat("Direction", (float)direction);
        animacaoSombra.SetFloat("Direction", (float)direction);
    }

    public void SetWalkSpeed(float speed)
    {
        animacao.SetFloat("WalkSpeed", speed);
        animacaoSombra.SetFloat("WalkSpeed", speed);
    }
}

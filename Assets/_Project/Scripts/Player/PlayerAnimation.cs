using BergamotaLibrary;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    //Components
    [SerializeField] private Animacao animacaoSombra;
    [SerializeField] private Animacao animacaoVaraDePescar;

    private Player player;
    private Animacao animacao;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        animacao = GetComponent<Animacao>();
    }

    public void Animate()
    {
        switch(player.GetEstadoPlayer)
        {
            case Player.EstadoPlayer.Normal:

                AnimacaoNormal();

                break;

            case Player.EstadoPlayer.Nadando:

                AnimacaoNadando();

                break;
        }

        if(player.GetEstadoPlayer == Player.EstadoPlayer.Pescando)
        {
            if (animacaoVaraDePescar.AnimatorEstaRodandoAAnimacao("Idle") == false)
            {
                animacaoVaraDePescar.TrocarAnimacao("Idle");
            }
        }
        else
        {
            if (animacaoVaraDePescar.AnimatorEstaRodandoAAnimacao("Vazio") == false)
            {
                animacaoVaraDePescar.TrocarAnimacao("Vazio");
            }
        }
    }

    private void AnimacaoNormal()
    {
        if ((player.PlayerMovement.Rb.velocity.x == 0 && player.PlayerMovement.Rb.velocity.y == 0) || !(LiBergamota.VetorDiferente(player.PlayerMovement.LastPos, player.transform.position)))
        {
            ChangeAnimation("Idle");
        }
        else if ((player.PlayerMovement.Rb.velocity.x != 0 || player.PlayerMovement.Rb.velocity.y != 0) && LiBergamota.VetorDiferente(player.PlayerMovement.LastPos, player.transform.position))
        {
            ChangeAnimation("Walk");
        }
    }

    private void AnimacaoNadando()
    {
        if ((player.PlayerMovement.Rb.velocity.x == 0 && player.PlayerMovement.Rb.velocity.y == 0) || !(LiBergamota.VetorDiferente(player.PlayerMovement.LastPos, player.transform.position)))
        {
            ChangeAnimation("Idle");
        }
        else if ((player.PlayerMovement.Rb.velocity.x != 0 || player.PlayerMovement.Rb.velocity.y != 0) && LiBergamota.VetorDiferente(player.PlayerMovement.LastPos, player.transform.position))
        {
            ChangeAnimation("Walk");
        }
    }

    public void ChangeAnimation(string animation)
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
        animacaoVaraDePescar.SetFloat("Direction", (float)direction);
    }

    public void SetWalkSpeed(float speed)
    {
        animacao.SetFloat("WalkSpeed", speed);
        animacaoSombra.SetFloat("WalkSpeed", speed);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BordaAgua : MonoBehaviour
{
    [SerializeField] private AguaNadavel aguaNadavel;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();

            if (player.GetEstadoPlayer == Player.EstadoPlayer.Nadando)
            {
                aguaNadavel.AnimacaoPararDeNadar();
            }
        }
    }
}

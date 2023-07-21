using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusEffect : MonoBehaviour
{
    Vector2 posicaoPlayer;
    Inventario inventario;
    public void Instantiate(Inventario inventarioPlayer)
    {
        inventario = inventarioPlayer;
    }
    public bool Main()
    {
        bool trocouDeTile = false;

        Vector2 posicaoTemp = AtualizarPosicaoPlayer(transform.position);

        if (posicaoTemp != posicaoPlayer)
        {
            posicaoPlayer = posicaoTemp;
            VerificarStatus();

            trocouDeTile = true;
        }

        posicaoPlayer = AtualizarPosicaoPlayer(transform.position);

        return trocouDeTile;
    }
    void VerificarStatus()
    {
        for (int i = 0; i < inventario.MonsterBag.Count; i++)
        {
            List<StatusEffectBase> indices = new List<StatusEffectBase>();

            for (int j = 0; j < inventario.MonsterBag[i].Status.Count; j++)
            {
                if(inventario.MonsterBag[i].Status[j].ExecutarOutBattle(inventario.MonsterBag[i]))
                    indices.Add(inventario.MonsterBag[i].Status[j]);
            }

            for (int n = 0; n < indices.Count; n++)
            {
                inventario.MonsterBag[i].Status.Remove(indices[n]);
            }
        }
        
    }
    Vector2 AtualizarPosicaoPlayer(Vector2 posicao)
    {
        return new Vector2((int)posicao.x, (int)posicao.y);
    }
}

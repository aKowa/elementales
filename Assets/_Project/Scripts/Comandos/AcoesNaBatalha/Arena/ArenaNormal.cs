using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Acoes/Arena/Arena Normal")]
public class ArenaNormal : AcaoNaBatalha
{
    public override void Executar(BattleManager battleManager, Comando comando)
    {
        // Debug.Log("Resetei todos os modificadores de arenas"); 

        comando.PodeMeRetirar = true;
    }
}

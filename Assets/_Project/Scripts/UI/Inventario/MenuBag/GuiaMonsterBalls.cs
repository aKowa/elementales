using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiaMonsterBalls : InventarioInfo
{
    public override void AtualizarInformacoes(Inventario inventario)
    {
        AtualizarItens(inventario.MonsterBalls);
    }
}

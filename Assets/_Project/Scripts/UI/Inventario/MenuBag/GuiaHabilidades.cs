using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiaHabilidades : InventarioInfo
{
    public override void AtualizarInformacoes(Inventario inventario)
    {
        AtualizarItens(inventario.Habilidades);
    }
}

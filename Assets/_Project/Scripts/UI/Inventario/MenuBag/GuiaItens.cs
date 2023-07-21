using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiaItens : InventarioInfo
{
    public override void AtualizarInformacoes(Inventario inventario)
    {
        AtualizarItens(inventario.Itens);
    }
}

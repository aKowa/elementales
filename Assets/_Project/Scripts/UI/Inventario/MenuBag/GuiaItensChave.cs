using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiaItensChave : InventarioInfo
{
    public override void AtualizarInformacoes(Inventario inventario)
    {
        AtualizarItens(inventario.ItensChave);
    }
}

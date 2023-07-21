using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Loja : MonoBehaviour
{
    //Componentes
    private MenuDaLojaController menuDaLojaController;

    //Variaveis
    [SerializeField] private InventarioLoja inventarioLoja;

    private void Awake()
    {
        menuDaLojaController = FindObjectOfType<MenuDaLojaController>();
    }

    public void AbrirLojaCompra()
    {
        menuDaLojaController.IniciarMenu(inventarioLoja, MenuDaLojaController.TipoLoja.Compra);
    }

    public void AbrirLojaVenda()
    {
        menuDaLojaController.IniciarMenu(inventarioLoja, MenuDaLojaController.TipoLoja.Venda);
    }
}

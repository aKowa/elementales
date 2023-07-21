using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_LojaIAP : MonoBehaviour
{
    //Componentes
    private MenuDaLojaIAPController menuDaLojaIAPController;

    //Variaveis
    [SerializeField] private InventarioLojaIAP inventarioLoja;

    private void Awake()
    {
        menuDaLojaIAPController = FindObjectOfType<MenuDaLojaIAPController>();
    }

    public void AbrirLoja()
    {
        menuDaLojaIAPController.IniciarMenu(inventarioLoja);
    }
}

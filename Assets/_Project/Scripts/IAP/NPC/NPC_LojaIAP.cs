using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_LojaIAP : MonoBehaviour
{
    //Componentes
    private MenuDaLojaIAPController menuDaLojaIAPController;

    private void Awake()
    {
        menuDaLojaIAPController = FindObjectOfType<MenuDaLojaIAPController>();
    }

    public void AbrirLoja()
    {
        menuDaLojaIAPController.IniciarMenu();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesteInventario : MonoBehaviour
{
    //Componentes
    private Inventario inventario;

    //Variaveis
    [SerializeField] private Item[] itens;
    [SerializeField] private int quantidade;

    private void Start()
    {
        inventario = PlayerData.Instance.Inventario;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            foreach(Item item in itens)
            {
                inventario.AddItem(item, quantidade);
            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            inventario.RemoverItem(itens[0], quantidade);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesteMonstroStatusForaCombate : MonoBehaviour
{
    public Inventario inventario;
    public int indiceMonstro;
    public StatusEffectBase statusPassar;


    public Item item;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            inventario.MonsterBag[indiceMonstro].AplicarStatus(statusPassar);

            
        
    }
}

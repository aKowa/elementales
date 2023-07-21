using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Battle/Comando de Arena")]

public class ComandoArena : Comando
{
    //Variaveis
    private int quantidadeTurno;
    private bool turnosInfinitos;

    //Getters
    public int QuantidadeTurno
    {
        get => quantidadeTurno;
        set => quantidadeTurno = value;
    }

    public bool TurnosInfinitos
    {
        get => turnosInfinitos;
        set => turnosInfinitos = value;
    }

    public void ReceberVariaveis(int numTurno)
    {
        quantidadeTurno = numTurno;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackData
{
    //Enum
    public enum CategoriaEnum { Fisico, Especial, Status }

    //Variaveis
    [SerializeField] private CategoriaEnum categoria;
    [SerializeField] MonsterType _tipoAtaque;
    [SerializeField] private int _poder;
    [SerializeField] private float _chanceAcerto;

    //Getters
    public CategoriaEnum Categoria => categoria;
    public MonsterType TipoAtaque
    {
        get => _tipoAtaque;
        set => _tipoAtaque = value;
    }
    public int Poder
    {
        get => _poder;
        set => _poder = value;
    }

    public float ChanceAcerto => _chanceAcerto;
    
}

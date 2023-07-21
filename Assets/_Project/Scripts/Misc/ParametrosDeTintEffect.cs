using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Efeitos/Parametros de Tint Effect")]
public class ParametrosDeTintEffect : ScriptableObject
{
    //Variaveis
    [SerializeField] private Color corDoEfeito;
    [SerializeField] private float velocidadeDoEfeito;

    //Getters
    public Color CorDoEfeito => corDoEfeito;
    public float VelocidadeDoEfeito => velocidadeDoEfeito;
}
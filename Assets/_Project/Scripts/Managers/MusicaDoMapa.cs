using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Map Info/Musica do Mapa")]
public class MusicaDoMapa : ScriptableObject
{
    //Variaveis
    [SerializeField] private AudioClip musica;

    //Getters
    public AudioClip Musica => musica;
}

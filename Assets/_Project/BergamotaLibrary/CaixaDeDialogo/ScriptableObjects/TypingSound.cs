using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BergamotaDialogueSystem
{
    [CreateAssetMenu(menuName = "Dialogue/Typing Sound")]

    public class TypingSound : ScriptableObject
    {
        //Variaveis
        [SerializeField]
        [Tooltip("Deixe verdadeiro se voce quiser que a caixa de dialogo espere o typing sound terminar de tocar antes de tocar outro.")]
        private bool tocarUmSomPorVez;

        [SerializeField] private AudioClip[] som;

        //Getters
        public bool TocarUmSomPorVez => tocarUmSomPorVez;
        public AudioClip Som => som[Random.Range(0, som.Length)];
    }
}

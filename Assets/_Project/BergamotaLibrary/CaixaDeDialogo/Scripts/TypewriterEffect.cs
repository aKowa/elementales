using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BergamotaDialogueSystem
{
    public class TypewriterEffect : MonoBehaviour
    {
        [SerializeField] private float typewriterSpeed = 50f;

        public bool IsRunning { get; private set; }

        //Guarda caracteres especificos para os quais o jogo tem que esperar um intervalo maior para continuar escrevendo depois de escrever eles na tela
        private readonly List<Punctuation> punctuations = new List<Punctuation>()
        {
            new Punctuation(new HashSet<char>() {'.', '!', '?'}, 0.6f),
            new Punctuation(new HashSet<char>() {',', ';', ':'}, 0.3f)
        };

        private Coroutine typingCoroutine;

        public void Run(string textTotype, TMP_Text textLabel, TypingSound typingSound, DialogueUI dialogueUI)
        {
            typingCoroutine = StartCoroutine(TypeText(textTotype, textLabel, typingSound, dialogueUI));
        }

        public void Stop()
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            IsRunning = false;
        }

        private IEnumerator TypeText(string textTotype, TMP_Text textLabel, TypingSound typingSound, DialogueUI dialogueUI)
        {
            IsRunning = true;

            textLabel.text = textTotype;
            textLabel.maxVisibleCharacters = 0;

            float t = 0; //Guarda o tempo que se passa para escrever o tempo
            int charIndex = 0; //Guarda o numero de caracteres que devem aparecer com base no tempo passado

            while (charIndex < textTotype.Length)
            {
                int lastCharIndex = charIndex;

                t += Time.unscaledDeltaTime * typewriterSpeed; //Adiciona o tempo vezes a velocidade

                charIndex = Mathf.FloorToInt(t);
                charIndex = Mathf.Clamp(charIndex, 0, textTotype.Length); //Limita a variavel entre 0 e o numero de caracteres do texto atual

                //Caso a variavel de index das letras some mais de 1 desde a ultima frame, escreve todos os caracteres extras na tela, ainda com um intervalo caso tenha alguma pontuacao
                for (int i = lastCharIndex; i < charIndex; i++)
                {
                    bool isLast = i >= textTotype.Length - 1;

                    textLabel.maxVisibleCharacters = i + 1; //Atualiza os caracteres visiveis no texto

                    //Caso tenha sido passado um typing sound, toca ele
                    if (typingSound != null)
                    {
                        if(typingSound.TocarUmSomPorVez == false)
                        {
                            dialogueUI.TocarSomOneShot(typingSound.Som);
                        }
                        else
                        {
                            if(dialogueUI.SomTocando == false)
                            {
                                dialogueUI.TocarSom(typingSound.Som);
                            }
                        }
                    }

                    if (IsPunctuation(textTotype[i], out float waitTime) && !isLast && !IsPunctuation(textTotype[i + 1], out _)) //O _ e um descarte, uma variavel nao usada em sem valor e endereco na memoria
                    {
                        yield return new WaitForSecondsRealtime(waitTime);
                    }
                }

                yield return null;
            }

            textLabel.maxVisibleCharacters = textTotype.Length; //Atualiza os caracteres visiveis no texto

            IsRunning = false;
        }

        private bool IsPunctuation(char character, out float waitTime) //Out e uma variavel que vai ser criada e retornada para fora da funcao, ao escrever a funcao deve-se colocar o out tambem para poder usar a variavel, exemplo: IsPonctuation(character, out waitTime);
        {
            foreach (Punctuation punctuationCategory in punctuations)
            {
                if (punctuationCategory.Punctuations.Contains(character))
                {
                    waitTime = punctuationCategory.WaitTime;
                    return true;
                }
            }

            waitTime = default;
            return false;
        }

        private readonly struct Punctuation
        {
            public readonly HashSet<char> Punctuations;
            public readonly float WaitTime;

            public Punctuation(HashSet<char> punctuations, float waitTime)
            {
                Punctuations = punctuations;
                WaitTime = waitTime;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BotaoRepetidor : MonoBehaviour
{
    //Variaveis
    [Header("Opcoes")]
    [Tooltip("O valor inicial do intervalo para o botao se auto clicar quando estiver pressionado.")]
    [SerializeField] private float intervaloMaximo;

    [Tooltip("O valor minimo do intervalo para o botao se auto clicar quando estiver pressionado.")]
    [SerializeField] private float intervaloMinimo;

    [Tooltip("O valor que sera subtraido do intervalo a cada auto clique.")]
    [SerializeField] private float diminuicaoDoIntervalo;

    private bool apertado;
    private float intervalo;
    private float tempo;

    [Header("Eventos")]

    [SerializeField] private UnityEvent onClickEvent = new UnityEvent();

    //Getters
    public UnityEvent OnClickEvent => OnClickEvent;

    private void Awake()
    {
        //Componentes
        HoldButton holdButton = GetComponent<HoldButton>();

        //Variaveis
        apertado = false;
        intervalo = 0;
        tempo = 0;

        //Eventos
        holdButton.OnPointerDownEvent.AddListener(OnPointerDown);
        holdButton.OnPointerUpEvent.AddListener(OnPointerUp);
    }

    private void Update()
    {
        if(apertado == true)
        {
            tempo += Time.unscaledDeltaTime;

            if(tempo >= intervalo)
            {
                onClickEvent?.Invoke();

                tempo = 0;

                intervalo -= diminuicaoDoIntervalo;

                if(intervalo < intervaloMinimo)
                {
                    intervalo = intervaloMinimo;
                }
            }
        }
    }

    private void OnPointerDown(PointerEventData eventData)
    {
        apertado = true;

        intervalo = intervaloMaximo;
        tempo = 0;

        onClickEvent?.Invoke();
    }

    private void OnPointerUp(PointerEventData eventData)
    {
        apertado = false;
    }

    private void OnValidate()
    {
        if(diminuicaoDoIntervalo < 0)
        {
            diminuicaoDoIntervalo = 0;
        }
    }
}

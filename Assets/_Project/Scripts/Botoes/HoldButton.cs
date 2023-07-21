using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    //Variaveis
    [Header("Opcoes")]

    [SerializeField]
    [Tooltip("\"Solta\" o botao caso ele esteja pressionado e o mouse saia de cima dele.")]
    private bool soltarQuandoOMouseSair;

    [SerializeField]
    [Tooltip("Caso \"soltarQuandoOMouseSair\" seja verdadeiro, chama o evento OnPointerUp se o botao estiver pressionado e o mouse sair de cima dele.")]
    private bool chamarOnPointerUpQuandoOMouseSair;

    private bool apertado;

    [Header("Eventos")]

    [SerializeField] private UnityEvent<PointerEventData> onPointerDownEvent = new UnityEvent<PointerEventData>();
    [SerializeField] private UnityEvent<PointerEventData> onPointerUpEvent = new UnityEvent<PointerEventData>();

    //Getters
    public bool SoltarQuandoOMouseSair => soltarQuandoOMouseSair;

    public UnityEvent<PointerEventData> OnPointerDownEvent => onPointerDownEvent;
    public UnityEvent<PointerEventData> OnPointerUpEvent => onPointerUpEvent;

    public void OnPointerDown(PointerEventData eventData)
    {
        apertado = true;

        onPointerDownEvent?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (apertado == true)
        {
            apertado = false;

            onPointerUpEvent?.Invoke(eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(soltarQuandoOMouseSair == false)
        {
            return;
        }

        if(apertado == true)
        {
            apertado = false;

            if(chamarOnPointerUpQuandoOMouseSair == true)
            {
                onPointerUpEvent?.Invoke(eventData);
            }
        }
    }
}

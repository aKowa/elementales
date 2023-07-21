using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SlotDeObjeto : MonoBehaviour, IDropHandler
{
    //Variaveis
    [Header ("Opcoes")]
    [SerializeField] private bool ativado = true;

    [SerializeField]
    [Tooltip("Se estiver verdadeira, os objetos dropados nesse slot serao colocados no centro dele.")]
    private bool puxarObjetoParaOCentro = true;

    [Header("Eventos")]
    [SerializeField] private UnityEvent<PointerEventData> onDropEvent = new UnityEvent<PointerEventData>();

    //Getters
    public UnityEvent<PointerEventData> OnDropEvent => onDropEvent;

    public bool Ativado
    {
        get => ativado;
        set => ativado = value;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag != null && ObjetoArrastavel.ObjetoSendoArrastado != null)
        {
            if(ativado == false)
            {
                return;
            }

            if(puxarObjetoParaOCentro == true)
            {
                eventData.pointerDrag.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;
            }

            ObjetoArrastavel.ObjetoSendoArrastado.TrocouDePosicao = true;

            onDropEvent?.Invoke(eventData);
        }
    }
}

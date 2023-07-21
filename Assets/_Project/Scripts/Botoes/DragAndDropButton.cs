using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DragAndDropButton : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    //Variaveis
    [Header("Eventos")]

    [SerializeField] private UnityEvent<PointerEventData> onBeginDragEvent = new UnityEvent<PointerEventData>();
    [SerializeField] private UnityEvent<PointerEventData> onDragEvent = new UnityEvent<PointerEventData>();
    [SerializeField] private UnityEvent<PointerEventData> onEndDragEvent = new UnityEvent<PointerEventData>();

    //Getters
    public UnityEvent<PointerEventData> OnBeginDragEvent => onBeginDragEvent;
    public UnityEvent<PointerEventData> OnDragEvent => onDragEvent;
    public UnityEvent<PointerEventData> OnEndDragEvent => onEndDragEvent;

    public void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDragEvent?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        onDragEvent?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDragEvent?.Invoke(eventData);
    }
}

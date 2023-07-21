using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class ItemSlotLojaIAP : MonoBehaviour
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private IAPButton iapButton;

    private ScrollRect scrollRect;

    //Getters
    public IAPButton IAPButton => iapButton;

    private void Awake()
    {
        //Componentes
        DragAndDropButton dragAndDropButton = GetComponent<DragAndDropButton>();

        scrollRect = GetComponentInParent<ScrollRect>();

        //Eventos
        dragAndDropButton.OnBeginDragEvent.AddListener(OnBeginDrag);
        dragAndDropButton.OnDragEvent.AddListener(OnDrag);
        dragAndDropButton.OnEndDragEvent.AddListener(OnEndDrag);
    }

    private void OnBeginDrag(PointerEventData eventData)
    {
        if (scrollRect != null)
        {
            scrollRect.OnBeginDrag(eventData);
        }
    }

    private void OnDrag(PointerEventData eventData)
    {
        if (scrollRect != null)
        {
            scrollRect.OnDrag(eventData);
        }
    }

    private void OnEndDrag(PointerEventData eventData)
    {
        if (scrollRect != null)
        {
            scrollRect.OnEndDrag(eventData);
        }
    }
}

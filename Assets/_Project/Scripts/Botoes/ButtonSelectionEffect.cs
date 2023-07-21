using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSelectionEffect : Selectable
{
    //Componentes
    private HoldButton holdButton;

    protected override void Awake()
    {
        base.Awake();

        holdButton = GetComponent<HoldButton>();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (holdButton != null)
        {
            if(holdButton.SoltarQuandoOMouseSair == true)
            {
                if (!IsActive() || !IsInteractable())
                    return;

                DoStateTransition(SelectionState.Normal, false);
            }
        }
    }
}

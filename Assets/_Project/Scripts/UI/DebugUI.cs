using BergamotaDialogueSystem;
using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    //Componentes
    [SerializeField] private TMP_Text textoPermitirInput;
    [SerializeField] private TMP_Text textoPermitirInputGeral;
    [SerializeField] private TMP_Text textoDialogoAberto;

    private void Update()
    {
        textoPermitirInput.text = PauseManager.PermitirInput.ToString();
        textoPermitirInputGeral.text = PauseManager.PermitirInputGeral.ToString();
        textoDialogoAberto.text = DialogueUI.Instance.IsOpen.ToString();
    }
}

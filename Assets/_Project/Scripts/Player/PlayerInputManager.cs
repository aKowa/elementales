using BergamotaDialogueSystem;
using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    //Components
    [SerializeField] private InventarioController inventarioController;

    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>();
    }

    public void Interact()
    {
        playerInput.Interact();
    }

    public void Run(bool valor)
    {
        playerInput.Running = valor;
    }

    public void Move(Vector2 movementDirection)
    {
        playerInput.Move(movementDirection);
    }

    public void AbrirOInventario()
    {
        if (PauseManager.PermitirInput == false || PauseManager.PermitirInputGeral == false || PauseManager.JogoPausado == true || DialogueUI.Instance.IsOpen == true)
        {
            return;
        }

        inventarioController.OpenOrCloseView();
    }
}

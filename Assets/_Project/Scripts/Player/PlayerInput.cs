using BergamotaDialogueSystem;
using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    //Componentes
    private Player player;

    //Variaveis
    private bool running;

    //Getter

    public bool Running
    {
        get
        {
            return running;
        }

        set
        {
            running = value;
        }
    }

    private void Awake()
    {
        player = GetComponent<Player>();

        running = false;
    }

    private void Update()
    {
        if (PauseManager.PermitirInputGeral == false || PauseManager.PermitirInput == false || PauseManager.JogoPausado == true || DialogueUI.Instance.IsOpen == true)
        {
            return;
        }

        if (running == true)
        {
            Run();
        }
    }

    public void Move(Vector2 movementDirection)
    {
        if (PauseManager.PermitirInput == false || PauseManager.PermitirInputGeral == false || PauseManager.JogoPausado == true || DialogueUI.Instance.IsOpen == true)
        {
            return;
        }

        player.PlayerMovement.MovementDirection = movementDirection;
    }

    public void Run()
    {
        player.PlayerMovement.Running = true;
    }

    public void Interact()
    {
        if (PauseManager.PermitirInput == false || PauseManager.PermitirInputGeral == false || PauseManager.JogoPausado == true || DialogueUI.Instance.IsOpen == true)
        {
            return;
        }

        player.Interact();
    }
}

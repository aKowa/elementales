using UnityEngine;

#if PLATFORM_STANDALONE_WIN || UNITY_EDITOR
using BergamotaDialogueSystem;
#endif

public class PlayerDebugInput : MonoBehaviour
{
#if PLATFORM_STANDALONE_WIN || UNITY_EDITOR
    //Components
    private PlayerInput playerInput;
    private DialogueUI dialogueUI;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        dialogueUI = DialogueUI.Instance;
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            playerInput.Move(new Vector2(horizontal, vertical).normalized);
        }

        if(Input.GetButton("Run") == true)
        {
            playerInput.Run();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (dialogueUI.IsOpen == false)
            {
                playerInput.Interact();
            }
            else
            {
                dialogueUI.SetAvancarDialogo();
            }

            //Debug.Log($"Permitir Input: {BergamotaLibrary.PauseManager.PermitirInput}, Permitir Input Geral: {BergamotaLibrary.PauseManager.PermitirInputGeral}");
        }
    }
#endif
}
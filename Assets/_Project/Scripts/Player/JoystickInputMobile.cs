using System;
using UnityEngine;

public class JoystickInputMobile : MonoBehaviour
{
    //Componentes
    private Joystick joystick;
    private PlayerInputManager playerInputManager;

    private readonly float velMin = 0.15f;

    private void Awake()
    {
        joystick = GetComponent<Joystick>();
        playerInputManager = GetComponentInParent<PlayerInputManager>();
    }

    private void Update()
    {
        Move(joystick.Horizontal, joystick.Vertical);
    }
    
    void Move(float horizontal,float vertical)
    {
        if (MathF.Abs(horizontal) > velMin || MathF.Abs(vertical) > velMin)
        {
            playerInputManager.Move(new Vector2(horizontal, vertical));
        }
    }
}
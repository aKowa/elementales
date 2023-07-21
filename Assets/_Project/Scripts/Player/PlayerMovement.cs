using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Components
    private Player player;
    private Rigidbody2D rb;

    //Variables
    [Header("Movement Speed")]
    [SerializeField] private float velWalk;
    [SerializeField] private float velRun;

    [Header("Animation Speed")]
    [SerializeField] private float velAnimWalk;
    [SerializeField] private float velAnimRun;

    private Vector2 movementDirection;

    private Vector3 lastPos;

    private bool running;

    //Getters
    public Rigidbody2D Rb => rb;
    public Vector3 LastPos => lastPos;

    public Vector2 MovementDirection
    {
        get
        {
            return movementDirection;
        }

        set
        {
            movementDirection = value;
        }
    }

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
        //Components
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();

        //Variables
        movementDirection = Vector2.zero;
        lastPos = Vector3.zero;
    }

    private void FixedUpdate()
    {
        lastPos = transform.position;
    }

    public void Move()
    {
        if(running == false)
        {
            rb.velocity = movementDirection * (velWalk);
            player.Animacao.SetWalkSpeed(velAnimWalk);
        }
        else
        {
            rb.velocity = movementDirection * (velRun);
            player.Animacao.SetWalkSpeed(velAnimRun);
        }

        SetDirection();

        running = false;
        movementDirection = Vector2.zero;
    }

    private void SetDirection()
    {
        if(movementDirection == Vector2.zero)
        {
            return;
        }

        player.SetDirection(EntityModel.GetVectorDirection(movementDirection));
    }

    public void ZeroVelocity()
    {
        rb.velocity = Vector2.zero;
        movementDirection = Vector2.zero;
    }
}

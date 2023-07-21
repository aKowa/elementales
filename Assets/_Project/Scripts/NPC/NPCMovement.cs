using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    //Components
    private NPC npc;
    private Rigidbody2D rb;
    private AIPath aiPath;

    //Variables
    [Header("Movement Speed")]
    [SerializeField] private float vel;

    [Header("Animation Speed")]
    [SerializeField] private float velAnim;

    private Vector2 movementDirection;

    private Vector3 lastPos;

    private Vector3 velVector;

    //Getters
    public Vector3 LastPos => lastPos;
    public Vector3 VelVector => velVector;

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

    private void Awake()
    {
        //Components
        npc = GetComponent<NPC>();
        rb = GetComponent<Rigidbody2D>();
        aiPath = GetComponent<AIPath>();

        //Variables
        movementDirection = Vector2.zero;
        lastPos = Vector3.zero;
        velVector = Vector3.zero;
    }

    private void FixedUpdate()
    {
        velVector = transform.position - lastPos;

        lastPos = transform.position;
    }

    public void Move()
    {
        PathfindingAtivado(false);

        rb.velocity = movementDirection * vel;
        npc.Animacao.SetWalkSpeed(velAnim);

        SetDirection();
    }

    public void MoveWithPathfinding(Vector3 destino)
    {
        PathfindingAtivado(true);

        movementDirection = Vector2.zero;
        rb.velocity = Vector2.zero;
        npc.Animacao.SetWalkSpeed(velAnim);

        aiPath.maxSpeed = vel;
        aiPath.destination = destino;

        SetDirection();
    }

    private void SetDirection()
    {
        if (velVector == Vector3.zero)
        {
            return;
        }

        npc.SetDirection(EntityModel.GetVectorDirection(velVector));
    }

    public void ZeroVelocity()
    {
        rb.velocity = Vector2.zero;

        PathfindingAtivado(false);
    }

    private void PathfindingAtivado(bool ativado)
    {
        aiPath.enabled = ativado;
        aiPath.canMove = ativado;
    }
}

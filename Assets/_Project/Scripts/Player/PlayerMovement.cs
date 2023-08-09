using System;
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

    [Header("Traveled Tiles")]
    [SerializeField] private int maxTraveledTiles;

    private List<Vector2> traveledTiles = new List<Vector2>();

    private Vector2 movementDirection;

    private Vector3 lastPos;

    private Vector2 posicaoPlayer;

    private bool running;


    //Getters
    public Rigidbody2D Rb => rb;

    public List<Vector2> TraveledTiles { get => traveledTiles; set => traveledTiles = value; }

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
        Vector2 posicaoTemp = GetPosicaoPlayer(player.transform.position);

        if (posicaoTemp != posicaoPlayer)
        {
            AtualizarListaDeUltimasPosicoes();
        }

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

    public void AtualizarListaDeUltimasPosicoes()
    {
        Vector2 posicao = new Vector2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));

        traveledTiles.Insert(0, posicao);

        if (traveledTiles.Count >= maxTraveledTiles)
        {
            traveledTiles.RemoveAt(traveledTiles.Count - 1);
        }

        posicaoPlayer = GetPosicaoPlayer(transform.position);

    }

    private Vector2 GetPosicaoPlayer(Vector2 posicao)
    {
        return new Vector2(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.y));
    }
}

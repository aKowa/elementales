using BergamotaLibrary;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DirecoesSequenciaisNPC), typeof(WaypointsHolder))]
public class NPCComum : MonoBehaviour
{
    //Componentes
    protected NPC npc;
    protected DirecoesSequenciaisNPC direcoesSequenciais;
    protected WaypointsHolder waypointsHolder;

    protected BoxCollider2D boxCollider;

    protected Player player;

    //Enums
    public enum Tipo { Estatico, FicaVirando, AndaEmRota, AndaAleatoriamente }

    //Variaveis
    [SerializeField] protected LayerMask layerDasParedes;

    [Header("Opcoes de Movimento")]

    [SerializeField] protected EntityModel.Direction direcaoInicial;

    [EnumToggleButtons]
    [SerializeField] protected Tipo tipo;

    //NPC Anda Em Rota
    protected int waypointAtual;

    protected Vector3 posicaoOrigem,
                    posicaoDestino;

    protected bool colidindoComOPlayer;

    //NPC Anda Aleatoriamente
    [SerializeField]
    [ShowIf("tipo", Tipo.AndaAleatoriamente)]
    protected float tempoParado,
                  tempoAndando;

    protected float controladorAndarAleatoriamente;

    protected virtual void Awake()
    {
        //Componentes
        npc = GetComponent<NPC>();
        direcoesSequenciais = GetComponent<DirecoesSequenciaisNPC>();
        waypointsHolder = GetComponent<WaypointsHolder>();
        boxCollider = GetComponent<BoxCollider2D>();

        player = FindObjectOfType<Player>();

        //Variaveis
        colidindoComOPlayer = false;
        controladorAndarAleatoriamente = 0;
    }

    protected void Start()
    {
        if(waypointsHolder.Waypoints.Count > 0)
        {
            waypointAtual = waypointsHolder.IndiceWaypointMaisProximo(transform);

            posicaoOrigem = transform.position;
            posicaoDestino = LiBergamota.PosicaoDoTile(waypointsHolder.Waypoints[waypointAtual].position);
        }
        else
        {
            waypointAtual = 0;
            posicaoOrigem = Vector3.zero;
            posicaoDestino = Vector3.zero;
        }

        NovaDirecaoAleatoria();

        AtualizarDirecao(direcaoInicial);

        if(tipo == Tipo.FicaVirando)
        {
            direcoesSequenciais.AtualizarPosicao();
        }
    }

    protected void OnEnable()
    {
        StartCoroutine(AtualizarParametrosNoOnEnable());
    }

    protected void Update()
    {
        if (PauseManager.JogoPausado == true)
        {
            return;
        }

        npc.Animacao.Animate();
    }

    protected virtual void FixedUpdate()
    {
        if (PauseManager.JogoPausado == true)
        {
            return;
        }

        if(npc.DialogueUI.IsOpen == false && NPCManager.IniciandoBatalha == false && player.GetEstadoPlayer == Player.EstadoPlayer.Normal)
        {
            switch (tipo)
            {
                case Tipo.Estatico:
                    Estatico();
                    break;

                case Tipo.FicaVirando:
                    FicaVirando();
                    break;

                case Tipo.AndaEmRota:
                    AndaEmRota();
                    break;

                case Tipo.AndaAleatoriamente:
                    AndaAleatoriamente();
                    break;
            }
        }
        else
        {
            Estatico();
        }
    }

    protected void Estatico()
    {
        npc.NPCMovement.ZeroVelocity();
    }

    protected void FicaVirando()
    {
        direcoesSequenciais.AlterarPosicao();
    }

    protected void AndaEmRota()
    {
        if (colidindoComOPlayer == true)
        {
            npc.NPCMovement.ZeroVelocity();
            return;
        }

        //Vector3 direcao = (posicaoDestino - transform.position).normalized;

        AtualizarDirecaoMovimento(npc.NPCMovement.VelVector);

        npc.NPCMovement.MoveWithPathfinding(posicaoDestino);

        VerificarSeChegouNoWaypoint();
    }

    protected void AndaAleatoriamente()
    {
        if(ColisaoComUmaParede(npc.GetDirection) == true)
        {
            NovaDirecaoAleatoriaExceto(npc.GetDirection);
        }

        if (colidindoComOPlayer == true)
        {
            npc.NPCMovement.ZeroVelocity();
            return;
        }

        controladorAndarAleatoriamente += Time.deltaTime;

        if(controladorAndarAleatoriamente <= tempoParado)
        {
            npc.NPCMovement.ZeroVelocity();
        }
        else if (controladorAndarAleatoriamente < tempoParado + tempoAndando)
        {
            npc.NPCMovement.Move();
        }
        else
        {
            NovaDirecaoAleatoria();

            controladorAndarAleatoriamente = 0;
        }
    }

    protected void AtualizarDirecaoMovimento(Vector3 vetorDirecao)
    {
        npc.NPCMovement.MovementDirection = vetorDirecao;

        AtualizarDirecao(EntityModel.GetVectorDirection(vetorDirecao));
    }

    public virtual void AtualizarDirecao(EntityModel.Direction direcao)
    {
        npc.SetDirection(direcao);
    }

    protected void NovaDirecaoAleatoria()
    {
        EntityModel.Direction novaDirecao = EntityModel.GetRandomDirection();

        AtualizarDirecaoMovimento(EntityModel.GetDirectionVector(novaDirecao));
    }

    protected void NovaDirecaoAleatoriaExceto(EntityModel.Direction direcao)
    {
        EntityModel.Direction novaDirecao = EntityModel.GetRandomDirectionExcept(direcao);

        AtualizarDirecaoMovimento(EntityModel.GetDirectionVector(novaDirecao));
    }

    protected bool ColisaoComUmaParede(EntityModel.Direction direcao)
    {
        Rect retangulo = new Rect(0, 0, 1, 1);
        Vector2 distancia = Vector2.zero;

        switch(direcao)
        {
            case EntityModel.Direction.Up:
            case EntityModel.Direction.Down:
                    retangulo.size = new Vector2(boxCollider.size.x, 0.1f);
                    distancia = new Vector2(0, boxCollider.bounds.extents.y);
                    break;

            case EntityModel.Direction.Left:
            case EntityModel.Direction.Right:
                retangulo.size = new Vector2(0.1f, boxCollider.size.y);
                distancia = new Vector2(boxCollider.bounds.extents.x, 0);
                break;
        }

        Collider2D colidiu = Physics2D.OverlapBox((Vector2)boxCollider.bounds.center + (distancia * EntityModel.GetDirectionVector(direcao)), retangulo.size, 0, layerDasParedes);

        return colidiu != null;
    }

    protected void VerificarSeChegouNoWaypoint()
    {
        //Avanca o Waypoint caso o NPC passe do ponto de destino ou fique muito proximo dele
        if (waypointsHolder.ChegouNoWaypoint(posicaoOrigem, posicaoDestino, transform.position) == true)
        {
            AtualizarWaypoint();
        }
    }

    protected void AtualizarWaypoint()
    {
        waypointAtual++;

        if(waypointAtual >= waypointsHolder.Waypoints.Count)
        {
            waypointAtual = 0;
        }

        posicaoOrigem = transform.position;
        posicaoDestino = LiBergamota.PosicaoDoTile(waypointsHolder.Waypoints[waypointAtual].position);
    }

    protected virtual IEnumerator AtualizarParametrosNoOnEnable()
    {
        //Esperar o fim da frame pra nao acontecer junto com o Awake
        yield return null;

        AtualizarDirecao(npc.GetDirection);
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            colidindoComOPlayer = true;
        }
    }

    protected void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            colidindoComOPlayer = false;
        }
    }
}

using BergamotaDialogueSystem;
using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[SelectionBase]
public class Player : EntityModel
{
    //Managers
    private DialogueUI dialogueUI;

    //Components
    private PlayerData playerData;
    private PlayerAnimation animacao;
    private PlayerMovement playerMovement;
    private PlayerMoveInteractionBox playerInteracao;
    private InteracaoBoxCollider2D interacaoBoxCollider2D;
    private PlayerStatusEffect PlayerChangeTileInform;
    private TrocarAparenciaDoPersonagem trocarAparenciaDoPersonagem;

    //Enums
    public enum EstadoPlayer { Normal, Pescando, Nadando, RodandoAnimacao };

    //Variaveis
    private UnityEvent eventoRepelenteTerminou = new UnityEvent();
    private EstadoPlayer estadoPlayer;

    //Getters
    public DialogueUI DialogueUI => dialogueUI;
    public PlayerData PlayerData => playerData;
    public PlayerAnimation Animacao => animacao;
    public PlayerMovement PlayerMovement => playerMovement;
    public UnityEvent EventoRepelenteTerminou => eventoRepelenteTerminou;
    public EstadoPlayer GetEstadoPlayer => estadoPlayer;

    private void Awake()
    {
        //Managers
        dialogueUI = FindObjectOfType<DialogueUI>();

        //Componentes
        playerData = GetComponent<PlayerData>();
        playerInteracao = GetComponentInChildren<PlayerMoveInteractionBox>();
        interacaoBoxCollider2D = GetComponentInChildren<InteracaoBoxCollider2D>();
        animacao = GetComponentInChildren<PlayerAnimation>();
        playerMovement = GetComponent<PlayerMovement>();
        PlayerChangeTileInform = GetComponent<PlayerStatusEffect>();
        trocarAparenciaDoPersonagem = GetComponentInChildren<TrocarAparenciaDoPersonagem>();

        estadoPlayer = EstadoPlayer.Normal;
    }

    private void Start()
    {
        //Instantiate
        PlayerChangeTileInform.Instantiate(playerData.Inventario);

        AtualizarSprite();
    }

    protected void OnEnable()
    {
        StartCoroutine(AtualizarParametrosNoOnEnable());
    }

    private void Update()
    {
        if (PauseManager.JogoPausado == true)
        {
            return;
        }

        if(estadoPlayer == EstadoPlayer.Normal || estadoPlayer == EstadoPlayer.Nadando)
        {
            playerMovement.Move();
        }
        else
        {
            playerMovement.ZeroVelocity();
        }

        playerInteracao.ReceiveDirection(GetDirection);
        animacao.Animate();
    }
    private void FixedUpdate()
    {
        if(!BattleManager.InBattle)
        {
            if(PlayerChangeTileInform.Main() == true)
            {
                if(PlayerData.Repelente > 0)
                {
                    PlayerData.Repelente--;

                    if(PlayerData.Repelente <= 0)
                    {
                        eventoRepelenteTerminou?.Invoke();
                    }
                }
            }
        }
    }

    public override void SetDirection(Direction direcao)
    {
        base.SetDirection(direcao);
        animacao.SetDirection(direcao);
    }

    public void SetDirection(int direcao)
    {
        SetDirection((Direction)direcao);
    }

    public void SetEstado(EstadoPlayer estadoPlayer)
    {
        this.estadoPlayer = estadoPlayer;
    }
    public void Interact()
    {
        interacaoBoxCollider2D.Interagir(this);      
    }

    public void AtualizarSprite()
    {
        trocarAparenciaDoPersonagem.SpriteSheetTexture = PlayerData.GetPlayerSprite;
    }

    protected virtual IEnumerator AtualizarParametrosNoOnEnable()
    {
        //Esperar o fim da frame pra nao acontecer junto com o Awake
        yield return null;

        SetDirection(GetDirection);
    }
}


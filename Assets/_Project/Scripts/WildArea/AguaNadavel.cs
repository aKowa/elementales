using BergamotaDialogueSystem;
using BergamotaLibrary;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AguaNadavel : Interagivel
{
    //Componentes
    [Header("Componentes")]
    [SerializeField] private DialogueObject dialogoDesejaNadarOuPescar;
    [SerializeField] private AudioClip somPlayerEntrarNaAgua;

    [Space(10)]

    [SerializeField] private LayerMask layerMaskDaAgua;
    [SerializeField] private float duracaoDoPulo = 0.6f;

    [Space(10)]

    [SerializeField] private MonsterData monstroQuePodeNadar;
    [SerializeField] private bool podeNadar = false;

    private WildArea areaPesca;
    private Player player;
    private DialogueActivator dialogueActivator;
    private TilemapCollider2D tilemapCollider2D;

    private Sequence animacaoPlayerPulando;

    private void Awake()
    {
        areaPesca = GetComponent<WildArea>();
        dialogueActivator = GetComponent<DialogueActivator>();
        tilemapCollider2D = GetComponent<TilemapCollider2D>();
    }
    public override void Interagir(Player player)
    {
        this.player = player;

        switch (player.GetEstadoPlayer)
        {
            case Player.EstadoPlayer.Pescando:
                areaPesca.Interagir(player);
                return;

            case Player.EstadoPlayer.Nadando:
                return;
        }


        bool pescar = areaPesca.VerificarSePossuiItem(player);

        if(podeNadar == false)
        {
            if (pescar)
            {
                Pescar();
            }

            return;
        }

        bool nadar = VerificarSePlayerPossuiMonstroQueNada(player);


        if (nadar && pescar)
        {
            dialogueActivator.ShowDialogue(dialogoDesejaNadarOuPescar, player.DialogueUI);
        }
        else if(nadar)
        {
            AnimacaoNadar();
        }
        else if(pescar)
        {
            Pescar();
        }
    }
    private bool VerificarSePlayerPossuiMonstroQueNada(Player player)
    {
        if (player.PlayerData.Inventario.GetMonstroNaBag(monstroQuePodeNadar) == null)
            return false;

        return true;

    }
    public void Pescar()
    {
        areaPesca.Interagir(player);
    }

    public void AnimacaoNadar()
    {
        Vector3 posicao = ProcurarPosicaoValida(true, player.GetDirection, player.transform.position, out bool achouPosicaoValida);

        if (achouPosicaoValida == false)
        {
            return;
        }

        SetColisaoComoTrigger(true);

        PermitirInput(false);

        player.SetEstado(Player.EstadoPlayer.RodandoAnimacao);

        RodarAnimacao(posicao, Nadar);
    }

    private void Nadar()
    {
        PermitirInput(true);

        player.SetEstado(Player.EstadoPlayer.Nadando);

        SetColisaoComoTrigger(true);

        SoundManager.instance.TocarSom(somPlayerEntrarNaAgua);
    }

    public void AnimacaoPararDeNadar()
    {
        Vector3 posicao = ProcurarPosicaoValida(false, player.GetDirection, player.transform.position, out bool achouPosicaoValida);

        if (achouPosicaoValida == false)
        {
            return;
        }

        PermitirInput(false);

        player.SetEstado(Player.EstadoPlayer.RodandoAnimacao);

        RodarAnimacao(posicao, PararDeNadar);
    }

    private void PararDeNadar()
    {
        PermitirInput(true);

        player.SetEstado(Player.EstadoPlayer.Normal);

        SetColisaoComoTrigger(false);
    }

    public void SetColisaoComoTrigger(bool valor)
    {
        tilemapCollider2D.isTrigger = valor;
    }

    private void PermitirInput(bool value)
    {
        PauseManager.PermitirInput = value;
    }

    private Vector3 ProcurarPosicaoValida(bool procurarTileDeAgua, EntityModel.Direction direcao, Vector3 posicaoPlayer, out bool achouPosicaoValida)
    {
        Vector3 posicao;
        Vector3 posicaoDoTile;

        //Debug.Log($"Posicao do Player: {posicaoPlayer}");

        for(int i = 0; i < 4; i++)
        {
            posicao = (Vector2)posicaoPlayer + EntityModel.GetDirectionVector(direcao);
            posicaoDoTile = new Vector2(Mathf.FloorToInt(posicao.x), Mathf.FloorToInt(posicao.y)) + (Vector2.one / 2);

            //Debug.Log($"Posicao: {posicao}, Posicao do Tie: {posicaoDoTile}, Direcao: {direcao}");

            Collider2D[] colisoes = Physics2D.OverlapCircleAll(posicaoDoTile, 0.1f, layerMaskDaAgua);

            bool achouTileDeAgua = false;

            foreach(Collider2D colisao in colisoes)
            {
                if(colisao.GetComponent<AguaNadavel>() == true)
                {
                    achouTileDeAgua = true;
                    break;
                }
            }

            if((achouTileDeAgua == true && procurarTileDeAgua == true) || (achouTileDeAgua == false && procurarTileDeAgua == false))
            {
                achouPosicaoValida = true;

                return posicaoDoTile;
            }

            switch(direcao)
            {
                case EntityModel.Direction.Down:

                    direcao = EntityModel.Direction.Left;

                    break;

                case EntityModel.Direction.Left:

                    direcao = EntityModel.Direction.Up;

                    break;

                case EntityModel.Direction.Up:

                    direcao = EntityModel.Direction.Right;

                    break;

                case EntityModel.Direction.Right:

                    direcao = EntityModel.Direction.Down;

                    break;
            }
        }

        Debug.LogWarning($"Nenhuma posicao era valida! Procurando tile de agua: {procurarTileDeAgua} .");

        achouPosicaoValida = false;

        posicao = (Vector2)posicaoPlayer + EntityModel.GetDirectionVector(direcao);
        posicaoDoTile = new Vector2((int)posicao.x, (int)posicao.y) + (Vector2.one / 2);

        return posicaoDoTile;
    }

    private void RodarAnimacao(Vector3 posicaoParaPular, Action onAnimationEnd)
    {
        player.Animacao.ChangeAnimation("Jump");
        player.VirarNaDirecao(posicaoParaPular);

        PararAnimacao();

        animacaoPlayerPulando = DOTween.Sequence();

        animacaoPlayerPulando.Append(player.transform.DOMove(posicaoParaPular, duracaoDoPulo));
        animacaoPlayerPulando.AppendCallback(() => onAnimationEnd?.Invoke());
        animacaoPlayerPulando.AppendCallback(() => animacaoPlayerPulando = null);
    }

    private void PararAnimacao()
    {
        if (animacaoPlayerPulando != null)
        {
            animacaoPlayerPulando.Kill();

            animacaoPlayerPulando = null;
        }
    }
}

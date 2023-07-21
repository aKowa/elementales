using BergamotaDialogueSystem;
using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCEstatico : MonoBehaviour
{
    //Componentes
    protected NPC npc;

    protected Player player;

    [SerializeField] protected EntityModel.Direction direcao;

    [Tooltip("Se ativa, mesmo que o NPC vire de direcao para falar com o jogador, ele voltara a olhar na direcao inicial logo em seguida.")]
    [SerializeField] private bool sempreOlharNaDirecao = false;

    [Header("Condicoes Especiais")]
    [SerializeField] private ListaDeFlags listaDeFlags;
    [SerializeField] private ConditionalDialogues.CondicaoDeFlag[] condicoesParaSpawnar;

    protected virtual void Awake()
    {
        //Componentes
        npc = GetComponent<NPC>();

        player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        AtualizarDirecao(direcao);
    }

    private void OnEnable()
    {
        StartCoroutine(AtualizarParametrosNoOnEnable());
    }

    private void Update()
    {
        if (PauseManager.JogoPausado == true)
        {
            return;
        }

        npc.Animacao.Animate();

        if(npc.DialogueUI.IsOpen == false && sempreOlharNaDirecao == true && player.GetEstadoPlayer == Player.EstadoPlayer.Normal)
        {
            AtualizarDirecao(direcao);
        }
    }

    public virtual void AtualizarDirecao(EntityModel.Direction direcao)
    {
        npc.SetDirection(direcao);
    }

    private void ConferirCondicoesParaSpawnar()
    {
        if(listaDeFlags == null)
        {
            return;
        }

        gameObject.SetActive(ConditionalDialogues.CondicoesVerdadeiras(listaDeFlags.name, condicoesParaSpawnar));
    }

    private IEnumerator AtualizarParametrosNoOnEnable()
    {
        //Esperar o fim da frame pra nao acontecer junto com o Awake
        yield return null;

        AtualizarDirecao(npc.GetDirection);

        ConferirCondicoesParaSpawnar();
    }
}

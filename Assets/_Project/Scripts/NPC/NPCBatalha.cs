using BergamotaDialogueSystem;
using BergamotaLibrary;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class NPCBatalha : NPCComum
{
    //Componentes
    private IniciadorDeBatalhaNPC iniciadorDeBatalhaNPC;
    private DialogueActivator dialogueActivator;

    //Enums
    public enum Estado { Normal, AndandoAteOJogador }

    //Variaveis

    private Estado estado;

    [Header("Opcoes de Movimento da Batalha")]

    [SerializeField] private AudioClip somDaExclamacao;

    [Space(10)]

    [SerializeField] private float tempoComAExclamacao;

    [SerializeField] private bool andarAteOJogador;

    [ShowIf("andarAteOJogador", true)]
    [SerializeField] [Range(1.1f, 5f)] private float distanciaParaComecarODialogo;

    [SerializeField] private bool ficarParadoDepoisQuePerderABatalha;

    [Header("Flag da Batalha")]

    [SerializeField] private ListaDeFlags listaDeFlags;
    [SerializeField] private string nomeDaFlagDeBatalha;

    [Header("Dialogos")]

    [SerializeField] private DialogueObject dialogoInicioDaBatalha;

    [Header("Informacoes do NPC")]

    [SerializeField] private string nome;
    [SerializeField] private int dinheiroPelaVitoria;
    [SerializeField] private InventarioNPC inventarioNPC;

    [Header("Informacoes da Batalha")]
    private ComandoArena arenaPadrao;
    [SerializeField][Range(1, 2)] private int numeroDeMonstrosPorTime;
    [SerializeField] private Sprite backgroundDaBatalha;

    [Space(10)]
    [SerializeField] private AudioClip musicaDeInicioDeBatalha;
    [SerializeField] private AudioClip musicaDeBatalha;

    [Header("Componentes")]
    [SerializeField] private SpriteRenderer exclamacao;

    [Header("Condicoes Especiais")]

    [Tooltip("Se verdadeiro, inicia a batalha como uma batalha selvagem contra o primeiro monstro do inventario do NPC, o metodo do NPC perder a batalha sera rodado se o monstro for derrotado ou capturado.")]
    [SerializeField] private bool iniciarBatalhaComoWild = false;

    [Space(10)]

    [SerializeField] private ConditionalDialogues.CondicaoDeFlag[] condicoesParaSpawnar;

    [Space(10)]

    [SerializeField] private UnityEvent eventoPerdeuABatalha = new UnityEvent();

    //Getters
    public string Nome => nome;
    public int DinheiroPelaVitoria => dinheiroPelaVitoria;

    public InventarioNPC InventarioNPC
    {
        get { return inventarioNPC; }
        set
        {
            inventarioNPC = value;
        }
    }
    protected override void Awake()
    {
        base.Awake();

        //Componentes
        iniciadorDeBatalhaNPC = GetComponentInChildren<IniciadorDeBatalhaNPC>();
        dialogueActivator = GetComponentInChildren<DialogueActivator>();

        exclamacao.enabled = false;

        CalcDinheiro();

        ConferirCondicoesParaSpawnar();
    }

    protected override void FixedUpdate()
    {
        switch(estado)
        {
            case Estado.Normal:
                EstadoNormal();
                break;

            case Estado.AndandoAteOJogador:
                EstadoAndandoAteOJogador();
                break;
        }
    }

    public override void AtualizarDirecao(EntityModel.Direction direcao)
    {
        base.AtualizarDirecao(direcao);

        iniciadorDeBatalhaNPC.Rotacionar(EntityModel.GetDirectionVector(direcao));
    }

    public bool JaFezABatalha()
    {
        return Flags.GetListaDeFlags(listaDeFlags.name).GetFlag(nomeDaFlagDeBatalha);
    }

    private void EstadoNormal()
    {
        if (JaFezABatalha() == false || ficarParadoDepoisQuePerderABatalha == false)
        {
            base.FixedUpdate();
        }
        else
        {
            if (PauseManager.JogoPausado == true)
            {
                return;
            }

            Estatico();
        }
    }

    private void EstadoAndandoAteOJogador()
    {
        npc.NPCMovement.MoveWithPathfinding(player.transform.position);

        if(LiBergamota.Distancia(transform.position, player.transform.position) <= (distanciaParaComecarODialogo * distanciaParaComecarODialogo))
        {
            estado = Estado.Normal;

            AbrirCaixaDeDialogo();
        }
    }

    public void IniciarEventoDaBatalha(Player player)
    {
        PauseManager.PermitirInput = false;

        player.PlayerMovement.MovementDirection = Vector2.zero;
        player.PlayerMovement.ZeroVelocity();

        this.player = player;

        NPCManager.IniciandoBatalha = true;

        npc.VirarNaDirecao(player.transform.position);

        StartCoroutine(TempoSurpreso(tempoComAExclamacao));

        MapMusic.SalvarTempoDaMusicaDoMapaETocarOutra(musicaDeInicioDeBatalha);
    }

    private void IniciarDialogoOuAndar()
    {
        if (andarAteOJogador == true)
        {
            estado = Estado.AndandoAteOJogador;
        }
        else
        {
            AbrirCaixaDeDialogo();
        }
    }

    public void AbrirCaixaDeDialogo()
    {
        npc.VirarNaDirecao(player.transform.position);
        player.VirarNaDirecao(transform.position);

        dialogueActivator.ShowDialogue(dialogoInicioDaBatalha, player.DialogueUI);
        StartCoroutine(IniciarBatalhaCorrotina(player.DialogueUI));
    }

    public void IniciarBatalha()
    {
        if(iniciarBatalhaComoWild == false)
        {
            GameManager.Instance.StartBattle(null, numeroDeMonstrosPorTime, player.PlayerData, backgroundDaBatalha, this, musicaDeBatalha, false);
        }
        else
        {
            MonsterData monsterData = inventarioNPC.MonsterBag[0].MonsterData;
            int attacksCount = monsterData.GetMonsterUpgradesPerLevel.Where(upgrade => upgrade.Level <= inventarioNPC.MonsterBag[0].Nivel && upgrade.Ataque != null).Count();

            Monster monstro = new Monster(monsterData, inventarioNPC.MonsterBag[0].Nivel, null, WildMonsterGetNumOfAttacks(inventarioNPC.MonsterBag[0].Nivel, attacksCount));

            GameManager.Instance.StartBattle(null, 1, player.PlayerData, backgroundDaBatalha, monstro, musicaDeBatalha, false, false, PerdeuABatalha);
        }
    }

    public void PerdeuABatalha()
    {
        NPCManager.IniciandoBatalha = false;

        Flags.GetListaDeFlags(listaDeFlags.name).SetFlag(nomeDaFlagDeBatalha, true);

        eventoPerdeuABatalha?.Invoke();
    }

    private void CalcDinheiro()
    {
        if (dinheiroPelaVitoria <= 0)
        {
            dinheiroPelaVitoria = 0;
            int temp = 0;

            foreach (Monster monstro in inventarioNPC.MonsterBag)
            {
                temp += monstro.MonsterData.GetBaseMonsterAttributes.TotalPontosAtributosBase();
                temp = temp * monstro.Nivel / 10;
            }

            dinheiroPelaVitoria = temp;
        }
    }

    public int WildMonsterGetNumOfAttacks(int nivel, int attacksCount)
    {
        int numMinOfAttacks = (int)Mathf.Clamp(nivel * 0.2f * Random.Range(1, 4), 1, 4);
        int numMaxOfAttacks = Mathf.Clamp(attacksCount, 1, 4);
        return Random.Range(numMinOfAttacks, numMaxOfAttacks + 1);
    }

    private void ConferirCondicoesParaSpawnar()
    {
        gameObject.SetActive(ConditionalDialogues.CondicoesVerdadeiras(listaDeFlags.name, condicoesParaSpawnar));
    }

    private IEnumerator IniciarBatalhaCorrotina(DialogueUI dialogueUI)
    {
        while (dialogueUI.IsOpen == true)
        {
            yield return null;
        }

        IniciarBatalha();
    }

    private IEnumerator TempoSurpreso(float tempo)
    {
        if(somDaExclamacao != null)
        {
            SoundManager.instance.TocarSom(somDaExclamacao);
        }

        exclamacao.enabled = true;

        yield return new WaitForSeconds(tempo);

        exclamacao.enabled = false;

        IniciarDialogoOuAndar();
    }

    protected override IEnumerator AtualizarParametrosNoOnEnable()
    {
        //Esperar o fim da frame pra nao acontecer junto com o Awake
        yield return null;

        AtualizarDirecao(npc.GetDirection);

        ConferirCondicoesParaSpawnar();
    }
}

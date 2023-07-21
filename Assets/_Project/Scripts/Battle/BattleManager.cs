using BergamotaDialogueSystem;
using BergamotaLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using System;

public class BattleManager : MonoBehaviour
{
    //Instancia
    private static BattleManager instance;
    private static bool inBattle;

    //Enums
    public enum TipoBatalha { MonstroSelvagem, Player, Npc }
    public enum Turno { Comeco, Escolha, Batalha, Fim, Status }

    public enum EtapaBatalha { RolarDados, VerificarCombatLessons, ExecutarComandos, ConferirResultados, TrocarMonstroMorto, FinalizarTurno }

    //Componentes
    [Header("Objetos")]
    [SerializeField] private GameObject monstroAnimacaoBase;
    [SerializeField] private GameObject monstroSlotJogadorBase;
    [SerializeField] private GameObject monstroSlotNPCBase;
    [SerializeField] private GameObject efeitoQuantidadeDanoBase;
    [SerializeField] private BattleEffectAnimation efeitoAtaque;

    [Header("Componentes")]
    [SerializeField] private List<Transform> posicoes2Monstros;
    [SerializeField] private List<Transform> posicoes4Monstros;

    [SerializeField] private Transform monstrosHolder;
    [SerializeField] private RectTransform monstroSlotsHolderTime0;
    [SerializeField] private RectTransform monstroSlotsHolderTime1;
    [SerializeField] private Transform quantidadeDanoHolder;

    [SerializeField] private SpriteRenderer backgroundSpriteRenderer;

    private DialogueUI dialogueUI;
    private DialogueActivator dialogueActivator;
    private BattleUI battleUI;

    //Variaveis
    [Header("Variaveis")]
    [SerializeField] private TipoBatalha tipoBatalha;
    [SerializeField] private int quantidadeDeMonstrosPorTime;
    [SerializeField] private Turno turno;
    [SerializeField] private EtapaBatalha etapaBatalha;

    [SerializeField] private List<Comando> comandos = new List<Comando>();
    [SerializeField] private List<Comando> comandosParaRemover = new List<Comando>();
    [SerializeField] private List<Integrante> integrantes = new List<Integrante>();

    private ComandoArena arenaAtual;
    private int tentativasCorrer;
    private Monster monstroCapturado;
    private NPCBatalha npcAtual;

    [Header("Comandos Especiais")]
    [SerializeField]
    private Comando correr;
    [SerializeField]
    private ComandoTrocar trocarMonstro;
    [SerializeField]
    private ComandoArena arenaPadrao;
    [SerializeField]
    private ComandoDeAtaque struggle;
    [SerializeField]
    private ComandoDeAtaque ataqueConfusion;

    private int idTimePerdedor;

    private Monster monstroParaAprenderOAtaque;
    private ComandoDeAtaque ataqueParaAprenderNoNivel;

    private List<BattleAnimation> battleAnimations = new List<BattleAnimation>();

    List<int> indiceIntegranteTrocarMonstro = new List<int>();
    List<int> indiceMonstroAtualTrocarMonstro = new List<int>();

    private List<MonstroParaTrocar> monstrosParaTrocar = new List<MonstroParaTrocar>();
    private List<Integrante.MonstroAtual> monstrosParaMorrer = new List<Integrante.MonstroAtual>();
    private List<EfeitoParaRodar> efeitosParaRodar = new List<EfeitoParaRodar>();
    private List<EfeitoAtributoParaRodar> efeitosAtributosParaRodar = new List<EfeitoAtributoParaRodar>();
    private List<MonstroMortoParaTrocar> monstrosMortosDoJogadorParaTrocar = new List<MonstroMortoParaTrocar>();
    private List<EfeitoSecundarioParaRodar> efeitosSecundariosParaRodar = new List<EfeitoSecundarioParaRodar>();
    private List<CombatLessonParaMostrar> combatLessonsParaMostrar = new List<CombatLessonParaMostrar>();

    [Header("Dialogos")]
    [SerializeField] private DialogueObject dialogoUsouComando;
    [SerializeField] private DialogueObject dialogoUsouItem;
    [SerializeField] private DialogueObject dialogoJogouMonstro;
    [SerializeField] private DialogueObject[] dialogosTirouMonstro;
    [SerializeField] private DialogueObject dialogoModificouAtributo;
    [SerializeField] private DialogueObject dialogoResetouAtributo;
    [SerializeField] private DialogueObject dialogoGanhouExp;
    [SerializeField] private DialogueObject dialogoAumentouDeNivel;
    [SerializeField] private DialogueObject dialogoDerrotouNpc;
    [SerializeField] private DialogueObject dialogoMonstroDesmaiou;
    [SerializeField] private DialogueObject dialogoMonstroCapturado;
    [SerializeField] private DialogueObject dialogoMonstroNaoCapturado;
    [SerializeField] private DialogueObject dialogoMonsterEntryAdicionada;
    [SerializeField] private DialogueObject dialogoMonstroTransferidoParaBox;
    [SerializeField] private DialogueObject dialogoQuerDarUmNicknameProMonstro;
    [SerializeField] private DialogueObject dialogoCorreuDaBatalha;
    [SerializeField] private DialogueObject dialogoNaoConseguiuCorrer;
    [SerializeField] private DialogueObject dialogoGanhouDinheiro;
    [SerializeField] private DialogueObject dialogoGanhouDinheiroComMagnet;
    [SerializeField] private DialogueObject dialogoAprendeuCombatLesson;
    [SerializeField] private DialogueObject dialogoGanhouUmDado;
    [SerializeField] private DialogueObject dialogoMelhorouUmDado;

    [Header("Dices")] 
    [SerializeField] private DiceRoller diceRoller;
    [SerializeField] private List<Transform> dicePos_2Monstros;
    [SerializeField] private List<Transform> dicePos_4Monstros;
    [SerializeField] private List<Transform> dicePos_2Final;
    [SerializeField] private List<Transform> dicePos_4Final;

    [Header("Tutorial")]
    [SerializeField] private DialogueObject dialogoTutorialBatalha;

    [Space(10)]
    [SerializeField] private ListaDeFlags listaDeFlagsTutorialBatalha;
    [SerializeField] private string flagTutorialBatalha;

    [Header("Sons")]
    [SerializeField] private ListaDeSons sonsGenericos;

    [Space(10)]
    [SerializeField] private AudioClip musicaVitoriaMonstroSelvagem;
    [SerializeField] private AudioClip musicaVitoriaNPC;

    //Variaveis de controle
    private int indiceEscolha;
    private bool esperandoComando;
    private int indiceComando;
    private bool esperandoAnimacao;
    private int countTurno;

    private bool iniciarTrocarMonstro;
    private bool trocandoMonstro;
    private bool iniciarAnimacaoDeMorte;
    private bool rodandoAnimacaoDeMorte;
    private bool iniciarRodarEfeitos;
    private bool rodandoEfeitos;
    private bool iniciarRodarEfeitosAtributos;
    private bool rodandoEfeitosAtributos;
    private bool iniciarRodarEfeitosSecundarios;
    private bool rodandoEfeitosSecundarios;
    private bool iniciarTrocarMonstrosMortosDoJogador;
    private bool trocandoMonstrosMortosDoJogador;
    private bool iniciarTelaDeFimDeBatalha;
    private bool iniciarTelaDeCorrer;
    private bool iniciarMostrarCombatLessons;
    private bool mostrandoCombatLessons;
    private bool fimDaBatalha;
    private bool dialogoAberto;

    private bool esperandoEscolherMonstro;
    private int indiceMonstroEscolhido;

    private int numeroDeVezesParaRodarAAnimacao;
    private bool isDiceRolling;
    private bool xpExtraComerFruta;
    private bool batalhaQuePodeCorrer;
    private Action eventoVenceuABatalha;

    //Getters
    public static BattleManager Instance => instance;
    public static bool InBattle => inBattle;
    public TipoBatalha GetTipoBatalha => tipoBatalha;
    public ComandoArena ArenaAtual => arenaAtual;
    public int GetTentativasCorrer => tentativasCorrer;
    public List<Integrante> Integrantes => integrantes;
    public int QuantidadeDeMonstrosPorTime => quantidadeDeMonstrosPorTime;
    public ComandoDeAtaque Struggle => struggle;
    public bool GetDialogoAberto => dialogoAberto;
    public Monster MonstroCapturado => monstroCapturado;
    public ComandoDeAtaque AtaqueConfusion => ataqueConfusion;

    public List<Comando> Comandos => comandos;
    public bool XpExtraComerFruta
    {
        get => xpExtraComerFruta;
        set => xpExtraComerFruta = value;
    }
    public bool BatalhaQuePodeCorrer
    {
        get => batalhaQuePodeCorrer;
        set => batalhaQuePodeCorrer = value;
    }
    private void Awake()
    {
        instance = this;

        battleUI = FindObjectOfType<BattleUI>();
        dialogueUI = DialogueUI.Instance;
        dialogueActivator = GetComponent<DialogueActivator>();

        LimparVariaveis();
    }

    private void LimparVariaveis()
    {
        etapaBatalha = EtapaBatalha.ExecutarComandos;
        turno = Turno.Comeco;

        tentativasCorrer = 0;
        idTimePerdedor = -1;
        monstroCapturado = null;
        tentativasCorrer = 0;
        indiceEscolha = 0;
        esperandoComando = false;
        indiceComando = 0;
        esperandoAnimacao = false;
        countTurno = 0;
        npcAtual = null;

        iniciarTrocarMonstro = false;
        trocandoMonstro = false;
        iniciarAnimacaoDeMorte = false;
        rodandoAnimacaoDeMorte = false;
        iniciarRodarEfeitos = false;
        rodandoEfeitos = false;
        iniciarRodarEfeitosAtributos = false;
        rodandoEfeitosAtributos = false;
        iniciarRodarEfeitosSecundarios = false;
        rodandoEfeitosSecundarios = false;
        iniciarTrocarMonstrosMortosDoJogador = false;
        trocandoMonstrosMortosDoJogador = false;
        iniciarTelaDeFimDeBatalha = false;
        iniciarTelaDeCorrer = false;
        iniciarMostrarCombatLessons = false;
        mostrandoCombatLessons = false;
        fimDaBatalha = false;
        dialogoAberto = false;
        batalhaQuePodeCorrer = false;
        monstroParaAprenderOAtaque = null;
        ataqueParaAprenderNoNivel = null;

        esperandoEscolherMonstro = false;
        indiceMonstroEscolhido = 0;

        numeroDeVezesParaRodarAAnimacao = 0;

        eventoVenceuABatalha = null;
        xpExtraComerFruta = false;
        comandos.Clear();
        comandosParaRemover.Clear();
        battleAnimations.Clear();
        monstrosParaTrocar.Clear();
        monstrosParaMorrer.Clear();
        efeitosParaRodar.Clear();
        efeitosAtributosParaRodar.Clear();
        monstrosMortosDoJogadorParaTrocar.Clear();
        efeitosSecundariosParaRodar.Clear();
        combatLessonsParaMostrar.Clear();

        foreach (var item in integrantes)
        {
            item.LimparListas();
        }

        integrantes.Clear();
        diceRoller.CleanVariablesAndDestroyLeftoverDice();
    }

    #region Create

    private void CreateNpc(InventarioNPC inventarioNPC, string nome, int idTime, int dinheiroPelaVitoria)
    {
        Integrante integranteNpc = new Integrante(TipoIntegrante.Npc, idTime, nome, inventarioNPC, dinheiroPelaVitoria);
        List<MonsterInBattle> listMonsterBattleTemp = new List<MonsterInBattle>();

        foreach (Monster monstroBag in inventarioNPC.MonsterBag)
        {
            Monster novoMonstro = new Monster(monstroBag);

            MonsterInBattle monsterBattleTemp = new MonsterInBattle(novoMonstro);
            monsterBattleTemp.GetMonstro.AtributosAtuais.GerarNovosAtributos(novoMonstro.MonsterData);
            monsterBattleTemp.GetMonstro.AtributosAtuais.LimparModificadores();
            monsterBattleTemp.GetMonstro.LimparStatusSecundario();
            listMonsterBattleTemp.Add(monsterBattleTemp);
        }

        integranteNpc.ReceberLista(listMonsterBattleTemp);
        integrantes.Add(integranteNpc);
    }

    private void CreateWild(Monster monstroSelvagem, int idTime)
    {
        Integrante integrante = new Integrante(TipoIntegrante.Selva, idTime, monstroSelvagem.MonsterData.GetName);//Monta integrante
        List<MonsterInBattle> listMonsterBattleTemp = new List<MonsterInBattle>();

        MonsterInBattle monsterBattle = new MonsterInBattle(monstroSelvagem);//Cria monstro
        monsterBattle.GetMonstro.AtributosAtuais.GerarNovosAtributos(monstroSelvagem.MonsterData);//Arruma vida do monstro

        listMonsterBattleTemp.Add(monsterBattle);
        integrante.ReceberLista(listMonsterBattleTemp); // Adiciona mosntro ao "inventario" e adiciona integrante a lita
        integrantes.Add(integrante);
    }
    public void CreatePlayer(PlayerData playerData, int idTime)
    {
        Integrante integrantePlayer = new Integrante(TipoIntegrante.Jogador, idTime, playerData.GetPlayerName, playerData.Inventario);
        List<MonsterInBattle> listMonstros = new List<MonsterInBattle>();
        foreach (Monster monstroBag in integrantePlayer.Inventario.MonsterBag)
        {
            MonsterInBattle monsterTemp = new MonsterInBattle(monstroBag);
            listMonstros.Add(monsterTemp);
            monsterTemp.GetMonstro.AtributosAtuais.LimparModificadores();
            monsterTemp.GetMonstro.LimparStatusSecundario();

        }
        integrantePlayer.ReceberLista(listMonstros);
        integrantes.Add(integrantePlayer);
    }
    //Batlaha que pode correr
    public void CreateBattle(ComandoArena arena, int numeroMonstrosBatalhandoCadaTimeAoMesmoTempo, PlayerData playerData, Sprite backgroundDaBatalha, Monster monstroSelvagem, Action eventoVenceuABatalha,bool podeCorrer)
    {
        tipoBatalha = TipoBatalha.MonstroSelvagem;
        quantidadeDeMonstrosPorTime = numeroMonstrosBatalhandoCadaTimeAoMesmoTempo;

        backgroundSpriteRenderer.sprite = backgroundDaBatalha;

        this.eventoVenceuABatalha = eventoVenceuABatalha;

        batalhaQuePodeCorrer = podeCorrer;

        CreateWild(monstroSelvagem, 0);
        CreatePlayer(playerData, 1);

        IniciarVariaveis(arena, quantidadeDeMonstrosPorTime);
    }
    //batalha wild
    public void CreateBattle(ComandoArena arena, int numeroMonstrosBatalhandoCadaTimeAoMesmoTempo, PlayerData playerData, Sprite backgroundDaBatalha, Monster monstroSelvagem, Action eventoVenceuABatalha)
    {
        tipoBatalha = TipoBatalha.MonstroSelvagem;
        quantidadeDeMonstrosPorTime = numeroMonstrosBatalhandoCadaTimeAoMesmoTempo;

        backgroundSpriteRenderer.sprite = backgroundDaBatalha;

        this.eventoVenceuABatalha = eventoVenceuABatalha;

        batalhaQuePodeCorrer = true;

        CreateWild(monstroSelvagem, 0);
        CreatePlayer(playerData, 1);

        IniciarVariaveis(arena, quantidadeDeMonstrosPorTime);
    }
    //batalha npcs
    public void CreateBattle(ComandoArena arena, int numeroMonstrosBatalhandoCadaTimeAoMesmoTempo, PlayerData playerData, Sprite backgroundDaBatalha, NPCBatalha npcBatalha, Action eventoVenceuABatalha)
    {
        tipoBatalha = TipoBatalha.Npc;
        npcAtual = npcBatalha;
        quantidadeDeMonstrosPorTime = numeroMonstrosBatalhandoCadaTimeAoMesmoTempo;

        backgroundSpriteRenderer.sprite = backgroundDaBatalha;

        this.eventoVenceuABatalha = eventoVenceuABatalha;

        batalhaQuePodeCorrer = true;

        CreateNpc(npcBatalha.InventarioNPC, npcBatalha.Nome, 0, npcBatalha.DinheiroPelaVitoria);
        CreatePlayer(playerData, 1);

        IniciarVariaveis(arena, quantidadeDeMonstrosPorTime);
    }

    private void IniciarVariaveis(ComandoArena arena, int numeroMonstrosAtuais)
    {
        //arenaPadrao = ScriptableObject.Instantiate(arena);
        //arenaPadrao.name = arena.name;
        //arenaPadrao.TurnosInfinitos = true;
        inBattle = true;

        ArrumarListaMonstroMorto();
        //TrocarArena(arenaPadrao);

        int indicePosicao = 0;

        if (numeroMonstrosAtuais <= 1)
        {
            for (int i = 0; i < integrantes.Count; i++)
            {
                BattleAnimation battleAnimation = Instantiate(monstroAnimacaoBase, monstrosHolder).GetComponent<BattleAnimation>();
                battleAnimation.transform.position = posicoes2Monstros[i].position;

                battleAnimation.gameObject.SetActive(true);

                battleAnimations.Add(battleAnimation);
            }
        }
        else
        {
            for (int i = 0; i < integrantes.Count; i++)
            {
                if (integrantes[i].ListaMonstros.Count <= 1)
                {
                    BattleAnimation battleAnimation = Instantiate(monstroAnimacaoBase, monstrosHolder).GetComponent<BattleAnimation>();
                    battleAnimation.transform.position = posicoes2Monstros[i].position;

                    battleAnimation.gameObject.SetActive(true);

                    battleAnimations.Add(battleAnimation);
                }
                else
                {
                    int monstrosCriados = 0;

                    for (int y = i * 2; monstrosCriados < numeroMonstrosAtuais; y++)
                    {
                        BattleAnimation battleAnimation = Instantiate(monstroAnimacaoBase, monstrosHolder).GetComponent<BattleAnimation>();
                        battleAnimation.transform.position = posicoes4Monstros[y].position;

                        battleAnimation.gameObject.SetActive(true);

                        battleAnimations.Add(battleAnimation);

                        monstrosCriados++;
                    }
                }
            }
        }

        for (int i = 0; i < integrantes.Count; i++)
        {
            integrantes[i].CriarListaMonstros(numeroMonstrosAtuais, battleAnimations, ref indicePosicao, monstroSlotJogadorBase, monstroSlotNPCBase, monstroSlotsHolderTime0, monstroSlotsHolderTime1, i);
        }

        battleUI.IniciarMonstroSlots();

        CreateDices();
        
        StartCoroutine(SurgirMonstrosNoInicio());
    }
    
    private void CreateDices()
    {
        diceRoller.Initialize();

        GenerateDicesAndPositions();
        
        diceRoller.AssignStopDiceAction(() => StartCoroutine(DiceResultsIntoUI()));
    }

    private void GenerateDicesAndPositions()
    {
        diceRoller.CleanVariables();
        
        //Passa posicoes para os dados baseando-se se é 1v1 ou 2v2
        var posicoes = QuantidadeDeMonstrosPorTime == 1 ? dicePos_2Monstros : dicePos_4Monstros;
        int dicePos = 0;

        integrantes.ForEach(i => i.MonstrosAtuais.ForEach(m =>
        {
            if (m.Monstro.DiceInstances.IsNullOrEmpty() && m.GetMonstro.IsFainted == false)
            {
                m.Monstro.DiceInstances = diceRoller.GenerateDices(m.GetMonstro.Dices, m.GetMonstro.DiceMaterial, posicoes[dicePos].position);
            }

            dicePos++;
            
        }));
        
        SortDices();

        diceRoller.InitializeDiceRoller();
    }

    private IEnumerator DiceResultsIntoUI()
    {
        List<Transform> positions = QuantidadeDeMonstrosPorTime == 1 ? dicePos_2Final : dicePos_4Final;
        yield return new WaitForSeconds(1.5f);

        for (var i = 0; i < integrantes.Count; i++)
        {
            Integrante integrante = integrantes[i];
            for (var m = 0; m < integrante.MonstrosAtuais.Count; m++)
            {
                Integrante.MonstroAtual monstroAtual = integrante.MonstrosAtuais[m];
                if (monstroAtual.GetMonstro.IsFainted == false)
                {
                    foreach (var comando in comandos)
                    {
                        if (comando is ComandoDeAtaque)
                        {
                            if (comando.GetMonstro == monstroAtual.GetMonstro)
                            {
                                if (comando.QuantidadeVezesComandoRodou < 1)
                                {
                                    for (var dI = 0; dI < monstroAtual.Monstro.DiceInstances.Count; dI++)
                                    {
                                        var diceInstance = monstroAtual.Monstro.DiceInstances[dI];
                                        diceInstance.transform.DOMove(positions[m + i * QuantidadeDeMonstrosPorTime].position, 0.8f)
                                            .SetEase(Ease.InSine);
                                        diceInstance.transform.DORotate(new Vector3(0, 720, 0), 1.2f, RotateMode.FastBeyond360);
                                        diceInstance.transform.DOScale(0, 1.2f);
                                        yield return new WaitForSeconds(0.5f);
                                        diceRoller.AddDiceIcon(monstroAtual.GetMonstro.Dices[dI], monstroAtual.Monstro.DiceResults[dI], monstroAtual.GetMonstro.DiceMaterial, monstroAtual.MonstroSlotBattle.MonsterDicesHolder);
                                        monstroAtual.MonstroSlotBattle.FindDiceIcons();
                                        //TODO: Add a visual effect if a dice critical is in place
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        yield return new WaitForSeconds(1);
        
        etapaBatalha = EtapaBatalha.VerificarCombatLessons;
        isDiceRolling = false;

        diceRoller.ResetDicePositions();
    }
    
    private void ArrumarListaMonstroMorto()
    {
        for (int i = 0; i < integrantes.Count; i++)
        {
            for (int j = 0; j < integrantes[i].ListaMonstros.Count; j++) //procura por algum monstro desmaiado
            {
                if (integrantes[i].ListaMonstros[j].GetMonstro.IsFainted)//caso enconte
                {
                    MonsterInBattle monstroMorto = null;
                    MonsterInBattle monstroVivo = null;

                    monstroMorto = integrantes[i].ListaMonstros[j];

                    for (int k = j; k < integrantes[i].ListaMonstros.Count; k++)// procura proximo monstro vivo a partir desse monstro alvo
                    {
                        if (integrantes[i].ListaMonstros[k].GetMonstro.IsFainted == false)//Caso encontre um monstro morto
                        {
                            monstroVivo = integrantes[i].ListaMonstros[k];

                            integrantes[i].ListaMonstros[j] = monstroVivo;
                            integrantes[i].ListaMonstros[k] = monstroMorto;
                            break;
                        }
                    }

                }
            }
        }
    }
    
    #endregion

    private void Update()
    {
        if (inBattle == false)
        {
            return;
        }

        if (dialogoAberto == true || fimDaBatalha == true || trocandoMonstro == true || rodandoAnimacaoDeMorte == true || rodandoEfeitos == true || rodandoEfeitosSecundarios == true || rodandoEfeitosAtributos == true || trocandoMonstrosMortosDoJogador == true || mostrandoCombatLessons == true)
        {
            return;
        }

        if (iniciarTelaDeFimDeBatalha == true)
        {
            fimDaBatalha = true;

            StartCoroutine(TelaFimDeBatalha());

            return;
        }

        if (iniciarTelaDeCorrer == true)
        {
            fimDaBatalha = true;

            StartCoroutine(TelaDeCorrer());

            return;
        }

        if(iniciarMostrarCombatLessons == true)
        {
            iniciarMostrarCombatLessons = false;
            mostrandoCombatLessons = true;

            StartCoroutine(MostrarCombatLessons());

            return;

        }

        if (iniciarRodarEfeitos == true)
        {
            iniciarRodarEfeitos = false;
            rodandoEfeitos = true;

            StartCoroutine(RodarEfeitosDeStatus());

            return;
        }

        if (iniciarRodarEfeitosSecundarios == true)
        {
            iniciarRodarEfeitosSecundarios = false;
            rodandoEfeitosSecundarios = true;

            StartCoroutine(RodarEfeitosDeStatusSecundarios());

            return;
        }

        if (iniciarRodarEfeitosAtributos == true)
        {
            iniciarRodarEfeitosAtributos = false;
            rodandoEfeitosAtributos = true;

            StartCoroutine(RodarEfeitosDeAtributos());

            return;
        }

        if (iniciarAnimacaoDeMorte == true)
        {
            iniciarAnimacaoDeMorte = false;
            rodandoAnimacaoDeMorte = true;

            StartCoroutine(RodarAnimacaoDeMorteDosMonstros());

            return;
        }

        if (iniciarTrocarMonstrosMortosDoJogador == true)
        {
            iniciarTrocarMonstrosMortosDoJogador = false;
            trocandoMonstrosMortosDoJogador = true;

            esperandoEscolherMonstro = false;

            StartCoroutine(TrocarMonstrosMortosDoJogador());

            return;
        }

        if (iniciarTrocarMonstro == true)
        {
            iniciarTrocarMonstro = false;
            trocandoMonstro = true;

            StartCoroutine(TrocarMonstrosDaLista());

            return;
        }

        Turnos();
    }

    void Turnos()
    {
        switch (turno)
        {
            case Turno.Comeco:
                Comeco();
                break;
            case Turno.Status:
                VerificarStatus();
                break;
            case Turno.Escolha:
                Choose();
                break;
            case Turno.Batalha:
                Battle();
                break;
            case Turno.Fim:
                Fim();
                break;
        }
    }

    void Comeco()
    {
        foreach (Integrante integrante in integrantes)
        {
            foreach (Integrante.MonstroAtual monstroAtual in integrante.MonstrosAtuais)
            {
                monstroAtual.Monstro.EntrouEmCombate = true;
            }
        }
        
        IniciarEscolhas();
    }

    void Machine(int indice)
    {
        for (int i = 0; i < integrantes[indice].MonstrosAtuais.Count; i++)
        {
            bool liberado = true;
            foreach (var item in integrantes[indice].MonstrosAtuais[i].GetMonstro.StatusSecundario)
            {
                if (item.ForaDeCombate() == true)
                    liberado = false;
            }
            if (liberado == true)
            {
                if (!integrantes[indice].JaEscolheu && !TemComandoQueBloqueiaAcao(integrantes[indice].MonstrosAtuais[i].Monstro) && !integrantes[indice].MonstrosAtuais[i].GetMonstro.IsFainted)
                {
                    Comando comando = (CombateIA.EscolherAtaque(integrantes, indice, i, 1, integrantes[indice].MonstrosAtuais.Count));
                    if (comando != null)
                        comandos.Add(comando);
                }
            }
        }

        integrantes[indice].JaEscolheu = true;

        esperandoComando = false;
    }

    void VerificarStatus()
    {
        indiceIntegranteTrocarMonstro.Clear();
        indiceMonstroAtualTrocarMonstro.Clear();

        for (int i = 0; i < integrantes.Count; i++)
        {
            for (int j = 0; j < integrantes[i].MonstrosAtuais.Count; j++)
            {
                ExecutarStatusInBattle(integrantes[i].MonstrosAtuais[j]);
                integrantes[i].MonstrosAtuais[j].GetMonstro.AtributosAtuais.AvancarStatus();//Avança os modificadores
                if (integrantes[i].MonstrosAtuais[j].GetMonstro.IsFainted == true)
                {
                    integrantes[i].MonstrosAtuais[j].GetMonstro.LimparMonstroMorto();
                    indiceIntegranteTrocarMonstro.Add(i);
                    indiceMonstroAtualTrocarMonstro.Add(j);
                }
            }
        }

        TrocarMonstroMorto();
        IniciarEscolhas();

        int contadorPessoasPerderam = 0;
        for (int i = 0; i < integrantes.Count; i++)
        {
            int tempQuantidadeMonstrosDerrotados = 0;
            for (int j = 0; j < integrantes[i].ListaMonstros.Count; j++)
            {
                if (integrantes[i].ListaMonstros[j].GetMonstro.IsFainted)
                    tempQuantidadeMonstrosDerrotados++;
            }

            if (tempQuantidadeMonstrosDerrotados >= integrantes[i].ListaMonstros.Count)
            {
                integrantes[i].Perdeu = true;
                idTimePerdedor = integrantes[i].Time;
                turno = Turno.Fim;
                contadorPessoasPerderam++;
            }
        }
        if(contadorPessoasPerderam >= integrantes.Count)
        {
            for (int i = 0; i < integrantes.Count; i++)
            {
                if (integrantes[i].TipoIntegrante != TipoIntegrante.Jogador)
                {
                    integrantes[i].Perdeu = false;
                }
                else
                {
                    integrantes[i].Perdeu = true;
                    idTimePerdedor = integrantes[i].Time;
                }

            }
        }
    }
    private void IniciarEscolhas()
    {
        for (int i = 0; i < integrantes.Count; i++)
        {
            integrantes[i].JaEscolheu = false;
        }

        indiceEscolha = 0;
        esperandoComando = false;
        turno = Turno.Escolha;
    }

    void Choose()
    {
        if (esperandoComando == true)
        {
            return;
        }

        if (indiceEscolha >= integrantes.Count)
        {
            OrdenarListaDeComandos();
            indiceComando = 0;
            turno = Turno.Batalha;
            etapaBatalha = EtapaBatalha.RolarDados;

            return;
        }

        if (integrantes[indiceEscolha].JaEscolheu == false)
        {
            esperandoComando = true;

            if (integrantes[indiceEscolha].TipoIntegrante == TipoIntegrante.Jogador)
            {
                battleUI.IniciarMenu(integrantes[indiceEscolha]);
            }
            else
            {
                Machine(indiceEscolha);
            }

            indiceEscolha++;
        }
        else
        {
            indiceEscolha++;
        }
    }

    public void ReceberComandoDoMenu(Comando comando, bool ultimoComando)
    {
        if (comando != null)
        {
            comandos.Add(comando);
        }

        if (ultimoComando == true)
        {
            esperandoComando = false;
        }
    }

    void Battle()
    {
        if (esperandoAnimacao == true)
        {
            return;
        }

        switch (etapaBatalha)
        {
            case EtapaBatalha.RolarDados:
                if(isDiceRolling == false)
                    RolarDados();
                break;
            case EtapaBatalha.VerificarCombatLessons:
                VerificarCombatLessons();
                break;
            case EtapaBatalha.ExecutarComandos:
                ExecutarComandos();
                break;

            case EtapaBatalha.ConferirResultados:
                ConferirResultados();
                break;

            case EtapaBatalha.TrocarMonstroMorto:
                TrocarMonstroMorto();
                etapaBatalha = EtapaBatalha.ExecutarComandos;
                break;

            case EtapaBatalha.FinalizarTurno:
                FinalizarTurno();
                break;
        }
    }

    private void RolarDados()
    {

        diceRoller.ClearDiceResults();
        diceRoller.CleanDicesListAndPositions();
        
        foreach (var integrante in integrantes)
        {
            List<Integrante.MonstroAtual> monsters = integrante.MonstrosAtuais;
            List<Monster> monstrosComComandoDeAtaque = new List<Monster>();
            foreach (var monstroAtual in monsters)
            {
                if (monstroAtual.GetMonstro.IsFainted == false)
                {
                    // Debug.Log("Nome: "+ monstroAtual.GetMonstro.NickName);
                    foreach (var comando in comandos)
                    {
                        if (comando is ComandoDeAtaque)
                        {
                            if (comando.GetMonstro == monstroAtual.GetMonstro)
                            {
                                monstrosComComandoDeAtaque.Add(comando.GetMonstro);
                            }
                        }
                    }
                    
                    foreach (var monstroTemp in monstrosComComandoDeAtaque)
                    {
                        if (monstroTemp == monstroAtual.GetMonstro)
                        {
                            // Debug.Log("normal" + monstroAtual.GetMonstro.NickName);
                            MonsterInBattle monsterInBattle = monstroAtual.Monstro;
                            List<int> previousDiceResults = monsterInBattle.DiceResults.ToList();
                            monsterInBattle.DiceResults.Clear();
                            monsterInBattle.DiceResults = GetDiceResults(monstroAtual.GetMonstro.Dices);
                            diceRoller.SendDiceResultsToWantedRolls(monsterInBattle.DiceResults);
                            if(monsterInBattle.CriticoGarantido == false)
                                monsterInBattle.CriticoGarantido = VerificarCriticoGarantido(previousDiceResults, monsterInBattle.DiceResults);
                        }
                    }

                    foreach (var monstroSemComandoDeAtaque in monsters.Where(m =>
                                 monstrosComComandoDeAtaque.Contains(m.GetMonstro) == false))
                    {
                        if (monstroSemComandoDeAtaque.GetMonstro == monstroAtual.GetMonstro)
                        {
                            // Debug.Log("removendo da lista");
                            diceRoller.AddToAvoidList(monstroAtual.Monstro.DiceInstances);
                        }
                    }

                }
            }
        }
        
        diceRoller.RollDice();
        isDiceRolling = true;
    }
    
    public List<int> GetDiceResults(List<DiceType> dices)
    {
        List<int> resultados = new List<int>();

        foreach (int dieValue in dices)
        {
            int rng = Random.Range(1, dieValue + 1);
            resultados.Add(rng);
        }

        return resultados;
    }

    private bool VerificarCriticoGarantido(List<int> previousDiceResults, List<int> thisDiceResults)
    {
        //Se há mais de um dado da rodada atual com resultado repetido. Isto só acontece se o monstro tem mais de um dado.
        bool playedDuplicateDices = thisDiceResults.Count != thisDiceResults.Distinct().Count() && thisDiceResults.Count > 1;
        //Se o dado saiu igual duas rodadas seguidas. Isto só acontece se o monstro tem apenas um dado.
        bool playedDuplicateDicesOnTwoTurns = previousDiceResults.Intersect(thisDiceResults).Any() && thisDiceResults.Count == 1;

        if (playedDuplicateDices || playedDuplicateDicesOnTwoTurns)
        {
            Debug.Log($"Distincts? {playedDuplicateDices} // Intersecting? {playedDuplicateDicesOnTwoTurns}");
            Debug.Log($"Um monstro tem critico garantido");
            
            return true;
        }

        return false;
    }


    private void VerificarCombatLessons()
    {
        integrantes.ForEach(integrante => integrante.MonstrosAtuais.
            Where(monstroAtual => monstroAtual.GetMonstro.IsFainted == false && monstroAtual.GetMonstro.CombatLessonsAtivos.IsNullOrEmpty() == false).
            ForEach(monstroAtual =>
        {
            Monster monster = monstroAtual.GetMonstro;
            MonsterInBattle monsterInBattle = monstroAtual.Monstro;
            int indiceMonstro = integrante.MonstrosAtuais.IndexOf(monstroAtual);

            List<ComandoDeAtaque> comandosDeAtaque = comandos
                .Where(comando => comando.GetMonstro == monster && comando.TurnosParaSerExecutado == 0 && comando is ComandoDeAtaque)
                .Cast<ComandoDeAtaque>()
                .Where(c => c.QuantidadeVezesComandoRodou == 0)
                .ToList();

            if (comandosDeAtaque.Any())
            {
                List<int> diceResults = monsterInBattle.DiceResults;
                List<DiceType> diceTypes = monster.Dices;

                List<CombatLesson> instantiatedCombatLessons = new List<CombatLesson>();
                foreach (CombatLesson cL in monster.CombatLessonsAtivos)
                {
                    Debug.Log($"Combat Lessons Ativos: {monster.CombatLessonsAtivos.Count}, adicionando {cL.Nome}");
                    instantiatedCombatLessons.Add(Instantiate(cL));
                }

                instantiatedCombatLessons = instantiatedCombatLessons.OrderByDescending(cL => cL.Prioridade).ToList();
                
                foreach (CombatLesson cL in instantiatedCombatLessons)
                {
                    foreach (EffectStructure ef in cL.Effects)
                    {
                        bool canUseCombatLesson = true;
                        comandosDeAtaque.ForEach(cA => diceTypes.ForEach(dT =>
                        {
                            int cAIndex = comandosDeAtaque.IndexOf(cA);
                            int dTIndex = diceTypes.IndexOf(dT);
                            if (ef.VerifyIfCanUseEffect(comandosDeAtaque[cAIndex], diceTypes[dTIndex], diceResults[dTIndex]) == false)
                            {
                                canUseCombatLesson = false;
                            }
                        }));

                        if (canUseCombatLesson)
                        {
                            int indiceIntegrante = integrantes.IndexOf(integrante);
                            int indiceOpostoDeIntegrante = indiceIntegrante == 0 ? 1 : 0;
                            cL.AlvoAcao.Clear();
                            if (quantidadeDeMonstrosPorTime > 1)
                            {
                                switch (ef.TipoTarget)
                                {
                                    case TipoTarget.Self:
                                        cL.ReceberVars(integrante, monstroAtual, indiceMonstro);
                                        break;
                    
                                    case TipoTarget.Target:
                                        foreach (var comando in comandosDeAtaque)
                                        {
                                            for (int i = 0; i < comando.AlvoAcao.Count; i++)
                                            {
                                                cL.ReceberVars(integrante, comando.AlvoAcao[i], indiceMonstro);
                                            }
                                        }
                                        break;
                                    case TipoTarget.TimeAliado:
                                        integrante.MonstrosAtuais.Where(mA => mA.GetMonstro.IsFainted == false).ForEach(
                                            alvoAliado =>
                                            {
                                                cL.ReceberVars(integrante, alvoAliado, indiceMonstro);
                                            });
                                        break;
                                    case TipoTarget.TimeInimigo:
                                        integrantes[indiceOpostoDeIntegrante].MonstrosAtuais.Where(mA => mA.GetMonstro.IsFainted == false).ForEach(
                                            alvoInimigo =>
                                            {
                                                cL.ReceberVars(integrante, alvoInimigo, indiceMonstro);
                                            });
                                        break;
                                    case TipoTarget.TodosExcetoSelf:
                                        integrantes.ForEach(i => i.MonstrosAtuais
                                            .Where(mA => monster != mA.GetMonstro).ForEach(alvoDiferente =>
                                            {
                                                cL.ReceberVars(integrante, alvoDiferente, indiceMonstro);
                                            }));
                                        break;
                                    case TipoTarget.Aleatorio:
                                        List<Integrante.MonstroAtual> monstrosVivos = integrantes[indiceOpostoDeIntegrante].MonstrosAtuais
                                            .Where(mA => mA.GetMonstro.IsFainted == false).ToList();
                                        var monstroAleatorio = monstrosVivos.ElementAt(Random.Range(0, monstrosVivos.Count));
                        
                                        cL.ReceberVars(integrante, monstroAleatorio, indiceMonstro);
                                        break;
                                }
                            }
                            else
                            {
                                switch (ef.TipoTarget)
                                {
                                    case TipoTarget.Self or TipoTarget.TimeAliado:
                                        cL.ReceberVars(integrante, monstroAtual, indiceMonstro);
                                        break;
                                    case TipoTarget.Target:
                                        foreach (var comando in comandosDeAtaque)
                                        {
                                            for (int i = 0; i < comando.AlvoAcao.Count; i++)
                                            {
                                                cL.ReceberVars(integrante, comando.AlvoAcao[i], indiceMonstro);
                                            }
                                        }
                                        break;
                                    case TipoTarget.TimeInimigo or TipoTarget.Aleatorio or TipoTarget.TodosExcetoSelf:
                                        integrantes[indiceOpostoDeIntegrante].MonstrosAtuais.Where(mA => mA.GetMonstro.IsFainted == false).ForEach(
                                            alvoInimigo =>
                                            {
                                                cL.ReceberVars(integrante, alvoInimigo, indiceMonstro);
                                            });
                                        break;
                                }
                            }
            
                            Debug.Log($"{monster.NickName} pode usar o combat lesson {cL}");
                            cL.ExecutarCombatLesson(this);
            
                            Debug.Log($"{cL.Nome} é do {cL.GetMonstro.NickName}");
                            MostrarCombatLesson(cL.GetMonstro.MonsterData.Miniatura, cL.Nome);
                        }
                    }
                }
            }

        }));
        
        etapaBatalha = EtapaBatalha.ExecutarComandos;
    }

    private void ExecutarComandos()
    {
        if (indiceComando >= comandos.Count)
        {
            etapaBatalha = EtapaBatalha.FinalizarTurno;
            return;
        }

        Comando comando = comandos[indiceComando];

        if (comando.PodeMeRetirar)//se comando podeMeRetirar pula
        {
            comandosParaRemover.Add(comando);
            indiceComando++;
            return;
        }

        if (comando.Origem != null)
        {

            if (!comando.ComandoSolto)
            {
                bool temp = true;
                for (int i = 0; i < comando.Origem.MonstrosAtuais.Count; i++)
                {
                    if (comando.Origem.MonstrosAtuais[i].GetMonstro == comando.GetMonstro) // Caso monstro autor do comando esteja em batalha
                    {
                        temp = false;
                        break;
                    }
                }
                comando.PodeMeRetirar = temp;
            }

            if (comando.GetMonstro.IsFainted)
            {
                comando.PodeMeRetirar = true;
            }

            else
            {
                StartCoroutine(comando.Origem.MonstrosAtuais[comando.IndiceMonstro].MonstroSlotBattle.AtualizarBarras());

                //se acao bloqueia ataque , ataque solto(missel e talz)
                bool statusBloqueiaAtaque = false;
                if (!comando.ComandoSolto && comando is ComandoDeAtaque)
                {
                    foreach (StatusEffectBase statusEffect in comando.GetMonstro.Status)
                    {
                        if(statusEffect is StatusEffectConfusion)
                        {
                            StatusEffectConfusion statusConfusion = (StatusEffectConfusion)statusEffect;
                            Comando comandoTemporario = statusConfusion.Executar(comando); //Verifica se confusion vai ter efeito, caso tenha retorna um comando para ser usado no lugar do antigo
                            if (comandoTemporario != null)
                            {
                                comando = comandoTemporario;
                                comandos[indiceComando] = comando;
                            }
                        }
                        else if (statusEffect.GetStatusEffectOpcoesDentroCombate.GetstatusEffectBloquearComando.BloqueiaComando)
                        {
                            statusBloqueiaAtaque = true;
                            Debug.Log("O ataque de " + comando.GetMonstro.NickName + " Foi bloqueado por causa do status de " + statusEffect.Nome);
                            comando.PodeMeRetirar = true;

                            RodarEfeito(comando.Origem.MonstrosAtuais[comando.IndiceMonstro], statusEffect, -1, false, true);
                            break;
                        }
                    }
                }

                if (!statusBloqueiaAtaque && !comando.GetMonstro.IsFainted)
                {
                    if (comando is ComandoDeAtaque)
                    {
                        ComandoDeAtaque comandoAtaque = (ComandoDeAtaque)comando;
                        comandoAtaque.VerificarAlvoVivo(integrantes);

                        if (comandoAtaque.AlvoAcao.Count > 0)
                        {
                            comandoAtaque.ConsumirRecurso();
                            comandoAtaque.VerificarAlvoStatusDebilitante();

                            comando.Acao.DialogoComando(this, comandoAtaque);

                            comando.IniciarAnimacao(this, comandoAtaque);
                        }
                        else
                        {
                            comando.PodeMeRetirar = true;
                        }
                    }
                    else if (comando is ComandoTrocar)
                    {
                        //comando.Acao.DialogoComando(this, comando);

                        comando.IniciarAnimacao(this);
                    }
                    else
                    {
                        comando.Acao.DialogoComando(this, comando);
                        comando.IniciarAnimacao(this);
                    }
                }
            }

            comando.Origem.MonstrosAtuais
                .Where(mA => mA.Monstro == comando.GetMonstroInBattle)
                .Single().MonstroSlotBattle
                .RemoveDiceIcons();
        }
        else
        {
            comando.Acao.DialogoComando(this, comando);

            comando.IniciarAnimacao(this);
        }

        etapaBatalha = EtapaBatalha.ConferirResultados;
    }

    private void ConferirResultados()
    {
        Comando comando = comandos[indiceComando];

        if (comando.PodeMeRetirar == true)
        {
            comandosParaRemover.Add(comando);//Adicionoa para remover comandos
        }


        //Verificar derrota caso sim termina o combate 


        indiceIntegranteTrocarMonstro.Clear();
        indiceMonstroAtualTrocarMonstro.Clear();

        for (int i = 0; i < integrantes.Count; i++)
        {
            int tempQuantidadeMonstrosDerrotados = 0;
            for (int j = 0; j < integrantes[i].ListaMonstros.Count; j++)
            {
                if (integrantes[i].ListaMonstros[j].GetMonstro.IsFainted)
                {
                    tempQuantidadeMonstrosDerrotados++;
                    integrantes[i].ListaMonstros[j].GetMonstro.LimparMonstroMorto();
                }
            }

            if (tempQuantidadeMonstrosDerrotados >= integrantes[i].ListaMonstros.Count)
            {
                integrantes[i].Perdeu = true;
                idTimePerdedor = integrantes[i].Time;
                turno = Turno.Fim;
            }

            for (int j = 0; j < integrantes[i].MonstrosAtuais.Count; j++)
            {
                if (integrantes[i].MonstrosAtuais[j].GetMonstro.IsFainted)//Caso monstro esteja morto e 
                {
                    indiceIntegranteTrocarMonstro.Add(i);
                    indiceMonstroAtualTrocarMonstro.Add(j);
                    monstrosParaMorrer.Add(integrantes[i].MonstrosAtuais[j]);
                    diceRoller.DestroyDices(integrantes[i].MonstrosAtuais[j].Monstro.DiceInstances);
                    integrantes[i].MonstrosAtuais[j].MonstroSlotBattle.RemoveDiceIcons();
                    integrantes[i].MonstrosAtuais[j].Monstro.DiceInstances = null;
                    iniciarAnimacaoDeMorte = true;
                }
            }
        }

        indiceComando++;


        if (indiceIntegranteTrocarMonstro.Count > 0)
        {
            //Se tiver monstro pra trocar
            etapaBatalha = EtapaBatalha.TrocarMonstroMorto;
        }
        else
        {
            //Se nao
            etapaBatalha = EtapaBatalha.ExecutarComandos;
        }
    }

    private void SortDices()
    {
        for (int i = 0; i < integrantes.Count; i ++)
        {
            Integrante integrante = integrantes[i];
            for (int m = 0; m < integrante.MonstrosAtuais.Count; m ++)
            {
                Integrante.MonstroAtual monstroAtual = integrante.MonstrosAtuais[m];
                if (monstroAtual.Monstro.DiceInstances.IsNullOrEmpty() == false)
                {
                    for (int d = 0; d < monstroAtual.Monstro.DiceInstances.Count; d++)
                    {
                        GameObject diceInstance = monstroAtual.Monstro.DiceInstances[d];
                        diceInstance.transform.SetSiblingIndex(i*4 + m*2 + d);
                    }
                }
            }
        }
    }

    private void TrocarMonstroMorto()
    {
        List<int> indiceNpcMonstrosDisponiveis = new List<int>();
        List<int> indiceNpcMonstrosJaAdicionados = new List<int>();

        for (int i = 0; i < indiceIntegranteTrocarMonstro.Count; i++)
        {
            switch (integrantes[indiceIntegranteTrocarMonstro[i]].TipoIntegrante)
            {
                case TipoIntegrante.Jogador:
                    MonstroMortoDoJogadorParaTrocar(indiceIntegranteTrocarMonstro[i], indiceMonstroAtualTrocarMonstro[i]);
                    break;

                case TipoIntegrante.Npc:
                    for (int j = 0; j < integrantes[indiceIntegranteTrocarMonstro[i]].ListaMonstros.Count; j++)
                    {
                        if (integrantes[indiceIntegranteTrocarMonstro[i]].ListaMonstros[j].GetMonstro.IsFainted == false)       //Caso o monstro não esteja morto
                        {
                            bool monstroEmBatalha = false;
                            foreach (var item in integrantes[indiceIntegranteTrocarMonstro[i]].MonstrosAtuais)
                            {
                                if (item.GetMonstro == integrantes[indiceIntegranteTrocarMonstro[i]].ListaMonstros[j].GetMonstro)
                                    monstroEmBatalha = true;
                            }
                            if (!indiceNpcMonstrosJaAdicionados.Contains(j))
                            {
                                if (!monstroEmBatalha)                                                                     //Adiciona indice dos monstros disponiveis para trocar
                                {
                                    indiceNpcMonstrosDisponiveis.Add(j);
                                    indiceNpcMonstrosJaAdicionados.Add(j);
                                }
                            }
                        }
                    }
                    foreach (var item in indiceNpcMonstrosDisponiveis)
                    {
                        Debug.Log("Posso trocar com " + integrantes[indiceIntegranteTrocarMonstro[i]].ListaMonstros[item].GetMonstro.NickName);
                    }
                    if (indiceNpcMonstrosDisponiveis.Count > 0) //Caso possua algum slot disponivel para trocar
                    {
                        Comando comando = CombateIA.EscolherTrocarMonstro(ref indiceNpcMonstrosDisponiveis, indiceIntegranteTrocarMonstro[i], indiceMonstroAtualTrocarMonstro[i], Integrantes);
                        comando.Executar(this);
                    }
                    else
                    {
                        foreach (var monstroAtual in integrantes[indiceIntegranteTrocarMonstro[i]].MonstrosAtuais) //Caso não possua deleta os slots
                        {
                            if (monstroAtual.GetMonstro.IsFainted)
                            {
                                //Object.Destroy(monstroAtual.BattleAnimation.gameObject);
                                //Object.Destroy(monstroAtual.MonstroSlotBattle.transform.parent.gameObject);
                            }
                        }
                    }

                    break;
            }
        }
        //Confeir quem vai trocar um monstro, e depois colocar no estado de trocar as animacoes, dar opcao pro player escolher o monstro, etc.

        //No fim
    }

    private void FinalizarTurno()
    {
        foreach (Comando comando in comandosParaRemover) // Verifica comandos para remover
        {
            comando.SeRetirarDaLista(comandos);
        }

        integrantes.ForEach(i => i.MonstrosAtuais
            .Where(m => m.GetMonstro.IsFainted == false)
            .ForEach(m => diceRoller.AddToDicesList(m.Monstro.DiceInstances)));
        
        comandosParaRemover.Clear();

        bool temArena = false;
        foreach (Comando comando in comandos)
        {
            if (comando is ComandoArena)
            {
                if (comando.PodeMeRetirar == false)
                {
                    temArena = true;
                    break;
                }
            }
        }

        if (temArena == false)
        {
            TrocarArena(arenaPadrao);
        }

        countTurno++;
        turno = Turno.Status;
    }

    private void Fim()
    {
        battleUI.SetMenu(BattleUI.Menu.Nenhum);

        iniciarTelaDeFimDeBatalha = true;
    }

    private void AtualizarFlagsDeNpcs()
    {
        foreach (Integrante integrante in integrantes)
        {
            if (idTimePerdedor == integrante.Time)
            {
                if (integrante.TipoIntegrante == TipoIntegrante.Npc)
                {
                    npcAtual.PerdeuABatalha();
                }
            }
        }
    }

    private void TransicaoFimDeBatalha()
    {
        MapMusic.ResumirMusicaDoMapa();

        Transition.GetInstance().DoTransition("FadeIn", 0, () =>
        {
            FinalizarBatalha();

            Transition.GetInstance().DoTransition("FadeOut", 0.2f, () =>
            {
                PauseManager.PermitirInput = true;
            });
        });
    }

    public void FinalizarBatalha()
    {
        NPCManager.IniciandoBatalha = false;
        inBattle = false;

        LimparVariaveis();
        efeitoAtaque.ResetarEfeito();

        GameManager.Instance.FinishBattle();
    }

    private string AddCapturedMonster()
    {
        foreach (Integrante integrante in integrantes)
        {
            if (integrante.Time != idTimePerdedor)
            {
                return integrante.Inventario.MonsterCaptured(monstroCapturado);
            }
        }

        return "Nao achou o integrante para adicionar o monstro!";
    }

    private int GainExperienceFromFaintedMonster(Monster victorious, Monster defeated, int survivedCount, bool isTrainer)
    {
        float baseXpYield = GlobalSettings.Instance.Balance.ExperienceYield(defeated.MonsterData.MonsterRarity);
        float dLevel = defeated.AtributosAtuais.Nivel;
        float pLevel = victorious.AtributosAtuais.Nivel;
        float t = isTrainer ? 1.5f : 1;
        return (int)(((baseXpYield * dLevel) / (5 * survivedCount) * Mathf.Pow((2 * dLevel + 10) / (dLevel + pLevel + 10), 2.5f) + 1) * t);
    }

    private void TrocarArena(ComandoArena novaArena)
    {
        /*
        foreach (Comando arena in comandos)
        {
            if (arena is ComandoArena)
            {
                arena.PodeMeRetirar = true;
            }
        }

        arenaAtual = ScriptableObject.Instantiate(novaArena);
        arenaAtual.name = novaArena.name;

        comandos.Add(arenaAtual);
        */
    }

    private void ExecutarStatusInBattle(Integrante.MonstroAtual monstroAtual)
    {
        Monster monstro = monstroAtual.GetMonstro;

        if (countTurno <= 0)
        {
            for (int n = 0; n < monstro.Status.Count; n++) // Executar os efeitos dos status (dano queimadura)
            {
                if (!monstro.Status[n].GetSeRemover)// Caso status se marque como para remover da lista
                {
                    monstro.Status[n].AplicarModificador(monstro);//aplica os modificadores
                }
            }
        }
        else
        {
            List<StatusEffectBase> indices = new List<StatusEffectBase>();
            for (int n = 0; n < monstro.Status.Count; n++) // Executar os efeitos dos status (dano queimadura)
            {
                if (!monstro.Status[n].GetSeRemover)// Caso status se marque como para remover da lista
                {
                    monstro.Status[n].AplicarModificador(monstro);//aplica os modificadores
                    if (monstro.Status[n].ExecutarInBattle(monstroAtual))
                    {
                        indices.Add(monstro.Status[n]);
                    }

                }
            }
            for (int n = 0; n < indices.Count; n++)
            {
                monstro.RemoverStatus(indices[n]);//remove os modificadores
            }
        }
    }

    public void ExcluirItem(Integrante integrante, ItemHolder itemHolder)
    {
        integrante.Inventario.RemoverItem(itemHolder.Item, 1);
    }

    private void OrdenarListaDeComandos()
    {
        comandos = comandos.OrderByDescending(comando => comando.Velocidade).ToList();
        comandos = comandos.OrderByDescending(comando => comando.Prioridade).ToList();
    }

    #region Comandos
    Comando ChooseItem(Integrante integrante, int indiceMonstroAtual, ItemHolder item)
    {
        //Usar Item
        ComandoDeItem comandoItem = ScriptableObject.Instantiate(item.Item.ComandoNaBatalha);
        comandoItem.name = item.Item.ComandoNaBatalha.name;
        comandoItem.ReceberVariaveis(integrante, item, integrante.MonstrosAtuais[indiceMonstroAtual]);
        return comandoItem;
    }

    Comando ChooseMonsterBall(Integrante integrante, ItemHolder item)
    {
        Integrante.MonstroAtual alvo = null;

        for (int i = 0; i < integrantes.Count; i++)
        {
            if (integrantes[i] != integrante)
            {
                alvo = integrantes[i].MonstrosAtuais[0];
            }
        }

        ComandoDeItem comandoItem = ScriptableObject.Instantiate(item.Item.ComandoNaBatalha);
        comandoItem.name = item.Item.ComandoNaBatalha.name;
        comandoItem.ReceberVariaveis(integrante, item, alvo);
        return comandoItem;
    }

    Comando ChooseAttack(Integrante integrante, AttackHolder attackHolder, int indiceIntegranteAlvo, int indiceMonstroGrupo, int indiceAlvoGrupo)
    {
        //Passar Ataque
        ComandoDeAtaque comandoDeAtaque = ScriptableObject.Instantiate(attackHolder.Attack);
        comandoDeAtaque.name = attackHolder.Attack.name;
        comandoDeAtaque.ReceberVariaves(integrante, integrantes[indiceIntegranteAlvo].MonstrosAtuais[indiceAlvoGrupo], indiceMonstroGrupo);
        return comandoDeAtaque;
    }
    Comando ChooseAttack(Integrante integrante, AttackHolder attackHolder, List<int> indiceIntegranteAlvo, int indiceMonstroGrupo, List<int> indiceAlvoGrupo)
    {
        //Passar Ataque
        ComandoDeAtaque comandoDeAtaque = ScriptableObject.Instantiate(attackHolder.Attack);
        comandoDeAtaque.name = attackHolder.Attack.name;
        List<Integrante.MonstroAtual> listaAlvos = new List<Integrante.MonstroAtual>();
        for (int i = 0; i < indiceAlvoGrupo.Count; i++)
        {
            listaAlvos.Add(integrantes[indiceIntegranteAlvo[i]].MonstrosAtuais[indiceAlvoGrupo[i]]);
        }
        comandoDeAtaque.ReceberVariaves(integrante, listaAlvos, indiceMonstroGrupo);
        return comandoDeAtaque;
    }
    public Comando ChooseAttackNoPP(Integrante integrante, int indiceIntegranteAlvo, int indiceMonstroGrupo, int indiceAlvoGrupo)
    {
        ComandoDeAtaque comandoDeAtaque = ScriptableObject.Instantiate(struggle);
        comandoDeAtaque.name = struggle.name;
        comandoDeAtaque.ReceberVariaves(integrante, integrantes[indiceIntegranteAlvo].MonstrosAtuais[indiceAlvoGrupo], indiceMonstroGrupo);
        return comandoDeAtaque;
    }

    public Comando ChooseChangeMonster(Integrante integrante, int indiceTroca, int indiceMonstroAtual)
    {
        //Trocar Monstro
        ComandoTrocar comandoTrocar = ScriptableObject.Instantiate(trocarMonstro);
        comandoTrocar.name = trocarMonstro.name;
        comandoTrocar.ReceberVariaveis(integrante, indiceTroca, indiceMonstroAtual);
        return comandoTrocar;
    }

    Comando ChooseRun(Integrante integrante)
    {
        //Tentar Correr
        Comando comandoCorrer = ScriptableObject.Instantiate(correr);
        comandoCorrer.name = correr.name;
        comandoCorrer.ReceberVariavel(integrante);

        foreach (var item in integrantes)
        {
            if (item != integrante)
            {
                comandoCorrer.AlvoAcao.Add(item.MonstrosAtuais[0]);
            }
        }
        tentativasCorrer++;
        return comandoCorrer;
    }

    public Comando PlayerEscolheuItem(Integrante integrante, int indiceMonstroAtual, ItemHolder item)
    {
        return ChooseItem(integrante, indiceMonstroAtual, item);
    }
    public Comando PlayerEscolheuMonsterBall(Integrante integrante, ItemHolder item)
    {
        return ChooseMonsterBall(integrante, item);
    }
    public Comando PlayerEscolheuCorrer(Integrante integrante)
    {
        return ChooseRun(integrante);
    }
    public Comando PlayerEscolheuAtaque(AttackHolder attackHolder, int indiceMonstroOrigem, Integrante integranteOrigem, int IndiceintegranteAlvo, int indiceMonstroAtualAlvo)
    {
        return ChooseAttack(integranteOrigem, attackHolder, IndiceintegranteAlvo, indiceMonstroOrigem, indiceMonstroAtualAlvo);
    }
    public Comando PlayerEscolheuAtaque(AttackHolder attackHolder, int indiceMonstroOrigem, Integrante integranteOrigem, List<int> IndiceintegranteAlvo, List<int> indiceMonstroAtualAlvo)
    {
        return ChooseAttack(integranteOrigem, attackHolder, IndiceintegranteAlvo, indiceMonstroOrigem, indiceMonstroAtualAlvo);
    }
    public Comando PlayerEscolheuTroca(Integrante integrante, int indiceTroca, int indiceMonstroAtual)
    {
        return ChooseChangeMonster(integrante, indiceTroca, indiceMonstroAtual);
    }
    #endregion

    public bool TemComandoQueBloqueiaAcao(MonsterInBattle monstro)
    {
        for (int i = 0; i < comandos.Count; i++)
        {
            if (comandos[i].Origem == null)
            {
                continue;
            }

            if (comandos[i].GetMonstroInBattle == monstro && comandos[i].BloqueioAcaoDaOrigem == true)
            {
                return true;
            }
        }

        return false;
    }

    public void SetarAnimacao()
    {
        numeroDeVezesParaRodarAAnimacao = 1;

        IniciarAnimacao();
    }

    public void SetarAnimacao(int numeroDeVezesParaRodarAAnimacao)
    {
        this.numeroDeVezesParaRodarAAnimacao = numeroDeVezesParaRodarAAnimacao;

        IniciarAnimacao();
    }

    private void IniciarAnimacao()
    {
        esperandoAnimacao = true;

        Comando comando = comandos[indiceComando];
        efeitoAtaque.IniciarEfeito(comando);

        if (comando is ComandoDeAtaque && comando.Origem != null)
        {
            ComandoDeAtaque comandoDeAtaque = (ComandoDeAtaque)comando;
            comandoDeAtaque.GetBattleAnimation.TrocarAnimacao(comandoDeAtaque.AnimacaoSprite, comandoDeAtaque.AnimacaoMovimento);

            if (comandoDeAtaque.RodarEfeitoAposOFimDaAnimacao == true)
            {
                comandoDeAtaque.GetBattleAnimation.ExecutarUmMetodoAposOFimDaAnimacaoDoSprite(efeitoAtaque.IniciarAnimacao);
            }
            else
            {
                efeitoAtaque.IniciarAnimacao();
            }
        }
        else
        {
            efeitoAtaque.IniciarAnimacao();
        }
    }

    public void ExecutarAcao()
    {
        Comando comando = comandos[indiceComando];

        comando.Executar(this);

        if (comando is ComandoDeAtaque)
        {
            ComandoDeAtaque comandoDeAtaque = (ComandoDeAtaque)comando;
            for (int i = 0; i < comando.GetMonstro.Attacks.Count; i++)
            {
                if (comando.GetMonstro.Attacks[i].Attack.name == comando.name)
                {                   
                    if (comando.Origem != null)
                    {
                        StartCoroutine(comando.Origem.MonstrosAtuais[comando.IndiceMonstro].MonstroSlotBattle.AtualizarBarras());
                    }
                }
            }
        }

        if (comando is ComandoDeItem)
        {
            for (int i = 0; i < comando.AlvoAcao.Count; i++)
            {
                StartCoroutine(comando.AlvoAcao[i].MonstroSlotBattle.AtualizarBarras());
            }
        }
    }

    public void AnimacaoTomarDano(Integrante.MonstroAtual monstro, bool rodarAnimacaoDano, bool exibirQuantidadeDano, int vidaPerdida, bool ataqueCritico, float modificadorDeAtaque)
    {
        if (rodarAnimacaoDano == true)
        {
            if (ataqueCritico) // caso acerte critico
            {
                monstro.BattleAnimation.AnimacaoTomarDano(sonsGenericos.GetSom("HitCritico"));
            }
            else 
            {
                monstro.BattleAnimation.AnimacaoTomarDano(sonsGenericos.GetSom("Hit"));
            }
        }
        else    // caso erre
        {
            monstro.BattleAnimation.AnimacaoTomarDano(sonsGenericos.GetSom("Miss"));

        }

        if (exibirQuantidadeDano == true)
        {
            EfeitoQuantidadeDano efeitoQuantidadeDano = Instantiate(efeitoQuantidadeDanoBase, quantidadeDanoHolder).GetComponent<EfeitoQuantidadeDano>();
            efeitoQuantidadeDano.transform.position = monstro.BattleAnimation.PosicaoQuantidadeDano.transform.position;

            efeitoQuantidadeDano.Iniciar(vidaPerdida, ataqueCritico);
        }

        if (modificadorDeAtaque >= 2)
        {
            monstro.MonstroSlotBattle.TrocarAnimacao(MonstroSlotBattle.AnimacaoSlot.AtaqueEfetivo);
        }

        StartCoroutine(monstro.MonstroSlotBattle.AtualizarBarras());
    }

    public void FinalizarEsperarAnimacao()
    {
        Comando comando = comandos[indiceComando];

        numeroDeVezesParaRodarAAnimacao--;

        if (numeroDeVezesParaRodarAAnimacao <= 0)
        {
            esperandoAnimacao = false;

            if (comando.Origem != null)
            {
                if ((comando.GetBattleAnimation.AnimacaoAtualSprite != BattleAnimation.AnimacaoSprite.Idle.ToString()) || (comando.GetBattleAnimation.AnimacaoAtualMovimento != BattleAnimation.AnimacaoMovimento.Nenhum.ToString()))
                {
                    comando.GetBattleAnimation.TrocarAnimacao(BattleAnimation.AnimacaoSprite.Idle, BattleAnimation.AnimacaoMovimento.Nenhum);
                }
            }
        }
        else
        {
            if (comando is ComandoDeAtaque)
            {
                ComandoDeAtaque comandoDeAtaque = (ComandoDeAtaque)comando;

                comandoDeAtaque.VerificarAlvoVivo(integrantes);

                if (comandoDeAtaque.AlvoAcao.Count > 0)
                {
                    bool temAlvosValidosParaAcertar = false;

                    for (int i = 0; i < comandoDeAtaque.AlvoAcao.Count; i++)
                    {
                        if (comandoDeAtaque.AlvoComAtaquesValidos[i] == true)
                        {
                            temAlvosValidosParaAcertar = true;
                            break;
                        }
                    }

                    if (temAlvosValidosParaAcertar == true)
                    {
                        IniciarAnimacao();
                    }
                    else
                    {
                        esperandoAnimacao = false;

                        if (comando.Origem != null)
                        {
                            if ((comando.GetBattleAnimation.AnimacaoAtualSprite != BattleAnimation.AnimacaoSprite.Idle.ToString()) || (comando.GetBattleAnimation.AnimacaoAtualMovimento != BattleAnimation.AnimacaoMovimento.Nenhum.ToString()))
                            {
                                comando.GetBattleAnimation.TrocarAnimacao(BattleAnimation.AnimacaoSprite.Idle, BattleAnimation.AnimacaoMovimento.Nenhum);
                            }
                        }
                    }
                }
                else
                {
                    esperandoAnimacao = false;

                    if (comando.Origem != null)
                    {
                        comando.GetBattleAnimation.TrocarAnimacao(BattleAnimation.AnimacaoSprite.Idle, BattleAnimation.AnimacaoMovimento.Nenhum);
                    }
                }
            }
            else
            {
                IniciarAnimacao();
            }
        }
    }

    public void PlayerConseguiCorrer(bool valor)
    {
        if (!valor)
        {
            SetNomeDoIntegrante(comandos[indiceComando].Origem.Nome);
            AbrirDialogo(dialogoNaoConseguiuCorrer);
            return;
        }

        iniciarTelaDeCorrer = true;
    }

    public void CapturarMonstro(bool captured, Monster monstro)
    {
        if (!captured)
        {
            efeitoAtaque.FazerOAlvoSurgir();

            SetNomeDoMonstro(monstro.NickName);
            AbrirDialogo(dialogoMonstroNaoCapturado);
        }
        else
        {
            monstroCapturado = monstro;

            idTimePerdedor = GetIntegrante(monstro).Time;

            Fim();
        }
    }

    public void RemoverComandosMonstroMorto(Monster monstro)
    {
        for (int i = 0; i < comandos.Count; i++)
        {
            if (comandos[i].Origem != null)
            {
                if (comandos[i].GetMonstroInBattle.GetMonstro == monstro)
                {
                    comandos[i].PodeMeRetirar = true;
                }
            }
        }
    }

    public Integrante GetIntegrante(Monster monstro)
    {
        for (int i = 0; i < integrantes.Count; i++)
        {
            for (int y = 0; y < integrantes[i].ListaMonstros.Count; y++)
            {
                if (integrantes[i].ListaMonstros[y].GetMonstro == monstro)
                {
                    return integrantes[i];
                }
            }
        }

        return null;
    }

    public void TrocarMonstro(MonsterInBattle monstro1, MonsterInBattle monstro2, Integrante origem, int indiceOrigem, int indiceTroca, int indice, int indiceMonstroAtual)
    {
        iniciarTrocarMonstro = true;
        foreach (var comando in comandos)
        {
            if(comando.GetMonstroInBattle == monstro1 || comando.GetMonstroInBattle == monstro2)
            {
                comando.PodeMeRetirar = true;
            }
        }
        monstrosParaTrocar.Add(new MonstroParaTrocar(monstro1, monstro2, origem, indiceOrigem, indiceTroca, indice, indiceMonstroAtual));
    }

    public void RodarEfeito(Integrante.MonstroAtual monstro, StatusEffectBase statusEffect, int danoRecebido, bool terminouOEfeito, bool efeitoBloqueouAtaque)
    {
        iniciarRodarEfeitos = true;

        efeitosParaRodar.Add(new EfeitoParaRodar(monstro, statusEffect, danoRecebido, terminouOEfeito, efeitoBloqueouAtaque));
    }

    public void RodarEfeitoAtributo(Integrante.MonstroAtual monstroAtual, StatusEffectDebufAtributo modificadorDeAtributo, bool resetouOModificador)
    {
        iniciarRodarEfeitosAtributos = true;

        efeitosAtributosParaRodar.Add(new EfeitoAtributoParaRodar(monstroAtual, modificadorDeAtributo, resetouOModificador));
    }

    public void RodarEfeitoStatusSecundario(Integrante.MonstroAtual monstroAtual, StatusEffectSecundario statusEffectSecundario)
    {
        iniciarRodarEfeitosSecundarios = true;

        efeitosSecundariosParaRodar.Add(new EfeitoSecundarioParaRodar(monstroAtual, statusEffectSecundario));
    }

    public void MonstroMortoDoJogadorParaTrocar(int indiceIntegrante, int indiceMonstroAtual)
    {
        iniciarTrocarMonstrosMortosDoJogador = true;

        monstrosMortosDoJogadorParaTrocar.Add(new MonstroMortoParaTrocar(indiceIntegrante, indiceMonstroAtual));
    }

    public void MostrarCombatLesson(Sprite iconeElemon, string nomeCombatLesson)
    {
        iniciarMostrarCombatLessons = true;

        combatLessonsParaMostrar.Add(new CombatLessonParaMostrar(iconeElemon, nomeCombatLesson));
    }

    public void DialogoUsouComando(Comando comando)
    {
        SetNomeDoComando(comando.Nome);

        if (comando.Origem != null)
        {
            SetNomeDoMonstro(comando.GetMonstro.NickName);
        }
        else
        {
            SetNomeDoMonstro("Don't Have an Origin");
        }

        AbrirDialogo(dialogoUsouComando);
    }
    public void DialogoUsouItem(Comando comando, Item item)
    {
        SetNomeDoItem(item.Nome);
        SetNomeDoIntegrante(comando.Origem.Nome);

        AbrirDialogo(dialogoUsouItem);
    }

    public void DialogoTirouMonstro(Integrante integrante, Monster monstro)
    {
        SetNomeDoIntegrante(integrante.Nome);
        SetNomeDoMonstro(monstro.NickName);

        AbrirDialogo(dialogosTirouMonstro[Random.Range(0, dialogosTirouMonstro.Length)]);
    }

    public void DialogoJogouMonstro(Integrante integrante, Monster monstro)
    {
        SetNomeDoIntegrante(integrante.Nome);
        SetNomeDoMonstro(monstro.NickName);

        AbrirDialogo(dialogoJogouMonstro);
    }

    public void DialogoStatusTerminou(StatusEffectBase status, Monster monstro)
    {
        SetNomeDoMonstro(monstro.NickName);
        SetNomeDoStatusEffect(status.Nome);

        AbrirDialogo(status.DialogoTerminouEfeito);
    }

    public void DialogoModificouAtributo(StatusEffectDebufAtributo modificadorAtributo, Monster monstro)
    {
        SetNomeDoMonstro(monstro.NickName);
        SetNomeDoAtributo(modificadorAtributo);
        SetNomeDoValorDoAtributo(modificadorAtributo);

        AbrirDialogo(dialogoModificouAtributo);
    }

    public void DialogoResetouAtributo(StatusEffectDebufAtributo modificadorAtributo, Monster monstro)
    {
        SetNomeDoMonstro(monstro.NickName);
        SetNomeDoAtributo(modificadorAtributo);
        SetNomeDoValorDoAtributo(modificadorAtributo);

        AbrirDialogo(dialogoResetouAtributo);
    }

    public void DialogoGanhouExp(Monster monstro, int quantidadeExp)
    {
        SetNomeDoMonstro(monstro.NickName);
        SetQuantidade(quantidadeExp.ToString());

        AbrirDialogo(dialogoGanhouExp);
    }

    public void DialogoAumentouDeNivel(Monster monstro)
    {
        SetNomeDoMonstro(monstro.NickName);
        SetQuantidade(monstro.AtributosAtuais.Nivel.ToString());

        AbrirDialogo(dialogoAumentouDeNivel);
    }

    public void DialogoMonstroDesmaiou(Monster monstro)
    {
        SetNomeDoMonstro(monstro.NickName);

        AbrirDialogo(dialogoMonstroDesmaiou);
    }

    public void DialogoMonstroCapturado(Monster monstro)
    {
        SetNomeDoMonstro(monstro.NickName);

        AbrirDialogo(dialogoMonstroCapturado);
    }

    public void DialogoDerrotouNPC(Integrante npc)
    {
        SetNomeDoIntegrante(npc.Nome);

        AbrirDialogo(dialogoDerrotouNpc);
    }

    public void DialogoMonsterEntryAdicionadaAMonsterBook(MonsterData monstro)
    {
        SetNomeDoMonstro(monstro.GetName);

        AbrirDialogo(dialogoMonsterEntryAdicionada);
    }

    public void DialogoMonstroTransferidoParaBox(Monster monstro, string nomeDaBox)
    {
        SetNomeDoMonstro(monstro.NickName);
        SetNomeDaBox(nomeDaBox);

        AbrirDialogo(dialogoMonstroTransferidoParaBox);
    }

    public void DialogoQuerDarUmNicknameProMonstro(Monster monstro)
    {
        SetNomeDoMonstro(monstro.MonsterData.GetName);

        AbrirDialogo(dialogoQuerDarUmNicknameProMonstro);
    }

    public void DialogoGanhouDinheiro(int quantidade, DialogueObject dialogo)
    {
        SetQuantidade(quantidade.ToString());

        AbrirDialogo(dialogo);
    }

    public void DialogoUsouAtaqueProxTurno(Comando comando)
    {
        ComandoDeAtaque comandoDeAtaque = (ComandoDeAtaque)comando;
        SetNomeDoComando(comando.Nome);
        SetNomeDoMonstro(comando.GetMonstro.NickName);
        AbrirDialogo(comandoDeAtaque.DialogoGolpeParaTurnoSeguinte);
    }

    public void DialogoMonstroComStatusTrancado(ComandoTrocar comando)
    {
        SetNomeDoMonstro(comando.GetMonstro.NickName);
        SetNomeDoIntegrante(comando.Origem.Nome);
        AbrirDialogo(comando.Dialogue);
    }

    public void SetNomeDoComando(string nomeDoComando)
    {
        dialogueUI.SetPlaceholderDeTexto("%comando", nomeDoComando);
    }

    public void SetNomeDoIntegrante(string nomeDoIntegrante)
    {
        dialogueUI.SetPlaceholderDeTexto("%integrante", nomeDoIntegrante);
    }

    public void SetNomeDoMonstro(string nomeDoMonstro)
    {
        dialogueUI.SetPlaceholderDeTexto("%monstro", nomeDoMonstro);
    }

    public void SetNomeDoItem(string nomeDoItem)
    {
        dialogueUI.SetPlaceholderDeTexto("%item", nomeDoItem);
    }

    public void SetNomeDoStatusEffect(string nomeDoStatusEffect)
    {
        dialogueUI.SetPlaceholderDeTexto("%statusEffect", nomeDoStatusEffect);
    }

    public void SetQuantidade(string quantidade)
    {
        dialogueUI.SetPlaceholderDeTexto("%quantidade", quantidade);
    }

    public void SetNomeDaBox(string nomeDaBox)
    {
        dialogueUI.SetPlaceholderDeTexto("%boxName", nomeDaBox);
    }

    public void SetNomeDoAtributo(StatusEffectDebufAtributo modificadorAtributo)
    {
        switch (modificadorAtributo.GetAtributo)
        {
            case Modificador.Atributo.ataque:
                dialogueUI.SetPlaceholderDeTexto("%atributo", "Attack");
                break;

            case Modificador.Atributo.spAtaque:
                dialogueUI.SetPlaceholderDeTexto("%atributo", "Special Attack");
                break;

            case Modificador.Atributo.defesa:
                dialogueUI.SetPlaceholderDeTexto("%atributo", "Defense");
                break;

            case Modificador.Atributo.spDefesa:
                dialogueUI.SetPlaceholderDeTexto("%atributo", "Special Defense");
                break;

            case Modificador.Atributo.velocidade:
                dialogueUI.SetPlaceholderDeTexto("%atributo", "Speed");
                break;
        }
    }

    public void SetNomeDoValorDoAtributo(StatusEffectDebufAtributo modificadorAtributo)
    {
        if (modificadorAtributo.GetvalorDebuff >= 0)
        {
            dialogueUI.SetPlaceholderDeTexto("%modificador", "increased");
        }
        else
        {
            dialogueUI.SetPlaceholderDeTexto("%modificador", "decreased");
        }
    }

    public void AbrirDialogo(DialogueObject dialogo)
    {
        dialogoAberto = true;

        dialogueActivator.ShowDialogue(dialogo, dialogueUI);

        StartCoroutine(DialogoAberto(dialogueUI));
    }

    private void FecharDialogo()
    {
        dialogoAberto = false;
    }

    private IEnumerator DialogoAberto(DialogueUI dialogueUI)
    {
        while (dialogueUI.IsOpen == true)
        {
            yield return null;
        }

        FecharDialogo();
    }

    private IEnumerator TrocarMonstrosDaLista()
    {
        for (int i = 0; i < monstrosParaTrocar.Count; i++)
        {
            monstrosParaTrocar[i].Origem.ListaMonstros[monstrosParaTrocar[i].Indice] = monstrosParaTrocar[i].Monstro2;
            monstrosParaTrocar[i].Origem.ListaMonstros[monstrosParaTrocar[i].IndiceTroca] = monstrosParaTrocar[i].Monstro1;
            monstrosParaTrocar[i].Origem.MonstrosAtuais[monstrosParaTrocar[i].IndiceMonstroAtual].Monstro = monstrosParaTrocar[i].Monstro2;
            monstrosParaTrocar[i].Monstro1.EntrouEmCombate = true;
            monstrosParaTrocar[i].Monstro2.EntrouEmCombate = true;

            if (monstrosParaTrocar[i].Monstro1.GetMonstro.IsFainted == false)
            {
                esperandoAnimacao = true;

                monstrosParaTrocar[i].Origem.MonstrosAtuais[monstrosParaTrocar[i].IndiceMonstroAtual].BattleAnimation.TrocarAnimacao(BattleAnimation.AnimacaoSprite.Idle, BattleAnimation.AnimacaoMovimento.Desaparecendo);
                monstrosParaTrocar[i].Origem.MonstrosAtuais[monstrosParaTrocar[i].IndiceMonstroAtual].BattleAnimation.ExecutarUmMetodoAposOFimDaAnimacaoDoMovimento(() => esperandoAnimacao = false);

                if (monstrosParaTrocar[i].Origem.TipoIntegrante == TipoIntegrante.Jogador)
                {
                    DialogoTirouMonstro(monstrosParaTrocar[i].Origem, monstrosParaTrocar[i].Monstro1.GetMonstro);
                }
            }

            while (esperandoAnimacao == true || dialogoAberto == true)
            {
                yield return null;
            }

            diceRoller.DestroyDices(monstrosParaTrocar[i].Monstro1.DiceInstances);
            monstrosParaTrocar[i].Monstro1.DiceInstances = null;

            monstrosParaTrocar[i].Origem.MonstrosAtuais[monstrosParaTrocar[i].IndiceMonstroAtual].AtualizarMonstro();

            esperandoAnimacao = true;

            monstrosParaTrocar[i].Origem.MonstrosAtuais[monstrosParaTrocar[i].IndiceMonstroAtual].BattleAnimation.TrocarAnimacao(BattleAnimation.AnimacaoSprite.Idle, BattleAnimation.AnimacaoMovimento.Surgindo);
            monstrosParaTrocar[i].Origem.MonstrosAtuais[monstrosParaTrocar[i].IndiceMonstroAtual].BattleAnimation.ExecutarUmMetodoAposOFimDaAnimacaoDoMovimento(() => esperandoAnimacao = false);

            DialogoJogouMonstro(monstrosParaTrocar[i].Origem, monstrosParaTrocar[i].Monstro2.GetMonstro);

            monstrosParaTrocar[i].Origem.MonstrosAtuais[monstrosParaTrocar[i].IndiceMonstroAtual].BattleAnimation.TocarSom(monstrosParaTrocar[i].Origem.MonstrosAtuais[monstrosParaTrocar[i].IndiceMonstroAtual].GetMonstro.MonsterData.AudioEntrada);

            while (esperandoAnimacao == true || dialogoAberto == true)
            {
                yield return null;
            }
        }

        GenerateDicesAndPositions();

        monstrosParaTrocar.Clear();
        trocandoMonstro = false;
    }

    private IEnumerator SurgirMonstrosNoInicio()
    {
        trocandoMonstro = true;

        foreach (Integrante integrante in integrantes)
        {

            if (integrante.TipoIntegrante == TipoIntegrante.Selva)
            {
                foreach (Integrante.MonstroAtual monstroAtual in integrante.MonstrosAtuais)
                {
                    monstroAtual.BattleAnimation.TrocarAnimacao(BattleAnimation.AnimacaoSprite.Idle, BattleAnimation.AnimacaoMovimento.Nenhum);
                }
            }
            else
            {
                foreach (Integrante.MonstroAtual monstroAtual in integrante.MonstrosAtuais)
                {
                    monstroAtual.BattleAnimation.TrocarAnimacao(BattleAnimation.AnimacaoSprite.Vazio, BattleAnimation.AnimacaoMovimento.Nenhum);
                }
            }
        }

        yield return null;

        yield return new WaitUntil(() => Transition.GetInstance().FazendoTransicao == false);

        foreach (Integrante integrante in integrantes)
        {
            if (integrante.TipoIntegrante != TipoIntegrante.Selva)
            {
                foreach (Integrante.MonstroAtual monstroAtual in integrante.MonstrosAtuais)
                {
                    if (monstroAtual.GetMonstro.IsFainted == false)
                    {
                        esperandoAnimacao = true;

                        monstroAtual.BattleAnimation.TrocarAnimacao(BattleAnimation.AnimacaoSprite.Idle, BattleAnimation.AnimacaoMovimento.Surgindo);
                        monstroAtual.BattleAnimation.ExecutarUmMetodoAposOFimDaAnimacaoDoMovimento(() => esperandoAnimacao = false);

                        monstroAtual.BattleAnimation.TocarSom(monstroAtual.GetMonstro.MonsterData.AudioEntrada);

                        yield return new WaitUntil(() => esperandoAnimacao == false);
                    }
                    else
                    {
                        monstroAtual.BattleAnimation.TrocarAnimacao(BattleAnimation.AnimacaoSprite.Morrendo, 1, BattleAnimation.AnimacaoMovimento.Morrendo, 1);
                    }
                }
            }
        }

        if(BergamotaLibrary.Flags.GetFlag(listaDeFlagsTutorialBatalha.name, flagTutorialBatalha) == true)
        {
            AbrirDialogo(dialogoTutorialBatalha);

            yield return new WaitUntil(() => dialogoAberto == false);
            yield return new WaitUntil(() => battleUI.MenuTutorialDeBatalha.IsViewOpenned() == false);

            BergamotaLibrary.Flags.SetFlag(listaDeFlagsTutorialBatalha.name, flagTutorialBatalha, false);
        }

        trocandoMonstro = false;
    }

    private IEnumerator RodarAnimacaoDeMorteDosMonstros()
    {
        int monstrosMorrendo = monstrosParaMorrer.Count;

        for (int i = 0; i < monstrosParaMorrer.Count; i++)
        {
            if (monstrosParaMorrer[i].BattleAnimation.AnimacaoAtualSprite != BattleAnimation.AnimacaoSprite.Morrendo.ToString())
            {
                monstrosParaMorrer[i].BattleAnimation.TrocarAnimacao(BattleAnimation.AnimacaoSprite.Morrendo, BattleAnimation.AnimacaoMovimento.Morrendo);
                monstrosParaMorrer[i].BattleAnimation.ExecutarUmMetodoAposOFimDaAnimacaoDoMovimento(() => monstrosMorrendo--);

                monstrosParaMorrer[i].BattleAnimation.TocarSom(monstrosParaMorrer[i].GetMonstro.MonsterData.AudioClipMorrendo);


            }
            else
            {
                monstrosMorrendo--;
            }
        }

        while (monstrosMorrendo > 0)
        {
            yield return null;
        }

        monstrosParaMorrer.Clear();
        rodandoAnimacaoDeMorte = false;
    }

    private IEnumerator RodarEfeitosDeStatus()
    {
        while (esperandoAnimacao == true)
        {
            yield return null;
        }

        Debug.Log("Efeitos: " + efeitosParaRodar.Count);

        foreach (EfeitoParaRodar efeito in efeitosParaRodar)
        {
            Debug.Log("Rodou o efeito de " + efeito.StatusEffect.Nome + " do monstro " + efeito.Monstro.GetMonstro.NickName);

            if (efeito.Monstro.BattleAnimation.AnimacaoAtualSprite == BattleAnimation.AnimacaoSprite.Morrendo.ToString())
            {
                continue;
            }

            if (efeito.TerminouOEfeito == false)
            {
                switch (efeito.StatusEffect.TipoDeEfeitoVisual)
                {
                    case StatusEffectBase.TipoDeEfeitoVisualEnum.TintEffect:
                        efeito.Monstro.BattleAnimation.SetTintEffect(efeito.StatusEffect.CorDoEfeito, efeito.StatusEffect.VelocidadeDoEfeito);

                        esperandoAnimacao = true;
                        efeito.Monstro.BattleAnimation.EfeitoTerminou.AddListener(() => esperandoAnimacao = false);
                        break;

                    case StatusEffectBase.TipoDeEfeitoVisualEnum.TintEffectSlow:
                        efeito.Monstro.BattleAnimation.SetTintEffectSlow(efeito.StatusEffect.CorDoEfeito, efeito.StatusEffect.VelocidadeDoEfeito);

                        esperandoAnimacao = true;
                        efeito.Monstro.BattleAnimation.EfeitoTerminou.AddListener(() => esperandoAnimacao = false);
                        break;

                    case StatusEffectBase.TipoDeEfeitoVisualEnum.TintSolidEffect:
                        efeito.Monstro.BattleAnimation.SetTintSolidEffect(efeito.StatusEffect.CorDoEfeito, efeito.StatusEffect.VelocidadeDoEfeito);

                        esperandoAnimacao = true;
                        efeito.Monstro.BattleAnimation.EfeitoTerminou.AddListener(() => esperandoAnimacao = false);
                        break;

                    case StatusEffectBase.TipoDeEfeitoVisualEnum.TintSolidEffectSlow:
                        efeito.Monstro.BattleAnimation.SetTintSolidEffectSlow(efeito.StatusEffect.CorDoEfeito, efeito.StatusEffect.VelocidadeDoEfeito);

                        esperandoAnimacao = true;
                        efeito.Monstro.BattleAnimation.EfeitoTerminou.AddListener(() => esperandoAnimacao = false);
                        break;
                }

                while (esperandoAnimacao == true)
                {
                    yield return null;
                }

                efeito.Monstro.BattleAnimation.EfeitoTerminou.RemoveAllListeners();

                if (efeito.DanoRecebido >= 0)
                {
                    esperandoAnimacao = true;

                    AnimacaoTomarDano(efeito.Monstro, true, true, efeito.DanoRecebido, false, 0);
                    efeito.Monstro.BattleAnimation.ExecutarUmMetodoAposOFimDaAnimacaoDoMovimento(() => esperandoAnimacao = false);

                    while (esperandoAnimacao == true)
                    {
                        yield return null;
                    }
                }
                else
                {
                    StartCoroutine(efeito.Monstro.MonstroSlotBattle.AtualizarBarras());
                }

                if (efeito.EfeitoBloqueouAtaque == true)
                {
                    if (efeito.StatusEffect.DialogoBloqueouComando != null)
                    {
                        SetNomeDoMonstro(efeito.Monstro.GetMonstro.NickName);
                        SetNomeDoStatusEffect(efeito.StatusEffect.Nome);

                        AbrirDialogo(efeito.StatusEffect.DialogoBloqueouComando);

                        while (dialogoAberto == true)
                        {
                            yield return null;
                        }
                    }
                }
            }
            else
            {
                StartCoroutine(efeito.Monstro.MonstroSlotBattle.AtualizarBarras());

                DialogoStatusTerminou(efeito.StatusEffect, efeito.Monstro.GetMonstro);

                while (dialogoAberto == true)
                {
                    yield return null;
                }
            }

            if (efeito.Monstro.GetMonstro.IsFainted == true)
            {
                Debug.Log("O monstro " + efeito.Monstro.GetMonstro.NickName + " morreu para o efeito " + efeito.StatusEffect.Nome + ".");
                monstrosParaMorrer.Add(efeito.Monstro);
                yield return RodarAnimacaoDeMorteDosMonstros();
            }
        }

        efeitosParaRodar.Clear();
        rodandoEfeitos = false;
    }

    private IEnumerator RodarEfeitosDeAtributos()
    {
        while (esperandoAnimacao == true)
        {
            yield return null;
        }

        foreach (EfeitoAtributoParaRodar efeitoAtributo in efeitosAtributosParaRodar)
        {
            if (efeitoAtributo.ResetouOModificador == false)
            {
                DialogoModificouAtributo(efeitoAtributo.ModificadorDeAtributo, efeitoAtributo.MonstroAtual.GetMonstro);
            }
            else
            {
                DialogoResetouAtributo(efeitoAtributo.ModificadorDeAtributo, efeitoAtributo.MonstroAtual.GetMonstro);
            }

            while (dialogoAberto == true)
            {
                yield return null;
            }
        }

        efeitosAtributosParaRodar.Clear();
        rodandoEfeitosAtributos = false;
    }

    private IEnumerator RodarEfeitosDeStatusSecundarios()
    {
        while (esperandoAnimacao == true)
        {
            yield return null;
        }

        Debug.Log("Efeitos Secundarios: " + efeitosSecundariosParaRodar.Count);

        foreach (EfeitoSecundarioParaRodar efeito in efeitosSecundariosParaRodar)
        {
            Debug.Log("Rodou o efeito de " + efeito.StatusEffectSecundario.name + " do monstro " + efeito.Monstro.GetMonstro.NickName);

            if (efeito.Monstro.BattleAnimation.AnimacaoAtualSprite == BattleAnimation.AnimacaoSprite.Morrendo.ToString())
            {
                continue;
            }

            switch (efeito.StatusEffectSecundario.TipoDeEfeitoVisual)
            {
                case StatusEffectSecundario.TipoDeEfeitoVisualEnum.TintEffect:
                    efeito.Monstro.BattleAnimation.SetTintEffect(efeito.StatusEffectSecundario.CorDoEfeito, efeito.StatusEffectSecundario.VelocidadeDoEfeito);

                    esperandoAnimacao = true;
                    efeito.Monstro.BattleAnimation.EfeitoTerminou.AddListener(() => esperandoAnimacao = false);
                    break;

                case StatusEffectSecundario.TipoDeEfeitoVisualEnum.TintEffectSlow:
                    efeito.Monstro.BattleAnimation.SetTintEffectSlow(efeito.StatusEffectSecundario.CorDoEfeito, efeito.StatusEffectSecundario.VelocidadeDoEfeito);

                    esperandoAnimacao = true;
                    efeito.Monstro.BattleAnimation.EfeitoTerminou.AddListener(() => esperandoAnimacao = false);
                    break;

                case StatusEffectSecundario.TipoDeEfeitoVisualEnum.TintSolidEffect:
                    efeito.Monstro.BattleAnimation.SetTintSolidEffect(efeito.StatusEffectSecundario.CorDoEfeito, efeito.StatusEffectSecundario.VelocidadeDoEfeito);

                    esperandoAnimacao = true;
                    efeito.Monstro.BattleAnimation.EfeitoTerminou.AddListener(() => esperandoAnimacao = false);
                    break;

                case StatusEffectSecundario.TipoDeEfeitoVisualEnum.TintSolidEffectSlow:
                    efeito.Monstro.BattleAnimation.SetTintSolidEffectSlow(efeito.StatusEffectSecundario.CorDoEfeito, efeito.StatusEffectSecundario.VelocidadeDoEfeito);

                    esperandoAnimacao = true;
                    efeito.Monstro.BattleAnimation.EfeitoTerminou.AddListener(() => esperandoAnimacao = false);
                    break;
            }

            yield return new WaitUntil(() => esperandoAnimacao == false);

            efeito.Monstro.BattleAnimation.EfeitoTerminou.RemoveAllListeners();

            StartCoroutine(efeito.Monstro.MonstroSlotBattle.AtualizarBarras());

            SetNomeDoMonstro(efeito.Monstro.GetMonstro.NickName);
            AbrirDialogo(efeito.StatusEffectSecundario.DialogoDoEfeito);

            yield return new WaitUntil(() => dialogoAberto == false);
        }

        efeitosSecundariosParaRodar.Clear();
        rodandoEfeitosSecundarios = false;
    }

    private IEnumerator MostrarCombatLessons()
    {
        yield return new WaitUntil(() => esperandoAnimacao == false);

        foreach(CombatLessonParaMostrar combatLesson in combatLessonsParaMostrar)
        {
            esperandoAnimacao = true;

            battleUI.JanelaCombatLesson.MostrarCombatLesson(combatLesson.IconeElemon, combatLesson.NomeCombatLesson, () => esperandoAnimacao = false);

            yield return new WaitUntil(() => esperandoAnimacao == false);
        }

        combatLessonsParaMostrar.Clear();
        mostrandoCombatLessons = false;
    }

    private IEnumerator TrocarMonstrosMortosDoJogador()
    {
        List<MonsterInBattle> monstrosQueVaoSerTrocados = new List<MonsterInBattle>();

        foreach (MonstroMortoParaTrocar monstro in monstrosMortosDoJogadorParaTrocar)
        {
            battleUI.MenuTrocarMonstros.IniciarMenu(integrantes[monstro.IndiceIntegrante].ListaMonstros, integrantes[monstro.IndiceIntegrante].MonstrosAtuais, monstrosQueVaoSerTrocados, MenuTrocarMonstros.ModoMenu.MonstroMorreu);

            if (battleUI.MenuTrocarMonstros.SlotsAtivos() < 1)
            {
                battleUI.MenuTrocarMonstros.FecharMenu();
                battleUI.SetMenu(BattleUI.Menu.Nenhum);
                continue;
            }

            esperandoEscolherMonstro = true;

            while (esperandoEscolherMonstro == true)
            {
                yield return null;
            }

            monstrosQueVaoSerTrocados.Add(integrantes[monstro.IndiceIntegrante].ListaMonstros[indiceMonstroEscolhido]);

            battleUI.MenuTrocarMonstros.FecharMenu();
            battleUI.SetMenu(BattleUI.Menu.Nenhum);

            Comando comando = PlayerEscolheuTroca(integrantes[monstro.IndiceIntegrante], indiceMonstroEscolhido, monstro.IndiceMonstroAtual);
            comando.Executar(this);
        }
        
        GenerateDicesAndPositions();

        monstrosMortosDoJogadorParaTrocar.Clear();
        trocandoMonstrosMortosDoJogador = false;
    }

    public void EscolheuMonstroMortoDoJogadorParaTrocar(int indice)
    {
        esperandoEscolherMonstro = false;
        indiceMonstroEscolhido = indice;
    }

    public IEnumerator TrocarMonstrosAtuaisMortosDoJogador(Integrante integrante)
    {
        List<MonsterInBattle> monstrosQueVaoSerTrocados = new List<MonsterInBattle>();

        for (int i = 0; i < integrante.MonstrosAtuais.Count; i++)
        {
            if (integrante.MonstrosAtuais[i].GetMonstro.IsFainted == true)
            {
                for (int y = 0; y < integrante.ListaMonstros.Count; y++)
                {
                    bool podeTrocar = true;

                    for (int z = 0; z < integrante.MonstrosAtuais.Count; z++)
                    {
                        if (integrante.ListaMonstros[y] == integrante.MonstrosAtuais[z].Monstro)
                        {
                            podeTrocar = false;
                        }
                    }

                    for (int z = 0; z < monstrosQueVaoSerTrocados.Count; z++)
                    {
                        if (integrante.ListaMonstros[y] == monstrosQueVaoSerTrocados[z])
                        {
                            podeTrocar = false;
                        }
                    }

                    if ((podeTrocar == true) && (integrante.ListaMonstros[y].GetMonstro.IsFainted == false))
                    {
                        monstrosQueVaoSerTrocados.Add(integrante.ListaMonstros[y]);

                        Comando comando = PlayerEscolheuTroca(integrante, y, i);
                        comando.Executar(this);

                        break;
                    }
                }
            }
            else
            {
                if (integrante.MonstrosAtuais[i].BattleAnimation.AnimacaoAtualSprite == "Morrendo")
                {
                    integrante.MonstrosAtuais[i].BattleAnimation.TrocarAnimacao(BattleAnimation.AnimacaoSprite.Idle, BattleAnimation.AnimacaoMovimento.Surgindo);
                    StartCoroutine(integrante.MonstrosAtuais[i].MonstroSlotBattle.AtualizarBarras());
                }
            }
        }

        yield return TrocarMonstrosDaLista();
        iniciarTrocarMonstro = false;
    }

    private IEnumerator TelaFimDeBatalha()
    {
        //Pega integrantes vitoriosos e seleciona os monstros que entraram em combate e não morreram
        List<Monster> survivingMonsters = integrantes.Where(x => x.Time != idTimePerdedor)
            .SelectMany(x => x.ListaMonstros)
            .Where(x => x.EntrouEmCombate)
            .Select(x => x.GetMonstro)
            .Where(x => x.IsFainted == false).ToList();

        //Pega integrantes derrotados e seleciona os monstros que entraram em combate e morreram
        List<Monster> defeatedMonsters = integrantes.Where(x => x.Time == idTimePerdedor)
            .SelectMany(x => x.ListaMonstros)
            .Where(x => x.EntrouEmCombate)
            .Select(x => x.GetMonstro)
            .Where(x => x.IsFainted).ToList();

        Integrante integranteDerrotado = null;

        for (int i = 0; i < integrantes.Count; i++)
        {
            if (integrantes[i].Time == idTimePerdedor)
            {
                integranteDerrotado = integrantes[i];
            }
        }

        if(integranteDerrotado.TipoIntegrante == TipoIntegrante.Jogador)
        {
            Integrante npcVitorioso = null;

            for (int i = 0; i < integrantes.Count; i++)
            {
                if (integrantes[i].Time != idTimePerdedor)
                {
                    npcVitorioso = integrantes[i];
                    break;
                }
            }

            switch (tipoBatalha)
            {
                case TipoBatalha.MonstroSelvagem:
                    BlackOut.Instance.IniciarBlackOut();
                    break;

                case TipoBatalha.Npc:
                    BlackOut.Instance.IniciarBlackOutComNPC(npcVitorioso.Nome);
                    break;
            }
            
            yield break;
        }

        //Troca a musica para a musica de vitoria caso o jogador tenha vencido
        switch (tipoBatalha)
        {
            case TipoBatalha.MonstroSelvagem:
                MusicManager.instance.TocarMusica(musicaVitoriaMonstroSelvagem);
                break;

            case TipoBatalha.Npc:
            case TipoBatalha.Player:
                MusicManager.instance.TocarMusica(musicaVitoriaNPC);
                break;
        }

        bool isTrainerBattle = false;
        switch (tipoBatalha)
        {
            case TipoBatalha.MonstroSelvagem:

                if (monstroCapturado == null)
                {
                    DialogoMonstroDesmaiou(integranteDerrotado.ListaMonstros[0].GetMonstro);
                }
                else
                {
                    DialogoMonstroCapturado(monstroCapturado);
                }
                break;

            case TipoBatalha.Npc:
                isTrainerBattle = true;

                DialogoDerrotouNPC(integranteDerrotado);
                break;
        }

        yield return new WaitUntil(() => dialogoAberto == false);

        int dinheiroGanho = 0;

        foreach(Integrante integrante in integrantes)
        {
            if(integrante.TipoIntegrante == TipoIntegrante.Npc && integrante.Time == idTimePerdedor)
            {
                if(integrante.DinheiroPelaVitoria > 0)
                {
                    dinheiroGanho += integrante.DinheiroPelaVitoria;
                }
            }
        }

        if(dinheiroGanho > 0)
        {
            
            if (PlayerData.Instance.Inventario.HasMagnet())
            {
                dinheiroGanho *= 2;
                Debug.Log("Player has magnet and earned double the coins amount");
                DialogoGanhouDinheiro(dinheiroGanho, dialogoGanhouDinheiroComMagnet);
            }
            else
            {
                DialogoGanhouDinheiro(dinheiroGanho, dialogoGanhouDinheiro);
            }
            
            yield return new WaitUntil(() => dialogoAberto == false);

            PlayerData.Instance.Inventario.Dinheiro += dinheiroGanho;
        }

        foreach (Monster monster in survivingMonsters)
        {
            int xpEarned = 0;
            int xpParaSomar = 0;

            Integrante integranteAtual = null;
            Integrante.MonstroAtual monstroAtual = null;

            bool aumentouDeNivel = false;

            integranteAtual = GetIntegrante(monster);

            monstroParaAprenderOAtaque = monster;

            if (integranteAtual.TipoIntegrante != TipoIntegrante.Jogador)
            {
                continue;
            }

            foreach (var enemyMonster in defeatedMonsters)
            {
                xpEarned += GainExperienceFromFaintedMonster(monster, enemyMonster, survivingMonsters.Count, isTrainerBattle);
            }

            if (monstroCapturado != null)
            {
                xpEarned += GainExperienceFromFaintedMonster(monster, monstroCapturado, survivingMonsters.Count, isTrainerBattle);
            }
            if (xpExtraComerFruta)
            {
                xpEarned = (int)(xpEarned * 1.2f);
            }
            DialogoGanhouExp(monster, xpEarned);

            yield return new WaitUntil(() => dialogoAberto == false);

            while (xpEarned > 0)
            {
                monstroParaAprenderOAtaque = null;
                ataqueParaAprenderNoNivel = null;

                xpParaSomar = xpEarned;

                if (xpEarned > monster.AtributosAtuais.ExpParaOProxNivel())
                {
                    xpParaSomar = monster.AtributosAtuais.ExpParaOProxNivel();
                }

                for (int i = 0; i < integranteAtual.MonstrosAtuais.Count; i++)
                {
                    if (integranteAtual.MonstrosAtuais[i].GetMonstro == monster)
                    {
                        monstroAtual = integranteAtual.MonstrosAtuais[i];
                    }
                }

                battleUI.JanelaDeAtributosDoLevelUp.IniciarAtributos(monster.AtributosAtuais);

                aumentouDeNivel = monster.LevelUp(xpParaSomar);

                if (monstroAtual != null)
                {
                    if (aumentouDeNivel == true)
                    {
                        yield return monstroAtual.MonstroSlotBattle.BarraExp.EncherABarra();

                        if (monster.AtributosAtuais.Nivel < MonsterAttributes.nivelMax)
                        {
                            monstroAtual.MonstroSlotBattle.BarraExp.AtualizarBarra(0);
                        }
                        else
                        {
                            monstroAtual.MonstroSlotBattle.BarraExp.AtualizarBarra(monster);
                        }
                    }
                    else
                    {
                        yield return monstroAtual.MonstroSlotBattle.BarraExp.AumentarExp(monster);
                    }
                }

                if (aumentouDeNivel == true)
                {
                    DialogoAumentouDeNivel(monster);

                    if (monstroAtual != null)
                    {
                        monstroAtual.MonstroSlotBattle.AtualizarInformacoes();

                        battleUI.JanelaDeAtributosDoLevelUp.AbrirJanela(monster.AtributosAtuais);
                    }

                    yield return new WaitUntil(() => dialogoAberto == false);

                    battleUI.JanelaDeAtributosDoLevelUp.FecharJanela();

                    ataqueParaAprenderNoNivel = monster.VerificarSePodeAprenderUmAtaqueNoNivelAtual();

                    if (ataqueParaAprenderNoNivel != null)
                    {
                        battleUI.TelaAprenderAtaque.VerificarAprenderAtaqueLevelUp(monster, ataqueParaAprenderNoNivel);
                    }

                    yield return new WaitUntil(() => dialogoAberto == false);
                    yield return new WaitUntil(() => battleUI.TelaAprenderAtaque.IsViewOpenned() == false);

                    CombatLesson combatLessonParaAprenderNoNivel = monster.VerificarSePodeAprenderUmCombatLessonNoNivelAtual();

                    if(combatLessonParaAprenderNoNivel != null)
                    {
                        monster.AddCombatLesson(combatLessonParaAprenderNoNivel);

                        SetNomeDoMonstro(monster.NickName);
                        dialogueUI.SetPlaceholderDeTexto("%move", combatLessonParaAprenderNoNivel.Nome);

                        AbrirDialogo(dialogoAprendeuCombatLesson);

                        yield return new WaitUntil(() => dialogoAberto == false);
                    }

                    (UpgradesPerLevel.DiceImprovement tipoMelhoria, DiceType dado1, DiceType dado2) = monster.VerificarMelhoriaDadoNoNivelAtual();

                    switch(tipoMelhoria)
                    {
                        case UpgradesPerLevel.DiceImprovement.New:

                            SetNomeDoMonstro(monster.NickName);
                            dialogueUI.SetPlaceholderDeTexto("%dado1", dado1.ToString());

                            AbrirDialogo(dialogoGanhouUmDado);

                            yield return new WaitUntil(() => dialogoAberto == false);
                            
                            break;

                        case UpgradesPerLevel.DiceImprovement.Better:

                            SetNomeDoMonstro(monster.NickName);
                            dialogueUI.SetPlaceholderDeTexto("%dado1", dado1.ToString());
                            dialogueUI.SetPlaceholderDeTexto("%dado2", dado2.ToString());

                            AbrirDialogo(dialogoMelhorouUmDado);

                            yield return new WaitUntil(() => dialogoAberto == false);

                            break;
                    }
                }

                xpEarned -= xpParaSomar;
            }

            //Retirar status que so ficam na batalha
            List<StatusEffectBase> statusParaRemover = new List<StatusEffectBase>();
            for (int i = 0; i < monster.Status.Count; i++)
            {
                if (monster.Status[i].SairDoCombate(monster))
                    statusParaRemover.Add(monster.Status[i]);
            }

            for (int n = 0; n < statusParaRemover.Count; n++)
            {
                monster.Status.Remove(statusParaRemover[n]);
            }

            monster.AtributosAtuais.LimparModificadores();
            monster.RecuperarManaPorcentagem(50);//monstro recupera metade da mana ao fim da batalha
        }

        if (monstroCapturado != null)
        {
            bool jaTinhaSidoCapturado = PlayerData.MonsterBook.MonsterEntries[monstroCapturado.MonsterData.ID].WasCaptured;

            string nomeDaBox = AddCapturedMonster();

            DialogoQuerDarUmNicknameProMonstro(monstroCapturado);

            yield return new WaitUntil(() => dialogoAberto == false);
            yield return new WaitUntil(() => battleUI.JanelaTrocarNickname.IsViewOpenned() == false);

            if (jaTinhaSidoCapturado == false)
            {
                DialogoMonsterEntryAdicionadaAMonsterBook(monstroCapturado.MonsterData);

                yield return new WaitUntil(() => dialogoAberto == false);

                battleUI.AbrirMenuMonsterEntry(monstroCapturado.MonsterData);

                yield return new WaitUntil(() => battleUI.MenuMonsterEntryController.IsViewOpenned() == false);
            }

            if (nomeDaBox != string.Empty)
            {
                DialogoMonstroTransferidoParaBox(monstroCapturado, nomeDaBox);

                yield return new WaitUntil(() => dialogoAberto == false);
            }
        }
        foreach (var Integrante in integrantes)
        {
            foreach (var Monstro in Integrante.ListaMonstros)
            {
                Monstro.GetMonstro.BuffDano = false;
            }
        }

        AtualizarFlagsDeNpcs();

        eventoVenceuABatalha?.Invoke();

        TransicaoFimDeBatalha();
    }

    private IEnumerator TelaDeCorrer()
    {
        SetNomeDoIntegrante(comandos[indiceComando].Origem.Nome);
        AbrirDialogo(dialogoCorreuDaBatalha);

        yield return new WaitUntil(() => dialogoAberto == false);

        TransicaoFimDeBatalha();
    }

    public void AbrirTelaAprenderAtaque()
    {
        battleUI.TelaAprenderAtaque.AbrirTelaAprenderAtaque(ataqueParaAprenderNoNivel);
    }

    public void AbrirTelaNickname()
    {
        battleUI.JanelaTrocarNickname.IniciarMenu(monstroCapturado);
    }

    public void AbrirMenuTutorialDeBatalha()
    {
        battleUI.MenuTutorialDeBatalha.IniciarMenu();
    }
}

[System.Serializable]
public class MonsterInBattle
{
    //Variaveis
    [SerializeField] private Monster monstro;
    [SerializeField] private bool entrouEmcombate;
    [SerializeField] private List<int> diceResults = new List<int>();
    [SerializeField] private List<GameObject> diceInstances;
    [SerializeField] private bool criticoGarantido;

    //Getters
    public Monster GetMonstro => monstro;
    public bool EntrouEmCombate
    {
        get => entrouEmcombate;
        set => entrouEmcombate = value;
    }

    public List<int> DiceResults
    {
        get => diceResults;
        set => diceResults = value;
    }

    public List<GameObject> DiceInstances
    {
        get => diceInstances;
        set => diceInstances = value;
    }

    public bool CriticoGarantido
    {
        get => criticoGarantido;
        set => criticoGarantido = value;
    }

    //Construtores
    public MonsterInBattle(Monster monster)
    {
        this.monstro = monster;
    }

    #region DanoEAfins

    public void TomarAtaqueAtributo(Integrante.MonstroAtual monstroAtual, StatusEffectDebufAtributo statusEffectDebufAtributo, bool passaComTempo, int numeroRounds)
    {
        monstro.AtributosAtuais.ReceberModificadorStatus(statusEffectDebufAtributo.GetAtributo, statusEffectDebufAtributo.GetvalorDebuff, passaComTempo, numeroRounds);

        BattleManager.Instance.RodarEfeitoAtributo(monstroAtual, statusEffectDebufAtributo, false);
    }

    public (float dano, bool acertou) TomarAtaque(int atributoDeDano,ComandoDeAtaque comando, Integrante.MonstroAtual monstroAtual, bool rodarAnimacaoDano, bool exibirQuantidadeDano,bool usarAtributoDefesa)
    {
        if (monstro.VerificarSeVaiTomarAtaque(comando.AttackData.ChanceAcerto))//Verifica se acertou o monstro alvo
        {
            //Calculo Dano
            int multiplicadorTipoArena = 1;
            int valorCritico = CalcValorCritico(comando.GetMonstroInBattle.CriticoGarantido? 100 : 10);
            comando.GetMonstroInBattle.CriticoGarantido = false;

            int atributoDefesa = 1;
            if (usarAtributoDefesa)
            {
                if (comando.AttackData.Categoria == AttackData.CategoriaEnum.Fisico)
                {
                    atributoDefesa = comando.GetMonstro.AtributosAtuais.DefesaComModificador;
                }
                else
                {
                    atributoDefesa = comando.GetMonstro.AtributosAtuais.SpDefesaComModificador;
                }
            }

            int multiplicadorDanoAtaqueEspecifico = 1;
            foreach (var statusMonstro in monstro.StatusSecundario)
            {
                foreach (var statusComando in comando.StatusEffectSecundario)
                {
                    if (statusComando != null && statusMonstro != null)
                    {
                        if (statusMonstro.name == statusComando.name)
                        {
                            multiplicadorDanoAtaqueEspecifico++;
                        }
                    }
                }
            }

            float dano1 = (int)((2f * comando.GetMonstro.Nivel) / 5 + 2) * ((float)comando.AttackData.Poder / 150 * ((float)atributoDeDano / atributoDefesa));

            float dano2 = valorCritico + 2;

            float dano3 = multiplicadorTipoArena * UnityEngine.Random.Range(.9f, 1.1f);

            float diferencialDado = VerificarDiceResults(comando.GetMonstroInBattle, monstroAtual.Monstro);
            dano1 *= 1.25f;
            dano2 *= 1.5f;

            float danobuff = 1;
            if (comando.GetMonstro.BuffDano)
            {
                danobuff = 1.2f;
                Debug.Log(comando.GetMonstro.NickName+" : VOu usar o buff para dar dano extra");
            }
            else
            {
                Debug.Log(comando.GetMonstro.NickName + " : Vou causar o dano normal");
            }

            (float valorDano, float modificadorTipoMonstro) = monstro.TomarAtaque(Mathf.CeilToInt((multiplicadorDanoAtaqueEspecifico * dano1) * dano2 * dano3), comando);
            (bool isFainted, int vidaPerdida) = monstro.TomarDano(Mathf.CeilToInt(valorDano * modificadorTipoMonstro * diferencialDado * danobuff), comando.AttackData.TipoAtaque);

            if (isFainted == true)
            {
                BattleManager.Instance.RemoverComandosMonstroMorto(monstro);
                monstro.LimparMonstroMorto();
                //Monstro morre e tira seus ataques da lista, etc

            }

            BattleManager.Instance.AnimacaoTomarDano(monstroAtual, rodarAnimacaoDano, exibirQuantidadeDano, vidaPerdida, (valorCritico != 1), modificadorTipoMonstro);

            return (valorDano * modificadorTipoMonstro, true);
        }
        else //Caso erre
        {
            BattleManager.Instance.AnimacaoTomarDano(monstroAtual, false, exibirQuantidadeDano, -1, false, 0);

            return (-1, false);
        }
    }

    public (float dano, bool acertou) ForcarMiss(Integrante.MonstroAtual monstroAtual, bool exibirQuantidadeDano)
    {
        Debug.Log("Ataque forcado a errar, por causa de status.");
        BattleManager.Instance.AnimacaoTomarDano(monstroAtual, false, exibirQuantidadeDano, -1, false, 0);
        return (-1, false);
    }

    public void TomarAtaquePuro(int dano, Integrante.MonstroAtual monstroAtual, bool rodarAnimacaoDano, bool exibirQuantidadeDano)
    {
        int vidaPerdida = monstro.ReduzirVida(dano);

        BattleManager.Instance.AnimacaoTomarDano(monstroAtual, rodarAnimacaoDano, exibirQuantidadeDano, vidaPerdida, false, 0);
    }

    public void TomarAtaqueStatusEffect(StatusEffectParaAplicar status, int atributo, ComandoDeAtaque comandoDeAtaque, Integrante.MonstroAtual monstroAtual)
    {
        if (monstro.VerificarSeVaiTomarAtaque(comandoDeAtaque.AttackData.ChanceAcerto))
        {
            if (monstro.VerificarAplicarStatusEffect(status, atributo))
            {
                StatusEffectBase statusAplicado = monstro.AplicarStatus(status.GetStatus);

                if (statusAplicado != null)
                {
                    BattleManager.Instance.RodarEfeito(monstroAtual, statusAplicado, -1, false, false);
                }
            }
        }
    }
    public void AplicarStatus(StatusEffectParaAplicar status, Integrante.MonstroAtual monstroAtual)
    {
        StatusEffectBase statusAplicado = monstro.AplicarStatus(status.GetStatus);

        if (statusAplicado != null)
        {
            BattleManager.Instance.RodarEfeito(monstroAtual, statusAplicado, -1, false, false);
        }
    }

    public void AplicarStatusSecundario(Integrante.MonstroAtual monstroAtual, StatusEffectSecundario statusEffectSecundario)
    {
        bool aplicouStatus = monstro.AplicarStatusSecundario(statusEffectSecundario);

        if (aplicouStatus == true)
        {
            BattleManager.Instance.RodarEfeitoStatusSecundario(monstroAtual, statusEffectSecundario);
        }
    }

    private int CalcValorCritico(int valorCritcoChancePorcentagem)
    {
        // 10% de chance de critico
        if (UnityEngine.Random.Range(0, 101) <= valorCritcoChancePorcentagem)
        {
            return 2;
        }
        return 1;
    }

    private float VerificarDiceResults(MonsterInBattle monstroAtacante, MonsterInBattle monstroReceptor)
    {
        if (monstroAtacante.diceResults.Sum() >= monstroReceptor.diceResults.Sum())
        {
            // Debug.Log(($"100% because {monstroAtacante.monstro.NickName} got {monstroAtacante.diceResults.Sum() }" +
                       // $"against {monstroReceptor.monstro.NickName}'s {monstroReceptor.diceResults.Sum()} result. "));
            return 1; 
        }
        else
        {
            // Debug.Log(($"75% because {monstroAtacante.monstro.NickName} got {monstroAtacante.diceResults.Sum() }" +
                       // $"against {monstroReceptor.monstro.NickName}'s {monstroReceptor.diceResults.Sum()} result. "));
            return 0.75f;
        }
    }

    #endregion
}

public enum TipoIntegrante { Jogador, Npc, Selva }

[System.Serializable]
public class Integrante
{
    //Variaveis
    [SerializeField] private TipoIntegrante tipoIntegrante;
    [SerializeField] private string nome;
    [SerializeField] private int time;
    [SerializeField] private List<MonsterInBattle> listaMonstros;
    [SerializeField] private Inventario inventario;
    [SerializeField] private bool perdeu;
    [SerializeField] private bool jaEscolheu;
    [SerializeField] private List<MonstroAtual> monstrosAtuais;

    private int dinheiroPelaVitoria;
    private InventarioNPC inventarioNPC;

    //Getters
    public TipoIntegrante TipoIntegrante => tipoIntegrante;
    public string Nome => nome;
    public int Time => time;
    public List<MonsterInBattle> ListaMonstros => listaMonstros;
    public Inventario Inventario => inventario;
    public List<MonstroAtual> MonstrosAtuais => monstrosAtuais;
    public int DinheiroPelaVitoria => dinheiroPelaVitoria;
    public InventarioNPC InventarioNPC => inventarioNPC;

    public void LimparListas()
    {
        foreach (var item in monstrosAtuais)
        {
            Object.Destroy(item.BattleAnimation.gameObject);
            Object.Destroy(item.MonstroSlotBattle.transform.parent.gameObject);
        }
        monstrosAtuais.Clear();
    }

    public bool Perdeu
    {
        get => perdeu;
        set => perdeu = value;
    }

    public bool JaEscolheu
    {
        get => jaEscolheu;
        set => jaEscolheu = value;
    }

    public Integrante(TipoIntegrante _tipoIntegrante, int _time, string _nome, Inventario _inventario)
    {
        tipoIntegrante = _tipoIntegrante;
        time = _time;
        nome = _nome;
        inventario = _inventario;
    }

    public Integrante(TipoIntegrante _tipoIntegrante, int _time, string _nome, InventarioNPC inventarioNPC, int dinheiroPelaVitoria)
    {
        tipoIntegrante = _tipoIntegrante;
        time = _time;
        nome = _nome;

        this.inventarioNPC = inventarioNPC;
        this.dinheiroPelaVitoria = dinheiroPelaVitoria;
    }

    public Integrante(TipoIntegrante _tipoIntegrante, int _time, string _nome)
    {
        tipoIntegrante = _tipoIntegrante;
        time = _time;
        nome = _nome;
    }

    public void ReceberLista(List<MonsterInBattle> _monstros)
    {
        listaMonstros = _monstros;
    }

    public void CriarListaMonstros(int numeroMonstrosAtuais, List<BattleAnimation> battleAnimations, ref int indicePosicao, GameObject monstroSlotJogadorBase, GameObject monstroSlotNPCBase, RectTransform monstroSlotHolderTime0, RectTransform monstroSlotHolderTime1, int indiceIntegrante)
    {
        monstrosAtuais = new List<MonstroAtual>();

        for (int i = 0; i < numeroMonstrosAtuais; i++)
        {
            if (i >= listaMonstros.Count)
            {
                break;
            }

            BattleAnimation battleAnimation = battleAnimations[indicePosicao];
            MonstroSlotBattle monstroSlotBattle;
            RectTransform pai;

            if (time == 0)
            {
                pai = monstroSlotHolderTime0;
            }
            else
            {
                pai = monstroSlotHolderTime1;
            }

            if (tipoIntegrante == TipoIntegrante.Jogador)
            {
                monstroSlotBattle = UnityEngine.Object.Instantiate(monstroSlotJogadorBase, pai).GetComponentInChildren<MonstroSlotBattle>();
            }
            else
            {
                monstroSlotBattle = UnityEngine.Object.Instantiate(monstroSlotNPCBase, pai).GetComponentInChildren<MonstroSlotBattle>();
            }

            if (time == 0)
            {
                battleAnimation.transform.localScale = new Vector3(battleAnimation.transform.localScale.x * -1, battleAnimation.transform.localScale.y, battleAnimation.transform.localScale.z);
                monstroSlotBattle.AtualizarPosicaoMonstersInBag(MonstroSlotBattle.PosicaoMonsterInBag.Esquerda);
            }
            else
            {
                monstroSlotBattle.AtualizarPosicaoMonstersInBag(MonstroSlotBattle.PosicaoMonsterInBag.Direita);
            }

            if (i == 0 && (tipoIntegrante == TipoIntegrante.Jogador || tipoIntegrante == TipoIntegrante.Npc))
            {
                monstroSlotBattle.AtivarMonstersInBag(this);
            }

            if (tipoIntegrante == TipoIntegrante.Selva)
            {
                monstroSlotBattle.IconeMonstroCapturadoAtivado(true, listaMonstros[i].GetMonstro.MonsterData);
            }
            else
            {
                monstroSlotBattle.IconeMonstroCapturadoAtivado(false, listaMonstros[i].GetMonstro.MonsterData);
            }

            MonstroAtual monstro = new MonstroAtual(listaMonstros[i], battleAnimation, monstroSlotBattle);

            monstroSlotBattle.Iniciar(indiceIntegrante, i);
            monstro.AtualizarMonstro();

            monstrosAtuais.Add(monstro);

            indicePosicao++;
        }
        
    }

    [System.Serializable]
    public class MonstroAtual
    {
        //Variaveis
        [SerializeField] private MonsterInBattle monstro;
        private BattleAnimation battleAnimation;
        private MonstroSlotBattle monstroSlotBattle;

        //Getters
        public Monster GetMonstro => monstro.GetMonstro;

        public MonsterInBattle Monstro
        {
            get => monstro;
            set => monstro = value;
        }

        public BattleAnimation BattleAnimation
        {
            get => battleAnimation;
            set => battleAnimation = value;
        }

        public MonstroSlotBattle MonstroSlotBattle
        {
            get => monstroSlotBattle;
            set => monstroSlotBattle = value;
        }

        //Construtor
        public MonstroAtual(MonsterInBattle monstro, BattleAnimation battleAnimation, MonstroSlotBattle monstroSlotBattle)
        {
            this.monstro = monstro;
            this.battleAnimation = battleAnimation;
            this.monstroSlotBattle = monstroSlotBattle;
        }

        public void AtualizarMonstro()
        {
            monstroSlotBattle.AtualizarMonstro(monstro);
            battleAnimation.AtualizarAnimator(monstro.GetMonstro.MonsterData.Animator);

            monstroSlotBattle.AtualizarInformacoes();
        }
    }

    public void AtualizarMonstroSlots()
    {
        for (int i = 0; i < monstrosAtuais.Count; i++)
        {
            monstrosAtuais[i].AtualizarMonstro();
        }
    }
}

public struct MonstroParaTrocar
{
    private MonsterInBattle monstro1;
    private MonsterInBattle monstro2;
    private Integrante origem;
    private int indiceOrigem;
    private int indiceTroca;
    private int indice;
    private int indiceMonstroAtual;

    public MonsterInBattle Monstro1 => monstro1;
    public MonsterInBattle Monstro2 => monstro2;
    public Integrante Origem => origem;
    public int IndiceOrigem => indiceOrigem;
    public int IndiceTroca => indiceTroca;
    public int Indice => indice;
    public int IndiceMonstroAtual => indiceMonstroAtual;

    public MonstroParaTrocar(MonsterInBattle monstro1, MonsterInBattle monstro2, Integrante origem, int indiceOrigem, int indiceTroca, int indice, int indiceMonstroAtual)
    {
        this.monstro1 = monstro1;
        this.monstro2 = monstro2;
        this.origem = origem;
        this.indiceOrigem = indiceOrigem;
        this.indiceTroca = indiceTroca;
        this.indice = indice;
        this.indiceMonstroAtual = indiceMonstroAtual;
    }
}

public struct EfeitoParaRodar
{
    private Integrante.MonstroAtual monstro;
    private StatusEffectBase statusEffect;
    private int danoRecebido;
    private bool terminouOEfeito;
    private bool efeitoBloqueouAtaque;

    public Integrante.MonstroAtual Monstro => monstro;
    public StatusEffectBase StatusEffect => statusEffect;
    public int DanoRecebido => danoRecebido;
    public bool TerminouOEfeito => terminouOEfeito;
    public bool EfeitoBloqueouAtaque => efeitoBloqueouAtaque;

    public EfeitoParaRodar(Integrante.MonstroAtual monstro, StatusEffectBase statusEffect, int danoRecebido, bool terminouOEfeito, bool efeitoBloqueouAtaque)
    {
        this.monstro = monstro;
        this.statusEffect = statusEffect;
        this.danoRecebido = danoRecebido;
        this.terminouOEfeito = terminouOEfeito;
        this.efeitoBloqueouAtaque = efeitoBloqueouAtaque;
    }
}

public struct EfeitoAtributoParaRodar
{
    private Integrante.MonstroAtual monstroAtual;
    private StatusEffectDebufAtributo modificadorDeAtributo;
    private bool resetouOModificador;

    public Integrante.MonstroAtual MonstroAtual => monstroAtual;
    public StatusEffectDebufAtributo ModificadorDeAtributo => modificadorDeAtributo;
    public bool ResetouOModificador => resetouOModificador;

    public EfeitoAtributoParaRodar(Integrante.MonstroAtual monstroAtual, StatusEffectDebufAtributo modificadorDeAtributo, bool resetouOModificador)
    {
        this.monstroAtual = monstroAtual;
        this.modificadorDeAtributo = modificadorDeAtributo;
        this.resetouOModificador = resetouOModificador;
    }
}

public struct MonstroMortoParaTrocar
{
    private int indiceIntegrante;
    private int indiceMonstroAtual;

    public int IndiceIntegrante => indiceIntegrante;
    public int IndiceMonstroAtual => indiceMonstroAtual;

    public MonstroMortoParaTrocar(int indiceIntegrante, int indiceMonstroAtual)
    {
        this.indiceIntegrante = indiceIntegrante;
        this.indiceMonstroAtual = indiceMonstroAtual;
    }
}

public struct EfeitoSecundarioParaRodar
{
    private Integrante.MonstroAtual monstroAtual;
    private StatusEffectSecundario statusEffectSecundario;

    public Integrante.MonstroAtual Monstro => monstroAtual;
    public StatusEffectSecundario StatusEffectSecundario => statusEffectSecundario;

    public EfeitoSecundarioParaRodar(Integrante.MonstroAtual monstroAtual, StatusEffectSecundario statusEffectSecundario)
    {
        this.monstroAtual = monstroAtual;
        this.statusEffectSecundario = statusEffectSecundario;
    }
}

public struct CombatLessonParaMostrar
{
    private Sprite iconeElemon;
    private string nomeCombatLesson;

    public Sprite IconeElemon => iconeElemon;
    public string NomeCombatLesson => nomeCombatLesson;

    public CombatLessonParaMostrar(Sprite iconeElemon, string nomeCombatLesson)
    {
        this.iconeElemon = iconeElemon;
        this.nomeCombatLesson = nomeCombatLesson;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Comando")]
public class Comando : ScriptableObject
{
    //Enuns
    public enum TipoTarget { Self, Aliado, TimeAliado, Inimigo, TimeInimigo, TodosExcetoSelf, Aleatorio }

    //Variaveis
    [Header("Opcoes do Comando")]

    [SerializeField] protected string nome;

    [SerializeField] [Range(0, 10)]
    [Tooltip("Correr = 10\nTrocar Monstro = 9\nItem = 8\nArena = 7\nAtaque = 6 a 0")]
    protected int prioridade;

    [SerializeField] protected TipoTarget target;

    [Tooltip("quantos rounds esse comando leva para ser executado pela primeira vez)")]
    [SerializeField] protected int turnosParaSerExecutado;

    protected int velocidade = 0;
    protected Integrante origem;

    [SerializeField]protected bool bloqueioAcaoDaOrigem = false;
    protected bool podeMeRetirar = false;
    protected List<Integrante.MonstroAtual> alvoAcao = new List<Integrante.MonstroAtual>();
    protected List<bool> alvoComAtaquesValidos = new List<bool>();
    protected int indiceMonstro;
    protected bool comandoSolto = false;

    [SerializeField] protected AcaoNaBatalha acao;

    [Header("Animacoes")]

    [SerializeField] private AnimatorOverrideController animacaoEfeito;
    private int quantidadeVezesComandoRodou = 0;
    //Variaveis de controle
    protected bool comandoIniciado = false;

    //Getters
    public int ID => GlobalSettings.Instance.Listas.ListaDeComandos.GetId(this);
    public string Nome => nome;
    public int Prioridade => prioridade;
    public TipoTarget Target => target;
    public Monster GetMonstro => origem.MonstrosAtuais[indiceMonstro].GetMonstro;
    public MonsterInBattle GetMonstroInBattle => origem.MonstrosAtuais[indiceMonstro].Monstro;
    public AnimatorOverrideController AnimacaoEfeito => animacaoEfeito;
    public BattleAnimation GetBattleAnimation => origem.MonstrosAtuais[indiceMonstro].BattleAnimation;
    public int QuantidadeVezesComandoRodou => quantidadeVezesComandoRodou;
    public int Velocidade
    {
        get => GetMonstro.AtributosAtuais.Velocidade;
        set => velocidade = value;
    }

    public Integrante Origem
    {
        get => origem;
        set => origem = value;
    }

    public bool BloqueioAcaoDaOrigem
    {
        get => bloqueioAcaoDaOrigem;
        set => bloqueioAcaoDaOrigem = value;
    }

    public bool PodeMeRetirar
    {
        get => podeMeRetirar;
        set => podeMeRetirar = value;
    }

    public List<Integrante.MonstroAtual> AlvoAcao
    {
        get => alvoAcao;
        set => alvoAcao = value;
    }

    public AcaoNaBatalha Acao => acao;

    public int IndiceMonstro
    {
        get => indiceMonstro;
        set => indiceMonstro = value;
    }

    public bool ComandoIniciado
    {
        get => comandoIniciado;
        set => comandoIniciado = value;
    }

    public int TurnosParaSerExecutado
    {
        get => turnosParaSerExecutado;
        set => turnosParaSerExecutado = value;
    }

    public bool ComandoSolto
    {
        get => comandoSolto;
        set => comandoSolto = value;
    }

    public List<bool> AlvoComAtaquesValidos
    {
        get => alvoComAtaquesValidos;
        set => alvoComAtaquesValidos = value;
    }

    public void IniciarAnimacao(BattleManager battleManager)
    {
        acao.IniciarAnimacao(battleManager, this);
        quantidadeVezesComandoRodou++;
    }

    public void IniciarAnimacao(BattleManager battleManager, ComandoDeAtaque comandoAtaque)
    {
        comandoAtaque.AplicarStatusSecundarioEmSelf();

        if (comandoAtaque.TurnosParaSerExecutado <= 0)
        {
            IniciarAnimacao(battleManager);
        }
        else
        {
            comandoAtaque.TurnosParaSerExecutado--;
        }
    }

    public void Executar(BattleManager battleManager)
    {
        acao.Executar(battleManager, this);
        if(this is ComandoDeAtaque)
        {
            ComandoDeAtaque comandoAtaque = (ComandoDeAtaque)this;
            comandoAtaque.RemoverStatusSecundarioEmSelf();
        }
    }

    public void SeRetirarDaLista(List<Comando> listaDeComandos)
    {
        listaDeComandos.Remove(this);
    }

    public void ReceberVariavel(Integrante origem)
    {
        this.origem = origem;
    }
}

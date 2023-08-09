using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Monster/Status/Basico")]
public class StatusEffectBase : ScriptableObject
{
    //Enums
    public enum TipoDeEfeito { DentroCombate, ForaCombate, Ambos }
    public enum TipoDeEfeitoVisualEnum { Nenhum, TintEffect, TintEffectSlow, TintSolidEffect, TintSolidEffectSlow }

    [Header("Efeito")]
    [SerializeField] protected TipoDeEfeito tipoDeEfeito;

    public enum TipoStackStatus { Stacavel, Exclusivo }
    [SerializeField] protected TipoStackStatus tipoStackStatus;

    //Componentes
    [ShowIf("@tipoDeEfeito == TipoDeEfeito.DentroCombate || tipoDeEfeito == TipoDeEfeito.Ambos")]
    [SerializeField] protected StatusEffectOpcoesDentroCombate statusEffectOpcoesDentroCombate;

    [ShowIf("@tipoDeEfeito == TipoDeEfeito.ForaCombate || tipoDeEfeito == TipoDeEfeito.Ambos")]
    [SerializeField] protected StatusEffectOpcoesForaCombate statusEffectOpcoesForaCombate;

    //Variaveis
    [Header("Icone")]
    [SerializeField] protected string nome;
    [SerializeField] [HideInTables] private Color statusColor;

    [Header("Dialogos")]
    [SerializeField] protected BergamotaDialogueSystem.DialogueObject dialogoBloqueouComando;
    [SerializeField] protected BergamotaDialogueSystem.DialogueObject dialogoTerminouEfeito;

    [Header("Efeito Visual")]
    [SerializeField] protected TipoDeEfeitoVisualEnum tipoDeEfeitoVisual;

    [ShowIf("@tipoDeEfeitoVisual != TipoDeEfeitoVisualEnum.Nenhum")]
    [SerializeField] protected Color corDoEfeito;

    [ShowIf("@tipoDeEfeitoVisual != TipoDeEfeitoVisualEnum.Nenhum")]
    [SerializeField] protected float velocidadeDoEfeito;


    //Controles
    protected bool seRemover;
    protected int contador;

    //Getter
    public int ID => GlobalSettings.Instance.Listas.ListaDeStatusEffects.GetId(this);
    public string Nome => nome;
    public Color StatusColor => statusColor;
    public Color CorDoEfeito => corDoEfeito;
    public float VelocidadeDoEfeito => velocidadeDoEfeito;
    public TipoDeEfeitoVisualEnum TipoDeEfeitoVisual => tipoDeEfeitoVisual;
    public BergamotaDialogueSystem.DialogueObject DialogoBloqueouComando => dialogoBloqueouComando;
    public BergamotaDialogueSystem.DialogueObject DialogoTerminouEfeito => dialogoTerminouEfeito;
    public bool GetSeRemover => seRemover;
    public int Contador => contador;
    public StatusEffectOpcoesForaCombate GetStatusEffectOpcoesForaCombate => statusEffectOpcoesForaCombate;
    public StatusEffectOpcoesDentroCombate GetStatusEffectOpcoesDentroCombate => statusEffectOpcoesDentroCombate;
    public TipoStackStatus GetTipoStackStatus => tipoStackStatus;

    public void Init()
    {
        statusEffectOpcoesDentroCombate.Aplicado = false;
        seRemover = false;
        contador = 0;
        statusEffectOpcoesDentroCombate.GetquantidadeMaximaTurnos = Random.Range(statusEffectOpcoesDentroCombate.QuantidadeDuracaoTurnosMin,statusEffectOpcoesDentroCombate.QuantidadeDuracaoTurnosMax+1);
    }

    public void CarregarVariaveis(StatusEffectSave statusEffectSave)
    {
        contador = statusEffectSave.contador;
        statusEffectOpcoesDentroCombate.QuantidadeTurnosAtuais = statusEffectSave.quantidadeTurnosAtuais;
        statusEffectOpcoesForaCombate.QuantidadeTicksAtuais = statusEffectSave.quantidadeTicksAtuais;
    }

    public virtual bool ExecutarInBattle(Integrante.MonstroAtual monstroAtual)
    {
        Monster monstro = monstroAtual.GetMonstro;

        statusEffectOpcoesDentroCombate.QuantidadeTurnosAtuais++;
        if (statusEffectOpcoesDentroCombate.GetdanoNoCombate)
        {
            if (statusEffectOpcoesDentroCombate.GetdanoDentroCombate.GetTemDanoFixo)
            {
                float x = (statusEffectOpcoesDentroCombate.GetdanoDentroCombate.Dano);
                monstro.TomarDanoPuro(Mathf.CeilToInt(x));

                BattleManager.Instance.RodarEfeito(monstroAtual, this, Mathf.CeilToInt(statusEffectOpcoesDentroCombate.GetdanoDentroCombate.Dano), false, false);
            }
            else
            {
                float x = ((float)statusEffectOpcoesDentroCombate.GetdanoDentroCombate.Dano / 100) * monstro.AtributosAtuais.VidaMax;
                monstro.TomarDanoPuro(Mathf.CeilToInt(x));

                BattleManager.Instance.RodarEfeito(monstroAtual, this, (int)x, false, false);
            }

            Debug.Log("Tomei dano de " + nome + " dentro do combate");
        }

        if (statusEffectOpcoesDentroCombate.GetEfeitoPassaComTempo)
        {
            if (statusEffectOpcoesDentroCombate.QuantidadeTurnosAtuais >= statusEffectOpcoesDentroCombate.GetquantidadeMaximaTurnos)
            {
                seRemover = true;
                Debug.Log("Cessou efeito de " + nome);
            }
        }
        else
        {
            if (Random.Range(0f, 100f) <= statusEffectOpcoesDentroCombate.GetchancePorcentagemEfeitoPassar)
            {
                seRemover = true;
                Debug.Log("Cessou efeito de " + nome);
            }
        }

        if (seRemover)
        {
            RemoverModificador(monstro);
            BattleManager.Instance.RodarEfeito(monstroAtual, this, 0, true, false);
        }

        return seRemover;
    }

    public virtual bool ExecutarOutBattle(Monster monstro)
    {
        statusEffectOpcoesForaCombate.QuantidadeTicksAtuais++;
        if (statusEffectOpcoesForaCombate.QuantidadeTicksAtuais >= statusEffectOpcoesForaCombate.GetquantidadeTicksAtivarEfeito)
        {
            contador++;
            if (statusEffectOpcoesForaCombate.GetDesaparecer)
            {
                if (contador * statusEffectOpcoesForaCombate.GetquantidadeTicksAtivarEfeito >= statusEffectOpcoesForaCombate.GetquantidadeTicksMaximo)
                {
                    seRemover = true;
                    Debug.Log("Cessou efeito de " + nome + " fora do combate");

                }

            }
            if (statusEffectOpcoesForaCombate.GetDanoPosCombate)
            {
                if (statusEffectOpcoesForaCombate.GetDanoForaCombate.GetTemDanoFixo)
                {
                    float x = (statusEffectOpcoesForaCombate.GetDanoForaCombate.Dano);
                    monstro.TomarDanoPuro(Mathf.CeilToInt(x));      
                    Debug.Log("Tomei dano de " + nome + " fora do combate");

                }
                else
                {
                    float x = ((float)statusEffectOpcoesForaCombate.GetDanoForaCombate.Dano / 100) * monstro.AtributosAtuais.VidaMax;
                    monstro.TomarDanoPuro(Mathf.CeilToInt(x));
                    Debug.Log("Tomei dano de " + nome + " fora do combate");

                }
                if (monstro.AtributosAtuais.Vida <= 0)
                    monstro.AtributosAtuais.Vida = 1;
            }
            statusEffectOpcoesForaCombate.QuantidadeTicksAtuais = 0;
        }
        if (seRemover)
            RemoverModificador(monstro);
        return seRemover;
    }
    public bool SairDoCombate(Monster monstro)
    {
        RemoverModificador(monstro);
        if (!statusEffectOpcoesForaCombate.GetAtivoPosCombate)
        {
            seRemover = true;
        }
        return seRemover;
    }


    public void AplicarModificador(Monster monstro)
    {
        if (statusEffectOpcoesDentroCombate.Aplicado == false)
        {
            for (int i = 0; i < statusEffectOpcoesDentroCombate.GetstatusEffectDebufAtributo.Count; i++)
            {
                Modificador.Atributo atributo = statusEffectOpcoesDentroCombate.GetstatusEffectDebufAtributo[i].GetAtributo;
                statusEffectOpcoesDentroCombate.ID = monstro.AtributosAtuais.ReceberModificadorStatus(atributo, statusEffectOpcoesDentroCombate.GetstatusEffectDebufAtributo[i].GetvalorDebuff, false, 0);
                monstro.AtributosAtuais.TocarSomModificador(statusEffectOpcoesDentroCombate.GetstatusEffectDebufAtributo[i].GetvalorDebuff, monstro.MonsterData);

                Debug.Log(monstro);
            }
            statusEffectOpcoesDentroCombate.Aplicado = true;
        }
    }
    public void AplicarModificadorOutBattle(Monster monstro)
    {
        if (statusEffectOpcoesDentroCombate.Aplicado == false)
        {
            for (int i = 0; i < statusEffectOpcoesDentroCombate.GetstatusEffectDebufAtributo.Count; i++)
            {
                Modificador.Atributo atributo = statusEffectOpcoesDentroCombate.GetstatusEffectDebufAtributo[i].GetAtributo;
                statusEffectOpcoesDentroCombate.ID = monstro.AtributosAtuais.ReceberModificadorStatus(atributo, statusEffectOpcoesDentroCombate.GetstatusEffectDebufAtributo[i].GetvalorDebuff, false, 0);
                monstro.AtributosAtuais.TocarSomModificador(statusEffectOpcoesDentroCombate.GetstatusEffectDebufAtributo[i].GetvalorDebuff, monstro.MonsterData);

                Debug.Log(monstro);
            }
        }
    }
    public void RemoverModificador(Monster monster)
    {
        foreach (var item in statusEffectOpcoesDentroCombate.GetstatusEffectDebufAtributo)
        {
            monster.AtributosAtuais.RemoverModificador(item.GetAtributo, statusEffectOpcoesDentroCombate.ID);
        }   
        statusEffectOpcoesDentroCombate.Aplicado = false;
    }
    public virtual bool VerificarExcecao(MonsterType tipoAtaque)
    {
        for (int i = 0; i < statusEffectOpcoesDentroCombate.GetRemoverStatusComAtaqueTipo.Count; i++)
        {
            if (statusEffectOpcoesDentroCombate.GetRemoverStatusComAtaqueTipo[i] == tipoAtaque)
            {
                seRemover = true;
            }
        }
        return seRemover;
    }
}
[System.Serializable]
public class StatusEffectDebufAtributo
{
    [SerializeField] private Modificador.Atributo atributo;
    [Range(-6, 6)]
    [SerializeField] private int valorDebuff;

    public Modificador.Atributo GetAtributo => atributo;
    public int GetvalorDebuff => valorDebuff;

}
[System.Serializable]
public class StatusEffectBloquearComando
{

    [SerializeField] private bool bloqueiaComando;
    [ShowIf("bloqueiaComando")]
    [SerializeField] private bool porcentagem;
    [Tooltip("Usa x de chance para bloquear comandos")]
    [ShowIf("porcentagem")]
    [SerializeField] private float porcentagemBloquearAcao;

    public bool BloqueiaComando
    {
        get
        {
            if (bloqueiaComando)
            {
                if (porcentagem)
                {
                    if (Random.Range(0, 100) <= porcentagemBloquearAcao)
                    {
                        return true;
                    }
                    else
                        return false;
                }
                return true;
            }
            return false;
        }
    }
}
[System.Serializable]
public class StatusEffectDano
{
    public enum TipoDano { Fixo, Porcentagem };

    [SerializeField] private TipoDano tipoDano;
    [ShowIf("tipoDano", TipoDano.Fixo)]
    [SerializeField] private float danoFixo;

    [ShowIf("tipoDano", TipoDano.Porcentagem)]
    [Tooltip("porcentagem baseado na vida m�xima")]

    [SerializeField] private float danoEmPorcentagem;

    public bool GetTemDanoFixo
    {
        get
        {
            if (tipoDano == TipoDano.Fixo)
            {
                return true;
            }
            return false;
        }
    }
    public float Dano
    {
        get
        {
            if (tipoDano == TipoDano.Fixo)
            {
                return danoFixo;
            }
            return danoEmPorcentagem;
        }
    }
}
[System.Serializable]
public class StatusEffectOpcoesDentroCombate
{
    [SerializeField] private bool ativoDuranteCombate;

    [ShowIf("ativoDuranteCombate")]
    [SerializeField] private bool bloqueiFuga;
    [ShowIf("ativoDuranteCombate")]
    [SerializeField] private bool danoNoCombate;
    [ShowIf("danoNoCombate")]
    [SerializeField] private StatusEffectDano danoDentroCombate;

    [SerializeField] private bool efeitoPassaComTempo;

    [ShowIf("efeitoPassaComTempo")]
    [SerializeField] private int quantidadeDuracaoTurnosMin;
    [ShowIf("efeitoPassaComTempo")]
    [SerializeField] private int quantidadeDuracaoTurnosMax;
    private int quantidadeDuracaoTurnos;

    [HideIf("efeitoPassaComTempo")]
    [SerializeField] private int chancePorcentagemEfeitoPassar;


    [ShowIf("ativoDuranteCombate")]
    [SerializeField] private StatusEffectBloquearComando statusEffectBloquearComando;

    [ShowIf("ativoDuranteCombate")]
    [SerializeField] private List<StatusEffectDebufAtributo> statusEffectDebufAtributo;
    [SerializeField] private List<MonsterType> removerStatusComAtaqueTipo;
    [SerializeField] private List<MonsterType> tiposImunes;


    private bool aplicado;
    private int quantidadeTurnosAtuais;

    private int id;
    //Getter
    public StatusEffectBloquearComando GetstatusEffectBloquearComando => statusEffectBloquearComando;
    public StatusEffectDano GetdanoDentroCombate => danoDentroCombate;
    public List<StatusEffectDebufAtributo> GetstatusEffectDebufAtributo => statusEffectDebufAtributo;
    public List<MonsterType> GetRemoverStatusComAtaqueTipo => removerStatusComAtaqueTipo;
    public List<MonsterType> TiposImunes => tiposImunes;
    public bool GetBloqueiaFuga => bloqueiFuga;
    public bool GetdanoNoCombate => danoNoCombate;
    public int QuantidadeDuracaoTurnosMin => quantidadeDuracaoTurnosMin;
    public int QuantidadeDuracaoTurnosMax => quantidadeDuracaoTurnosMax;

    public bool GetEfeitoPassaComTempo => efeitoPassaComTempo;
    public int GetchancePorcentagemEfeitoPassar => chancePorcentagemEfeitoPassar;
    public int ID
    {
        get => id;
        set => id = value;
    }
    public bool Aplicado
    {
        get => aplicado;
        set => aplicado = value;
    }

    public int QuantidadeTurnosAtuais
    {
        get => quantidadeTurnosAtuais;
        set => quantidadeTurnosAtuais = value;
    }
    public int GetquantidadeMaximaTurnos
    {
        get => quantidadeDuracaoTurnos;
        set => quantidadeDuracaoTurnos = value;
    }
}

[System.Serializable]
public class StatusEffectOpcoesForaCombate
{
    [SerializeField] private bool ativoPosCombate;

    [ShowIf("ativoPosCombate")]
    [SerializeField] private bool danoPosCombate;
    [ShowIf("danoPosCombate")]
    [SerializeField] private StatusEffectDano danoForaCombate;
    [ShowIf("ativoPosCombate")]
    [SerializeField] private int quantidadeTicksAtivarEfeito;
    [ShowIf("ativoPosCombate")]
    [SerializeField] private bool desapareceAposTicks;
    [ShowIf("desapareceAposTicks")]
    [SerializeField] private int quantidadeTicksMaximo;

    private int quantidadeTicksAtuais;
    //Getter
    public bool GetAtivoPosCombate => ativoPosCombate;
    public bool GetDesaparecer => desapareceAposTicks;
    public bool GetDanoPosCombate => danoPosCombate;

    public StatusEffectDano GetDanoForaCombate => danoForaCombate;

    public int GetquantidadeTicksAtivarEfeito => quantidadeTicksAtivarEfeito;
    public int GetquantidadeTicksMaximo => quantidadeTicksMaximo;

    public int QuantidadeTicksAtuais
    {
        get => quantidadeTicksAtuais;
        set => quantidadeTicksAtuais = value;
    }
}
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat Lessons/Lesson")]
public class CombatLesson : ComandoDeAtaque
{
    [SerializeField] private List<EffectStructure> effects = new List<EffectStructure>();

    public List<EffectStructure> Effects => effects;

    public NivelMedio nivelPoder = NivelMedio.baixo;

    public void ExecutarCombatLesson(BattleManager battleManager)
    {
        AlvoComAtaquesValidos = new List<bool>();
        for (int i = 0; i < AlvoAcao.Count; i++)
        {
            alvoComAtaquesValidos.Add(true);
        }

        effects.ForEach(e => e.Efeito.Executar(battleManager, this));
    }
    public void ReceberVars(Integrante origem, Integrante.MonstroAtual alvo, int indice)
    {
        Debug.LogWarning($"{Nome}, de {origem.Nome}, alvoAcao: {alvo.GetMonstro.NickName}, indiceMonstro: {indice}");
        this.origem = origem;
        this.AlvoAcao.Add(alvo);
        indiceMonstro = indice;
    }

    public void ReceberVars(Integrante origem, List<Integrante.MonstroAtual> alvo, int indice)
    {
        Debug.LogWarning($"{Nome}, de {origem.Nome}, alvoAcao: {alvo[0].GetMonstro.NickName}, indiceMonstro: {indice}");
        this.origem = origem;
        this.AlvoAcao = alvo;
        indiceMonstro = indice;
    }
}

[System.Serializable]
public class EffectStructure
{
    [SerializeField] 
    private AcaoNaBatalha efeito;
    
    [SerializeField, EnableIf("efeito")] 
    private bool valorDinamico;

    [SerializeField, EnableIf("@efeito && valorDinamico")]
    private TipoValorSetado condicaoValorDinamico;
    
    [SerializeField, EnableIf("@efeito && !valorDinamico")] 
    private List<int> condicaoValor;
    
    [SerializeField, EnableIf("efeito")]
    private CondicaoTarget condicaoTarget;

    [SerializeField, EnableIf("efeito")] 
    private TipoAtaque condicaoAtaque;

    [SerializeField, EnableIf("efeito")] //Quem sofre a ação
    private TipoTarget targetDoEfeito;

    [SerializeField, EnableIf("efeito")] 
    private MonsterType attackType;
    public TipoTarget TipoTarget => targetDoEfeito;
    public AcaoNaBatalha Efeito => efeito;

    public bool VerifyIfCanUseEffect(Comando comando, DiceType diceType, int diceResult)
    {
        ComandoDeAtaque comandoDeAtaque = (ComandoDeAtaque)comando;

        bool valueCondition = VerifyValueConditions(diceType, diceResult);
        
        bool attackCondition = VerifyAttackCondition(comandoDeAtaque);
        
        bool targetCondition = VerifyTargetCondition(comandoDeAtaque);

        bool typeCondition = VerifyAttackTypeCondition(comandoDeAtaque);
        
        /*
        Debug.Log($"{Efeito}: valueCondition: {diceResult}: {valueCondition}," +
                  $"\n attackCondition: {condicaoAtaque}: {attackCondition}," +
                  $"\n targetCondition: {condicaoTarget}: {comandoDeAtaque.Target}: {targetCondition}," +
                  $"\n comando: {comandoDeAtaque.name}");
        */

        return targetCondition && attackCondition && valueCondition && typeCondition;
    }

    private bool VerifyAttackTypeCondition(ComandoDeAtaque comandoDeAtaque)
    {
        if (attackType == null)
            return true;

        if (comandoDeAtaque.AttackData.TipoAtaque == attackType)
            return true;
        else
            return false;
    }

    private bool VerifyTargetCondition(ComandoDeAtaque comandoDeAtaque)
    {
        switch (condicaoTarget)
        {
            case CondicaoTarget.Any:
                return true;
            case CondicaoTarget.Enemy:
                if (comandoDeAtaque.Target == Comando.TipoTarget.Inimigo ||
                    comandoDeAtaque.Target == Comando.TipoTarget.TimeInimigo ||
                    comandoDeAtaque.Target == Comando.TipoTarget.Aleatorio)
                    return true;
                break;
            case CondicaoTarget.Ally:
                if (comandoDeAtaque.Target == Comando.TipoTarget.Aliado ||
                    comandoDeAtaque.Target == Comando.TipoTarget.TimeAliado)
                    return true;
                break;
            case CondicaoTarget.Self:
                if (comandoDeAtaque.Target == Comando.TipoTarget.Self)
                    return true;
                break;
        }

        return false;
    }

    private bool VerifyAttackCondition(ComandoDeAtaque comandoDeAtaque)
    {
        List<string> ataquesCondicionais = condicaoAtaque.ToString().Split(", ").ToList();
        return condicaoAtaque == TipoAtaque.Any || ataquesCondicionais.Contains(comandoDeAtaque.AttackData.Categoria.ToString());
    }


    private bool VerifyValueConditions(DiceType diceType, int diceResult)
    {
        if (valorDinamico)
        {
            if (condicaoValorDinamico == TipoValorSetado.Maximo)
            {
                if (diceResult == (int)diceType)
                {
                    return true;
                }
            }
        }
        else
        {
            if (condicaoValor.Contains(diceResult))
            {
                return true;
            }
            else
            {
                if (condicaoValor.Count == 0)
                {
                    return true;
                }
            }
        }

        return false;
    }
}

[System.Flags]
public enum CondicaoTarget
{
    Any = 0,
    Enemy = 1,
    Ally = 1 << 1,
    Self = 1 << 2
}

[System.Flags]
public enum TipoAtaque
{
    Any = 0,
    Fisico = 1,
    Especial = 1 << 1,
    Status = 1 << 2
}

public enum TipoTarget 
{ 
    Self, 
    Target,
    TimeAliado, 
    TimeInimigo, 
    TodosExcetoSelf,
    Aleatorio 
}
public enum TipoValorSetado
{
    Maximo
}

public enum NivelMedio
{
    baixo = 26,
    medio = 50,
    alto = 90,
    special = 0
}
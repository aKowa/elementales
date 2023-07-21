using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Comando de Ataque")]
public class ComandoDeAtaque : Comando
{
    //Variaveis
    [SerializeField] private BattleAnimation.AnimacaoSprite animacaoSprite;
    [SerializeField] private BattleAnimation.AnimacaoMovimento animacaoMovimento;

    [SerializeField]
    [Tooltip("Se estiver verdadeiro, a animacao do efeito so comecara a rodar depois do fim da animacao do sprite.")]
    private bool rodarEfeitoAposOFimDaAnimacao;

    [Header("Dialgo - Ataque Proximo Turno")]
    [SerializeField] private BergamotaDialogueSystem.DialogueObject dialogoGolpeParaTurnoSeguinte;

    [Header("Opcoes de Ataque")]

    [SerializeField] private AttackData attackData;
    [SerializeField] private int maxPP;
    [SerializeField] private int custoMana;
    [Tooltip("quantos rounds esse comando permanece em jogo apos ser executado(ele sera executado novamente em cada round)")]
    [SerializeField] private int numeroRoundsComandoVivo;
    [SerializeField] private bool consomePP;
    private bool consumiuRecurso = false;
    [SerializeField] [TextArea(2, 5)] private string descricao;

    [Header("Status Secundario - Aplicar Self")]
    [SerializeField] private StatusEffectSecundario statusAplicarSelf;

    [Header("Status Secundario - Dano Extra")]
    [SerializeField] private List<StatusEffectSecundario> statusSecundarios;

    public int powerMargin;
    public int setPowerMargin;

    //Controle
    private bool statusSecundarioAplicado;

    //Getters
    public AttackData AttackData => attackData;
    public int MaxPP => maxPP;
    public int CustoMana => custoMana;
    public bool ConsomePP => consomePP;
    public string Descricao => descricao;
    public BattleAnimation.AnimacaoSprite AnimacaoSprite => animacaoSprite;
    public BattleAnimation.AnimacaoMovimento AnimacaoMovimento => animacaoMovimento;
    public bool RodarEfeitoAposOFimDaAnimacao => rodarEfeitoAposOFimDaAnimacao;
    public BergamotaDialogueSystem.DialogueObject DialogoGolpeParaTurnoSeguinte => dialogoGolpeParaTurnoSeguinte;
    public StatusEffectSecundario StatusEffectSecundarioSelf => statusAplicarSelf;
    public List<StatusEffectSecundario> StatusEffectSecundario => statusSecundarios;
    public int NumeroRoundsComandoVivo
    {
        get => numeroRoundsComandoVivo;
        set
        {            
            numeroRoundsComandoVivo = value;
        }
    }

    public void ReceberVariaves(Integrante origem, Integrante.MonstroAtual alvo, int indice)
    {
        Debug.Log($"Receber Variaveis: {Nome}, origem: {origem.Nome}, alvo: {alvo.GetMonstro.NickName}, indice: {indice}");
        this.origem = origem;
        this.AlvoAcao.Add(alvo);
        indiceMonstro = indice;
    }

    public void ReceberVariaves(Integrante origem, List<Integrante.MonstroAtual> alvo, int indice)
    {
        this.origem = origem;
        this.AlvoAcao = alvo;
        indiceMonstro = indice;
    }

    public void VerificarAlvoVivo(List<Integrante> integrantes)
    {
        List<Integrante.MonstroAtual> alvosInvalidos = new List<Integrante.MonstroAtual>();

        foreach (Integrante.MonstroAtual alvo in AlvoAcao) //Verifica se algum alvo esta morto
        {
            if (alvo.GetMonstro.IsFainted)
            {
                alvosInvalidos.Add(alvo);
            }
        }

        if (alvosInvalidos.Count >= AlvoAcao.Count) // Se todos os alvos estiver mortos
        {
            for (int i = 0; i < integrantes.Count; i++)
            {
                if (integrantes[i] != origem)
                {
                    for (int j = 0; j < integrantes[i].MonstrosAtuais.Count; j++)
                    {
                        if (integrantes[i].MonstrosAtuais[j].GetMonstro.IsFainted == false)
                        {
                             AlvoAcao.Add(integrantes[i].MonstrosAtuais[j]);  
                        }
                    }
                }
            }
        }

        foreach (Integrante.MonstroAtual alvo in alvosInvalidos) //Remove os alvos mortos da lista
        {
            AlvoAcao.Remove(alvo);
        }
    }

    public void VerificarAlvoStatusDebilitante()
    {
        List<Integrante.MonstroAtual> alvosInvalidos = new List<Integrante.MonstroAtual>();

        foreach (var alvo in AlvoAcao)//varre alvos
        {
            foreach (var statusMonstro in alvo.GetMonstro.StatusSecundario)//varre statusSec dos alvos
            {
                if (alvo.GetMonstro.StatusSecundario.Count > 0 && alvo.GetMonstro != GetMonstro) // Verifica se o monstro possui algum statusSecundario
                {
                    if (statusSecundarios.Count > 0) //Caso o ataque tenha algum statusSecundario para Verificar
                    {
                        foreach (var statusSecundariosComando in statusSecundarios)//varre statusSec do comando
                        {
                            if (statusMonstro.ForaDeCombate() == true) //Caso o status seja um que impe�a o alvo
                            {
                                if (statusMonstro.name != statusSecundariosComando.name) // caso seja um ataqueExec��o em rela��o ao status
                                {
                                    alvosInvalidos.Add(alvo);
                                }
                            }
                        }
                    }
                    else if (statusMonstro.GetTipoStatus != global::StatusEffectSecundario.TipoStatus.Locked) //Se n�o possui automaticamente o ataque n�o pode acertar alvo
                    {
                        alvosInvalidos.Add(alvo);
                    }
                }
            }
        }
        List<int> indices = new List<int>();

        for (int i = 0; i < AlvoAcao.Count; i++)
        {
            for (int j = 0; j < alvosInvalidos.Count; j++)
            {
                if(AlvoAcao[i].GetMonstro==alvosInvalidos[j].GetMonstro)
                {
                    indices.Add(i);
                }
            }
        }
        AlvoComAtaquesValidos = new List<bool>();
        for (int i = 0; i < AlvoAcao.Count; i++)
        {
            alvoComAtaquesValidos.Add(true);
        }
        for (int i = 0; i < alvoComAtaquesValidos.Count; i++)
        {
            for (int j = 0; j < indices.Count; j++)
            {
                if(i==indices[j])
                {
                    alvoComAtaquesValidos[i] = false;
                }
            }
        }
    }
    public void AplicarStatusSecundarioEmSelf()
    {
        if (statusAplicarSelf == null)
            return;

        if (!statusSecundarioAplicado)
        {
            GetMonstroInBattle.AplicarStatusSecundario(origem.MonstrosAtuais[indiceMonstro], StatusEffectSecundarioSelf);
            statusSecundarioAplicado = true;
        }
    }
    public void ConsumirRecurso()
    {
        if (consumiuRecurso)
            return;
        
        if (ConsomePP)
        {
            foreach (AttackHolder ataqueDoMonstro in GetMonstro.Attacks)
            {
                ataqueDoMonstro.PP--;
            }
        }
        else
        {
            GetMonstro.AtributosAtuais.Mana -= CustoMana;
        }
        
        consumiuRecurso = true;
    }
    
    public void RemoverStatusSecundarioEmSelf()
    {
        if (statusAplicarSelf == null)
            return;

        GetMonstro.RemoverStatusSecundario(statusAplicarSelf);
        statusSecundarioAplicado = false;

    }

    private void OnValidate()
    {
        powerMargin = CreatePowerMargin();
    }

    public int CreatePowerMargin()
    {
        var manaDiff = custoMana * 0.5f;
        var chanceAcertoDiff = ((attackData.ChanceAcerto - 100) * -0.5f);
        var poder = attackData.Poder;
        var prioridadeDiff = prioridade > 2 ? (prioridade - 2) * 5 : 0;
        var targetTypeDiff = GetPowerFromTargetType();
        return (int) (poder - chanceAcertoDiff - manaDiff + prioridadeDiff + targetTypeDiff + setPowerMargin);
    }

    private int GetPowerFromTargetType()
    {
        switch (target)
        {
            case TipoTarget.Self:
                return 1;

            case TipoTarget.Aliado:
                return 2;

            case TipoTarget.TimeAliado:
                return 7;

            case TipoTarget.Inimigo:
                return 2;

            case TipoTarget.TimeInimigo:
                return 7;

            case TipoTarget.TodosExcetoSelf:
                return 1;

            case TipoTarget.Aleatorio:
                return 1;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

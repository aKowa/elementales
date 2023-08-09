using BergamotaLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class Monster
{
    //Constantes
    public const int maxCombatLessonsAtivas = 2;

    //Variaveis
    [SerializeField] private string nickName;

    [SerializeField] private MonsterData _monsterData;

    [SerializeField] private List<StatusEffectBase> status = new List<StatusEffectBase>();
    private List<StatusEffectSecundario> statusSecundario = new List<StatusEffectSecundario>();

    [Header("Ataques")]
    [SerializeField] private List<AttackHolder> attacks;

    [Header("Atributos")]
    [SerializeField] private MonsterAttributes _atributosAtuais;

    [Header("Info")]
    [SerializeField] private MonsterInfo monsterInfo;

    [Header("Dices")] 
    [SerializeField] private string diceMaterial = "Default";
    
    [SerializeField] private List<DiceType> dices = new List<DiceType>();
    
    [SerializeField] private List<CombatLesson> combatLessonsAtivos = new List<CombatLesson>();
    [SerializeField] private List<CombatLesson> combatLessons = new List<CombatLesson>();

    private bool buffDano;
    //Getters
    public string NickName
    {
        get
        {
            if (nickName != null && nickName != string.Empty)
            {
                return nickName;
            }
            else
            {
                return _monsterData.GetName;
            }
        }
        set => nickName = value;
    }

    public MonsterInfo MonsterInfo
    {
        get => monsterInfo;
        set => monsterInfo = value;
    }

    public string DiceMaterial
    {
        get => diceMaterial;
        set => diceMaterial = value;
    }
    public bool BuffDano
    {
        get => buffDano;
        set => buffDano = value;
    }
    public string NicknameRaw => nickName;

    public int Nivel => _atributosAtuais.Nivel;

    public List<StatusEffectBase> Status => status;
    public List<StatusEffectSecundario> StatusSecundario => statusSecundario;

    public List<AttackHolder> Attacks => attacks;
    public MonsterAttributes AtributosAtuais => _atributosAtuais;
    public MonsterData MonsterData => _monsterData;

    public List<DiceType> Dices => dices;

    public List<CombatLesson> CombatLessonsAtivos => combatLessonsAtivos;
    public List<CombatLesson> CombatLessons => combatLessons;
    public bool IsFainted => _atributosAtuais.Vida <= 0;

    //Contrutores
    public Monster(MonsterSave monsterSave)
    {
        nickName = monsterSave.nickName;
        _monsterData = GlobalSettings.Instance.Listas.ListaDeMonsterData.GetData(monsterSave.monsterDataID);

        attacks = new List<AttackHolder>();

        foreach (AttackHolderSave ataque in monsterSave.attacks)
        {
            attacks.Add(new AttackHolder(ataque));
        }

        _atributosAtuais = new MonsterAttributes(monsterSave.monsterAttributes);

        monsterInfo = new MonsterInfo(monsterSave.monsterInfo);
        
        foreach (StatusEffectSave statusEffect in monsterSave.status)
        {
            StatusEffectBase novoStatus = AplicarStatus(GlobalSettings.Instance.Listas.ListaDeStatusEffects.GetData(statusEffect.statusID));

            if (novoStatus != null)
            {
                novoStatus.CarregarVariaveis(statusEffect);
            }
        }

        //Dados

        diceMaterial = monsterSave.diceMaterial;
        dices = monsterSave.dices.ToList();

        foreach(int combatLessonID in monsterSave.combatLessonsAtivosID)
        {
            Comando comando = GlobalSettings.Instance.Listas.ListaDeComandos.GetData(combatLessonID);

            if (comando is CombatLesson)
            {
                combatLessonsAtivos.Add((CombatLesson)comando);
            }
            else
            {
                Debug.LogWarning("O comando " + comando.name + " nao e um Combat Lesson!");
            }
        }

        foreach (int combatLessonID in monsterSave.combatLessonsID)
        {
            Comando comando = GlobalSettings.Instance.Listas.ListaDeComandos.GetData(combatLessonID);

            if (comando is CombatLesson)
            {
                combatLessons.Add((CombatLesson)comando);
            }
            else
            {
                Debug.LogWarning("O comando " + comando.name + " nao e um Combat Lesson!");
            }
        }
    }

    public Monster(MonsterData monsterData, int nivel, List<ComandoDeAtaque> ataques = null, int howManyAttacks = 4)
    {
        _monsterData = monsterData;

        attacks = new List<AttackHolder>();
        _atributosAtuais = new MonsterAttributes(_monsterData, nivel);
        
        if (ataques != null)
        {
            foreach (ComandoDeAtaque ataque in ataques)
            {
                attacks.Add(new AttackHolder(ataque));
            }
        }
        else
        {
            GetRandomAttacksFromAvailableUpgrades(monsterData, nivel, howManyAttacks)
                .ForEach(attack => attacks.Add(new AttackHolder(attack)));
        }

        CriarDadosNivelPorNivel(nivel);
        CriarCombatLessonsPorNivel(nivel);
    }

    public Monster(Monster monstro)
    {
        nickName = monstro.NickName;
        _monsterData = monstro.MonsterData;

        attacks = new List<AttackHolder>();

        foreach (AttackHolder ataque in monstro.Attacks)
        {
            attacks.Add(new AttackHolder(ataque.Attack));
        }

        _atributosAtuais = new MonsterAttributes(monstro.AtributosAtuais);

        dices = monstro.dices.ToList();
        diceMaterial = monstro.diceMaterial;

        if(string.IsNullOrEmpty(diceMaterial))
        {
            diceMaterial = "Default";
        }

        combatLessonsAtivos = monstro.CombatLessonsAtivos.ToList();
        combatLessons = monstro.CombatLessons.ToList();
    }

    public bool LevelUp(int quantidadExp)
    {
        //Debug.Log($"{NickName} recebeu {quantidadExp} XP.");
        bool passouDeNivel = _atributosAtuais.SomarExp(quantidadExp);
        if (passouDeNivel)
        {
            _atributosAtuais.GerarNovosAtributos(_monsterData);
        }

        return passouDeNivel;
    }

    #region Dados

    public (UpgradesPerLevel.DiceImprovement, DiceType, DiceType) VerificarMelhoriaDadoNoNivelAtual()
    {
        for (int i = 0; i < MonsterData.GetMonsterUpgradesPerLevel.Count; i++)
        {
            if (MonsterData.GetMonsterUpgradesPerLevel[i].Level == Nivel)//procura o upgrade do nivel correspondente
            {
                return VerificarProgressaoDado(i);
            }
        }

        return (UpgradesPerLevel.DiceImprovement.None, DiceType.D4, DiceType.D4);
    }

    public void CriarDadosNivelPorNivel(int nivelDoMonstro)
    {
        for (int i = 0; i < MonsterData.GetMonsterUpgradesPerLevel.Count; i++)
        {
            if (MonsterData.GetMonsterUpgradesPerLevel[i].Level > nivelDoMonstro)//monstro para o for caso o nivel do upgrade seja maior que o nivel do proprio monstro
                break;

            VerificarProgressaoDado(i);
        }
    }

    private void CriarCombatLessonsPorNivel(int nivelDoMonstro)
    {
        for (int i = 0; i < MonsterData.GetMonsterUpgradesPerLevel.Count; i++)
        {
            UpgradesPerLevel upgrade = MonsterData.GetMonsterUpgradesPerLevel[i];
            if (upgrade.Level > nivelDoMonstro)//monstro para o for caso o nivel do upgrade seja maior que o nivel do proprio monstro
                break;

            if (upgrade.Lesson)
            {
                AddCombatLesson(upgrade.Lesson);
            }
        }
    }

    private (UpgradesPerLevel.DiceImprovement, DiceType, DiceType) VerificarProgressaoDado(int indiceUpgradePerLevel)
    {
        UpgradesPerLevel.DiceImprovement tipoDeUpgrade = MonsterData.GetMonsterUpgradesPerLevel[indiceUpgradePerLevel].DiceUpgrade;
        DiceType dado1 = DiceType.D4;
        DiceType dado2 = DiceType.D4;

        switch (tipoDeUpgrade)
        {
            case UpgradesPerLevel.DiceImprovement.New://Adiciona um d4
                if(MonsterData.GetMonsterUpgradesPerLevel[indiceUpgradePerLevel].Level == 1)
                {
                    //Debug.Log($"{nickName}: New, lvl 1: Adding a D6");
                    dices.Add(DiceType.D6);

                    dado1 = DiceType.D6;
                }
                else
                {
                    //Debug.Log($"{nickName}: New: Adding a D4");
                    dices.Add(DiceType.D4);

                    dado1 = DiceType.D4;
                }

                break;

            case UpgradesPerLevel.DiceImprovement.Better://melhora o menor dado

                (DiceType valorNovoDado, int indiceDado) = MelhorarMenorDado(dices);

                dado1 = dices[indiceDado];
                dado2 = valorNovoDado;

                //Debug.Log($"{nickName}: Better: Improving dice {indiceDado} to a {valorNovoDado.ToString()}");

                dices[indiceDado] = valorNovoDado;

                break;
        }

        return (tipoDeUpgrade, dado1, dado2);
    }

    public List<ComandoDeAtaque> GetRandomAttacksFromAvailableUpgrades(MonsterData monster, int nivel, int howManyAttacks)
    {
        List<ComandoDeAtaque> ataquesFinais = new List<ComandoDeAtaque>();
        List<UpgradesPerLevel> upgradesPerLevel = new List<UpgradesPerLevel>();
        List<WeightedAttack> weightedAttacks = new List<WeightedAttack>();

        int currentMaxWeight = 0;
        int totalWeight = 0;
        // bool canGetStatus = false;
        upgradesPerLevel = monster.GetMonsterUpgradesPerLevel.Where(upgrade => upgrade.Level <= nivel && upgrade.Ataque != null).ToList();
        for (int i = 0; i < upgradesPerLevel.Count; i++)
        {
            int maxWeight = CalculateWeigth(upgradesPerLevel[i].Level, ref currentMaxWeight, ref totalWeight);
            weightedAttacks.Add(new WeightedAttack()
            {
                ataque = upgradesPerLevel[i].Ataque,
                maxWeight = maxWeight,
                weight = currentMaxWeight
            });
        }

        int totalAvailableWeight = totalWeight;
        for (int i = 0; i < howManyAttacks; i++)
        {
            WeightedAttack chosenAttack = new WeightedAttack();
            int randomWeight;
            
            //Choose a non-status attack for the first attack
            if (ataquesFinais.Count == 0)
            {
                chosenAttack = weightedAttacks.First(weightedAttack =>
                    weightedAttack.ataque.AttackData.Categoria != AttackData.CategoriaEnum.Status);
            }
            else
            {
                randomWeight = Random.Range(1, totalWeight + 1);
                chosenAttack = weightedAttacks.First(weightedAttack =>
                    weightedAttack.maxWeight >= randomWeight 
                    || weightedAttack.maxWeight >= totalAvailableWeight);
            }
            
            ataquesFinais.Add(chosenAttack.ataque);
            totalAvailableWeight -= (chosenAttack.weight);
            currentMaxWeight--;
            weightedAttacks.Remove(chosenAttack);
        }

        return ataquesFinais;
    }
    
    private int CalculateWeigth(int level, ref int currentMaxWeight, ref int maxWeight)
    {
        if (level > currentMaxWeight)
        {
            currentMaxWeight++;
        }

        maxWeight += currentMaxWeight;
        
        return maxWeight;
    }
    
    private (DiceType, int) MelhorarMenorDado(List<DiceType> listaDeDados)
    {
        int indiceMenorDado = -1;
        int valorMenorDado = 100;

        for (int j = 0; j < listaDeDados.Count; j++)
        {
            if ((int)listaDeDados[j] < valorMenorDado)
            {
                valorMenorDado = (int)listaDeDados[j];
                indiceMenorDado = j;
            }
        }

        switch (dices[indiceMenorDado])
        {
            case DiceType.D4:
                return (DiceType.D6, indiceMenorDado);
            case DiceType.D6:
                return (DiceType.D8, indiceMenorDado);
            case DiceType.D8:
                return (DiceType.D10, indiceMenorDado);
            case DiceType.D10:
                return (DiceType.D12, indiceMenorDado);
            case DiceType.D12:
                return (DiceType.D20, indiceMenorDado);
        }

        return (DiceType.D4, 0);
    }

    #endregion

    public bool VerificarAplicarStatusEffect(StatusEffectParaAplicar status, int atributo)
    {
        if (!status.GetPorcentagemVariavel)
        {
            return VerificarSeVaiTomarAtaque(status.GetPorcentagem);
        }

        return VerificarSeVaiTomarAtaque(status.GetPorcentagem * atributo);
    }

    public StatusEffectBase AplicarStatus(StatusEffectBase status)
    {
        int n1 = 0;
        int indice = 0;
        bool contem = false;
        for (int i = 0; i < this.status.Count; i++)
        {
            if (this.status[i].GetTipoStackStatus != StatusEffectBase.TipoStackStatus.Stacavel)
                n1++;

            if (this.status[i].name == status.name)
            {
                contem = true;
                indice = i;
            }
        }

        if (!contem)
        {
            //Se o tipo do monstro estiver na lista de imunes
            if (status.GetStatusEffectOpcoesDentroCombate.TiposImunes.Intersect(_monsterData.GetMonsterTypes).Any())
            {
                return null;
            }

            if (status.GetTipoStackStatus == StatusEffectBase.TipoStackStatus.Exclusivo && n1 != 0)
            {
                return null;
            }
            else
            {
                var temp = ScriptableObject.Instantiate(status);
                temp.name = status.name;
                temp.Init();
                this.status.Add(temp);
                temp.AplicarModificador(this);
                return temp;
            }
        }
        else
        {
            this.status.RemoveAt(indice);
            var temp = ScriptableObject.Instantiate(status);
            temp.name = status.name;
            temp.Init();
            this.status.Add(temp);
            temp.AplicarModificador(this);
            return temp;
        }
    }
    public bool AplicarStatusSecundario(StatusEffectSecundario status)
    {
        bool statusUnico = true;

        foreach (var item in this.statusSecundario)
        {
            if (item.name == status.name)
                statusUnico = false;
        }

        if (statusUnico)
        {
            StatusEffectSecundario statusSecundario = ScriptableObject.Instantiate(status);
            statusSecundario.name = status.name;
            this.statusSecundario.Add(statusSecundario);
        }

        return statusUnico;
    }
    public void RemoverStatus(StatusEffectBase status)
    {
        status.RemoverModificador(this);
        this.status.Remove(status);
    }
    public void LimparStatusPrimario()
    {
        status.Clear();
    }

    public void RemoverStatusPorTipo(StatusEffectBase status)
    {
        foreach (StatusEffectBase s in this.status)
        {
            if (s.name == status.name)
            {
                this.status.Remove(s);
                return;
            }
        }
    }
    public void RemoverStatusSecundario(StatusEffectSecundario status)
    {
        foreach (StatusEffectSecundario s in statusSecundario)
        {
            if (s.name == status.name)
            {
                statusSecundario.Remove(s);
                return;
            }
        }
    }
    public void  LimparStatusSecundario()
    {
        statusSecundario.Clear();
    }
    public void LimparTodosStatus()
    {
        LimparStatusPrimario();
        LimparStatusSecundario();
    }
    public void LimparModificadores()
    {
        AtributosAtuais.LimparModificadores();
    }
    public void LimparMonstroMorto()
    {
        LimparModificadores();
        LimparTodosStatus();
    }
    public bool VerificarSeVaiTomarAtaque(float chanceAtaque)
    {
        float n1 = UnityEngine.Random.Range(0f, 100f);
        float n2 = UnityEngine.Random.Range(0f, 100f);
        float media = (n1 + n2) / 2;
        if (media <= chanceAtaque)
        {
            return true;
        }
        return false;
    }

    public (float n1,float n2) TomarAtaque(int dano, ComandoDeAtaque comando)
    {
        float modificadorTipoMonstro = 1;
        foreach (TypeRelation tipoAtaque in comando.AttackData.TipoAtaque.VantagemContra)
        {
            foreach (MonsterType tipoMonstro in _monsterData.GetMonsterTypes)
            {
                if (tipoAtaque.GetMonsterType == tipoMonstro)
                {
                    modificadorTipoMonstro *= tipoAtaque.modifier;
                }
            }
        }
        return (dano, modificadorTipoMonstro);
    }

    public void TomarDanoPuro(int dano)
    {
        _atributosAtuais.Vida -= dano;
    }

    public (bool, int) TomarDano(int dano, MonsterType tipoAtaque)
    {
        if (IsFainted)
        {
            return (IsFainted, 0);
        }

        int vidaPerdida = ReduzirVida(dano);

        for (int i = 0; i < status.Count; i++)
        {
            if(status[i].VerificarExcecao(tipoAtaque))
            {
                //Debug.Log($"O status {status[i].Nome} reagiu com o ataque, o monstro se livrou");
                RemoverStatus(status[i]);
                break;
            }
        }

        return (IsFainted, vidaPerdida);
    }

    public int ReduzirVida(int dano)
    {
        int vidaInicio = _atributosAtuais.Vida;

        _atributosAtuais.Vida -= dano;

        return vidaInicio - _atributosAtuais.Vida;
    }

    public void ReceberCura(int quantidadeCura)
    {
        _atributosAtuais.Vida += quantidadeCura;

        if (_atributosAtuais.Vida > _atributosAtuais.VidaMax)
        {
            _atributosAtuais.Vida = _atributosAtuais.VidaMax;
        }
    }

    public void RecuperarMana(int quantidadeMana)
    {
        _atributosAtuais.Mana += quantidadeMana;

        if (_atributosAtuais.Mana > _atributosAtuais.ManaMax)
        {
            _atributosAtuais.Mana = _atributosAtuais.ManaMax;
        }
    }

    public void RecuperarManaPorcentagem(float porcentagemMana)
    {
        int quantidadeMana =(int)(_atributosAtuais.ManaMax* (porcentagemMana/100));
        _atributosAtuais.Mana += quantidadeMana;

        if (_atributosAtuais.Mana > _atributosAtuais.ManaMax)
        {
            _atributosAtuais.Mana = _atributosAtuais.ManaMax;
        }
    }

    public ComandoDeAtaque VerificarSePodeAprenderUmAtaqueNoNivelAtual()
    {
        return _monsterData.VerificarSePodeAprenderUmAtaqueNoNivel(_atributosAtuais.Nivel);
    }

    public CombatLesson VerificarSePodeAprenderUmCombatLessonNoNivelAtual()
    {
        return _monsterData.VerificarSePodeAprenderUmCombatLessonNoNivel(_atributosAtuais.Nivel);
    }

    public void AddCombatLesson(CombatLesson combatLesson)
    {
        if(combatLessons.Contains(combatLesson) == false)
        {
            combatLessons.Add(combatLesson);

            if(combatLessonsAtivos.Count < maxCombatLessonsAtivas)
            {
                combatLessonsAtivos.Add(combatLesson);
            }

            combatLessons = combatLessons.OrderBy(x => x.Nome).ToList();
        }
        else
        {
            Debug.Log($"O Combat Lesson \"{combatLesson.name}\" tentou ser adicionado no monstro \"{nickName}\", mas ele ja tem este Combat Lesson!");
        }
    }

    public bool VerificarSePossuiAtaque(ComandoDeAtaque ataque)
    {
        for (int i = 0; i < attacks.Count; i++)
        {
            if (attacks[i].Attack.ID == ataque.ID)
            {
                return true;
            }
        }

        return false;
    }

    public bool VerificarSePossuiCombatLesson(CombatLesson combatLesson)
    {
        for (int i = 0; i < combatLessons.Count; i++)
        {
            if (combatLessons[i].ID == combatLesson.ID)
            {
                return true;
            }
        }

        return false;
    }
}

[System.Serializable]
public struct MonsterInfo
{
    //Variaveis
    [SerializeField] private string firstTrainer;
    [SerializeField] private string where;
    [SerializeField] private int levelMet;
    private DateTime dataMet;

    //Getters
    public string FirstTrainer => firstTrainer;
    public string Where => where;
    public int LevelMet => levelMet;
    public DateTime DataMet => dataMet;

    public MonsterInfo(PlayerData playerData, Monster monster)
    {
        firstTrainer = playerData.GetPlayerName;
        dataMet = DateTime.Now;
        where = SceneSpawnManager.NomeDoMapaAtual;
        levelMet = monster.Nivel;
    }

    public MonsterInfo(MonsterInfoSave monsterInfoSave)
    {
        firstTrainer = monsterInfoSave.firstTrainer;
        dataMet = BergamotaLibrary.SerializableDateTime.NewDateTime(monsterInfoSave.dataMet);
        where = monsterInfoSave.where;
        levelMet = monsterInfoSave.levelMet;
    }
}

public struct WeightedAttack
{
    public ComandoDeAtaque ataque;
    public int maxWeight;
    public int weight;
}
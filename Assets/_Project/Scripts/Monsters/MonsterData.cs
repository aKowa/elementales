using System;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Monster", menuName = "Monster/Monster Data", order = 1)]
public class MonsterData : ScriptableObject
{
    [BoxGroup("Informações Básicas")]
    [SerializeField] private string _name;

    [BoxGroup("Informações Básicas")]
    [SerializeField] private string categoria;

    [BoxGroup("Informações Básicas"), TextArea(2, 5)]
    [SerializeField] private string descricao;

    [BoxGroup("Informações Básicas")]
    [SerializeField] private float altura, peso;

    [BoxGroup("Tipos"), GUIColor("GetRarityColor")]
    [SerializeField] private MonsterRarityEnum monsterRarity = MonsterRarityEnum.basico;

    [BoxGroup("Tipos"), ListDrawerSettings(ElementColor = "$GetElementColor")]
    [SerializeField] private List<MonsterType> monsterTypes;

    [HorizontalGroup("Atributos", 75), PreviewField(75), HideLabel]
    [SerializeField] private Sprite miniatura;

    [HorizontalGroup("Atributos/Stats/Split", MaxWidth = 100), VerticalGroup("Atributos/Stats/Split/Right")]
    [HideLabel, SuffixLabel("Total", true), ReadOnly]
    [SerializeField] private int _total;

    [VerticalGroup("Atributos/Stats"), HideLabel, LabelWidth(100)]
    [SerializeField] private MonsterBaseAttributes _attributesBase;

    [SerializeField] private AnimatorOverrideController animator;

    [BoxGroup("Ataques"), SerializeField, GUIColor("$IsComplete")] private bool complete;

    [BoxGroup("Ataques")]
    [SerializeField] private List<UpgradesPerLevel> upgradesPerLevels;

    [BoxGroup("Ataques")/*, TableList*/, ListDrawerSettings(ElementColor = "$GetColorForAttack")]
    [SerializeField] private List<ComandoDeAtaque> learnableAttacks;

    [BoxGroup("Ataques")] [SerializeField] private List<CombatLessonPorNivel> combatLessonsPorNivel = new List<CombatLessonPorNivel>();

    [BoxGroup("Ataques")] [SerializeField] private int numAttacks, numDiceUpgrades;

    [BoxGroup("Sons")]
    [SerializeField] private AudioClip audioEntrada, audioMorrendo;

    //Getter

    public int ID => GlobalSettings.Instance.Listas.ListaDeMonsterData.GetId(this);
    public int MonsterBookID => ID + 1;

    public string GetName => _name;
    public string Categoria => categoria;
    public string Descricao => descricao;
    public float Altura => altura;
    public float Peso => peso;

    public List<MonsterType> GetMonsterTypes => monsterTypes;

    public MonsterBaseAttributes GetBaseMonsterAttributes => _attributesBase;

    public List<UpgradesPerLevel> GetMonsterUpgradesPerLevel => upgradesPerLevels;

    public List<ComandoDeAtaque> GetMonsterLearnableAttacks => learnableAttacks;

    public Sprite Miniatura => miniatura;

    public AnimatorOverrideController Animator => animator;

    public MonsterRarityEnum MonsterRarity => monsterRarity;
    public AudioClip AudioClipMorrendo => audioMorrendo;

    public AudioClip AudioEntrada => audioEntrada;

    public ComandoDeAtaque VerificarSePodeAprenderUmAtaqueNoNivel(int nivel)
    {
        for(int i = 0; i < upgradesPerLevels.Count; i++)
        {
            if (upgradesPerLevels[i].Level == nivel)
            {
                if(upgradesPerLevels[i].Ataque)
                    return upgradesPerLevels[i].Ataque;
            }
        }

        return null;
    }

    public CombatLesson VerificarSePodeAprenderUmCombatLessonNoNivel(int nivel)
    {
        for(int i = 0; i < upgradesPerLevels.Count; i++)
        {
            if (upgradesPerLevels[i].Level == nivel)
            {
                if(upgradesPerLevels[i].Lesson)
                    return upgradesPerLevels[i].Lesson;
            }
        }

        return null;
    }
    
    public UpgradesPerLevel.DiceImprovement VerificarSePodeGanharMelhoriaDadoNoNivel(int nivel)
    {
        for(int i = 0; i < upgradesPerLevels.Count; i++)
        {
            if (upgradesPerLevels[i].Level == nivel)
            {
                if(upgradesPerLevels[i].DiceUpgrade != UpgradesPerLevel.DiceImprovement.None)
                    return upgradesPerLevels[i].DiceUpgrade;
            }
        }

        return UpgradesPerLevel.DiceImprovement.None;
    }

    #region Editor Stuff

#if UNITY_EDITOR
    
    private void OnValidate()
    {
        _total = _attributesBase.TotalPontosAtributosBase();
    }

    [Button]
    private void ApplyCombatLessonsToUpgrades()
    {
         for (int i = 0; i < combatLessonsPorNivel.Count; i++)
         {
             if (upgradesPerLevels.Exists(upgrade => upgrade.Level == combatLessonsPorNivel[i].nivel))
             {
                 UpgradesPerLevel upgrade = upgradesPerLevels.First(upgrade => upgrade.Level == combatLessonsPorNivel[i].nivel);
                 upgrade.Lesson = combatLessonsPorNivel[i].combatLesson;
             }
             else
             {
                 upgradesPerLevels.Add(new UpgradesPerLevel()
                 {
                     Level = combatLessonsPorNivel[i].nivel,
                     Lesson = combatLessonsPorNivel[i].combatLesson
                 });
             }
         }

         upgradesPerLevels = upgradesPerLevels.OrderBy(upgrade => upgrade.Level).ToList();
         // combatLessonsPorNivel.Clear();
    }

    [Button]
    private void AdaptCombatLessonsListToRarity()
    {
        CombatLesson lesson1 = combatLessonsPorNivel[0].combatLesson;
        CombatLesson lesson2 = combatLessonsPorNivel[1].combatLesson;
        CombatLesson lesson3 = combatLessonsPorNivel[2].combatLesson;
        CombatLesson lesson4 = combatLessonsPorNivel[3].combatLesson;
        CombatLesson lesson5 = combatLessonsPorNivel[4].combatLesson;
        CombatLesson lesson6 = combatLessonsPorNivel[5].combatLesson;

        switch (monsterRarity)
        {
            case MonsterRarityEnum.basico:
                combatLessonsPorNivel[0] = new CombatLessonPorNivel(){nivel = Random.Range(11, 14), combatLesson = lesson1};
                combatLessonsPorNivel[1] = new CombatLessonPorNivel(){nivel = Random.Range(20, 28), combatLesson = lesson2};
                combatLessonsPorNivel[2] = new CombatLessonPorNivel(){nivel = Random.Range(30, 34), combatLesson = lesson3};
                combatLessonsPorNivel[3] = new CombatLessonPorNivel(){nivel = Random.Range(40, 50), combatLesson = lesson4};
                combatLessonsPorNivel[4] = new CombatLessonPorNivel(){nivel = Random.Range(54, 60), combatLesson = lesson5};
                combatLessonsPorNivel[5] = new CombatLessonPorNivel(){nivel = Random.Range(60, 64), combatLesson = lesson6};
                break;
            case MonsterRarityEnum.raro:
                combatLessonsPorNivel[0] = new CombatLessonPorNivel(){nivel = Random.Range(8, 10), combatLesson = lesson1};
                combatLessonsPorNivel[1] = new CombatLessonPorNivel(){nivel = Random.Range(15, 23), combatLesson = lesson2};
                combatLessonsPorNivel[2] = new CombatLessonPorNivel(){nivel = Random.Range(24, 28), combatLesson = lesson3};
                combatLessonsPorNivel[3] = new CombatLessonPorNivel(){nivel = Random.Range(36, 40), combatLesson = lesson4};
                combatLessonsPorNivel[4] = new CombatLessonPorNivel(){nivel = Random.Range(47, 53), combatLesson = lesson5};
                combatLessonsPorNivel[5] = new CombatLessonPorNivel(){nivel = Random.Range(58, 64), combatLesson = lesson6};
                break;
            case MonsterRarityEnum.exotico:
                combatLessonsPorNivel[0] = new CombatLessonPorNivel(){nivel = Random.Range(6, 8), combatLesson = lesson1};
                combatLessonsPorNivel[1] = new CombatLessonPorNivel(){nivel = Random.Range(14, 23), combatLesson = lesson2};
                combatLessonsPorNivel[2] = new CombatLessonPorNivel(){nivel = Random.Range(24, 28), combatLesson = lesson3};
                combatLessonsPorNivel[3] = new CombatLessonPorNivel(){nivel = Random.Range(36, 40), combatLesson = lesson4};
                combatLessonsPorNivel[4] = new CombatLessonPorNivel(){nivel = Random.Range(47, 53), combatLesson = lesson5};
                combatLessonsPorNivel[5] = new CombatLessonPorNivel(){nivel = Random.Range(56, 64), combatLesson = lesson6};
                break;
            case MonsterRarityEnum.lendario:
                combatLessonsPorNivel[0] = new CombatLessonPorNivel(){nivel = Random.Range(5, 7), combatLesson = lesson1};
                combatLessonsPorNivel[1] = new CombatLessonPorNivel(){nivel = Random.Range(14, 23), combatLesson = lesson2};
                combatLessonsPorNivel[2] = new CombatLessonPorNivel(){nivel = Random.Range(24, 28), combatLesson = lesson3};
                combatLessonsPorNivel[3] = new CombatLessonPorNivel(){nivel = Random.Range(36, 40), combatLesson = lesson4};
                combatLessonsPorNivel[4] = new CombatLessonPorNivel(){nivel = Random.Range(47, 53), combatLesson = lesson5};
                combatLessonsPorNivel[5] = new CombatLessonPorNivel(){nivel = Random.Range(56, 64), combatLesson = lesson6};
                break;
        }
    }

    [Button]
    private void GenerateCombatLessons()
    {
        combatLessonsPorNivel = new List<CombatLessonPorNivel>()
        {
            new CombatLessonPorNivel(){nivel = 12, combatLesson = null},
            new CombatLessonPorNivel(){nivel = 26, combatLesson = null},
            new CombatLessonPorNivel(){nivel = 32, combatLesson = null},
            new CombatLessonPorNivel(){nivel = 50, combatLesson = null},
            new CombatLessonPorNivel(){nivel = 70, combatLesson = null},
            new CombatLessonPorNivel(){nivel = 90, combatLesson = null}
        };
        
        var combatLessonsParentFolder = GlobalSettings.Instance.OrganizationSettings.combatLessonsParentFolder;
        string[] assets = AssetDatabase.FindAssets($" t:CombatLesson", new[] {$"{combatLessonsParentFolder}/"});
        List<CombatLesson> combatLessons = assets.Select(asset =>
             (CombatLesson)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(asset),
                 typeof(CombatLesson))).ToList();
        for (var i = 0; i < combatLessonsPorNivel.Count; i++)
        {
            List<CombatLesson> list = combatLessons.Where(c => (int)c.nivelPoder >= combatLessonsPorNivel[i].nivel)
                .OrderBy(c => (int)c.nivelPoder)
                .ToList();
            List<CombatLesson> curatedList = list.Where(c => c.nivelPoder == list.First().nivelPoder).ToList();
            Debug.Log($"{list.First().nivelPoder} é o poder escolhido para para nivel {combatLessonsPorNivel[i].nivel}");
            Debug.Log($"{curatedList.Count}");
            CombatLesson combatLesson = curatedList[Random.Range(0, curatedList.Count)];
            int nivel = combatLessonsPorNivel[i].nivel;
            combatLessonsPorNivel[i] = new CombatLessonPorNivel(){nivel =  nivel, combatLesson = combatLesson};
            combatLessons.Remove(combatLesson);
        }
    }

    private Color GetElementColor(int index, Color defaultColor)
    {
        return monsterTypes[index].TypeColor;
    }

    private Color GetColorForAttack(int index, Color defaultColor)
    {
        return learnableAttacks[index].AttackData.TipoAtaque.TypeColor;
    }

    private Color GetRarityColor()
    {
        return new Color((int)MonsterRarity * 0.01f, 1, 1);
    }
//
//     [HorizontalGroup("Atributos/Stats/Split", order: -1, MaxWidth = 100)]
//     [VerticalGroup("Atributos/Stats/Split/Left", order: -1)]
//     [Button("Gerar")]
//     public void CriarAtributosBase()
//     {
//         _total = _attributesBase.CriarAPartirDeTotal(GlobalSettings.Instance.Balance.BaseStats(MonsterRarity));
//     }
//
//     [Button("Gerar Mana")]
//     public void CriarManaAttribute()
//     {
//         _attributesBase.CriarManaAPartirDeTotal(GlobalSettings.Instance.Balance.BaseStats(MonsterRarity),
//             monsterTypes[0]);
//     }
//
//     [Button, DisableIf("@animator != null")]
//     public void GenerateAnimatorController()
//     {
//         AnimationClip[] animations = MonsterAnimatorGenerator.FindAnimationsOfName(this, GlobalSettings.Instance.OrganizationSettings.GetAnimationNames());
//         animator = MonsterAnimatorGenerator.GenerateAnimator(this, animations);
//         if (miniatura == null) MonsterAnimatorGenerator.GetFirstSpriteFromClip(animations[0]);
//     }
//
//     [Button]
//     public void GenerateAnimationImageProperty()
//     {
//         if (animator == null) return;
//         List<AnimationClip> overridenClips = new List<AnimationClip>();
//         foreach (var clip in animator.animationClips)
//         {
//             if (overridenClips.Contains(clip) == false)
//                 AnimationPropertyGenerator.GenerateImageAnimationProperty(clip);
//             overridenClips.Add(clip);
//         }
//     }
//
//     [Button]
//     public void GetAttacksThatMatchType()
//     {
//         var attacksFolder = GlobalSettings.Instance.OrganizationSettings.attackCommandsParentFolder;
//         string[] assets = AssetDatabase.FindAssets($" t:ComandoDeAtaque", new[] {$"{attacksFolder}/"});
//         Debug.Log(attacksFolder);
//         var comandosDeAtaque = assets.Select(asset =>
//             (ComandoDeAtaque)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(asset),
//                 typeof(ComandoDeAtaque)));
//         var selectedAttacks = comandosDeAtaque.Where(asset => monsterTypes.Contains(asset.AttackData.TipoAtaque));
//         learnableAttacks.AddRange(selectedAttacks);
//     }
//
//     [Button]
//     public void ClearAttacks() => learnableAttacks.Clear();
//
//     [Button]
//     public void SortAttacksByPowerMargin()
//     {
//         learnableAttacks = learnableAttacks.OrderBy(x => x.powerMargin).ToList();
//     }
//     
//     [Button]
//     public void AddUpgradesPerLevelFromLearnableAttacks()
//     {
//         upgradesPerLevels.Clear();
//         List<ComandoDeAtaque> learnableAttacksTemp = learnableAttacks.ToList();
//
//         for (int i = 0; i < 4; i++)
//         {
//             upgradesPerLevels.Add(new UpgradesPerLevel()
//             {
//                 Ataque = learnableAttacks[i],
//                 Level = 1
//             });
//             learnableAttacksTemp.Remove(learnableAttacks[i]);
//         }
//         
//         int level = 90/learnableAttacksTemp.Count;
//         for (int i = 0; i < learnableAttacksTemp.Count; i++)
//         {
//             upgradesPerLevels.Add(new UpgradesPerLevel()
//             {
//                 Ataque = learnableAttacksTemp[i],
//                 Level = level
//             });
//             level += 90/learnableAttacksTemp.Count;
//         }
//
//         CheckNumOfAttacksAndDiceUpgrades();
//     }
//
//     [Button]
//     public void GenerateTypeOfAttackTooltip()
//     {
//         upgradesPerLevels.ForEach(upgrade => upgrade.TypeOfAttack = upgrade.Ataque.AttackData.Categoria);
//     }
//
//     [Button]
//     public void GenerateDicesPerLevel()
//     {
//         int diceCyclePerMonsterRarity = 0;
//         switch (monsterRarity)
//         {
//             case MonsterRarityEnum.basico:
//                 diceCyclePerMonsterRarity = 4;
//                 break;
//             case MonsterRarityEnum.raro:
//                 diceCyclePerMonsterRarity = 6;
//                 break;
//             case MonsterRarityEnum.exotico:
//                 diceCyclePerMonsterRarity = 7;
//                 break;
//             case MonsterRarityEnum.lendario:
//                 diceCyclePerMonsterRarity = 9;
//                 break;
//         }
//
//         List<int> levelsToReceiveDice = new List<int>()
//         {
//             8,
//             16,
//             24,
//             32,
//             40,
//             48,
//             56,
//             64,
//             72
//         };
//         
//         
//         //Add for level 1
//         if (upgradesPerLevels.Exists(upgrade => upgrade.Level == 1))
//         {
//             UpgradesPerLevel upgrade = upgradesPerLevels.First(upgrade => upgrade.Level == 1);
//             upgrade.DiceUpgrade = UpgradesPerLevel.DiceImprovement.New;
//         }
//         else
//         {
//             upgradesPerLevels.Add(new UpgradesPerLevel()
//             {
//                 DiceUpgrade = UpgradesPerLevel.DiceImprovement.New,
//                 Level = 1
//             });
//         }
//         
//         //Add for the other levels
//         for (int i = 0; i < diceCyclePerMonsterRarity; i++)
//         {
//             if (upgradesPerLevels.Exists(upgrade => upgrade.Level == levelsToReceiveDice[i]))
//             {
//                 UpgradesPerLevel upgrade = upgradesPerLevels.First(upgrade => upgrade.Level == levelsToReceiveDice[i]);
//                 upgrade.DiceUpgrade = UpgradesPerLevel.DiceImprovement.Better;
//             }
//             else
//             {
//                 upgradesPerLevels.Add(new UpgradesPerLevel()
//                 {
//                     DiceUpgrade = UpgradesPerLevel.DiceImprovement.Better,
//                     Level = levelsToReceiveDice[i]
//                 });
//             }
//         }
//
//         upgradesPerLevels = upgradesPerLevels.OrderBy(upgrade => upgrade.Level).ToList();
//         CheckNumOfAttacksAndDiceUpgrades();
//     }
//
//     [Button]
//     public void CheckNumOfAttacksAndDiceUpgrades()
//     {
//         numAttacks = 0;
//         numDiceUpgrades = 0;
//         foreach (UpgradesPerLevel upgradesPerLevel in upgradesPerLevels)
//         {
//             if (upgradesPerLevel.Ataque != null)
//                 numAttacks++;
//             if (upgradesPerLevel.DiceUpgrade != UpgradesPerLevel.DiceImprovement.None)
//                 numDiceUpgrades++;
//         }
//     }
//
    public Color IsComplete()
    {
        if(complete)
            return Color.green;
        else
            return Color.white;
    }

#endif

#endregion

}

[System.Serializable]
public struct CombatLessonPorNivel
{
    public int nivel;
    public CombatLesson combatLesson;
}

[System.Serializable]
public class MonsterAttributes
{
    //Constantes
    private const int ivMaxRdn = 16;
    public const int nivelMax = 100;

    //Variaveis
    [SerializeField] private int _nivel;
    [SerializeField] private int _exp;

    [Header("Atributos")]
    [SerializeField] private int _vida;
    [SerializeField] private int _vidaMax;
    [SerializeField] private int _mana;
    [SerializeField] private int _manaMax;

    [SerializeField] private int _ataque;
    [SerializeField] private int _defesa;
    [SerializeField] private int _spAtaque;
    [SerializeField] private int _spDefesa;
    [SerializeField] private int _velocidade;

    [Header("Ivs")]
    [SerializeField] [Range(0, 16)] private int ivVida;
    [SerializeField] [Range(0, 16)] private int ivMana;
    [SerializeField] [Range(0, 16)] private int ivAtk;
    [SerializeField] [Range(0, 16)] private int ivDef;
    [SerializeField] [Range(0, 16)] private int ivSpAtk;
    [SerializeField] [Range(0, 16)] private int ivSpDef;
    [SerializeField] [Range(0, 16)] private int ivVel;

    [SerializeField] private Modificador modificadorDebuffAtaque;
    [SerializeField] private Modificador modificadorDebuffDefesa;
    [SerializeField] private Modificador modificadorDebuffSpAtaque;
    [SerializeField] private Modificador modificadorDebuffSpDefesa;
    [SerializeField] private Modificador modificadorDebuffVelocidade;

    //Getters
    public int Vida
    {
        get => _vida;
        set
        {
            _vida = Mathf.Clamp(value, 0, 999);
        }
    }
    public int Mana
    {
        get => _mana;
        set
        {
            _mana = Mathf.Clamp(value, 0, 999);
        }
    }

    public int VidaMax => _vidaMax;
    public int ManaMax => _manaMax;
    public int Ataque => _ataque;
    public int Defesa => _defesa;
    public int SpAtaque => _spAtaque;
    public int SpDefesa => _spDefesa;
    public int Velocidade => _velocidade;
    public int AtaqueComModificador { get { return (int)((float)Ataque * (float)((float)modificadorDebuffAtaque.Calc() / 100)); } }
    public int DefesaComModificador { get { return (int)((float)Defesa * (float)((float)modificadorDebuffDefesa.Calc() / 100)); } }
    public int SpAtaqueComModificador { get { return (int)((float)SpAtaque * (float)((float)modificadorDebuffSpAtaque.Calc() / 100)); } }
    public int SpDefesaComModificador { get { return (int)((float)SpDefesa * (float)((float)modificadorDebuffSpDefesa.Calc() / 100)); } }
    public int VelocidadeComModificador { get { return (int)((float)Velocidade * (float)((float)modificadorDebuffVelocidade.Calc() / 100)); } }
    public int Nivel
    {
        get => _nivel;
        set => _nivel = value;
    }

    public int Exp
    {
        get => _exp;
        set => _exp = value;
    }

    public int IvVida => ivVida;
    public int IvMana => ivMana;
    public int IvAtk => ivAtk;
    public int IvDef => ivDef;
    public int IvSpAtk => ivSpAtk;
    public int IvSpDef => ivSpDef;
    public int IvVel => ivVel;
    public int ExpParaOProxNivelRaw()
    {
        return (int)Mathf.Pow(_nivel + 1, 3);
    }

    public int ExpParaOProxNivel()
    {
        return (int)Mathf.Pow(_nivel + 1, 3) - Exp;
    }

    public int ExpParaONivelAtual()
    {
        return (int)Mathf.Pow(_nivel, 3);
    }

    public int ExpEmRelacaoAoNivelAtual()
    {
        return _exp - ExpParaONivelAtual();
    }
    public Modificador ModificadorDebuffAtaque => modificadorDebuffAtaque;
    public Modificador ModificadorDebuffDefesa => modificadorDebuffDefesa;
    public Modificador ModificadorDebuffSpAtaque => modificadorDebuffSpAtaque;
    public Modificador ModificadorDebuffSpDefesa => modificadorDebuffSpDefesa;
    public Modificador ModificadorDebuffVelocidade => modificadorDebuffVelocidade;

    //Setters

    public bool SomarExp(int quantidadeExp)
    {
        _exp += quantidadeExp;
        bool passouDeNivel = false;
        while (_exp >= ExpParaOProxNivelRaw() && _nivel < nivelMax)
        {
            Nivel++;
            passouDeNivel = true;

            //Debug.Log($"Level Up! Estava no nivel {Nivel - 1}, recebeu {quantidadeExp} e evoluiu para o nivel {Nivel}.");
        }
        return passouDeNivel;
    }

    //Construtores
    public MonsterAttributes(MonsterData monsterData, int nivel)
    {
        _nivel = nivel;
        _exp = ExpParaONivelAtual();

        GerarIvs();
        GerarNovosAtributos(monsterData);
        InstanciarModificadores();
    }

    public MonsterAttributes(MonsterAttributesSave monsterAttributesSave)
    {
        _nivel = monsterAttributesSave.nivel;
        _exp = monsterAttributesSave.exp;

        _vida = monsterAttributesSave.vida;
        _vidaMax = monsterAttributesSave.vidaMax;
        _mana = monsterAttributesSave.mana;
        _manaMax = monsterAttributesSave.manaMax;

        _ataque = monsterAttributesSave.ataque;
        _defesa = monsterAttributesSave.defesa;
        _spAtaque = monsterAttributesSave.spAtaque;
        _spDefesa = monsterAttributesSave.spDefesa;
        _velocidade = monsterAttributesSave.velocidade;

        ivVida = monsterAttributesSave.ivVida;
        ivMana = monsterAttributesSave.ivMana;
        ivAtk = monsterAttributesSave.ivAtk;
        ivDef = monsterAttributesSave.ivDef;
        ivSpAtk = monsterAttributesSave.ivSpAtk;
        ivSpDef = monsterAttributesSave.ivSpDef;
        ivVel = monsterAttributesSave.ivVel;

        InstanciarModificadores();
    }

    public MonsterAttributes(MonsterAttributes monsterAttributes)
    {
        _nivel = monsterAttributes.Nivel;
        _exp = monsterAttributes.Exp;

        _vida = monsterAttributes.Vida;
        _vidaMax = monsterAttributes.VidaMax;
        _mana = monsterAttributes.Mana;
        _manaMax = monsterAttributes.ManaMax;

        _ataque = monsterAttributes.Ataque;
        _defesa = monsterAttributes.Defesa;
        _spAtaque = monsterAttributes.SpAtaque;
        _spDefesa = monsterAttributes.SpDefesa;
        _velocidade = monsterAttributes.Velocidade;

        ivVida = monsterAttributes.IvVida;
        ivMana = monsterAttributes.IvMana;
        ivAtk = monsterAttributes.IvAtk;
        ivDef = monsterAttributes.IvDef;
        ivSpAtk = monsterAttributes.IvSpAtk;
        ivSpDef = monsterAttributes.IvSpDef;
        ivVel = monsterAttributes.IvVel;

        InstanciarModificadores();
    }

    private void GerarIvs()
    {
        ivVida = Random.Range(0, ivMaxRdn + 1);
        ivMana = Random.Range(0, ivMaxRdn + 1);
        ivAtk = Random.Range(0, ivMaxRdn + 1);
        ivDef = Random.Range(0, ivMaxRdn + 1);
        ivSpAtk = Random.Range(0, ivMaxRdn + 1);
        ivSpDef = Random.Range(0, ivMaxRdn + 1);
        ivVel = Random.Range(0, ivMaxRdn + 1);
    }

    public void GerarNovosAtributos(MonsterData monsterData)
    {
        _vida = CalcVida(monsterData.GetBaseMonsterAttributes.Vida, ivVida, _nivel);
        _vidaMax = _vida;
        _mana = CalcMana(monsterData.GetBaseMonsterAttributes.Mana, ivMana, _nivel);
        _manaMax = _mana;
        _ataque = CalcAtributo(monsterData.GetBaseMonsterAttributes.Ataque, ivAtk, _nivel);
        _defesa = CalcAtributo(monsterData.GetBaseMonsterAttributes.Defesa, ivDef, _nivel);
        _spAtaque = CalcAtributo(monsterData.GetBaseMonsterAttributes.SpAtaque, ivSpAtk, _nivel);
        _spDefesa = CalcAtributo(monsterData.GetBaseMonsterAttributes.SpDefesa, ivSpDef, _nivel);
        _velocidade = CalcAtributo(monsterData.GetBaseMonsterAttributes.Velocidade, ivVel, _nivel);
    }

    private int CalcVida(int hpBase, int iv, int level)
    {
        return ((2 * hpBase + iv) * level) / 100 + level + 10;
    }

    private int CalcMana(int manaBase, int iv, int level)
    {
        return ((manaBase + iv) * level) / 200 + level + 30;
    }

    public int CalcManaRegenStruggle()
    {
        return Mathf.RoundToInt(_manaMax * 0.33f + 50 / _nivel);
    }

    public static void TesteDeAtributos(int attBase, int iv, int level)
    {
        Debug.Log($"Vida: {((2 * attBase + iv) * level) / 100 + level + 10}");
        Debug.Log($"Mana: {((attBase + iv) * level) / 200 + level + 30}");
    }

    private int CalcAtributo(int atributo, int iv, int level)
    {
        return ((2 * atributo + iv) * level) / 100 + 5;
    }

    public int ReceberModificadorStatus(Modificador.Atributo atributo, int valorDebuff, bool passaComTempo, int numeroRounds)
    {
        switch (atributo)
        {
            case Modificador.Atributo.ataque:
                return modificadorDebuffAtaque.Adicionar(new ModificadorTurno(numeroRounds, passaComTempo, valorDebuff, modificadorDebuffAtaque.IdModificador));
            case Modificador.Atributo.defesa:
                return modificadorDebuffDefesa.Adicionar(new ModificadorTurno(numeroRounds, passaComTempo, valorDebuff, modificadorDebuffDefesa.IdModificador));
            case Modificador.Atributo.spAtaque:
                return modificadorDebuffSpAtaque.Adicionar(new ModificadorTurno(numeroRounds, passaComTempo, valorDebuff, modificadorDebuffSpAtaque.IdModificador));
            case Modificador.Atributo.spDefesa:
                return modificadorDebuffSpDefesa.Adicionar(new ModificadorTurno(numeroRounds, passaComTempo, valorDebuff, modificadorDebuffSpDefesa.IdModificador));
            case Modificador.Atributo.velocidade:
                return modificadorDebuffVelocidade.Adicionar(new ModificadorTurno(numeroRounds, passaComTempo, valorDebuff, modificadorDebuffVelocidade.IdModificador));
        }
        return 0;
    }
    public void ReceberModificadorStatus(Modificador.Atributo atributo, ModificadorTurno modificador)
    {
        switch (atributo)
        {
            case Modificador.Atributo.ataque:
                modificadorDebuffAtaque.Adicionar(modificador);
                break;
            case Modificador.Atributo.defesa:
                modificadorDebuffDefesa.Adicionar(modificador);
                break;
            case Modificador.Atributo.spAtaque:
                modificadorDebuffSpAtaque.Adicionar(modificador);
                break;
            case Modificador.Atributo.spDefesa:
                modificadorDebuffSpDefesa.Adicionar(modificador);
                break;
            case Modificador.Atributo.velocidade:
                modificadorDebuffVelocidade.Adicionar(modificador);
                break;
        }
    }
    public void LimparModificadores()
    {
        modificadorDebuffAtaque.LimparVariaveis();
        modificadorDebuffDefesa.LimparVariaveis();
        modificadorDebuffSpAtaque.LimparVariaveis();
        modificadorDebuffSpDefesa.LimparVariaveis();
        modificadorDebuffVelocidade.LimparVariaveis();
    }
    public void InstanciarModificadores()
    {
        modificadorDebuffAtaque = new Modificador(Modificador.Atributo.ataque);
        modificadorDebuffDefesa = new Modificador(Modificador.Atributo.defesa);
        modificadorDebuffSpAtaque = new Modificador(Modificador.Atributo.spAtaque);
        modificadorDebuffSpDefesa = new Modificador(Modificador.Atributo.spDefesa);
        modificadorDebuffVelocidade = new Modificador(Modificador.Atributo.velocidade);
    }
    public void AvancarStatus()
    {
        modificadorDebuffAtaque.AvancarTurno();
        modificadorDebuffDefesa.AvancarTurno();
        modificadorDebuffSpAtaque.AvancarTurno();
        modificadorDebuffSpDefesa.AvancarTurno();
        modificadorDebuffVelocidade.AvancarTurno();
    }
    public void RemoverModificador(Modificador.Atributo atributo, int id)
    {
        switch (atributo)
        {
            case Modificador.Atributo.ataque:
                modificadorDebuffAtaque.RemoverStatus(id);
                break;
            case Modificador.Atributo.defesa:
                modificadorDebuffDefesa.RemoverStatus(id);

                break;
            case Modificador.Atributo.spAtaque:
                modificadorDebuffSpAtaque.RemoverStatus(id);

                break;
            case Modificador.Atributo.spDefesa:
                modificadorDebuffSpDefesa.RemoverStatus(id);

                break;
            case Modificador.Atributo.velocidade:
                modificadorDebuffVelocidade.RemoverStatus(id);

                break;
        }
    }
}

[System.Serializable]
public class MonsterBaseAttributes
{
    //Variaveis
    [SerializeField] private int _vida;
    [SerializeField] private int _mana;
    [SerializeField] private int _ataque;
    [SerializeField] private int _defesa;
    [SerializeField] private int _velocidade;
    [SerializeField] private int _spAtaque;
    [SerializeField] private int _spDefesa;

    //Getters
    public int Vida => _vida;
    public int Mana => _mana;
    public int Ataque => _ataque;
    public int Defesa => _defesa;
    public int SpAtaque => _spAtaque;
    public int SpDefesa => _spDefesa;
    public int Velocidade => _velocidade;

    public int TotalPontosAtributosBase() => _ataque + _defesa + _vida + _spAtaque + _spDefesa + _velocidade;

    public int CriarAPartirDeTotal(int total)
    {
        int i = Mathf.FloorToInt(total / 6);
        _vida = i;
        _mana = i;
        _defesa = i;
        _ataque = i;
        _spAtaque = i;
        _spDefesa = i;
        _velocidade = i;
        return i * 6;
    }

    public void CriarManaAPartirDeTotal(float total, MonsterType monsterType)
    {
        _mana = (int)(total/6 * monsterType.magicOrientation);
    }
}

[System.Serializable]
public class UpgradesPerLevel
{
    //Enums
    
    public enum DiceImprovement { None, New, Better}

    //Variaveis
    //TODO: Verificar e fazer o upgrade do dado
    [SerializeField, GUIColor("GetAttackTypeColor")] private ComandoDeAtaque ataque;
    [SerializeField] private CombatLesson combatLesson;
    [SerializeField, GUIColor("$IsDiceColor")] private DiceImprovement diceUpgrade;
    [SerializeField, ReadOnly, GUIColor("$GetTypeColor"), ShowIf("ataque")] private AttackData.CategoriaEnum typeOfAttack;
    [SerializeField] private int level;

    //Getters
    public ComandoDeAtaque Ataque
    {
        get
        {
            return ataque;
        }
        set
        {
            TypeOfAttack = value.AttackData.Categoria;
            ataque = value;
        }
    }

    public int Level
    {
        get => level;
        set => level = value;
    }

    public CombatLesson Lesson
    {
        get => combatLesson;
        set => combatLesson = value;
    }

    public DiceImprovement DiceUpgrade
    {
        get => diceUpgrade;
        set => diceUpgrade = value;
    }

    public AttackData.CategoriaEnum TypeOfAttack
    {
        get => typeOfAttack;
        set => typeOfAttack = value;
    }
    
    public Color GetTypeColor()
    {
        switch (typeOfAttack)
        {
            case AttackData.CategoriaEnum.Especial:
                return Color.magenta;
            case AttackData.CategoriaEnum.Fisico:
                return Color.blue;
            case AttackData.CategoriaEnum.Status:
                return Color.cyan;
        }
        return Color.gray;
    }

    public Color IsDiceColor()
    {
        if (diceUpgrade == DiceImprovement.Better)
        {
            return Color.green;
        }
        else if (diceUpgrade == DiceImprovement.New)
        {
            return Color.red;
        }

        return Color.white;
    }

    public Color GetAttackTypeColor()
    {
        if (ataque)
            return ataque.AttackData.TipoAtaque.TypeColor;
        else
            return Color.white;
    }
    
}

public enum MonsterTypeEnum { Normal, Fogo, Agua, Eletrico, Dragao, Nenhum }

//These values are only used in the editor. The gameplay values are on GlobalSettings/Balance
public enum MonsterRarityEnum { basico = 50, raro = 120, exotico = 180, lendario = 280 }
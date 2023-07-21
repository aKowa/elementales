using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Balance")]
public class BalanceSettings : ScriptableObject
{
    [SerializeField, BoxGroup("Base stats for new monsters")]
    private int weakStatsTotal, mediumStatsTotal, strongStatsTotal, powerfulStatsTotal;

    [SerializeField, BoxGroup("Experience yield")]
    private int weakXPYield, mediumXPYield, strongXPYield, powerfulXPYield;
    public int BaseStats(MonsterRarityEnum monsterRarity) => monsterRarity switch
    {
        MonsterRarityEnum.basico => weakStatsTotal,
        MonsterRarityEnum.raro => mediumStatsTotal,
        MonsterRarityEnum.exotico => strongStatsTotal,
        MonsterRarityEnum.lendario => powerfulStatsTotal,
        _ => 1
    };

    public int ExperienceYield(MonsterRarityEnum monsterDataMonsterRarity) => monsterDataMonsterRarity switch
    {
        MonsterRarityEnum.basico => weakXPYield,
        MonsterRarityEnum.raro => mediumXPYield,
        MonsterRarityEnum.exotico => strongXPYield,
        MonsterRarityEnum.lendario => powerfulXPYield,
        _ => 1
    };
}
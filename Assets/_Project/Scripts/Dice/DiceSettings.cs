using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Dice Settings")]
public class DiceSettings : SerializedScriptableObject
{
    [field: OdinSerialize] public Dictionary<DiceType, DiceTypePrefabAndSprite> DicePrefabDictionary { get; set; }
    
    [field: OdinSerialize] public Dictionary<string, Dictionary<DiceType, DiceMaterialAndSprite>> DiceArtDictionary { get; set; }
}

public struct DiceMaterialAndSprite
{
    public Material material;
    public Color color;
    //TODO: Add unique Sprite field in case there is a special skin that overwrites the original icon
}

public struct DiceTypePrefabAndSprite
{
    public GameObject prefab;
    public Sprite sprite;
}
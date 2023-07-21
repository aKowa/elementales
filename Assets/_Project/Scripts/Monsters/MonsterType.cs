using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Monster/Monster Type")]
public class MonsterType : ScriptableObject
{
    //Variaveis
    [SerializeField] private string nome;
    [SerializeField][HideInTables] private Color typeColor;
    [SerializeField][HideInTables] private List<TypeRelation> vantagemContra;
    [SerializeField, Range(0, 1)] public float magicOrientation;

    //Getters
    public string Nome => nome;
    public Color TypeColor => typeColor;
    public List<TypeRelation> VantagemContra => vantagemContra;

    private void OnValidate()
    {
        if (nome == string.Empty)
        {
            nome = name;
        }

        if (typeColor.a == 0)
        {
            typeColor = new Color(1, 1, 1, 1);
        }
    }
}

[System.Serializable]
public class TypeRelation
{
    [SerializeField] private MonsterType type;

    [Header("Multiplies the original value.")]
    [Tooltip("This modifier is not additive. This means that an addition of 75% more damage from a source would be 1.75. If it would be double damage, that would be 2.")]
    public float modifier = 2;

    public MonsterType GetMonsterType => type;

    public TypeRelation(MonsterType type, string modifier )
    {
        this.type = type;
        Debug.Log("Vl: " + modifier);
        this.modifier = float.Parse(modifier,System.Globalization.CultureInfo.InvariantCulture);
        Debug.Log("Vl2: " + modifier);
    }
}
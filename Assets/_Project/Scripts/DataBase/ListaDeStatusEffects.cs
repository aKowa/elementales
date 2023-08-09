using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data Base/Lista de Status Effects")]
public class ListaDeStatusEffects : DatabaseList<StatusEffectBase>
{
    [ListDrawerSettings(Expanded = true)]
    [SerializeField] private List<StatusEffectBase> listaDeItemData;
    public override List<StatusEffectBase> Data { get => listaDeItemData; set => listaDeItemData = value; }
    public override string DataPath => GetDataPath();

    private string GetDataPath()
    {
        return GlobalSettings.Instance.OrganizationSettings.statusEffectsParentFolder;
    }
}
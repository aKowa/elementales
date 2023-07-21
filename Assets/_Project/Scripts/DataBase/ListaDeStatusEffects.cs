using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Data Base/Lista de Status Effects")]
public class ListaDeStatusEffects : DatabaseList<StatusEffectBase>
{
    [ListDrawerSettings(Expanded = true)]
    [SerializeField] private StatusEffectBase[] listaDeItemData;
    public override StatusEffectBase[] Data { get => listaDeItemData; set => listaDeItemData = value; }
    public override string DataPath => GetDataPath();

    private string GetDataPath()
    {
        return GlobalSettings.Instance.OrganizationSettings.statusEffectsParentFolder;
    }
}
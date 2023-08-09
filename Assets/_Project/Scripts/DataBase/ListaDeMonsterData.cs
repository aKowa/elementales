using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data Base/Lista de Monster Data")]
public class ListaDeMonsterData : SerializedDatabaseList<MonsterData>, ISerializationCallbackReceiver
{
    [ListDrawerSettings(Expanded = true)]
    [SerializeField] private List<MonsterData> listaDeMonsterData;
    public override List<MonsterData> Data { get => listaDeMonsterData; set => listaDeMonsterData = value; }
    public override string DataPath => GetDataPath();

    private string GetDataPath()
    {
        return GlobalSettings.Instance.OrganizationSettings.monstersParentFolder;
    }
}
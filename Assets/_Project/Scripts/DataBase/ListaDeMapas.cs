using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Data Base/Lista de Mapas")]
public class ListaDeMapas : SerializedDatabaseList<MapData>, ISerializationCallbackReceiver
{
    [ListDrawerSettings(Expanded = true)]
    [SerializeField] private MapData[] listaDeMapas;
    public override MapData[] Data { get => listaDeMapas; set => listaDeMapas = value; }
    public override string DataPath => GetDataPath();

    private string GetDataPath()
    {
        return GlobalSettings.Instance.OrganizationSettings.mapsParentFolder;
    }
}
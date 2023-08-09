using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data Base/Lista de Mapas")]
public class ListaDeMapas : SerializedDatabaseList<MapData>, ISerializationCallbackReceiver
{
    [ListDrawerSettings(Expanded = true)]
    [SerializeField] private List<MapData> listaDeMapas;
    public override List<MapData> Data { get => listaDeMapas; set => listaDeMapas = value; }
    public override string DataPath => GetDataPath();

    private string GetDataPath()
    {
        return GlobalSettings.Instance.OrganizationSettings.mapsParentFolder;
    }
}
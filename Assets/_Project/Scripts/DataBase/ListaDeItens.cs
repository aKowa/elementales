using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Data Base/Lista de Itens")]
public class ListaDeItens : SerializedDatabaseList<Item>, ISerializationCallbackReceiver
{
    [ListDrawerSettings(Expanded = true)]
    [SerializeField] private List<Item> listaDeItemData;
    public override List<Item> Data { get => listaDeItemData; set => listaDeItemData = value; }
    public override string DataPath => GetDataPath();

    private string GetDataPath()
    {
        return GlobalSettings.Instance.OrganizationSettings.itemsParentFolder;
    }
}

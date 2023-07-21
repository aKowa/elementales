using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Data Base/Lista de Comandos")]
public class ListaDeComandos : SerializedDatabaseList<Comando>, ISerializationCallbackReceiver
{
    [ListDrawerSettings(Expanded = true)]
    [SerializeField] private Comando[] listaDeMonsterData;
    public override Comando[] Data { get => listaDeMonsterData; set => listaDeMonsterData = value; }
    public override string DataPath => GetDataPath();

    private string GetDataPath()
    {
        return GlobalSettings.Instance.OrganizationSettings.commandsParentFolder;
    }
}

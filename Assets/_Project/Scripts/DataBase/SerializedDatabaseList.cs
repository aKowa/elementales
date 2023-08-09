using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class DatabaseList<T> : ScriptableObject
    where T : ScriptableObject
{
    public virtual List<T> Data { get; set; }
    public virtual string DataPath { get; set; }

#if UNITY_EDITOR
    [Button]
    protected virtual void AtualizarLista()
    {
        //Marca o objeto como dirty e salva uma entrada dele no historico de edicoes para poder desfazer e refazer a acao de atualizar a lista
        UnityEditor.Undo.RecordObject(this, $"Atualizar Lista no \"{this.name}\"");

        try
        {
            //Procura os caminhos de todos os assets do tipo T nas pastas filhas da indicada no Data Path
            string dataParentsFolder = DataPath;
            var dataPaths = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T)}", new[] { $"{dataParentsFolder}/" });

            //Carrega cada um dos assets achados e, se eles nao estiverem na databaseList, adiciona eles no primeiro lugar com um item nulo, ou, se nao achar nenhum, no fim da lista
            for (int i = 0; i < dataPaths.Length; i++)
            {
                var path = dataPaths[i];
                T data = (T)UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(path), typeof(T));

                if (Data.Contains(data) == false)
                {
                    Data.Add(data);
                }
            }
        }
        catch (System.Exception e)
        {
            throw new System.Exception($"Nao foi possivel atualizar a DatabaseList.\n{e}");
        }
    }
#endif

    //Getters
    public int GetListaDataLenght => Data.Count;

    public virtual T GetData(int id)
    {
        return Data[id];
    }

    public virtual int GetId(T data)
    {
        for(int i = 0; i < Data.Count; i++)
        {
            if (Data[i].name == data.name)
            {
                return i;
            }
        }
        throw new KeyNotFoundException($"Nao foi encontrado esse {typeof(T)} na lista!");
    }
}

public class SerializedDatabaseList<T> : DatabaseList<T>
where T : ScriptableObject
{
    private Dictionary<int, T> dataDictionary = new Dictionary<int, T>();
    private Dictionary<T, int> idDictionary = new Dictionary<T, int>();
    
    public override T GetData(int id)
    {
        return dataDictionary[id];
    }

    public override int GetId(T item)
    {
        return idDictionary[item];
    }

    public void OnAfterDeserialize()
    {
        dataDictionary = new Dictionary<int, T>();
        idDictionary = new Dictionary<T, int>();

        for (int i = 0; i < Data.Count; i++)
        {
            dataDictionary.Add(i, Data[i]);
            idDictionary.Add(Data[i], i);
        }
    }

    public void OnBeforeSerialize()
    {
        //Nada
    }
}
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
    public virtual T[] Data { get; set; }
    public virtual string DataPath { get; set; }

#if UNITY_EDITOR
    [Button]
    protected virtual void AtualizarLista()
    {
        try
        {
            string dataParentsFolder = DataPath;
            var dataPaths = AssetDatabase.FindAssets($"t:{typeof(T)}", new[] {$"{dataParentsFolder}/"});
            T[] newData = new T[dataPaths.Length];
            for (var i = 0; i < dataPaths.Length; i++)
            {
                var path = dataPaths[i];
                T data = (T)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(path), typeof(T));
                newData[i] = data;
            }

            Data = (T[])newData.Clone();
        }
        catch (Exception)
        {
            Debug.LogError("Couldn't recreate the new data list.");
            throw;
        }
    }
#endif

    //Getters
    public int GetListaDataLenght => Data.Length;

    public virtual T GetData(int id)
    {
        return Data[id];
    }

    public virtual int GetId(T data)
    {
        for(int i = 0; i < Data.Length; i++)
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

        for (int i = 0; i < Data.Length; i++)
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
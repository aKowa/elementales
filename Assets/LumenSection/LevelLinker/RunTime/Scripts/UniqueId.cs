// Copyright (C) Lumen Section - All Rights Reserved
// Written by Nicolas Baillard <nicolas.baillard@gmail.com>
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif



namespace LumenSection.LevelLinker
{
[ExecuteInEditMode]
public class UniqueId : MonoBehaviour
{
  // Guid
  [SerializeField]
  private string _Guid;

  

  public string Guid
  {
    get
    {
      // Generate a GUID if we don't have one
      #if UNITY_EDITOR
      if (string.IsNullOrEmpty(_Guid))
      {
        _Guid = System.Guid.NewGuid().ToString();
        EditorUtility.SetDirty(this);
      }
      #endif
      return _Guid;
    }
  }
  
  #if UNITY_EDITOR
  private void OnValidate()
  {
    // Nothing to do if we don't have a GUID
    if (string.IsNullOrEmpty(_Guid))
      return;
    
    // If this object is a duplicate of another then reset GUID
    var objects = FindObjectsOfType<UniqueId>(includeInactive: true);
    foreach (var obj in objects)
    {
      if (obj == this)
        continue;
      if (obj._Guid == _Guid)
      {
        _Guid = null;
        return;
      }
    }
  }
  #endif
}
}

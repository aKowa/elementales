using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MapErrorHandler : Singleton<MapErrorHandler>
{
    public void HandleDuplicates(string nextSceneName, Action actionToPerform)
    {
        Debug.Log($"Searching for duplicates on {nextSceneName}");
        Scene scene = SceneManager.GetSceneByName(nextSceneName);

        GameObject[] gameObjects = scene.GetRootGameObjects();
        List<GameObject> objectsToDestroy = new List<GameObject>();
        
        foreach (GameObject obj in gameObjects)
        {
            Debug.Log($"Object: {obj}");
            if (obj.GetComponent<EventSystem>() || 
                obj.GetComponent<Player>() || 
                obj.GetComponent<Camera>())
            {
                Destroy(obj);
                objectsToDestroy.Add(obj);
            }

            List<GameObject> childs = new List<GameObject>();
            if (obj.transform.childCount > 0)
            {
                childs.Add(obj.transform.GetComponentInChildren<EventSystem>()?.gameObject);
                childs.Add(obj.transform.GetComponentInChildren<Player>()?.gameObject);
                childs.Add(obj.transform.GetComponentInChildren<Camera>()?.gameObject);
            }
            
            if (childs.Any())
            {
                foreach (var child in childs)
                {
                    Destroy(child);
                    objectsToDestroy.Add(child?.gameObject);
                }
            }
        }

        foreach (GameObject objDestroyed in objectsToDestroy)
        {
            Debug.LogWarning($"Destroyed {objDestroyed} because it contained a duplicate");
        }
        
        actionToPerform.Invoke();
    }
}
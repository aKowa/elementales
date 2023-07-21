using System;
using System.Collections;
using DG.Tweening;
using LumenSection.LevelLinker;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapsManager : Singleton<MapsManager>
{
    [SerializeField]private LevelConnectionMap Map;
    private AssetBundle SceneBundle;
    
    [SerializeField] private ViewController blackPanelView;
    
    public void LoadMapScene(string scenePath)
    {
        StartCoroutine(LoadLevel(scenePath));
    }

    private IEnumerator LoadLevel(string nextSceneName)
    {
        // Load next scene
        var operation = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            Debug.Log(progress);
            yield return null;
        }
        Scene scene = SceneManager.GetSceneByPath(nextSceneName);
        SceneManager.SetActiveScene(scene);
        //MapErrorHandler.GetInstance().HandleDuplicates(scene.name, () => GameManager.GetInstance().SetVirtualCameraBounds());
    }

    public void TakeDoor(Gateway gateway, Action onSceneLoadEnded = null)
    {
        // Get door GUI
        string currentDoorGuid = gateway.Guid;

        StartCoroutine(SwitchLevel(currentDoorGuid, onSceneLoadEnded));
    }

    public void TakeDoorWithGuid(string currentDoorGuid, Action onSceneLoadEnded = null)
    {

        StartCoroutine(SwitchLevel(currentDoorGuid, onSceneLoadEnded));
    }

    public void TakeDoor(GatewaySave gateway, Action onSceneLoadEnded = null)
    {
        StartCoroutine(SwitchLevel(gateway.currentDoorGuid, onSceneLoadEnded));
    }

    public void LoadScene(string currentDoorGuid, Action onSceneLoadEnded)
    {
        StartCoroutine(SwitchLevel(currentDoorGuid, onSceneLoadEnded));
    }

    public void LoadSceneByName(string sceneName, Action onSceneLoadEnded = null)
    {
        StartCoroutine(LoadSceneByNameCorroutine(sceneName, onSceneLoadEnded));
    }

    private IEnumerator SwitchLevel(string currentDoorGuid, Action onSceneLoadEnded)
    {
        // Get destination
        (string nextSceneName, string nextDoorName) = Map.GetDestination(currentDoorGuid);
        if (string.IsNullOrEmpty(nextSceneName) || string.IsNullOrEmpty(nextDoorName))
        {
            Debug.Log("-> no destination");
            yield break;
        }
        
        var loadOperation = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);

        while (!loadOperation.isDone)
        {
            //float progress = Mathf.Clamp01(loadOperation.progress / .9f);
            //Debug.Log(progress);
            
            yield return null;
        }
        
        // Find door in scene
        var door = GameObject.Find(nextDoorName).GetComponent<Gateway>();
        
        // Spawn character
        door.Spawn(currentDoorGuid);

        onSceneLoadEnded?.Invoke();
    }

    private IEnumerator LoadSceneByNameCorroutine(string sceneName, Action onSceneLoadEnded)
    {
        var loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        while (!loadOperation.isDone)
        {
            yield return null;
        }

        onSceneLoadEnded?.Invoke();
    }
}
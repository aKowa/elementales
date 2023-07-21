// using DG.Tweening;
// using System;
// using System.Collections;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;
//
// public class SceneLoadManager : Singleton<SceneLoadManager>
// {
//     private SceneData currentScene;
//     [SerializeField] private SceneData firstScene;
//     [SerializeField] private Image blackPanel;
//
//     public void LoadMapScene(SceneData sceneData = null, Action<AsyncOperation> action = null)
//     {
//         if (sceneData != null)
//         {
//             StartCoroutine(LoadAsynchronously(sceneData, LoadSceneMode.Additive, action));
//         }
//         else
//         {
//             StartCoroutine(LoadAsynchronously(firstScene, LoadSceneMode.Additive, action));
//         }
//     }
//
//     IEnumerator LoadAsynchronously(SceneData sceneData, LoadSceneMode mode = LoadSceneMode.Additive, Action<AsyncOperation> action = null)
//     {
//         AsyncOperation operation = SceneManager.LoadSceneAsync(sceneData.GetSceneRef, mode);
//
//         if (action != null && operation.progress == 0)
//         {
//             operation.completed += action;
//         }
//
//         var fadeIn = blackPanel.DOFade(1, 1f).OnComplete(() => { blackPanel.transform.GetChild(0).gameObject.SetActive(true); });
//
//         fadeIn.onComplete += () => fadeIn.Complete();
//         fadeIn.Pause();
//         fadeIn.SetAutoKill(false);
//
//         if (sceneData.fadeOut)
//         {
//             fadeIn.Play();
//         }
//         
//         TransitionsManager.GetInstance().LoadingScreenFadeIn();
//
//         while (!operation.isDone)
//         {
//             float progress = Mathf.Clamp01(operation.progress / .9f);
//             Debug.Log(progress);
//
//             yield return null;
//         }
//
//         if (sceneData.fadeOut)
//         {
//             StartCoroutine(TransitionsManager.GetInstance().LoadingScreenFadeOut());
//         }
//         yield return null;
//     }
//
//     private void FinishLoadOperation(SceneData sceneData)
//     {
//         Debug.Log("FinishLoadOperation");
//         SceneManager.SetActiveScene(SceneManager.GetSceneByPath(sceneData.GetSceneRef.ScenePath));
//
//         if (currentScene)
//         {
//             SceneManager.UnloadSceneAsync(currentScene.GetSceneRef);
//         }
//
//         currentScene = sceneData;
//     }
//
//     public void OpenNewSceneFile(SceneData sceneData)
//     {
//         Debug.Log("OpenNewSceneFile");
//         StartCoroutine(LoadAsynchronously(sceneData));
//     }
// }

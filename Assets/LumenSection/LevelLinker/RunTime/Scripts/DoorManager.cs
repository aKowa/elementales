// Copyright (C) Lumen Section - All Rights Reserved
// Written by Nicolas Baillard <nicolas.baillard@gmail.com>

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace LumenSection.LevelLinker
{
//     public class DoorManager : MonoBehaviour
//     {
//         private static DoorManager msInstance;
//         private static LevelConnectionMap msMap;
//         private static AssetBundle msSceneBundle;
//
//
//         public static DoorManager Instance
//         {
//             get
//             {
//                 if (msInstance == null)
//                 {
//                     var go = new GameObject("DoorManager");
//                     msInstance = go.AddComponent<DoorManager>();
//                     DontDestroyOnLoad(go);
//                 }
//
//                 return msInstance;
//             }
//         }
//
//         public void TakeDoor(Door door, Character character)
//         {
//             // Get door GUI and current scene name
//             string currentDoorGuid = door.Guid;
//             string currentSceneName = door.gameObject.scene.name;
//
//             // Switch level
//             StartCoroutine(SwitchLevel(currentSceneName, currentDoorGuid, character));
//         }
//
//         private static IEnumerator SwitchLevel(string currentSceneName, string currentDoorGuid, Character character)
//         {
//             // Stop time
//             Time.timeScale = 0f;
//
//             // Load connection map if it's not already
//             if (msMap == null)
//             {
//                 var request = Resources.LoadAsync<LevelConnectionMap>("LevelConnectionMap");
//                 while (!request.isDone)
//                     yield return null;
//                 msMap = request.asset as LevelConnectionMap;
//                 if (msMap == null)
//                 {
//                     Debug.Log("-> no map");
//                     Time.timeScale = 1f;
//                     yield break;
//                 }
//             }
//
//             // Load scenes bundle if it's not already
//             if (msSceneBundle == null)
//             {
// #if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
//               const string sceneBundlePath =
//          "Assets/LumenSection/LevelLinker/Sample/AssetBundles/Linux/lumensection/levellinker/sample/scenes";
// #elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
//                 const string sceneBundlePath =
//                     "Assets/LumenSection/LevelLinker/Sample/AssetBundles/Windows/lumensection/levellinker/sample/scenes";
// #elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
//               const string sceneBundlePath =
//          "Assets/LumenSection/LevelLinker/Sample/AssetBundles/OSX/lumensection/levellinker/sample/scenes";
// #endif
//                 var bundleCreateRequest = AssetBundle.LoadFromFileAsync(sceneBundlePath);
//                 while (!bundleCreateRequest.isDone)
//                     yield return null;
//                 msSceneBundle = bundleCreateRequest.assetBundle;
//             }
//
//             // Get destination
//             (string nextSceneName, string nextDoorName) = msMap.GetDestination(currentDoorGuid);
//             if (string.IsNullOrEmpty(nextSceneName) || string.IsNullOrEmpty(nextDoorName))
//             {
//                 Debug.Log("-> no destination");
//                 Time.timeScale = 1f;
//                 yield break;
//             }
//
//             // Load next scene
//             var operation = SceneManager.LoadSceneAsync(nextSceneName);
//             while (!operation.isDone)
//                 yield return null;
//
//             // Find door in scene
//             var door = GameObject.Find(nextDoorName).GetComponent<Door>();
//
//             // Spawn character
//             door.Spawn(character);
//
//             // Done
//             Time.timeScale = 1f;
//         }
//     }
}
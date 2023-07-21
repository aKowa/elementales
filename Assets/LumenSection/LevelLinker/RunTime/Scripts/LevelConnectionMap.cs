// Copyright (C) Lumen Section - All Rights Reserved
// Written by Nicolas Baillard <nicolas.baillard@gmail.com>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using static LumenSection.LevelLinker.MathUtils;


[assembly: InternalsVisibleTo("com.lumensection.levellinker.editor")]


namespace LumenSection.LevelLinker
{
    public interface IDoor
    {
        string Guid { get; }
        string Name { get; }
    }


    #region Scene Save Notifier

#if UNITY_EDITOR
    [InitializeOnLoad]
    internal static class LevelSceneSaveNotifier
    {
        static LevelSceneSaveNotifier()
        {
            EditorSceneManager.sceneSaving += OnSavingScene;
        }

        private static void OnSavingScene(Scene scene, string path)
        {
            // Get current scene name, ignore if it's not a level
            Path currentPath = path;
            (Path _, string currentName, string _) = currentPath.Decompose();

            // Get Guid
            var guid = AssetDatabase.AssetPathToGUID(path);

            // Get connection map
            var connectionMaps = LevelConnectionMap.GetLevelConnectionMaps();

            // Notify map
            foreach (var connectionMap in connectionMaps)
                connectionMap.NotifyLevelSaved(scene, guid, path, currentName);
        }
    }
#endif

    #endregion


    #region Asset Post Processor

#if UNITY_EDITOR
    internal class LevelMapAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            bool importedOneMap = false;
            foreach (string assetPath in importedAssets)
            {
                // Get imported asset type
                var assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);

                if (assetType == typeof(LevelConnectionMap))
                    importedOneMap = true;
            }

            // If just imported one or more map
            if (importedOneMap)
                LevelConnectionMap.NotifyFileChanged(null);
        }
    }
#endif

    #endregion


    #region Asset Modification Notifier

#if UNITY_EDITOR
    internal class LevelModificationProcessor : AssetModificationProcessor
    {
        public static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            // Get moved asset type
            var assetType = AssetDatabase.GetMainAssetTypeAtPath(sourcePath);

            // If moved asset is a map
            if (assetType == typeof(LevelConnectionMap))
            {
                LevelConnectionMap.NotifyFileChanged(null);
            }

            // If moved asset id a scene
            if (assetType == typeof(SceneAsset))
            {
                // Get new scene name
                Path newPath = destinationPath;
                (Path _, string newName, string _) = newPath.Decompose();

                // Get scene GUID
                var guid = AssetDatabase.AssetPathToGUID(sourcePath);

                // Get connection map
                var connectionMaps = LevelConnectionMap.GetLevelConnectionMaps();

                // Notify map
                foreach (var connectionMap in connectionMaps)
                    connectionMap.NotifySceneRenamed(guid, newName);
            }

            // Let Unity do the move
            return AssetMoveResult.DidNotMove;
        }

        public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            // Get deleted asset type
            var assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);

            // If deleted asset is a map
            if (assetType == typeof(LevelConnectionMap))
            {
                var deletedMap = AssetDatabase.LoadAssetAtPath<LevelConnectionMap>(assetPath);
                LevelConnectionMap.NotifyFileChanged(deletedMap);
            }

            // If deleted asset is a scene
            if (assetType == typeof(SceneAsset))
            {
                // Get scene GUID
                var guid = AssetDatabase.AssetPathToGUID(assetPath);

                // Get connection map
                var connectionMaps = LevelConnectionMap.GetLevelConnectionMaps();

                // Notify maps
                foreach (var connectionMap in connectionMaps)
                    connectionMap.NotifySceneDeleted(guid);
            }

            // Let Unity delete the asset
            return AssetDeleteResult.DidNotDelete;
        }
    }
#endif

    #endregion


    [CreateAssetMenu(menuName = "Level Connection Map", fileName = "LevelConnectionMap")]
    public class LevelConnectionMap : ScriptableObject, ISerializationCallbackReceiver
    {
        // Types
        [Serializable]
        public class LevelDoorDeclaration
        {
            public string Guid;
            public string Name;
            public bool LeftSide;
        }


        [Serializable]
        public class LevelDeclaration
        {
            public string Guid;
            public string Name;
            public Vector2 PositionOnMap;
            public LevelDoorDeclaration[] Doors;
        }


        [Serializable]
        public class LevelConnection
        {
            public string LevelGuid1;
            public string LevelGuid2;
            public string DoorGuid1;
            public string DoorGuid2;
            public List<Vector2> Points;


            public void SetAllPoints(Vector2[] points)
            {
                Points.Clear();
                Points.AddRange(points);
            }
        }


        [Serializable]
        public struct SerializedConnection
        {
            public string Door1Guid;
            public string Door2Guid;
            public string Level1Name;
            public string Level2Name;
            public string Door1Name;
            public string Door2Name;
        }


        internal delegate void MapChangeEvent(LevelConnectionMap map);

        internal delegate void ChangeEvent();


        // Constants for editor
        internal const float ConnectionThickness = 2f;
        internal const float ConnectionPointRadius = ConnectionThickness * 2f;
        internal static readonly Color ConnectionColor = new(0f, 0.78f, 1f);
        internal static readonly Color HighlightedConnectionColor = Color.white;
        internal static readonly Color SelectedConnectionColor = new(1f, 0.537f, 0f);
        internal static readonly Color SelectedHighlightedConnectionColor = Color.yellow;

        // Events
#if UNITY_EDITOR
        internal static event MapChangeEvent OnFileChanged;
        internal event ChangeEvent OnChanged;
#endif

        // Parameters
        [SerializeField] private string LevelPrefix;

        [SerializeField] private string LevelSuffix;

        // Editor Serialized
#if UNITY_EDITOR
        [SerializeField] [HideInInspector] private List<LevelDeclaration> mLevels = new();
        [SerializeField] [HideInInspector] private List<LevelConnection> mConnection = new();
#endif

        // Serialized
        [SerializeField] [HideInInspector] private SerializedConnection[] mSerializedConnection;


        #region Properties

#if UNITY_EDITOR
        internal IReadOnlyList<LevelDeclaration> Levels
        {
            get { return mLevels; }
        }

        internal IReadOnlyList<LevelConnection> Connections
        {
            get { return mConnection; }
        }
#endif

        #endregion


        #region Utils

        private static string MakePrettyName(string name)
        {
            name = name.Replace("_", " ");
            name = Regex.Replace(Regex.Replace(name, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
            name = char.ToUpper(name[0]) + name[1..];
            return name.Trim();
        }

        #endregion


        #region Editor Utils

#if UNITY_EDITOR
        private LevelDeclaration GetLevelForDoor(LevelDoorDeclaration door)
        {
            Debug.Assert(door != null);
            foreach (var level in mLevels)
            {
                if (Array.IndexOf(level.Doors, door) >= 0)
                    return level;
            }

            return null;
        }

        private (string levelName, string doorName) GetLevelAndDoorName(string levelGuid, string doorGuid)
        {
            foreach (var level in mLevels)
            {
                if (level.Guid == levelGuid)
                {
                    foreach (var door in level.Doors)
                    {
                        if (door.Guid == doorGuid)
                            return (level.Name, door.Name);
                    }

                    throw new ArgumentException($"Door {doorGuid} not found within level {level.Name} {level.Guid}");
                }
            }

            throw new ArgumentException($"Level {levelGuid} not found");
        }

        private static void SwapValues<T>(T[] source, int index1, int index2)
        {
            (source[index1], source[index2]) = (source[index2], source[index1]);
        }
#endif

        #endregion


        #region Events

#if UNITY_EDITOR
        internal static void NotifyFileChanged(LevelConnectionMap deletedMap)
        {
            OnFileChanged?.Invoke(deletedMap);
        }
#endif

        #endregion


        #region Derived

        protected virtual bool MustRegisterScene(Scene scene, string scenePath, string sceneName)
        {
            // Ignore scene if name doesn't start with prefix
            if (!string.IsNullOrEmpty(LevelPrefix) && !sceneName.StartsWith(LevelPrefix, StringComparison.Ordinal))
                return false;

            // Ignore scene if name doesn't end with suffix
            if (!string.IsNullOrEmpty(LevelSuffix) && !sceneName.EndsWith(LevelSuffix, StringComparison.Ordinal))
                return false;

            // Keep scene
            return true;
        }

        public virtual string FormatSceneName(string sceneName)
        {
            if (!string.IsNullOrEmpty(LevelPrefix) && sceneName.StartsWith(LevelPrefix, StringComparison.Ordinal))
                sceneName = sceneName[LevelPrefix.Length..];
            if (!string.IsNullOrEmpty(LevelSuffix) && sceneName.EndsWith(LevelSuffix, StringComparison.Ordinal))
                sceneName = sceneName[..^LevelSuffix.Length];
            return MakePrettyName(sceneName);
        }

        public virtual string FormatDoorName(string doorName)
        {
            return MakePrettyName(doorName);
        }

        #endregion


        #region Editor API

#if UNITY_EDITOR
        internal static LevelConnectionMap[] GetLevelConnectionMaps()
        {
            List<LevelConnectionMap> maps = new List<LevelConnectionMap>();
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(LevelConnectionMap)));
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                LevelConnectionMap map = AssetDatabase.LoadAssetAtPath<LevelConnectionMap>(path);
                if (map != null)
                    maps.Add(map);
            }

            return maps.ToArray();
        }

        internal void NotifyLevelSaved(Scene scene, string guid, string path, string levelName)
        {
            if (!MustRegisterScene(scene, path, levelName))
                return;

            // Get root objects
            var roots = scene.GetRootGameObjects();

            // Get doors
            List<IDoor> doors = new List<IDoor>();
            foreach (var root in roots)
            {
                var doorsInRoot = root.GetComponentsInChildren<IDoor>(includeInactive: true);
                doors.AddRange(doorsInRoot);
            }

            // Get or create level with that guid
            LevelDeclaration level = null;
            for (int i = 0; i < mLevels.Count; ++i)
            {
                if (mLevels[i].Guid == guid)
                {
                    level = mLevels[i];
                    break;
                }
            }

            if (level == null)
            {
                // Don't create if no doors
                if (doors.Count == 0)
                    return;

                level = new LevelDeclaration();
                level.Guid = guid;
                level.Name = levelName;
                mLevels.Add(level);
            }

            // Create door list
            var doorList = new List<LevelDoorDeclaration>();
            foreach (var door in doors)
            {
                var doorDeclaration = new LevelDoorDeclaration();
                doorDeclaration.Guid = door.Guid;
                doorDeclaration.Name = door.Name;
                doorList.Add(doorDeclaration);
            }

            // Set level door list
            level.Doors = doorList.ToArray();

            // Purge any connection from a door that no longer exist
            for (int i = 0; i < mConnection.Count;)
            {
                if (mConnection[i].LevelGuid1 == guid &&
                    !Array.Exists(level.Doors, (door) => door.Guid == mConnection[i].DoorGuid1))
                    mConnection.RemoveAt(i);
                else if (mConnection[i].LevelGuid2 == guid &&
                         !Array.Exists(level.Doors, (door) => door.Guid == mConnection[i].DoorGuid2))
                    mConnection.RemoveAt(i);
                else
                    ++i;
            }

            // Save
            OnChanged?.Invoke();
            EditorUtility.SetDirty(this);
        }

        internal void NotifySceneRenamed(string guid, string newName)
        {
            // Change name
            for (int i = 0; i < mLevels.Count; ++i)
            {
                if (mLevels[i].Guid == guid)
                    mLevels[i].Name = newName;
            }

            // Save
            OnChanged?.Invoke();
            EditorUtility.SetDirty(this);
        }

        internal void NotifySceneDeleted(string guid)
        {
            // Drop all levels with that Guid
            for (int i = 0; i < mLevels.Count;)
            {
                if (mLevels[i].Guid == guid)
                    mLevels.RemoveAt(i);
                else
                    ++i;
            }

            // Drop all connections with that Guid
            for (int i = 0; i < mConnection.Count;)
            {
                if (mConnection[i].LevelGuid1 == guid || mConnection[i].LevelGuid2 == guid)
                    mConnection.RemoveAt(i);
                else
                    ++i;
            }

            // Save
            OnChanged?.Invoke();
            EditorUtility.SetDirty(this);
        }

        internal void SetLevelPosition(string guid, Vector2 positionOnMap)
        {
            // Change position
            for (int i = 0; i < mLevels.Count; ++i)
            {
                if (mLevels[i].Guid == guid)
                    mLevels[i].PositionOnMap = positionOnMap;
            }
        }

        internal void BringLevelToFront(LevelDeclaration level)
        {
            // Level must be in the list already
            Debug.Assert(mLevels.Contains(level));

            // Make level the last element of the list
            mLevels.Remove(level);
            mLevels.Add(level);
        }

        internal void DeleteLevel(LevelDeclaration level)
        {
            NotifySceneDeleted(level.Guid);
        }

        internal LevelConnection GetConnectionForDoor(LevelDoorDeclaration door)
        {
            foreach (var connection in mConnection)
            {
                if (connection.DoorGuid1 == door.Guid || connection.DoorGuid2 == door.Guid)
                    return connection;
            }

            return null;
        }

        internal bool HasConnectionForDoor(LevelDoorDeclaration door)
        {
            if (GetConnectionForDoor(door) != null)
                return true;
            return false;
        }

        internal bool BelongToSameLevel(LevelDoorDeclaration door1, LevelDoorDeclaration door2)
        {
            foreach (var level in mLevels)
            {
                if (Array.IndexOf(level.Doors, door1) >= 0 && Array.IndexOf(level.Doors, door2) >= 0)
                    return true;
            }

            return false;
        }

        internal (int index, int nb) GetDoorIndexInLevel(LevelDoorDeclaration door)
        {
            foreach (var level in mLevels)
            {
                for (int i = 0; i < level.Doors.Length; i++)
                {
                    if (level.Doors[i] == door)
                        return (i, level.Doors.Length);
                }
            }

            return (0, 0);
        }

        internal void DecreaseDoorIndex(LevelDoorDeclaration door)
        {
            foreach (var level in mLevels)
            {
                int index = Array.IndexOf(level.Doors, door);
                if (index >= 0)
                {
                    SwapValues(level.Doors, index, index - 1);
                    return;
                }
            }
        }

        internal void IncreaseDoorIndex(LevelDoorDeclaration door)
        {
            foreach (var level in mLevels)
            {
                int index = Array.IndexOf(level.Doors, door);
                if (index >= 0)
                {
                    SwapValues(level.Doors, index, index + 1);
                    return;
                }
            }
        }

        internal LevelConnection CreateConnection(LevelDoorDeclaration door1, LevelDoorDeclaration door2)
        {
            Debug.Assert(door1 != null);
            Debug.Assert(door2 != null);
            Debug.Assert(door1 != door2);
            Debug.Log(door1.Guid);
            Debug.Log(door2.Guid);
            Debug.Assert(door1.Guid != door2.Guid);
            Debug.Assert(!BelongToSameLevel(door1, door2));
            Debug.Assert(!HasConnectionForDoor(door1));
            Debug.Assert(!HasConnectionForDoor(door2));

            var level1 = GetLevelForDoor(door1);
            var level2 = GetLevelForDoor(door2);
            Debug.Assert(level1 != null);
            Debug.Assert(level2 != null);

            var connection = new LevelConnection();
            connection.LevelGuid1 = level1.Guid;
            connection.LevelGuid2 = level2.Guid;
            connection.DoorGuid1 = door1.Guid;
            connection.DoorGuid2 = door2.Guid;
            connection.Points = new List<Vector2>();

            mConnection.Add(connection);

            // Done
            return connection;
        }

        internal void DeleteConnection(LevelConnection connection)
        {
            Debug.Assert(connection != null);
            mConnection.Remove(connection);
        }

        internal void SetConnectionPointPosition(LevelConnection connection, int pointIndex, Vector2 position,
            bool final)
        {
            Debug.Assert(connection != null);
            connection.Points[pointIndex] = position;

            // If move is finished
            if (final)
            {
                // Purge useless points
                for (int i = 1; i < connection.Points.Count - 1;)
                {
                    var point = connection.Points[i];
                    var previousPoint = connection.Points[i - 1];
                    var nextPoint = connection.Points[i + 1];

                    // Delete if too close from previous point
                    if ((point - previousPoint).magnitude <= ConnectionPointRadius)
                        connection.Points.RemoveAt(i);

                    // Delete if on segment between previous and next point
                    if (IsOnSegment2D(previousPoint, nextPoint, point, ConnectionThickness, out _, out _))
                        connection.Points.RemoveAt(i);

                    // Keep point
                    else
                        ++i;
                }
            }
        }

        internal void CreateConnectionPoint(LevelConnection connection, int pointIndex, Vector2 position)
        {
            Debug.Assert(connection != null);
            connection.Points.Insert(pointIndex, position);
        }
#endif

        #endregion


        #region Serialization

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            mSerializedConnection = new SerializedConnection[mConnection.Count];
            for (int i = 0; i < mConnection.Count; ++i)
            {
                SerializedConnection serializedConnection;
                serializedConnection.Door1Guid = mConnection[i].DoorGuid1;
                serializedConnection.Door2Guid = mConnection[i].DoorGuid2;
                (serializedConnection.Level1Name, serializedConnection.Door1Name) =
                    GetLevelAndDoorName(mConnection[i].LevelGuid1, mConnection[i].DoorGuid1);
                (serializedConnection.Level2Name, serializedConnection.Door2Name) =
                    GetLevelAndDoorName(mConnection[i].LevelGuid2, mConnection[i].DoorGuid2);
                mSerializedConnection[i] = serializedConnection;
            }
#endif
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
        }

        #endregion


        #region API

        public (string levelName, string doorName) GetDestination(string doorGuid)
        {
            foreach (var connection in mSerializedConnection)
            {
                if (connection.Door1Guid == doorGuid)
                    return (connection.Level2Name, connection.Door2Name);
                if (connection.Door2Guid == doorGuid)
                    return (connection.Level1Name, connection.Door1Name);
            }

            return ("", "");
        }

        #endregion
    }
}
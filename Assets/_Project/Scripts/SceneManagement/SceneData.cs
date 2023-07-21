using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class SceneData
{
    public string displayName;
    public List<AudioClip> AmbienceSounds;
    public List<AudioClip> TracksOnLoad;
}
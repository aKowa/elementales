using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using ObjectFieldAlignment = Sirenix.OdinInspector.ObjectFieldAlignment;

[CreateAssetMenu(menuName = "Map Data")]
public class MapData : SerializedScriptableObject
{
#if UNITY_EDITOR
    [VerticalGroup("Info")]public string mapName, mapDescription;
    [VerticalGroup("Info"), PreviewField(100, ObjectFieldAlignment.Left), HideLabel]
    public Sprite mapPreview;
    
    [TableMatrix(SquareCells = true, DrawElementMethod = nameof(DrawMonsterIcon))] public MonsterData[,] monsterSprites = new MonsterData[8, 2];

    public List<AudioClip> mapAmbientAudio;
    public List<AudioClip> mapAmbientMusic;

    
    private MonsterData DrawMonsterIcon(Rect rect, MonsterData value)
    {
        return (MonsterData) Sirenix.Utilities.Editor.SirenixEditorFields.UnityPreviewObjectField(
            rect: rect,
            value: value,
            texture: value?.Miniatura.texture, // We provide a custom preview texture
            type: typeof(MonsterData)
        );
    }
#endif
    
}
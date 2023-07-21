using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Organization")]
public class OrganizationSettings : ScriptableObject
{
    [BoxGroup("Folders")]
    public bool eachMonsterHasAFolder;

    [BoxGroup("Folders")]
    [FolderPath]
    public string monstersParentFolder;
    
    [BoxGroup("Folders")]
    [FolderPath]
    public string itemsParentFolder;
    
    [BoxGroup("Folders")]
    [FolderPath]
    public string commandsParentFolder;
    
    [BoxGroup("Folders")]
    [FolderPath]
    public string attackCommandsParentFolder;

    [BoxGroup("Folders")] [FolderPath] 
    public string combatLessonsParentFolder;

    [BoxGroup("Folders")] [FolderPath] 
    public string mapsParentFolder;
    
    [BoxGroup("Folders")]
    [FolderPath]
    public string statusEffectsParentFolder;
    
#if UNITY_EDITOR
    [BoxGroup("Monsters")]
    public AnimatorController baseAnimator;
#endif
    [BoxGroup("Monsters/Animation Import", VisibleIf = "@baseAnimator != null")]
    [FolderPath]
    public string animationsFolder;

    [FoldoutGroup("Monsters/Animation Import/Animation Names")]
    public string idleName, attackName, spAttackName, takeHitName, deathName;

    public string[] GetAnimationNames()
    {
        return new []{idleName, attackName, spAttackName, takeHitName, deathName};
    }

#region Editor Stuff

// #if UNITY_EDITOR
//     [Button]
//     public void GenerateAnimationImageProperty(AnimationClip clip)
//     {
//         AnimationPropertyGenerator.GenerateImageAnimationProperty(clip);
//     }
// #endif

#endregion

}
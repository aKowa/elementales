using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MonsterAnimatorGenerator
{

    public static AnimationClip[] FindAnimationsOfName(MonsterData monsterData, string[] animationNames)
    {
        OrganizationSettings organizationSettings = GlobalSettings.Instance.OrganizationSettings;
        
        var animationsFolder = organizationSettings.animationsFolder;
        
        List<string> foundAssetsByName = new List<string>();
        foreach (string animationName in animationNames)
        {
            string[] assets = AssetDatabase.FindAssets($"{animationName} t:AnimationClip", new[] {$"{animationsFolder}/{monsterData.GetName}"});
            foundAssetsByName.Add(assets[0]);
        }
        AnimationClip idle = (AnimationClip)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(foundAssetsByName[0]), typeof(AnimationClip));
        AnimationClip attack = (AnimationClip)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(foundAssetsByName[1]), typeof(AnimationClip));
        AnimationClip spAttack = (AnimationClip)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(foundAssetsByName[2]), typeof(AnimationClip));
        AnimationClip tomandoDano = (AnimationClip)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(foundAssetsByName[3]), typeof(AnimationClip));
        AnimationClip morrendo = (AnimationClip)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(foundAssetsByName[4]), typeof(AnimationClip));
        // Debug.Log(idle);
        // Debug.Log(attack);
        // Debug.Log(spAttack);
        // Debug.Log(tomandoDano);
        // Debug.Log(morrendo);
        return new []{idle, attack, spAttack, tomandoDano, morrendo};
    }

    public static AnimatorOverrideController GenerateAnimator(MonsterData monsterData, AnimationClip[] animations)
    {
        OrganizationSettings organizationSettings = GlobalSettings.Instance.OrganizationSettings;
        var animationsFolder = organizationSettings.animationsFolder;

        AnimatorOverrideController animatorOverride = new AnimatorOverrideController();
        var monsterNameSplit = RemoveWhitespace(monsterData.GetName);
        AssetDatabase.CreateAsset(animatorOverride, $"{animationsFolder}/{monsterData.GetName}/{monsterNameSplit}OverrideController.overrideController");
        animatorOverride.runtimeAnimatorController = organizationSettings.baseAnimator;
        animatorOverride["MonsterTest_Idle"] = animations[0];
        animatorOverride["MonsterTest_Atk"] = animations[1];
        animatorOverride["MonsterTest_SpAtk"] = animations[2];
        animatorOverride["MonsterTest_TomandoDano"] = animations[3];
        animatorOverride["MonsterTest_Morrendo"] = animations[4];
        return animatorOverride;
    }

    public static Sprite GetFirstSpriteFromClip(AnimationClip clip)
    {
        Sprite sprite = null;
        if (clip != null)
        {
            foreach (var binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
            {
                ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve (clip, binding);
                sprite = (Sprite)keyframes[0].value;
            }
        }
        return sprite;
    }
    
    public static string RemoveWhitespace(string input)
    {
        return new string(input.ToCharArray()
            .Where(c => !Char.IsWhiteSpace(c))
            .ToArray());
    }
}
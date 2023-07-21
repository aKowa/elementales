using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Global Settings")]
public class GlobalSettings : ScriptableSingleton<GlobalSettings>
{
    [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Hidden), TabGroup("Settings", "Balance")] 
    private BalanceSettings balanceSettings;
    
    [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Hidden), TabGroup("Settings", "Organization")] 
    private OrganizationSettings organizationSettings;
    
    [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Hidden), TabGroup("Settings", "Dices")] 
    private DiceSettings diceDiceSettings;

    [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Hidden), TabGroup("Lists")] 
    private Listas listas;
    public BalanceSettings Balance => balanceSettings;
    public OrganizationSettings OrganizationSettings => organizationSettings;
    public DiceSettings DiceSettings => diceDiceSettings;
    public Listas Listas => listas;

    public AnimationClip animationTest;

#region Editor Stuff

    // #if UNITY_EDITOR
//     [Button]
//     private void ReadAnimation() => AnimationPropertyGenerator.GenerateImageAnimationProperty(animationTest);
//
//     [SerializeField] private int attBaseTeste;
//     [SerializeField] private int ivTeste;
//     [SerializeField] private int levelTeste;
//     
//     [Button]
//     private void TestManaCalc() => MonsterAttributes.TesteDeAtributos(attBaseTeste, ivTeste, levelTeste);
// #endif
#endregion

}

// #if UNITY_EDITOR
// public class AnimationPropertyGenerator
// {
//     public static void GenerateImageAnimationProperty(AnimationClip clip)
//     {
//         string path = AssetDatabase.GetAssetPath(clip);
//         StreamReader reader = new StreamReader(path);
//         string message = reader.ReadToEnd();
//         var indexOfCurve = message.IndexOf("  - curve:");
//         var indexOfSampleRate = message.IndexOf("  m_SampleRate:");
//         var indexOfPath = message.IndexOf("    path: ");
//         string newMessage = message.Substring(indexOfCurve, indexOfPath - indexOfCurve);
//         newMessage +=
//             "    classID: 114\n    script: {fileID: 11500000, guid: fe87c0e1cc204ed48ad3b37840f39efc, type: 3}\n";
//         message = message.Insert(indexOfSampleRate, newMessage);
//         reader.Close();
//         using (var writer = new StreamWriter(path, false))
//         {
//             writer.Write(message);
//             writer.Close();
//         }
//     }
// }
//
// #endif



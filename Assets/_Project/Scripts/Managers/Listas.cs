using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Listas")]
public class Listas : ScriptableObject
{
    //Variaveis
    [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Hidden), TabGroup("Itens")] 
    private ListaDeItens listaDeItens;
    [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Hidden), TabGroup("Comandos")] 
    private ListaDeComandos listaDeComandos;
    [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Hidden), TabGroup("Monsters")]
    private ListaDeMonsterData listaDeMonsterData;
    [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Hidden), TabGroup("Status Effects")] 
    private ListaDeStatusEffects listaDeStatusEffects;
    [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Hidden), TabGroup("Maps")] 
    private ListaDeMapas _listaListaDeMapas;

    //Getters
    public ListaDeItens ListaDeItens => listaDeItens;
    public ListaDeComandos ListaDeComandos => listaDeComandos;
    public ListaDeMonsterData ListaDeMonsterData => listaDeMonsterData;
    public ListaDeStatusEffects ListaDeStatusEffects => listaDeStatusEffects;

    public ListaDeMapas ListaDeMapas => _listaListaDeMapas;

    #region Editor Stuff

    // #if UNITY_EDITOR
//
//     [Button, TabGroup("Monsters")]
//     private void GenerateBatchUIAnimations()
//     {
//         foreach (MonsterData monster in listaDeMonsterData.Data)
//         {
//             monster.GenerateAnimationImageProperty();
//         }
//     }
//
//     [Button, TabGroup("Monsters")]
//     private void GenerateBatchAnimatorOverrides()
//     {
//         foreach(MonsterData monster in listaDeMonsterData.Data)
//         {
//             if (!monster.Animator)
//             {
//                 monster.GenerateAnimatorController();
//             }
//         }
//     }
//
//     [Button, TabGroup("Monsters")]
//     private void GenerateBatchStats()
//     {
//         foreach (MonsterData monster in listaDeMonsterData.Data)
//         {
//             monster.CriarAtributosBase();
//         }
//     }
//
//     [Button, TabGroup("Monsters")]
//     private void ClearAllAttacks() => listaDeMonsterData.Data.ForEach(m => m.GetMonsterLearnableAttacks.Clear());
//     
//     [Button, TabGroup("Monsters")]
//     private void GetAttacksThatMatchType() => listaDeMonsterData.Data.ForEach(m => m.GetAttacksThatMatchType());
//
// #endif

#endregion

}

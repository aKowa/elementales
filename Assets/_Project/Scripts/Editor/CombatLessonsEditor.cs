using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

public class CombatLessonsEditor : OdinMenuEditorWindow
{
    [MenuItem("Development/Combat Lessons")]
    private static void OpenWindow()
    {
        GetWindow<CombatLessonsEditor>().Show();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.Add("Filter", new FilterCombatLessons());
        return tree;
    }

    public class FilterCombatLessons
    {
        [SerializeField] 
        private NivelMedio nivelMedio;
        private List<CombatLesson> combatLessons = new List<CombatLesson>();

        [SerializeField]
        private List<LessonLevelAndDescription> combatLessonsByLevel = new List<LessonLevelAndDescription>(); 

        [Button]
        private void OrderByLevel()
        {
            combatLessons.Clear();
            combatLessonsByLevel.Clear();
            combatLessons = GlobalSettings.Instance.Listas.ListaDeComandos.Data.FilterCast<CombatLesson>()
                .Where(cL => cL.nivelPoder == nivelMedio).ToList();
            
            combatLessons.ForEach(cL =>
            {
                combatLessonsByLevel.Add(new LessonLevelAndDescription()
                {
                    lesson = cL,
                    level = cL.nivelPoder,
                    description = cL.Descricao
                });
            });
            combatLessonsByLevel = combatLessonsByLevel.OrderBy(cL => cL.level).ToList();
        }

        [Serializable]
        private struct LessonLevelAndDescription
        {
            [HorizontalGroup("Split", Width = 0.25f)]
            [VerticalGroup("Split/Left"), HideLabel]
            public CombatLesson lesson;
            [VerticalGroup("Split/Left"), HideLabel]
            public NivelMedio level;
            [VerticalGroup("Split/Right"), MultiLineProperty, HideLabel]
            public string description;
        }
    }
}
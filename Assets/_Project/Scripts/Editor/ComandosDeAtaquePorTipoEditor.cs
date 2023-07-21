using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class ComandosDeAtaquePorTipoEditor : OdinMenuEditorWindow
{
    
    [MenuItem("Development/Monster Attacks")]
    private static void OpenWindow() 
    {
        GetWindow<ComandosDeAtaquePorTipoEditor>().Show();
    }
    
    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.Selection.SupportsMultiSelect = false;
        tree.Config.DrawSearchToolbar = true;
        tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;
        tree.FilterCast<ComandoDeAtaque>();
        tree.Add("Filter", new FilterAttack(tree));
        IEnumerable<OdinMenuItem> attacks = tree.AddAllAssetsAtPath("Monster Attacks", GlobalSettings.Instance.OrganizationSettings.attackCommandsParentFolder, typeof(ComandoDeAtaque));
        return tree;
    }

    public class FilterAttack
    {
        [OnValueChanged(nameof(ChangeFilterByRarity))]
        public MonsterType filterByRarity;

        [OnValueChanged(nameof(Sort))]
        [SerializeField] private SortingOptions sortingOptions = SortingOptions.Alphabet;

        private OdinMenuTree myTree;
        [SerializeField, TableList]
        private List<ComandoDeAtaque> comandosDeAtaque = new List<ComandoDeAtaque>();

        public FilterAttack(OdinMenuTree tree)
        {
            myTree = tree;
        }
        
        private void ChangeFilterByRarity()
        {
            var menuItems = myTree.MenuItems[1].GetChildMenuItemsRecursive(false);
            var menuList = menuItems.ToList();
            comandosDeAtaque.Clear();
            if (filterByRarity != null)
            {
                comandosDeAtaque = menuList.Where(mI =>
                {
                    ComandoDeAtaque comandoDeAtaque = (ComandoDeAtaque) mI.Value;
                    return comandoDeAtaque.AttackData.TipoAtaque == filterByRarity;
                }).Select(mI => mI.Value as ComandoDeAtaque).ToList();
            }
            else
            {
                comandosDeAtaque = menuList.Select(mI => mI.Value as ComandoDeAtaque).ToList();
            }
        }

        private void Sort()
        {
            switch (sortingOptions)
            {
                case SortingOptions.Alphabet:
                    comandosDeAtaque.Sort();
                    break;
                case SortingOptions.Attack:
                    comandosDeAtaque = comandosDeAtaque.OrderByDescending(c => c.AttackData.Poder).ToList();
                    break;
                case SortingOptions.ManaCost:
                    comandosDeAtaque = comandosDeAtaque.OrderByDescending(c => c.CustoMana).ToList();
                    break;
                case SortingOptions.PowerMargin:
                    comandosDeAtaque = comandosDeAtaque.OrderByDescending(c => c.powerMargin).ToList();
                    break;
            }
        }
    }

    protected override void OnBeginDrawEditors()
    {
        OdinMenuTreeSelection selected = this.MenuTree.Selection;

        SirenixEditorGUI.BeginHorizontalToolbar();
        {
            GUILayout.FlexibleSpace();
        }
        SirenixEditorGUI.EndHorizontalToolbar(); 
    }

    private enum SortingOptions
    {
        Attack,
        Alphabet,
        ManaCost,
        PowerMargin
    }
}
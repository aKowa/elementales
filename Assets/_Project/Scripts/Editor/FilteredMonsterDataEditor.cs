using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

public class MonsterDataEditor : OdinMenuEditorWindow
{
    
    [MenuItem("Development/Monster Data")]
    private static void OpenWindow() 
    {
        GetWindow<MonsterDataEditor>().Show();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.Selection.SupportsMultiSelect = false;
        tree.Config.DrawSearchToolbar = true;
        tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;
        // tree.Add("Menu Style", tree.DefaultMenuStyle);
        
        tree.Add("Create New", new CreateNewEnemyData());
        tree.AddAllAssetsAtPath("Monster Data", GlobalSettings.Instance.OrganizationSettings.monstersParentFolder, typeof(MonsterData));
        // tree.AddAllAssetsAtPath("Monsters Without Sounds", GlobalSettings.Instance.OrganizationSettings.monstersParentFolder, typeof(MonsterData));
        tree.EnumerateTree().AddIcons<MonsterData>(x => x.Miniatura);
        tree.Add("Filtered By Rarity", new FilterRarity(tree));
        // var customMenuItem = new OdinMenuItem(tree, "Menu Style", tree.DefaultMenuStyle);
        // tree.MenuItems[1].ChildMenuItems.Select(x =>
        // {
        //     var monster = (MonsterData)x.Value;
        //     return monster.AudioEntrada != null;
        // });
        // ForceMenuTreeRebuild();
        return tree;
    }
    
    public class FilterRarity
    {
        [OnValueChanged("ChangeFilter")]
        public MonsterRarityEnum rarityFilter;

        private OdinMenuTree myTree;
        [SerializeField, InlineEditor]
        private List<MonsterData> monstersByRarity = new List<MonsterData>();

        public FilterRarity(OdinMenuTree tree)
        {
            myTree = tree;
        }
        
        private void ChangeFilter()
        {
            var menuItems = myTree.MenuItems[1].GetChildMenuItemsRecursive(false);
            var menuList = menuItems.ToList();
            monstersByRarity.Clear();
            monstersByRarity = menuList.Where(mI =>
            {
                MonsterData monster = (MonsterData) mI.Value;
                return monster.MonsterRarity == rarityFilter;
            }).Select(mI => mI.Value as MonsterData).ToList();
        }
    }
    
    public class CreateNewEnemyData
    {
        public CreateNewEnemyData()
        {
            monsterData = ScriptableObject.CreateInstance<MonsterData>();
            monsterData.name = "New Monster Data";
        }

        [InlineEditor(Expanded = true)]
        public MonsterData monsterData;

        [Button("Add New Enemy SO")]
        private void CreateNewData()
        {
            if (GlobalSettings.Instance.OrganizationSettings.eachMonsterHasAFolder)
            {
                string folderName;
                folderName = AssetDatabase.CreateFolder(GlobalSettings.Instance.OrganizationSettings.monstersParentFolder, monsterData.GetName);
                AssetDatabase.CreateAsset(monsterData, AssetDatabase.GUIDToAssetPath(folderName) + "/" + monsterData.GetName + ".asset");
            }
            else
            {
                AssetDatabase.CreateAsset(monsterData, GlobalSettings.Instance.OrganizationSettings.monstersParentFolder + "/" + monsterData.GetName + ".asset");
            }
            AssetDatabase.SaveAssets();
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
}

public class FilteredMonsterDataEditor : OdinMenuEditorWindow
{
    
    [MenuItem("Development/Filtered Monster Data")]
    private static void OpenWindow() 
    {
        GetWindow<FilteredMonsterDataEditor>().Show();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.Selection.SupportsMultiSelect = false;
        tree.Config.DrawSearchToolbar = true;
        tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;
        
        tree.Add("Filtered By Rarity", new FilterRarity(GetWindow<MonsterDataEditor>().MenuTree));
        tree.Add("Filtered by Type", new FilterMonsterType(GetWindow<MonsterDataEditor>().MenuTree));
        return tree;
    }
    
    public class FilterRarity
    {
        [OnValueChanged("ChangeFilter")]
        public MonsterRarityEnum rarityFilter;

        private OdinMenuTree myTree;
        [SerializeField, InlineEditor]
        private List<MonsterData> monstersByRarity = new List<MonsterData>();

        public FilterRarity(OdinMenuTree tree)
        {
            myTree = tree;
        }
        
        private void ChangeFilter()
        {
            var menuItems = myTree.MenuItems[1].GetChildMenuItemsRecursive(false);
            var menuList = menuItems.ToList();
            monstersByRarity.Clear();
            monstersByRarity = menuList.Where(mI =>
            {
                MonsterData monster = (MonsterData) mI.Value;
                return monster.MonsterRarity == rarityFilter;
            }).Select(mI => mI.Value as MonsterData).ToList();
        }
    }

    public class FilterMonsterType
    {
        [OnValueChanged("ChangeFilter")]
        public MonsterType monsterType;

        private OdinMenuTree myTree;
        [SerializeField, InlineEditor]
        private List<MonsterData> monstersByRarity = new List<MonsterData>();

        public FilterMonsterType(OdinMenuTree tree)
        {
            myTree = tree;
        }
        
        private void ChangeFilter()
        {
            var menuItems = myTree.MenuItems[1].GetChildMenuItemsRecursive(false);
            var menuList = menuItems.ToList();
            monstersByRarity.Clear();
            if (monsterType != null)
            {
                monstersByRarity = menuList.Where(mI =>
                {
                    MonsterData monster = (MonsterData) mI.Value;
                    return monster.GetMonsterTypes.Contains(monsterType);
                }).Select(mI => mI.Value as MonsterData).ToList();
            }
            else
            {
                monstersByRarity = menuList.Select(mI => mI.Value as MonsterData).ToList();
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
}
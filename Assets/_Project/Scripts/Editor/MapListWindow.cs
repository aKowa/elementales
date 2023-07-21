using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using ObjectFieldAlignment = Sirenix.OdinInspector.ObjectFieldAlignment;

public class MapListWindow : OdinMenuEditorWindow
{
    [MenuItem("Development/Maps")]
    private static void Open()
    {
        var window = GetWindow<MapListWindow>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree(true);
        tree.DefaultMenuStyle.IconSize = 28.00f;
        tree.Config.DrawSearchToolbar = true;

        // tree.Add("Maps", new MapTable(GlobalSettings.Instance.Listas.ListaDeMapas.Data));

        tree.AddAllAssetsAtPath("Maps", GlobalSettings.Instance.OrganizationSettings.mapsParentFolder, typeof(MapData));

        tree.EnumerateTree().AddIcons<MapData>(x => x.mapPreview);
        tree.SortMenuItemsByName();

        return tree;
    }
}

public class MapTable
{
    [TableList(IsReadOnly = true, AlwaysExpanded = true), ShowInInspector]
    private readonly List<MapDataWrapper> allMaps;

    public MapData this[int index]
    {
        get { return this.allMaps[index].MapData; }
    }

    public MapTable(IEnumerable<MapData> maps)
    {
        this.allMaps = maps.Select(x => new MapDataWrapper(x)).ToList();
    }

    private class MapDataWrapper
    {
        private MapData mapData;

        [HorizontalGroup("Map")]
        [TableColumnWidth(50, false)]
        [ShowInInspector, PreviewField(45, ObjectFieldAlignment.Center)]
        public Sprite Icon { get { return this.MapData.mapPreview; } set { this.MapData.mapPreview = value; EditorUtility.SetDirty(this.MapData); } }

        [HorizontalGroup("Monsters")]
        [TableColumnWidth(75, false)]
        [ShowInInspector, PreviewField(45, ObjectFieldAlignment.Center), ListDrawerSettings(Expanded = true)]

        public MapData MapData => mapData;

        public MapDataWrapper(MapData map)
        {
            mapData = map;
        }
    }
}


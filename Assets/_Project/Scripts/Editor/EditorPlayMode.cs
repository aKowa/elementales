using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class MenuTest
{
    [MenuItem("Development/Play From Menu")]
    static void PlayGameFromBeginning()
    {
        EditorSceneManager.OpenScene("Assets/_Project/Scenes/Menus/Logos.unity");
        EditorApplication.ExecuteMenuItem("Edit/Play");
    }
}
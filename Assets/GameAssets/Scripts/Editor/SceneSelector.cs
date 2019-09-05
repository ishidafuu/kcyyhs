using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneSelector : Editor
{
    [MenuItem("SceneSelector/Main _F1")]
    static public void OpenMainScene()
    {
        OpenScene("Assets/GameAssets/Scenes/Main.unity");
    }

    [MenuItem("SceneSelector/AnimationEditor _F2")]
    static public void OpenAnimationEditorScene()
    {
        OpenScene("Assets/GameAssets/Scenes/AnimationEditor.unity");
    }

    private static void OpenScene(string scene_path)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scene_path);
        }
    }

}
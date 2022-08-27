using UnityEngine;
using UnityEditor;

// Prints the name of the focused window to a label

public class CustomTools : EditorWindow
{
    [MenuItem("Examples/FocusedWindow")]
    public static void Init()
    {
        GetWindow<CustomTools>("FocusedWindow");
    }

    void OnGUI()
    {
        GUILayout.Label(EditorWindow.focusedWindow.GetType().ToString());
        if(focusedWindow.GetType().ToString() == "UnityEditor.ShaderGraph.Drawing.MaterialGraphEditWindow")
        {
            GUILayout.Label(focusedWindow.titleContent.text);
            Shader myShader = Shader.Find("Shader Graphs/" + focusedWindow.titleContent.text);
            Debug.Log(AssetDatabase.GetAssetPath(myShader));
        }
    }
}
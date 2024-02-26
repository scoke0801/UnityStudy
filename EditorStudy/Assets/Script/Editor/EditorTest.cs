using UnityEditor;
using UnityEngine;

public class EditorTest : EditorWindow
{
    [MenuItem("Tools/Open My Tool %g")]
    static void Open()
    {
        var window = GetWindow<EditorTest>();
        window.title = "ToolTest";
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Label");
        EditorGUILayout.TextField("TextField");
        GUILayout.Button("Button");
    }
}

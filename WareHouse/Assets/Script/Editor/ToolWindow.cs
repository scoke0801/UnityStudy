using UnityEditor;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.Rendering;

public enum EditorMode
{ 
    None = 0,
    Create,
    Edit,
}

public class ToolWindow : EditorWindow
{
    public EditorMode CurrentMode = EditorMode.None;

    bool IsCreateable => false;

    [MenuItem("Tools/Open My Tool")]
    static void Open()
    {
        var window = GetWindow<ToolWindow>();
        window.title = "This is my Tool";
    }

    private void OnEnable()
    {
        ChangeMode(EditorMode.Create);
    }

    private void OnGUI()
    {
        if (CurrentMode == EditorMode.Create)
        {
            DrawCreateMode();
        }
        else if(CurrentMode == EditorMode.Edit)
        {
            DrawEditMode();
        }
    }

    void DrawCreateMode()
    {
        EditorHelper.DrawCenterLabel(new GUIContent("积己 葛靛"), Color.green, 20, FontStyle.BoldAndItalic);
        using (var scope = new GUILayout.VerticalScope(GUI.skin.window))
        {
        }

        bool prev = GUI.enabled;
        GUI.enabled = IsCreateable;
        if(EditorHelper.DrawCenterButton("积己窍扁", new Vector2(100,50)))
        {
        }
        GUI.enabled = prev;
    }

    void DrawEditMode()
    {
        
    }

    void ChangeMode(EditorMode newMode)
    {
        if (CurrentMode == newMode)
        {
            return;
        }

        switch (newMode)
        {
            case EditorMode.None:
                {
                    Debug.Log("Switched to Create Mode");
                }
                break;
            case EditorMode.Edit:
                {
                    Debug.Log("Switched To Edit Mode");
                }
                break;
            case EditorMode.Create:
                {

                }
                break;
            default: 
                break;
        }

        CurrentMode = newMode;
    }
}

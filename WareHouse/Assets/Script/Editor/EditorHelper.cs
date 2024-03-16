using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;
using UnityEngine.UIElements;

public class EditorHelper
{
    public static void DrawCenterLabel(GUIContent content, Color color, int size, FontStyle style)
    {
        var guiStyle = new GUIStyle();
        guiStyle.fontSize = size;
        guiStyle.fontStyle = style;
        guiStyle.normal.textColor = color;
        guiStyle.padding.top = 10;
        guiStyle.padding.bottom = 10;

        GUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();

            GUILayout.Label(content, guiStyle);

            GUILayout.FlexibleSpace();
        }
        GUILayout.EndHorizontal();
    }

    public static bool DrawCenterButton(string text, Vector2 size)
    {
        bool clicked = false;

        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();

            clicked = GUILayout.Button(text, GUILayout.Width(size.x), GUILayout.Height(size.y));

            GUILayout.FlexibleSpace();
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(5);

        return clicked;
    }
}

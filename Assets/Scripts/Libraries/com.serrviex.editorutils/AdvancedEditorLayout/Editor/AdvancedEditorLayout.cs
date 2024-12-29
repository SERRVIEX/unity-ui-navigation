using UnityEngine;

using UnityEditor;

public static class AdvancedEditorLayout
{
    public static void Headline(string label)
    {
        {
            Rect rect = EditorGUILayout.GetControlRect(true, 0);
            rect.position = new Vector2(0, rect.position.y);
            rect.width = EditorGUIUtility.currentViewWidth;
            rect.height = 20;

            if (EditorGUIUtility.isProSkin)
                EditorGUI.DrawRect(rect, new Color32(40, 40, 40, 255));
            else
                EditorGUI.DrawRect(rect, new Color32(150, 150, 150, 255));
        }

        {
            Rect rect = EditorGUILayout.GetControlRect(false, 16);
            rect.position = new Vector2(16, rect.position.y);

            GUIStyle labelStyle = new GUIStyle("label");
           
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.fixedWidth = EditorGUIUtility.currentViewWidth - 5;
            labelStyle.stretchWidth = true;
            EditorGUI.LabelField(rect, label, labelStyle);
        }

        GUILayout.Space(3);

        GUI.color = Color.white;
    }

    public static void HorizontalLine()
    {
        GUILayout.Space(5);

        if (EditorGUIUtility.isProSkin)
            GUI.color = new Color32(50, 50, 50, 255);
        else
            GUI.color = new Color32(150, 150, 150, 255);

        GUIStyle style = new GUIStyle();
        style.normal.background = EditorGUIUtility.whiteTexture;
        style.margin = new RectOffset(0, 0, 4, 4);
        style.fixedHeight = 1;

        GUILayout.Box(GUIContent.none, style);
        GUI.color = Color.white;

        GUILayout.Space(5);
    }

    public static Texture2D CreateTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }
}
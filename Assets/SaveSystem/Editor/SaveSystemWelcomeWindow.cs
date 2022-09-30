using UnityEditor;
using UnityEngine;

internal class SaveSystemWelcomeWindow : EditorWindow
{
    [InitializeOnLoadMethod]
    private static void OnProjectLoadedInEditor()
    {
        if (EditorPrefs.GetBool(SaveSystemEditorPrefsKeys.IsSaveSystemFirstStatrt.ToString(), false))
        {
            ShowWindow();
            EditorPrefs.SetBool(SaveSystemEditorPrefsKeys.IsSaveSystemFirstStatrt.ToString(), true);
        }
    }

    [MenuItem("SaveSystem/Get Started!")]
    public static void ShowWindow()
    {
        var window = GetWindow<SaveSystemWelcomeWindow>("SaveSystem by Psyxoz13");

        window.maxSize = new Vector2(450f, 400f);
        window.minSize = new Vector2(450f, 400f);
    }

    private void CreateGUI()
    {
        SaveSystemEditorIconsData.LoadIcon(ref SaveSystemEditorIconsData.Nick, "Psyxoz13.png");
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        var text = "<b><size=25>  Greetings!</size></b>\n\n" +
                   "  To configure the asset, go to the <b><i><color=#e06666>SaveSystem -> Settings</color></i></b> tab!\n\n" +
                   "  Thanks for using!";

        var textStyle = new GUIStyle()
        {
            fontSize = 14
        };
        textStyle.normal.textColor = Color.white;

        EditorGUILayout.LabelField(text, textStyle, GUILayout.Height(310));

        EditorGUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();

        EditorGUILayout.LabelField("by", textStyle, GUILayout.Width(20));

        var nickIcon = new GUIContent(SaveSystemEditorIconsData.Nick.Texture);

        if (GUILayout.Button(nickIcon, new GUIStyle() { margin = new RectOffset(5, 15, -29, 0) }, GUILayout.Height(80), GUILayout.Width(120)))
        {
            Application.OpenURL("https://github.com/Psyxoz13");
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }
}

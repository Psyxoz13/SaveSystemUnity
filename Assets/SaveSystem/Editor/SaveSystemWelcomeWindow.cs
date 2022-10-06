using UnityEditor;
using UnityEngine;

namespace SSystem
{
    internal class SaveSystemWelcomeWindow : EditorWindow
    {
        private static ISaveSystem _saveSystem;

        [InitializeOnLoadMethod]
        private static void OnProjectLoadedInEditor()
        {
            _saveSystem = new SaveSystemJSON("json", "ProjectSettings");

            var model = _saveSystem.Load<SaveSystemWelcome>();

            if (model.IsFirstShowed == false)
            {
                SaveSystemEditorIconsData.LoadIcon(ref SaveSystemEditorIconsData.Nick, "Psyxoz13.png");

                ShowWindow();

                model.IsFirstShowed = true;

                _saveSystem.Save(model);
            }
        }

        [MenuItem("SaveSystem/Get Started!", priority = 0)]
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
                       "     To configure the asset, go to the <b><i><color=#b71540>SaveSystem -> Settings</color></i></b> tab \n" +
                       "  or click the <b><i><color=#b71540>Open Settings</color></i></b> button!\n\n";

            var textStyle = new GUIStyle()
            {
                fontSize = 14,
                padding = new RectOffset(5, 5, 5, 5),
                richText = true
            };
            textStyle.normal.textColor = Color.white;

            EditorGUILayout.LabelField(text, textStyle, GUILayout.Height(100));

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Open Settings", new GUIStyle("button") { fontSize = 15 }, GUILayout.Width(130), GUILayout.Height(40)))
            {
                GetWindow<SaveSystemSettingsWindow>();
                Close();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(170);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("  Thanks for using!", textStyle, GUILayout.Height(100));
            
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

    [System.Serializable]
    internal class SaveSystemWelcome
    {
        public bool IsFirstShowed;
    }
}
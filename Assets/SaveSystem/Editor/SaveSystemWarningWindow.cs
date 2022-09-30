using UnityEditor;
using UnityEngine;

public class SaveSystemWarningWindow : EditorWindow
{
    internal enum WarningType : uint
    {
        Warning = 0xFFE084,
        Error = 0xFF5A5A,
        Message = 0xffffff
    }

    private static WarningType _warningType;
    private static string _message;

    internal static void Show(string message, WarningType warningType)
    {
        _warningType = warningType;
        _message = message;

        var window = GetWindow<SaveSystemWarningWindow>(warningType.ToString());

        window.maxSize = new Vector2(220f, 80f);
        window.minSize = new Vector2(220f, 80f);
    }

    private void OnGUI()
    {
        string text = string.Empty;

        var hex = string.Format("{0:X}", _warningType).Substring(2);

        text += $"<b><color=#{hex}><size=16>{ _warningType }</size></color></b>" + "\n\n";
        text += _message;

        var textStyle = new GUIStyle()
        {
            fontSize = 13,
            richText = true,
            margin = new RectOffset(10, 10, 10, 10)
        };
        textStyle.normal.textColor = Color.white;

        EditorGUILayout.LabelField(text, textStyle, GUILayout.Height(310));
    }
}

using UnityEditor;
using UnityEngine;

public class ReadonlyField : IField
{
    public SerializedProperty SerializedProperty { get; set; }
    public GUIContent Content { get; set; }

    public Rect Rectangle { get; set; }

    public bool IsShow { get; set; }

    public void Draw()
    {
        if (!IsShow)
        {
            GUI.enabled = false;
        }

        EditorGUI.PropertyField(Rectangle, SerializedProperty, Content, true);
        GUI.enabled = true;
    }
}


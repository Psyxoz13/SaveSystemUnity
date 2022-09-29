using UnityEditor;
using UnityEngine;

public class DontDrawField : IField
{
    public SerializedProperty SerializedProperty { get; set; }
    public GUIContent Content { get; set; }

    public Rect Rectangle { get; set; }

    public bool IsShow { get; set; }

    public void Draw()
    {
        if (IsShow)
        {
            EditorGUI.PropertyField(Rectangle, SerializedProperty, Content, true);
        }
        else
        {
            Rectangle = Rect.zero;
        }
    }
}

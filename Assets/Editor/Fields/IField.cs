using UnityEditor;
using UnityEngine;

public interface IField
{
    SerializedProperty SerializedProperty { get; set; }
    GUIContent Content { get; set; }

    Rect Rectangle { get; set; }

    bool IsShow { get; set; }

    void Draw();
}



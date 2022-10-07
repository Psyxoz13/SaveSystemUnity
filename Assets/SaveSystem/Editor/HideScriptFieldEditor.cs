using UnityEditor;
using UnityEngine;

public class HideScriptFieldEditor : Editor
{
    private bool _isHideScriptField;

    private void OnEnable()
    {
        _isHideScriptField = target.GetType().GetCustomAttributes(_isHideScriptField.GetType(), false).Length > 0;
    }

    public override void OnInspectorGUI()
    {
        if (_isHideScriptField)
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
        else
        {
            base.OnInspectorGUI();
        }
    }
}

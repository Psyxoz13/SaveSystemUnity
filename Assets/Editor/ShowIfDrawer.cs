using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    private IField _field;
    private ShowIfAttribute _attribute;

    public override float GetPropertyHeight(
        SerializedProperty property,
        GUIContent label)
    {
        _attribute = attribute as ShowIfAttribute;

        _field = GetField(_attribute.FieldType);

        var conditionPropertyField = property.serializedObject.FindProperty(_attribute.FieldName);
        var conditionPropertyFieldValue = GetValue(conditionPropertyField);
        _field.IsShow = CompareValues(
            _attribute.ConditionalValue,
            conditionPropertyFieldValue,
            _attribute.ComparisonType);

        if (_field.IsShow)
            return EditorGUI.GetPropertyHeight(property, label, true);
        else
            return 0f;
    }

    public override void OnGUI(
        Rect position,                        
        SerializedProperty property,
        GUIContent label)
    {

        _field.Rectangle = position;
        _field.SerializedProperty = property;
        _field.Content = label;

        _field.Draw();
    }

    private object GetValue(SerializedProperty property)
    {
        var targetObject = property.serializedObject.targetObject;
        var targetObjectClassType = targetObject.GetType();
        var field = targetObjectClassType.GetField(property.propertyPath);

        return field.GetValue(targetObject);
    }

    private bool CompareValues(object a, object b, ComparisonType comparisonType)
    {
        switch (comparisonType)
        {
            case ComparisonType.Equals:
                return a.Equals(b);
            case ComparisonType.NotEqual:
                return a.Equals(b) == false;
            default:
                throw new System.ArgumentException("Invalid enum value", nameof(comparisonType));
        };
    }


    private IField GetField(FieldType fieldType)
    {
        switch(fieldType)
        {
            case FieldType.DontDraw: 
                return new DontDrawField();
            case FieldType.Readonly: 
                return new ReadonlyField();
            case FieldType.DontDrawReadonly: 
                return new DontDrawReadonlyField();
            default:
                throw new System.ArgumentException("Invalid enum value", nameof(fieldType));
        };
    }
}
using System;
using UnityEngine;

public class ShowIfAttribute : PropertyAttribute
{
    public string FieldName { get; private set; }
    public object ConditionalValue { get; private set; }
    public ComparisonType ComparisonType { get; private set; }
    public FieldType FieldType;

    public ShowIfAttribute(string fieldName, object conditionalValue, FieldType fieldType, ComparisonType comparisonType = ComparisonType.Equals)
    {
        FieldName = fieldName;
        ConditionalValue = conditionalValue;
        FieldType = fieldType;
        ComparisonType = comparisonType;
    }
}

public enum ComparisonType
{
    Equals,
    NotEqual
}

public enum FieldType
{
    DontDraw,
    Readonly,
    DontDrawReadonly
}

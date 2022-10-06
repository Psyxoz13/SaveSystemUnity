using SSystem;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class SaveSystemSaveEditorWindow : EditorWindow
{
    private TypeMemoryCache<SaveSystemSettingsWindow.CacheStyleType, GUIStyle> _stylesCache = new TypeMemoryCache<SaveSystemSettingsWindow.CacheStyleType, GUIStyle>();

    private Type _saveFileType;

    private ISaveSystem _saveSystem;

    private Vector2 _scrollPosition;

    private object _saveObject;
    private string _saveFilePath;

    private void CreateGUI()
    {
        SaveSystemEditorIconsData.LoadIcon(ref SaveSystemEditorIconsData.File, "file-icon.png");
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        DrawReadonlyTextField("Save File", _saveFilePath);

        if (GUILayout.Button(
            SaveSystemEditorIconsData.File.Texture,
            _stylesCache.Get(
                SaveSystemSettingsWindow.CacheStyleType.SelectDirectory),
            GUILayout.MaxWidth(20),
            GUILayout.MaxHeight(18)))
        {
            var saveSystemConfigData = Resources.Load<SaveSystemConfigData>("SaveSystem/SaveSystemData");
            var saveFilePath = EditorUtility.OpenFilePanel("Select Save File", saveSystemConfigData.Path, saveSystemConfigData.FileFormat);

            if (string.IsNullOrEmpty(saveFilePath) == false)
            {
                _saveFilePath = saveFilePath;
            }

            switch (saveSystemConfigData.SaveSystemVariation)
            {
                case SaveSystem.SaveVariations.Json:
                    _saveSystem = new SaveSystemJSON(saveSystemConfigData.FileFormat, saveSystemConfigData.Path);
                    break;
                case SaveSystem.SaveVariations.Binary:
                    _saveSystem = new SaveSystemBinary(saveSystemConfigData.FileFormat, saveSystemConfigData.Path);
                    break;
                default:
                    break;
            }

            var name = Path.GetFileNameWithoutExtension(saveFilePath);

            _saveFileType = GetTypeByName(name);
            _saveObject = _saveSystem.Load(_saveFileType);
        }

        EditorGUILayout.EndHorizontal();

        if (string.IsNullOrEmpty(_saveFilePath))
        {
            return;
        }

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, EditorStyles.helpBox);

        EditorGUI.BeginChangeCheck();

        SerializeObjectReflection(_saveObject, _saveFileType);

        if (EditorGUI.EndChangeCheck())
        {
            _saveSystem.Save(_saveObject, _saveFileType);
        }

        EditorGUILayout.EndScrollView();
    }

    private void SerializeObjectReflection(object obj, Type type)
    {
        var fields = type.GetFields();

        foreach (var field in fields)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(field.FieldType.Name, GUILayout.Width(100));

            switch (field.FieldType.Name)
            {
                case "Single":
                    field.SetValue(obj, EditorGUILayout.FloatField(field.Name, (float)field.GetValue(obj)));
                    break;
                case "Int32":
                    field.SetValue(obj, EditorGUILayout.IntField(field.Name, (int)field.GetValue(obj)));
                    break;
                case "String":
                    field.SetValue(obj, EditorGUILayout.TextField(field.Name, field.GetValue(obj).ToString()));
                    break;
                case "Double":
                    field.SetValue(obj, EditorGUILayout.DoubleField(field.Name, (double)field.GetValue(obj)));
                    break;
                case "Enum":
                    field.SetValue(obj, EditorGUILayout.EnumPopup(field.Name, (Enum)field.GetValue(obj)));
                    break;
                default:
                    try
                    {
                        field.SetValue(obj, EditorGUILayout.ObjectField(field.Name, (UnityEngine.Object)field.GetValue(obj), field.FieldType, true));
                    }
                    catch
                    {
                        try
                        {
                            EditorGUILayout.BeginVertical();

                            SerializeObjectReflection(field.GetValue(obj), field.FieldType);

                            EditorGUILayout.EndVertical();
                        }
                        catch
                        {
                            EditorGUILayout.LabelField(field.FieldType.Name, field.GetValue(obj).ToString());
                        }
                    }
                    break;
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawReadonlyTextField(string label, string text)
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth - 1));
            EditorGUILayout.SelectableLabel(text, _stylesCache.Get(SaveSystemSettingsWindow.CacheStyleType.ReadonlyTextField), GUILayout.Height(EditorGUIUtility.singleLineHeight));
        }
        EditorGUILayout.EndHorizontal();
    }

    private Type GetTypeByName(string name)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Reverse())
        {
            var tt = assembly.GetType(name);
            if (tt != null)
            {
                return tt;
            }
        }

        return null;
    }
}

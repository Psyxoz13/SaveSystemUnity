using SSystem;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SaveSystemSaveEditorWindow : EditorWindow
{
    private TypeMemoryCache<CacheStyleType, GUIStyle> _stylesCache = new TypeMemoryCache<CacheStyleType, GUIStyle>();

    private Type _saveFileType;

    private ISaveSystem _saveSystem;

    private Vector2 _scrollPosition;

    private object _saveObject;
    private string _saveFilePath;

    private void CreateGUI()
    {
        SaveSystemEditorIconsData.LoadIcon(ref SaveSystemEditorIconsData.File, "file-icon.png");

        _stylesCache.Cache(
            CacheStyleType.RedLabel,
            new GUIStyle(EditorStyles.label)
            {
                normal = new GUIStyleState()
                {
                    textColor = new Color(.89804f, .31373f, .22353f)
                }
            });

        _stylesCache.Cache(
            CacheStyleType.BlueLabel,
            new GUIStyle(EditorStyles.label)
            {
                normal = new GUIStyleState()
                {
                    textColor = new Color(.29f, .4117f, .741f)
                }
            });
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        DrawReadonlyTextField("Save File", _saveFilePath);

        if (GUILayout.Button( 
            SaveSystemEditorIconsData.File.Texture,
            _stylesCache.Get(
                CacheStyleType.SelectDirectory),
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

        DrawTypeLabel(_saveFileType, new GUIStyle(EditorStyles.largeLabel));

        EditorGUI.BeginChangeCheck();

        SerializeObjectReflection(_saveObject, _saveFileType);

        if (EditorGUI.EndChangeCheck())
        {
            _saveSystem.Save(_saveObject, _saveFileType);
        }

        EditorGUILayout.EndScrollView();
    }

    private void SerializeObjectReflection(object obj, Type type, bool isSubClass = false)
    {
        var fields = type.GetFields();

        foreach (var field in fields)
        {
            EditorGUILayout.BeginHorizontal();

            switch (field.FieldType.Name)
            {
                case "Single":
                    DrawTypeLabel(field.FieldType, _stylesCache.Get(CacheStyleType.BlueLabel));
                    field.SetValue(obj, EditorGUILayout.FloatField(field.Name, (float)field.GetValue(obj)));
                    break;
                case "Int32":
                    DrawTypeLabel(field.FieldType, _stylesCache.Get(CacheStyleType.BlueLabel));
                    field.SetValue(obj, EditorGUILayout.IntField(field.Name, (int)field.GetValue(obj)));
                    break;
                case "String":
                    DrawTypeLabel(field.FieldType, _stylesCache.Get(CacheStyleType.BlueLabel));
                    field.SetValue(obj, EditorGUILayout.TextField(field.Name, field.GetValue(obj).ToString()));
                    break;
                case "Double":
                    DrawTypeLabel(field.FieldType, _stylesCache.Get(CacheStyleType.BlueLabel));
                    field.SetValue(obj, EditorGUILayout.DoubleField(field.Name, (double)field.GetValue(obj)));
                    break;
                case "Enum":
                    DrawTypeLabel(field.FieldType, _stylesCache.Get(CacheStyleType.BlueLabel));
                    field.SetValue(obj, EditorGUILayout.EnumPopup(field.Name, (Enum)field.GetValue(obj)));
                    break;
                default:
                    try
                    {
                        var value = (UnityEngine.Object)field.GetValue(obj);

                        DrawTypeLabel(field.FieldType, _stylesCache.Get(CacheStyleType.BlueLabel));
                        field.SetValue(obj, EditorGUILayout.ObjectField(
                            field.Name,
                            value,
                            field.FieldType,
                            true));
                    }
                    catch
                    {
                        try
                        {
                            EditorGUILayout.Space();

                            EditorGUILayout.BeginVertical();
                            EditorGUILayout.Space();

                            DrawTypeLabel(field.FieldType, _stylesCache.Get(CacheStyleType.RedLabel));
                            SerializeObjectReflection(field.GetValue(obj), field.FieldType, true);

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

    private void DrawTypeLabel(Type type, GUIStyle style)
    {
        EditorGUILayout.LabelField(
            type.Name,
            style,
            GUILayout.MaxWidth(200));
    }

    private void DrawReadonlyTextField(string label, string text)
    {
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth - 1));
            EditorGUILayout.SelectableLabel(text, _stylesCache.Get(CacheStyleType.ReadonlyTextField), GUILayout.Height(EditorGUIUtility.singleLineHeight));
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

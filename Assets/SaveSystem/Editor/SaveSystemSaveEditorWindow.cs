using SSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SaveSystemSaveEditorWindow : EditorWindow
{
    [SerializeReference] private dynamic _saveObject;

    private TypeMemoryCache<CacheStyleType, GUIStyle> _stylesCache = new TypeMemoryCache<CacheStyleType, GUIStyle>();

    private Editor _saveFileEditor;
    private Type _saveFileType;

    private ISaveSystem _saveSystem;

    private Vector2 _scrollPosition;

    private string _saveFilePath;

    private static readonly string[] _excludedFields = 
    { 
        "m_Script"
    };

    private static readonly Dictionary<string, string> _renameFields = new Dictionary<string, string>
    {
        { "_saveObject", "View" }
    };

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

        _saveFileEditor = Editor.CreateEditor(this);
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



        DrawProperties(_saveFileEditor.serializedObject, _excludedFields);



        EditorGUILayout.EndScrollView();
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

    private void DrawProperties(SerializedObject obj, params string[] propertyToExclude)
    {
        EditorGUI.BeginChangeCheck();

        SerializedProperty iterator = obj.GetIterator();

        bool enterChildren = true;

        while (iterator.NextVisible(enterChildren))
        {
            enterChildren = false;

            var label = new GUIContent();

            if (_renameFields.ContainsKey(iterator.name))
            {
                label.text = _renameFields[iterator.name];
            }

            if (!propertyToExclude.Contains(iterator.name))
            {
                EditorGUILayout.PropertyField(iterator, label, true);
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            _saveFileEditor.serializedObject.ApplyModifiedProperties();
            _saveSystem.Save(_saveObject, _saveFileType);
        }
    }
}

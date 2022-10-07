using System.IO;
using UnityEditor;
using UnityEngine;
using SSystem;
using System;

public class SaveSystemSettingsWindow : EditorWindow
{
    [SerializeField] private string _configGUID;

    [SerializeField] private SaveSystem.SaveVariations _saveSystemVariation;
    [SerializeField] private PathOptions _pathOption;

    [SerializeField] private string _path;
    [SerializeField] private string _subPath;
    [SerializeField] private string _fileFormat;
    [SerializeField] private bool _isFormatEdit;
    [SerializeField] private bool _isToolsShow;
    [SerializeField] private bool _isDebugShow;

    private const string SaveSystemConfigDataPath = "Assets/Resources/SaveSystem/SaveSystemData.asset";
    private const string SaveSystemConfigsDirectory = "Assets/SaveSystemConfigs/";

    private TypeMemoryCache<CacheStyleType, GUIStyle> _stylesCache = new TypeMemoryCache<CacheStyleType, GUIStyle>();

    private SaveSystemJSON _saveSystem;
    private SaveSystemConfig _config;

    [MenuItem("SaveSystem/Settings")]
    public static void ShowWindow()
    {       
        var window = GetWindow<SaveSystemSettingsWindow>("SaveSystem Settings");

        window.minSize = new Vector2(500f, 400f);
    }

    private void CreateGUI()
    {
        SaveSystemEditorIconsData.LoadIcon(ref SaveSystemEditorIconsData.Folder, "folder-icon.png");
        SaveSystemEditorIconsData.LoadIcon(ref SaveSystemEditorIconsData.Edit, "edit-icon.png");

        _stylesCache.Cache(CacheStyleType.ToolButton, new GUIStyle("button")
        {
            margin = new RectOffset(5, 5, 5, 5),
            imagePosition = ImagePosition.ImageAbove,
            padding = new RectOffset(5, 5, 5, 5),
            fixedWidth = 70,
            fixedHeight = 50,
            richText = true,
            fontSize = 11
        });

        _stylesCache.Cache(CacheStyleType.SelectDirectory, new GUIStyle("button")
        {
            padding = new RectOffset(3, 3, 3, 3)
        });

        _stylesCache.Cache(CacheStyleType.ContentBox, new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(10, 7, 7, 7)
        });

        _stylesCache.Cache(CacheStyleType.ContentBoxSmall, new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(10, 7, 3, 3)
        });

        _stylesCache.Cache(CacheStyleType.ReadonlyTextField, new GUIStyle(EditorStyles.textField)
        {
            normal = new GUIStyleState()
            {
                textColor = Color.gray
            },
            focused = new GUIStyleState()
            {
                textColor = new Color(.98f, .83f, .565f)
            }
        });

        TrySetFormat();
    }

    private void OnEnable()
    {
        _saveSystem = new SaveSystemJSON("json", "ProjectSettings");
        _saveSystem.Overwrite(this);

        var configPath = AssetDatabase.GUIDToAssetPath(_configGUID);
        _config = AssetDatabase.LoadAssetAtPath<SaveSystemConfig>(configPath);
    }

    private void OnDisable()
    {
        if (_config != null && AssetDatabase.TryGetGUIDAndLocalFileIdentifier(_config, out string guid, out long _))
        {
            _configGUID = guid;
        }

        _saveSystem = new SaveSystemJSON("json", "ProjectSettings");
        _saveSystem.Save(this);
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal(_stylesCache.Get(CacheStyleType.ContentBoxSmall));

        _config = (SaveSystemConfig)EditorGUILayout.ObjectField("Config", _config, typeof(SaveSystemConfig), true);

        if (GUILayout.Button("New", GUILayout.Width(40)))
        {
            string path;

            if (_config != null)
            {
                path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(_config)) + "/";
            }
            else
            {
                path = SaveSystemConfigsDirectory;
            }

            if (Directory.Exists(SaveSystemConfigsDirectory) == false)
            {
                Directory.CreateDirectory(SaveSystemConfigsDirectory);
            }

            var uniqueFilePath = AssetDatabase.GenerateUniqueAssetPath(path + "SaveSystemConfig.asset");
            var config = CreateInstance<SaveSystemConfig>();

            config.Path = Application.persistentDataPath;

            AssetDatabase.CreateAsset(config, uniqueFilePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            _config = config;

            EditorGUIUtility.PingObject(config);
        }

        EditorGUILayout.EndHorizontal();

        if (_config == null)
        {
            return;
        }

        if (EditorGUI.EndChangeCheck())
        {
            TrySetFormat();
            SetParams();
            SetConfig();
        }

        EditorGUILayout.Space(2);

        EditorGUILayout.BeginVertical(_stylesCache.Get(CacheStyleType.ContentBox));

        EditorGUI.BeginChangeCheck();

        _saveSystemVariation = (SaveSystem.SaveVariations)EditorGUILayout.EnumPopup("Save System", _saveSystemVariation);

        EditorGUILayout.Space();

        _pathOption = (PathOptions)EditorGUILayout.EnumPopup("Path Option", _pathOption);

        string pathPattern;

        switch (_pathOption)
        {
            case PathOptions.PersistentDataPath:
                {
                    pathPattern = Application.persistentDataPath + "/";
                    DrawSubPathInput(pathPattern);
                }
                break;
#if UNITY_STANDALONE_WIN
            case PathOptions.Documents:
                {
                    pathPattern = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments).Replace('\\', '/') + "/";
                    DrawSubPathInput(pathPattern);
                }
                break;
#endif
            case PathOptions.Manual:
                {
                    EditorGUILayout.BeginHorizontal();

                    _path = EditorGUILayout.TextField("Path", _path);

                    if (GUILayout.Button(SaveSystemEditorIconsData.Folder.Texture, _stylesCache.Get(CacheStyleType.SelectDirectory), GUILayout.MaxWidth(20), GUILayout.MaxHeight(18)))
                    {
                        if (Directory.Exists(_path) == false)
                        {
                            Debug.unityLogger.LogError("Error", "Directory does not exist.", this);
                        }
                        else
                        {
                            var selectedPath = EditorUtility.OpenFolderPanel("Select Directory", _path, "");

                            if (string.IsNullOrEmpty(selectedPath) == false)
                            {
                                _path = selectedPath;
                            }
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }
                break;
        }

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        _isFormatEdit = EditorGUILayout.Toggle("Edit File Format", _isFormatEdit);
        
        if (_isFormatEdit)
        {
            _fileFormat = EditorGUILayout.TextField(_fileFormat);
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        if (EditorGUI.EndChangeCheck())
        {
            TrySetFormat();
            SetConfig();
        }

        EditorGUILayout.Space(15);

        _isDebugShow = EditorGUILayout.Foldout(_isDebugShow, "Debug", true, new GUIStyle(EditorStyles.foldout) 
        { 
            onNormal =
            {
                textColor = new Color(.92157f, .18431f, .02353f)
            }
        });

        if (_isDebugShow)
        {
            EditorGUILayout.BeginVertical(_stylesCache.Get(CacheStyleType.ContentBoxSmall));

            switch (_pathOption)
            {
                case PathOptions.PersistentDataPath:
                    DrawReadonlyTextField("IOS Path", "/var/mobile/Containers/Data/Application/<guid>/Documents/" + _subPath);
                    DrawReadonlyTextField("Android Path", "/storage/emulated/0/Android/data/<packagename>/files/" + _subPath);
                    break;
                case PathOptions.Manual:
                    DrawReadonlyTextField("IOS Path", "/var/mobile/" + _path);
                    DrawReadonlyTextField("Android Path", "/storage/emulated/0/" + _path);
                    break;
                default:
                    break;
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space(15);

        _isToolsShow = EditorGUILayout.Foldout(_isToolsShow, "Tools", true, new GUIStyle(EditorStyles.foldout)
        {
            onNormal =
            {
                textColor = new Color(.98039f, .59608f, .22745f)
            }
        });

        if (_isToolsShow)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();

            var openPathFolderContent = new GUIContent("Open Path", SaveSystemEditorIconsData.Folder.Texture);

            if (GUILayout.Button(openPathFolderContent, _stylesCache.Get(CacheStyleType.ToolButton)))
            {
                if (Directory.Exists(_path) == false)
                {
                    Debug.unityLogger.LogError("Error", "Directory does not exist.", this);
                }
                else
                {
                    Application.OpenURL(_path);
                }
            }

            var editSaveContent = new GUIContent("Save Editor", SaveSystemEditorIconsData.Edit.Texture);

            if (GUILayout.Button(editSaveContent, _stylesCache.Get(CacheStyleType.ToolButton)))
            {
                var window = GetWindow<SaveSystemSaveEditorWindow>("Save Editor");
                window.minSize = new Vector2(500f, 400f);
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }
    }

    private void DrawSubPathInput(string path)
    {
        EditorGUILayout.BeginHorizontal();

        DrawReadonlyTextField("Path", path);
        _path = path;

        EditorGUILayout.LabelField("+", GUILayout.MaxWidth(10));

        EditorGUILayout.EndHorizontal();

        _subPath = EditorGUILayout.TextField("Sub Directory", _subPath);
        _path += _subPath;
    }

    private void SetParams()
    {
        _saveSystemVariation = _config.SaveSystemVariation;
        _pathOption = _config.PathOption;
        _path = _config.Path;
        _subPath = _config.SubPath;
        _fileFormat = _config.FileFormat;
    }

    private void SetConfig()
    {
        _config.SaveSystemVariation = _saveSystemVariation;
        _config.PathOption = _pathOption;
        _config.Path = _path;
        _config.SubPath = _subPath;
        _config.FileFormat = _fileFormat;

        Save();
    }

    private void Save()
    {
        EditorUtility.SetDirty(_config);
        AssetDatabase.SaveAssetIfDirty(_config);

        var configData = CreateInstance<SaveSystemConfigData>();

        configData.SaveSystemVariation = _config.SaveSystemVariation;
        configData.PathOption = _config.PathOption;
        configData.Path = _config.Path;
        configData.SubPath = _config.SubPath;
        configData.FileFormat = _config.FileFormat;

        var pathDirectory = Path.GetDirectoryName(SaveSystemConfigDataPath);

        if (Directory.Exists(pathDirectory) == false)
        {
            Directory.CreateDirectory(pathDirectory);
        }

        AssetDatabase.CreateAsset(configData, SaveSystemConfigDataPath);
        EditorUtility.SetDirty(configData);
        AssetDatabase.SaveAssetIfDirty(_config);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        CallSaveSystemInit();
    }

    private void CallSaveSystemInit()
    {
        var methodInfo = typeof(SaveSystem)
            .GetMethod(
            "Init",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Static);

        methodInfo.Invoke(null, null);
    }

    private void TrySetFormat()
    {
        if (_isFormatEdit)
        {
            return;
        }

        switch (_saveSystemVariation)
        {
            case SaveSystem.SaveVariations.Json:
                _fileFormat = "json";
                break;
            case SaveSystem.SaveVariations.Binary:
                _fileFormat = "bin";
                break;
        }
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
}


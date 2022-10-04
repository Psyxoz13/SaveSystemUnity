using System.IO;
using UnityEditor;
using UnityEngine;
using SSystem;

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

    private const string SaveSystemConfigPath = "Assets/Resources/SaveSystem/SaveSystemData.asset";

    private TypeMemoryCache<GUIStyle> _stylesCache = new TypeMemoryCache<GUIStyle>();
    private SaveSystemConfig _config;

    private enum CacheStyleType
    {
        ToolButton,
        SelectDirectory
    }

    [MenuItem("SaveSystem/Settings")]
    public static void ShowWindow()
    {       
        var window = GetWindow<SaveSystemSettingsWindow>("SaveSystem Settings");

        window.minSize = new Vector2(500f, 400f);
    }

    private void CreateGUI()
    {
        SaveSystemEditorIconsData.LoadIcon(ref SaveSystemEditorIconsData.Folder, "folder-icon.png");

        _stylesCache.Cache(CacheStyleType.ToolButton, new GUIStyle("button")
        {
            margin = new RectOffset(5, 5, 5, 5),
            imagePosition = ImagePosition.ImageAbove,
            padding = new RectOffset(5, 5, 5, 5)
        });
        _stylesCache.Cache(CacheStyleType.SelectDirectory, new GUIStyle("button")
        {
            padding = new RectOffset(3, 3, 3, 3)
        });

        TrySetFormat();
    }

    private void OnDestroy()
    {
        _stylesCache.ClearCache();
    }

    private void OnEnable()
    {
        var data = EditorPrefs.GetString(SaveSystemEditorPrefsKeys.SaveSystemSettings.ToString(), JsonUtility.ToJson(this, false));
        JsonUtility.FromJsonOverwrite(data, this);

        var configPath = AssetDatabase.GUIDToAssetPath(_configGUID);
        _config = AssetDatabase.LoadAssetAtPath<SaveSystemConfig>(configPath);
    }

    private void OnDisable()
    {
        if (_config != null && AssetDatabase.TryGetGUIDAndLocalFileIdentifier(_config, out string guid, out long _))
        {
            _configGUID = guid;
        }

        var data = JsonUtility.ToJson(this, false);
        EditorPrefs.SetString(SaveSystemEditorPrefsKeys.SaveSystemSettings.ToString(), data);
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();

        _config = (SaveSystemConfig)EditorGUILayout.ObjectField("Config", _config, typeof(SaveSystemConfig), true);

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

        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();

        _saveSystemVariation = (SaveSystem.SaveVariations)EditorGUILayout.EnumPopup("Save system", _saveSystemVariation);

        EditorGUILayout.Space();

        _pathOption = (PathOptions)EditorGUILayout.EnumPopup("Path option", _pathOption);

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

                            //SaveSystemWarningWindow.Show("Directory does not exist.", SaveSystemWarningWindow.WarningType.Error);
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

        _isFormatEdit = EditorGUILayout.Toggle("Edit file format", _isFormatEdit);
        
        if (_isFormatEdit)
        {
            _fileFormat = EditorGUILayout.TextField(_fileFormat);
        }

        EditorGUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck())
        {
            TrySetFormat();
            SetConfig();
        }

        EditorGUILayout.Space();

        _isDebugShow = EditorGUILayout.Foldout(_isDebugShow, "Debug", true);

        if (_isDebugShow)
        {
            GUI.enabled = false;
            switch (_pathOption)
            {
                case PathOptions.PersistentDataPath:
                    EditorGUILayout.TextField("IOS Path", "/var/mobile/Containers/Data/Application/<guid>/Documents/" + _subPath);
                    EditorGUILayout.TextField("Android Path", "/storage/emulated/0/Android/data/<packagename>/files/" + _subPath);
                    break;
                case PathOptions.Manual:
                    EditorGUILayout.TextField("IOS Path", "/var/mobile/" + _path);
                    EditorGUILayout.TextField("Android Path", "/storage/emulated/0/" + _path);
                    break;
                default:
                    break;
            }
            GUI.enabled = true;
        }

        EditorGUILayout.Space();

        _isToolsShow = EditorGUILayout.Foldout(_isToolsShow, "Tools", true);
        
        if (_isToolsShow)
        {
            EditorGUILayout.BeginVertical("TextArea");

            var openPathFolderContent = new GUIContent("Open path", SaveSystemEditorIconsData.Folder.Texture);

            if (GUILayout.Button(openPathFolderContent, _stylesCache.Get(CacheStyleType.ToolButton), GUILayout.Width(75), GUILayout.Height(40)))
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

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }
    }

    private void DrawSubPathInput(string path)
    {
        EditorGUILayout.BeginHorizontal();

        GUI.enabled = false;

        _path = EditorGUILayout.TextField("Path", path);

        GUI.enabled = true;

        EditorGUILayout.LabelField("+", GUILayout.MaxWidth(15));

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

        var pathDirectory = Path.GetDirectoryName(SaveSystemConfigPath);

        if (Directory.Exists(pathDirectory) == false)
        {
            Directory.CreateDirectory(pathDirectory);
        }

        AssetDatabase.CreateAsset(configData, SaveSystemConfigPath);
        EditorUtility.SetDirty(configData);
        AssetDatabase.SaveAssetIfDirty(_config);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
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
}

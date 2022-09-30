using UnityEngine;

public static class SaveSystem
{
    public enum SaveVariations
    {
        Json,
        Binary
    }

    private static ISaveSystem _saveSystem;

    static SaveSystem()
    {
        var saveSystemConfigData = Resources.Load<SaveSystemConfigData>("SaveSystem/SaveSystemData");

        string path = string.Empty;

        switch (saveSystemConfigData.PathOption)
        {
            case PathOptions.PersistentDataPath:
                path = Application.persistentDataPath + "/" + saveSystemConfigData.SubPath;
                break;
#if UNITY_STANDALONE_WIN
            case PathOptions.Documents:
                path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments).Replace('\\', '/') + "/" + saveSystemConfigData.SubPath;
                break;
#endif
            case PathOptions.Manual:
                path = saveSystemConfigData.Path;
                break;
        }

        switch (saveSystemConfigData.SaveSystemVariation)
        {
            case SaveVariations.Json:
                _saveSystem = new SaveSystemJSON(saveSystemConfigData.FileFormat, path);
                break;
            case SaveVariations.Binary:
                _saveSystem = new SaveSystemBinary(saveSystemConfigData.FileFormat, path);
                break;
        }
    }

    public static void Save<T>(T data)
    {
        _saveSystem.Save(data);
    }

    public static T Load<T>()
    {
        return _saveSystem.Load<T>();
    }

    public static void Rewrite<T>(T data)
    {
        _saveSystem.Rewrite(data);
    }

    public static void Delete<T>()
    {
        _saveSystem.Delete<T>();
    }
}

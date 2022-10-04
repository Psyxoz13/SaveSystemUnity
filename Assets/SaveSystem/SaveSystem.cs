using UnityEngine;

namespace SSystem
{
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
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                    path = saveSystemConfigData.Path;
#elif UNITY_IPHONE
                path = "/var/mobile/" + saveSystemConfigData.Path;   
#elif UNITY_ANDROID
                path = "/storage/emulated/0/" + saveSystemConfigData.Path;
#endif
                    break;
            }

            var format = saveSystemConfigData.FileFormat;

            switch (saveSystemConfigData.SaveSystemVariation)
            {
                case SaveVariations.Json:
                    _saveSystem = new SaveSystemJSON(format, path);
                    break;
                case SaveVariations.Binary:
                    _saveSystem = new SaveSystemBinary(format, path);
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
}

using System.IO;
using UnityEngine;

namespace SSystem
{
    public class SaveSystemJSON : SaveSystemData, ISaveSystem
    {
        public SaveSystemJSON(string format, string directoryPath) : base(new FileDataConfig(format, directoryPath))
        { }

        public SaveSystemJSON() : base()
        { }

        public void Save<T>(T data)
        {
            string json = JsonUtility.ToJson(data, true);

            var fileStream = GetFileStream(typeof(T).Name, FileMode.Create);

            using var streamWriter = new StreamWriter(fileStream);

            streamWriter.WriteLine(json);

            streamWriter.Close();
            fileStream.Close();
        }

        public T Load<T>()
        {
            try
            {
                var json = ReadFile(typeof(T).Name);

                var data = JsonUtility.FromJson<T>(json);

                return data;
            }
            catch
            {
                return (T)System.Activator.CreateInstance(typeof(T));
            }
        }

        public void Rewrite<T>(T data)
        {
            Delete<T>();
            Save(data);
        }

        public void Delete<T>()
        {
            DeleteFile(typeof(T).Name);
        }

        public void Overwrite<T>(T target)
        {
            try
            {
                var json = ReadFile(typeof(T).Name);

                JsonUtility.FromJsonOverwrite(json, target);
            }
            catch { }
        }
    }
}

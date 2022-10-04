using System.IO;
using UnityEngine;

namespace SSystem
{
    internal class SaveSystemJSON : SaveSystemData, ISaveSystem
    {
        internal SaveSystemJSON(string format, string directoryPath) : base(new FileDataConfig(format, directoryPath))
        { }

        internal SaveSystemJSON() : base()
        { }

        public void Save<T>(T data)
        {
            string json = JsonUtility.ToJson(data);

            var fileStream = CreateFileStream(typeof(T).Name);

            using var streamWriter = new StreamWriter(fileStream);

            streamWriter.Write(json);

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
    }
}

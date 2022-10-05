using System;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SSystem
{
    public class SaveSystemBinary : SaveSystemData, ISaveSystem
    {
        public SaveSystemBinary(string format, string directoryPath) : base(new FileDataConfig(format, directoryPath))
        { }

        public SaveSystemBinary() : base()
        { }

        public void Save<T>(T data)
        {
            var binaryFormatter = new BinaryFormatter();
            var dataFile = GetFileStream(typeof(T).Name, System.IO.FileMode.Create);

            binaryFormatter.Serialize(dataFile, data);

            dataFile.Close();
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

        public T Load<T>()
        {
            try
            {
                var binaryFormatter = new BinaryFormatter();
                var fileStream = GetFileStream(typeof(T).Name, System.IO.FileMode.Open);

                var data = (T)binaryFormatter.Deserialize(fileStream);

                fileStream.Close();

                return data;
            }
            catch
            {
                return (T)System.Activator.CreateInstance(typeof(T));
            }
        }
    }
}
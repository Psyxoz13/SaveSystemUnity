using System.Runtime.Serialization.Formatters.Binary;

namespace SSystem
{
    internal class SaveSystemBinary : SaveSystemData, ISaveSystem
    {
        internal SaveSystemBinary(string format, string directoryPath) : base(new FileDataConfig(format, directoryPath))
        { }

        internal SaveSystemBinary() : base()
        { }

        public void Save<T>(T data)
        {
            var binaryFormatter = new BinaryFormatter();
            var dataFile = CreateFileStream(typeof(T).Name);

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
                var fileStream = CreateFileStream(typeof(T).Name);

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
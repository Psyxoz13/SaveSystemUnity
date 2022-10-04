using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SSystem
{
    public abstract class SaveSystemData
    {
        protected static List<string> SaveFiles
        {
            get => GetSavesList();
        }

        protected static string FileFormat { get; private set; } = ".save";
        protected static string DirectoryPath { get; private set; } = Application.persistentDataPath + "/";

        protected SaveSystemData(FileDataConfig fileDataConfig)
        {
            if (fileDataConfig.FileFormat.ToCharArray().First() == '.')
            {
                FileFormat = fileDataConfig.FileFormat;
            }
            else
            {
                FileFormat = "." + fileDataConfig.FileFormat;
            }

            if (fileDataConfig.DirectoryPath.ToCharArray().Last() == '/')
            {
                DirectoryPath = fileDataConfig.DirectoryPath;
            }
            else
            {
                DirectoryPath = fileDataConfig.DirectoryPath + "/";
            }
        }

        protected SaveSystemData()
        { }

        protected FileStream CreateFileStream(string fileName)
        {
            if (Directory.Exists(DirectoryPath) == false)
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            var path = DirectoryPath + fileName + FileFormat;
            var dataFile = File.Open(path, FileMode.OpenOrCreate);

            if (SaveFiles.Contains(path) == false)
            {
                SaveFiles.Add(DirectoryPath + fileName + FileFormat);
            }

            return dataFile;
        }

        protected string ReadFile(string fileName)
        {
            var fileText = File.ReadAllText(
                DirectoryPath + fileName + FileFormat,
                Encoding.UTF8);

            return fileText;
        }

        protected void DeleteFile(string fileName)
        {
            File.Delete(DirectoryPath + fileName + FileFormat);

            SaveFiles.Remove(DirectoryPath + fileName + FileFormat);
        }

        protected struct FileDataConfig
        {
            public string FileFormat;
            public string DirectoryPath;

            public FileDataConfig(string fileFormat, string directoryPath)
            {
                FileFormat = fileFormat;
                DirectoryPath = directoryPath;
            }
        }

        private static List<string> GetSavesList()
        {
            var savesList = new List<string>();

            savesList.AddRange(
                Directory.GetFiles(DirectoryPath)
                    .Where(
                        path => Path.GetExtension(path) == FileFormat));

            return savesList;
        }
    }
}
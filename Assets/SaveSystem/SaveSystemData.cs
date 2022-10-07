using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SSystem
{
    public abstract class SaveSystemData
    {
        protected string FileFormat { get; private set; } = ".save";
        protected string DirectoryPath { get; private set; } = Application.persistentDataPath + "/";

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

        protected FileStream GetFileStream(string fileName, FileMode fileMode)
        {
            if (Directory.Exists(DirectoryPath) == false)
            {
                Directory.CreateDirectory(DirectoryPath);
            }

            var path = DirectoryPath + fileName + FileFormat;
            var dataFile = File.Open(path, fileMode);

            return dataFile;
        }

        protected string ReadFile(string fileName)
        {
            var fileText = FileTools.ReadFile(
                DirectoryPath + fileName + FileFormat);

            return fileText;
        }

        protected void DeleteFile(string fileName)
        {
            File.Delete(DirectoryPath + fileName + FileFormat);
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
    }
}
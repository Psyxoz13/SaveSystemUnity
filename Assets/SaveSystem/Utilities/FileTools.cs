using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class FileTools
{
    public static string Search(string directory, string fileName)
    {
        string path = string.Empty;

        string[] files = Directory.GetFiles(directory);
        string[] directories = Directory.GetDirectories(directory);

        for (int i = 0; i < files.Length; i++)
        {
            if (Path.GetFileName(files[i]) == fileName)
            {
                path = files[i];
            }
        }

        for (int i = 0; i < directories.Length; i++)
        {
            var subFile = Search(directories[i], fileName);
            if (subFile != string.Empty)
            {
                path = subFile;
            }
        }

        return path;
    }

    public static string ReadFile(string path)
    {
        var fileText = File.ReadAllText(
            path,
            Encoding.UTF8);

        return fileText;
    }
}

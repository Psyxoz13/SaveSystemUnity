using UnityEditor;
using UnityEngine;

public static class SaveSystemEditorIconsData
{
    public static Icon Folder;

    private static TypeMemoryCache<Icon> _iconsCache = new TypeMemoryCache<Icon>();

    public static void LoadIcon(ref Icon icon, string fileName)
    {
        if (_iconsCache.TryGet(fileName, out Icon cachedIcon))
        {
            icon = cachedIcon;
            return;
        }

        icon = new Icon
        {
            Texture = (Texture2D)AssetDatabase.LoadAssetAtPath(
                FileTools.Search(
                    "Assets",
                    fileName),
                typeof(Texture2D)),

            FileName = fileName
        };
        _iconsCache.Cache(fileName, icon);
    }
}

public class Icon
{
    public string FileName;
    public Texture2D Texture;
}

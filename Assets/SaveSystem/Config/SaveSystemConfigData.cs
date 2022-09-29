using UnityEngine;

public class SaveSystemConfigData : ScriptableObject
{
    public SaveSystem.SaveVariations SaveSystemVariation;
    public PathOptions PathOption;

    public string Path;
    public string SubPath;
    public string FileFormat;
}

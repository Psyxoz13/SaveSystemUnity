using UnityEngine;

public class SaveSystemConfigData : ScriptableObject
{
    public SaveSystem.SaveVariations SaveSystemVariation { get; set; }
    public PathOptions PathOption { get; set; }

    public string Path { get; set; }
    public string SubPath { get; set; }
    public string FileFormat { get; set; }
}

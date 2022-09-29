using UnityEngine;

[CreateAssetMenu(fileName = "SaveSystemConfig", menuName = "SaveSystem/Config", order = 1), System.Serializable]
public class SaveSystemConfig : ScriptableObject
{
    public SaveSystem.SaveVariations SaveSystemVariation;
    public PathOptions PathOption;

    public string Path;
    public string SubPath;
    public string FileFormat;
}

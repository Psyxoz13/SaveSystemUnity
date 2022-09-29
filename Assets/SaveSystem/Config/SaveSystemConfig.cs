using UnityEngine;

[CreateAssetMenu(fileName = "SaveSystemConfig", menuName = "SaveSystem/Config", order = 1), System.Serializable]
public class SaveSystemConfig : ScriptableObject
{
    [Readonly] public SaveSystem.SaveVariations SaveSystemVariation;
    [HideInInspector] public PathOptions PathOption;

    [Readonly] public string Path;
    [HideInInspector] public string SubPath;
    [Readonly] public string FileFormat;
}

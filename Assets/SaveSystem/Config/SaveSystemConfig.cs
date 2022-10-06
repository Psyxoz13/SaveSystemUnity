using UnityEngine;

namespace SSystem
{
    [CreateAssetMenu(fileName = "SaveSystemConfig", menuName = "SaveSystem/Config", order = 1), System.Serializable]
    public class SaveSystemConfig : ScriptableObject
    {
        [Readonly] public SaveSystem.SaveVariations SaveSystemVariation = SaveSystem.SaveVariations.Json;
        [HideInInspector] public PathOptions PathOption = PathOptions.PersistentDataPath;

        [Readonly] public string Path;
        [HideInInspector] public string SubPath;
        [Readonly] public string FileFormat = "json";
    }
}


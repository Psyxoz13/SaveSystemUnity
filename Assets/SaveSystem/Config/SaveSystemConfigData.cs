using UnityEngine;

namespace SSystem
{
    public class SaveSystemConfigData : ScriptableObject
    {
        [HideInInspector] public SaveSystem.SaveVariations SaveSystemVariation;
        [HideInInspector] public PathOptions PathOption;

        [HideInInspector] public string Path;
        [HideInInspector] public string SubPath;
        [HideInInspector] public string FileFormat;
    }
}


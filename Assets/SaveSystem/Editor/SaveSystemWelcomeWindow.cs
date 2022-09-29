using UnityEditor;
using UnityEngine;

internal class SaveSystemWelcomeWindow : EditorWindow
{
    public static void ShowWindow()
    {
        GetWindow<SaveSystemWelcomeWindow>("SaveSystem by Psyxoz13");
    }

    private void OnGUI()
    {
        
    }
}

internal class SaveSystemAssetImport : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        Debug.Log(1);
        if (assetPath.Contains("SaveSystem"))
        {
            ModelImporter modelImporter = assetImporter as ModelImporter;
            modelImporter.materialImportMode = ModelImporterMaterialImportMode.None;

            SaveSystemWelcomeWindow.ShowWindow();
        }
    }
}

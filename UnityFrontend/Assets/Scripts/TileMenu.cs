using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public static class TileMenu
{
    [MenuItem("Assets/Create/2D/Tiles/Tile", priority = 200)]
    public static void CreateTile()
    {
        string selectedFolder = "Assets";
        if (Selection.activeObject != null)
        {
            selectedFolder = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!AssetDatabase.IsValidFolder(selectedFolder))
            {
                selectedFolder = System.IO.Path.GetDirectoryName(selectedFolder);
            }
        }

        string defaultName = "New Tile";
        string path = EditorUtility.SaveFilePanelInProject("Save Tile", defaultName, "asset", "Save Tile", selectedFolder);

        if (string.IsNullOrEmpty(path))
            return;

        Tile tile = ScriptableObject.CreateInstance<Tile>();
        AssetDatabase.CreateAsset(tile, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = tile;
    }
}
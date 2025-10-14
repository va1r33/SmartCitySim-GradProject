// Assets/Editor/TileGenerator.cs
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class TileGenerator : EditorWindow
{
    [MenuItem("Tools/Generate All City Tiles")]
    static void GenerateAllTiles()
    {
        string tilesFolderPath = "Assets/Tiles";

        // Create Tiles folder if it doesn't exist
        if (!AssetDatabase.IsValidFolder(tilesFolderPath))
        {
            AssetDatabase.CreateFolder("Assets", "Tiles");
        }

        // Tile names and expected sprite names
        string[] tileNames = { "Grass", "Road", "Residential", "Commercial", "Industrial", "Park" };

        foreach (string tileName in tileNames)
        {
            CreateTileAsset(tileName, tilesFolderPath);
        }

        AssetDatabase.Refresh();
        Debug.Log("All 6 city tiles generated successfully!");
    }

    static void CreateTileAsset(string tileName, string folderPath)
    {
        // Create the tile asset
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        string assetPath = $"{folderPath}/{tileName}Tile.asset";

        // Find and assign the corresponding sprite
        string spritePath = FindSpritePath(tileName);
        if (!string.IsNullOrEmpty(spritePath))
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (sprite != null)
            {
                tile.sprite = sprite;
                Debug.Log($"Assigned {tileName}Sprite to {tileName}Tile");
            }
            else
            {
                Debug.LogWarning($"Could not load sprite at: {spritePath}");
            }
        }
        else
        {
            Debug.LogWarning($"Could not find sprite for: {tileName}");
        }

        // Save tile asset
        AssetDatabase.CreateAsset(tile, assetPath);
        Debug.Log($"Created: {assetPath}");
    }

    static string FindSpritePath(string tileName)
    {
        // Look for sprites in common locations
        string[] searchPaths = {
            $"Assets/Sprites/{tileName}Sprite.png",
            $"Assets/Sprites/{tileName}Sprite.asset",
            $"Assets/{tileName}Sprite.png",
            $"Assets/{tileName}Sprite.asset"
        };

        foreach (string path in searchPaths)
        {
            if (System.IO.File.Exists(path) || AssetDatabase.LoadAssetAtPath<Sprite>(path) != null)
            {
                return path;
            }
        }

        string[] guids = AssetDatabase.FindAssets($"{tileName}Sprite t:Sprite");
        if (guids.Length > 0)
        {
            return AssetDatabase.GUIDToAssetPath(guids[0]);
        }

        return null;
    }
}
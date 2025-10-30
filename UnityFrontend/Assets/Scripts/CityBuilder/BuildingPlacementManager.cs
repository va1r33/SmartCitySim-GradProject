using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingPlacementManager : MonoBehaviour
{
    [Header("Tilemap Reference")]
    public Tilemap buildingTilemap;

    [Header("Building Tiles")]
    public TileBase residentialTile;
    public TileBase commercialTile;
    public TileBase industrialTile;
    public TileBase parkTile;

    [Header("Environment Tiles")]
    public TileBase grassTile;
    public TileBase roadTile;

    private string currentBuildMode = "";

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !string.IsNullOrEmpty(currentBuildMode))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = buildingTilemap.WorldToCell(worldPos);

            if (currentBuildMode == "erase")
            {
                buildingTilemap.SetTile(cellPos, null);
            }
            else
            {
                TileBase tileToPlace = GetTileByType(currentBuildMode);
                if (tileToPlace != null)
                    buildingTilemap.SetTile(cellPos, tileToPlace);
                else
                    Debug.LogWarning("No tile assigned for type: " + currentBuildMode);
            }
        }
    }

    TileBase GetTileByType(string type)
    {
        return type switch
        {
            "residential" => residentialTile,
            "commercial" => commercialTile,
            "industrial" => industrialTile,
            "park" => parkTile,
            "grass" => grassTile,
            "road" => roadTile,
            _ => null
        };
    }

    public void SetBuildMode(string mode)
    {
        currentBuildMode = mode;
        Debug.Log($"SetBuildMode CALLED with: {mode}");
    }
}

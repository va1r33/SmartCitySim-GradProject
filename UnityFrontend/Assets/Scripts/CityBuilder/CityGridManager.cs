using UnityEngine;
using UnityEngine.Tilemaps;

public class CityGridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 20;
    public int gridHeight = 15;

    [Header("Tile References")]
    public TileBase grassTile;
    public TileBase roadTile;
    public TileBase residentialTile;
    public TileBase commercialTile;
    public TileBase industrialTile;
    public TileBase parkTile;

    [Header("Building Placement")]
    public TileBase selectedBuildingType;

    private Grid grid;
    private Tilemap groundTilemap;
    private Tilemap buildingTilemap;

    [System.Serializable]
    public class CityLayout
    {
        public string layoutType;
        public BuildingData[] buildings;
    }

    [System.Serializable]
    public class BuildingData
    {
        public string type;
        public int count;
    }

    void Start()
    {
        CreateGrid();
        BuildDefaultCity();
        Debug.Log("City grid ready: " + gridWidth + "x" + gridHeight);
    }

    void Update()
    {
        HandleBuildingInput();
    }

    void CreateGrid()
    {
        grid = gameObject.AddComponent<Grid>();

        GameObject groundObj = new GameObject("GroundTilemap");
        groundObj.transform.SetParent(transform);
        groundTilemap = groundObj.AddComponent<Tilemap>();
        groundObj.AddComponent<TilemapRenderer>();

        GameObject buildingObj = new GameObject("BuildingTilemap");
        buildingObj.transform.SetParent(transform);
        buildingTilemap = buildingObj.AddComponent<Tilemap>();
        buildingObj.AddComponent<TilemapRenderer>();
    }

    void BuildDefaultCity()
    {
        // Fill ground with grass
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                groundTilemap.SetTile(new Vector3Int(x, y, 0), grassTile);
            }
        }

        BuildRoads();
        CreateStarterDistricts();
    }

    void BuildRoads()
    {
        // Horizontal road
        for (int x = 0; x < gridWidth; x++)
        {
            buildingTilemap.SetTile(new Vector3Int(x, gridHeight / 2, 0), roadTile);
        }

        // Vertical road
        for (int y = 0; y < gridHeight; y++)
        {
            buildingTilemap.SetTile(new Vector3Int(gridWidth / 2, y, 0), roadTile);
        }
    }

    void CreateStarterDistricts()
    {
        CreateDistrict(2, 10, 4, 3, residentialTile, "residential");
        CreateDistrict(12, 10, 4, 3, commercialTile, "commercial");
        CreateDistrict(2, 2, 4, 3, industrialTile, "industrial");
        CreateDistrict(12, 2, 4, 3, parkTile, "park");
    }

    void CreateDistrict(int startX, int startY, int width, int height, TileBase tile, string type)
    {
        for (int x = startX; x < startX + width; x++)
        {
            for (int y = startY; y < startY + height; y++)
            {
                buildingTilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
    }

    void HandleBuildingInput()
    {
        // Left click to place selectedBuildingType
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = buildingTilemap.WorldToCell(worldPos);

            if (selectedBuildingType == null)
            {
                // null = "erase" mode, left-click does nothing in this design
            }
            else if (CanBuildHere(cellPos))
            {
                buildingTilemap.SetTile(cellPos, selectedBuildingType);
                UpdateSimulation();
            }
        }

        // Right click to erase
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = buildingTilemap.WorldToCell(worldPos);

            if (IsInsideGrid(cellPos) && buildingTilemap.GetTile(cellPos) != null)
            {
                buildingTilemap.SetTile(cellPos, null);
                UpdateSimulation();
            }
        }
    }

    bool IsInsideGrid(Vector3Int cellPos)
    {
        return !(cellPos.x < 0 || cellPos.x >= gridWidth || cellPos.y < 0 || cellPos.y >= gridHeight);
    }

    bool CanBuildHere(Vector3Int cellPos)
    {
        if (!IsInsideGrid(cellPos)) return false;
        if (buildingTilemap.GetTile(cellPos) == roadTile) return false;
        return true;
    }

    public void SetBuildingType(string type)
    {
        switch (type.ToLower())
        {
            case "residential": selectedBuildingType = residentialTile; break;
            case "commercial": selectedBuildingType = commercialTile; break;
            case "industrial": selectedBuildingType = industrialTile; break;
            case "park": selectedBuildingType = parkTile; break;
            case "erase": selectedBuildingType = null; break;
            default: selectedBuildingType = null; break;
        }
        Debug.Log("Selected: " + type);
    }

    void UpdateSimulation()
    {
        APIManager apiManager = FindFirstObjectByType<APIManager>();
        if (apiManager != null)
        {
            apiManager.SimulateCurrentCity();
        }
    }

    public CityLayout GetCityLayout()
    {
        int residentialCount = CountBuildings(residentialTile);
        int commercialCount = CountBuildings(commercialTile);
        int industrialCount = CountBuildings(industrialTile);

        BuildingData[] buildings = new BuildingData[]
        {
            new BuildingData { type = "residential", count = residentialCount },
            new BuildingData { type = "commercial",  count = commercialCount  },
            new BuildingData { type = "industrial",  count = industrialCount  }
        };

        return new CityLayout
        {
            layoutType = "player_city",
            buildings = buildings
        };
    }

    int CountBuildings(TileBase tileType)
    {
        int count = 0;
        BoundsInt bounds = buildingTilemap.cellBounds;

        foreach (var position in bounds.allPositionsWithin)
        {
            if (buildingTilemap.GetTile(position) == tileType)
                count++;
        }
        return count;
    }
}

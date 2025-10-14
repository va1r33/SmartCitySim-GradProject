using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class APIManager : MonoBehaviour
{
    [Header("API Configuration")]
    public string apiUrl = "http://127.0.0.1:5001/api/simulate";

    [Header("UI References")]
    public TMP_Text trafficText;
    public TMP_Text co2Text;
    public TMP_Text energyText;
    public TMP_Text messageText;

    [Header("Visual Indicators")]
    public Image trafficIndicator;
    public Image co2Indicator;
    public Image energyIndicator;

    [Header("City Visual Feedback")]
    public Tilemap cityTilemap;      // Drag BuildingTilemap or GroundTilemap here
    public bool useFog = true;       // Optional toggle in Inspector

    private CityGridManager cityGrid;

    void Start()
    {
        cityGrid = FindFirstObjectByType<CityGridManager>();

        if (cityGrid != null)
        {
            StartCoroutine(TestAPIConnection());
        }
        else
        {
            Debug.LogError("CityGridManager not found!");
        }
    }

    public void SimulateCurrentCity()
    {
        if (cityGrid != null)
        {
            StartCoroutine(SendSimulationData(cityGrid.GetCityLayout()));
        }
        else
        {
            Debug.LogError("Cannot simulate - missing CityGridManager!");
        }
    }

    // --- Test connection with sample data ---
    IEnumerator TestAPIConnection()
    {
        SimulationRequest testData = new SimulationRequest
        {
            layout = "test_grid",
            buildings = new APIBuildingData[]
            {
                new APIBuildingData { type = "residential", count = 5 },
                new APIBuildingData { type = "commercial", count = 2 },
                new APIBuildingData { type = "industrial", count = 1 }
            }
        };

        yield return StartCoroutine(PostToAPI(testData));
    }

    // --- Send actual simulation data ---
    IEnumerator SendSimulationData(CityGridManager.CityLayout cityLayout)
    {
        APIBuildingData[] apiBuildings = new APIBuildingData[cityLayout.buildings.Length];

        for (int i = 0; i < cityLayout.buildings.Length; i++)
        {
            apiBuildings[i] = new APIBuildingData
            {
                type = cityLayout.buildings[i].type,
                count = cityLayout.buildings[i].count
            };
        }

        SimulationRequest requestData = new SimulationRequest
        {
            layout = cityLayout.layoutType,
            buildings = apiBuildings
        };

        yield return StartCoroutine(PostToAPI(requestData));
    }

    // --- Core API POST handler ---
    IEnumerator PostToAPI(SimulationRequest requestData)
    {
        string jsonData = JsonUtility.ToJson(requestData);

        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                HandleSuccessfulResponse(www.downloadHandler.text);
            }
            else
            {
                HandleErrorResponse(www.error);
            }
        }
    }

    // --- Handle successful response ---
    void HandleSuccessfulResponse(string jsonResponse)
    {
        SimulationResponse response = JsonUtility.FromJson<SimulationResponse>(jsonResponse);

        if (trafficText != null) trafficText.text = $"Traffic: {response.traffic}%";
        if (co2Text != null) co2Text.text = $"CO2: {response.co2}%";
        if (energyText != null) energyText.text = $"Energy: {response.energy}MW";
        if (messageText != null) messageText.text = response.message;

        UpdateIndicatorColors(response);
        UpdateVisualFeedback(response);

        Debug.Log($"Simulation Complete! Traffic: {response.traffic}%, CO2: {response.co2}%, Energy: {response.energy}MW");
    }

    // --- Color-coded indicators ---
    void UpdateIndicatorColors(SimulationResponse response)
    {
        if (trafficIndicator != null)
            trafficIndicator.color = GetColorForValue(response.traffic, true);
        if (co2Indicator != null)
            co2Indicator.color = GetColorForValue(response.co2, true);
        if (energyIndicator != null)
            energyIndicator.color = GetColorForValue(response.energy, false);
    }

    Color GetColorForValue(int value, bool lowerIsBetter)
    {
        if (lowerIsBetter)
        {
            return value < 50 ? Color.green :
                   value < 75 ? Color.yellow : Color.red;
        }
        else
        {
            return value > 80 ? Color.green :
                   value > 50 ? Color.yellow : Color.red;
        }
    }

    // --- Tilemap & Fog feedback ---
    void UpdateVisualFeedback(SimulationResponse response)
    {
        // 1) Traffic color tint
        if (cityTilemap != null)
        {
            if (response.traffic > 70)
                cityTilemap.color = Color.red;
            else if (response.traffic > 40)
                cityTilemap.color = Color.yellow;
            else
                cityTilemap.color = Color.green;
        }

        // 2) Fog based on CO2
        if (useFog)
        {
            if (response.co2 > 60)
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = new Color(0.5f, 0.5f, 0.55f, 0.9f);
                RenderSettings.fogDensity = 0.015f;
            }
            else
            {
                RenderSettings.fog = false;
            }
        }
    }

    void HandleErrorResponse(string error)
    {
        Debug.LogError($"API Error: {error}");
        if (messageText != null) messageText.text = "Connection Failed!";
    }
}

// --- Data Models ---
[System.Serializable]
public class APIBuildingData
{
    public string type;
    public int count;
}

[System.Serializable]
public class SimulationRequest
{
    public string layout;
    public APIBuildingData[] buildings;
}

[System.Serializable]
public class SimulationResponse
{
    public int traffic;
    public int co2;
    public int energy;
    public string message;
}

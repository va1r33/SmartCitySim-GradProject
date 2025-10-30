using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

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

    [Header("Visual Feedback - Fog & Tinting")]
    public UnityEngine.UI.Image fogOverlay;
    public Tilemap groundTilemap;
    public Tilemap buildingTilemap;
    public bool useFog = true;

    [Header("Fog Color Theme")]
    public FogColorTheme fogTheme = FogColorTheme.StandardGray;

    public enum FogColorTheme
    {
        StandardGray,
        LightBlueHaze,
        BrownPollution,
        GreenSmog,
        YellowSmog
    }

    [Header("SmartThings Integration")]
    public string currentSmartThingsMode = "";

    [Header("Scenario Integration")]
    public ScenarioManager scenarioManager;

    private CityGridManager cityGrid;
    private SmartThingsManager smartThingsManager;
    private Coroutine fogCoroutine;

    // Last effective metrics used by UI/SmartThings
    public SimulationResult simulationResult = new SimulationResult();

    // IMPORTANT: respect backend (we’re not overriding locally)
    private const bool USE_LOCAL_METRICS = false;

    void Start()
    {
        cityGrid = FindFirstObjectByType<CityGridManager>();
        smartThingsManager = FindFirstObjectByType<SmartThingsManager>();
        if (scenarioManager == null) scenarioManager = FindFirstObjectByType<ScenarioManager>();

        if (cityGrid != null)
            StartCoroutine(TestAPIConnection());
        else
            Debug.LogError("CityGridManager not found!");
    }

    public void SimulateCurrentCity()
    {
        if (cityGrid != null)
            StartCoroutine(SendSimulationData(cityGrid.GetCityLayout()));
        else
            Debug.LogError("Cannot simulate - missing CityGridManager!");
    }

    // SmartThings mode setters
    public void SetEcoMode() { currentSmartThingsMode = "eco"; SimulateCurrentCity(); }
    public void SetTrafficControl() { currentSmartThingsMode = "traffic_control"; SimulateCurrentCity(); }
    public void SetAlertMode() { currentSmartThingsMode = "alert"; SimulateCurrentCity(); }
    public void ClearSmartThingsMode() { currentSmartThingsMode = ""; SimulateCurrentCity(); }

    IEnumerator TestAPIConnection()
    {
        SimulationRequest testData = new SimulationRequest
        {
            layout = "test_grid",
            buildings = new List<APIBuildingData>
            {
                new APIBuildingData { type = "residential", count = 5 },
                new APIBuildingData { type = "commercial",  count = 2 },
                new APIBuildingData { type = "industrial",  count = 1 }
            },
            smartthings_mode = currentSmartThingsMode
        };

        yield return StartCoroutine(PostToAPI(testData));
    }

    IEnumerator SendSimulationData(CityGridManager.CityLayout cityLayout)
    {
        List<APIBuildingData> apiBuildings = new List<APIBuildingData>();
        for (int i = 0; i < cityLayout.buildings.Length; i++)
        {
            apiBuildings.Add(new APIBuildingData
            {
                type = cityLayout.buildings[i].type,
                count = cityLayout.buildings[i].count
            });
        }

        SimulationRequest requestData = new SimulationRequest
        {
            layout = cityLayout.layoutType,
            buildings = apiBuildings,
            smartthings_mode = currentSmartThingsMode
        };

        yield return StartCoroutine(PostToAPI(requestData));
    }

    IEnumerator PostToAPI(SimulationRequest requestData)
    {
        string jsonData = JsonUtility.ToJson(requestData);
        Debug.Log($"POSTing to API: {jsonData}");

        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"API Response: {www.downloadHandler.text}");
                HandleSuccessfulResponse(www.downloadHandler.text);
            }
            else
            {
                HandleErrorResponse(www.error);
            }
        }
    }

    void HandleSuccessfulResponse(string jsonResponse)
    {
        SimulationResponse serverResponse = JsonUtility.FromJson<SimulationResponse>(jsonResponse);

        SimulationResponse effective = USE_LOCAL_METRICS ? ComputeLocalFromGrid() : serverResponse;

        ApplySmartThingsMode(ref effective, currentSmartThingsMode);

        simulationResult.traffic = effective.traffic;
        simulationResult.co2 = effective.co2;
        simulationResult.energy = effective.energy;

        // UI
        if (trafficText != null) trafficText.text = $"Traffic: {effective.traffic}%";
        if (co2Text != null) co2Text.text = $"CO2: {effective.co2}%";
        if (energyText != null) energyText.text = $"Energy: {effective.energy}MW";
        if (messageText != null) messageText.text = string.IsNullOrEmpty(effective.message) ? "City analyzed" : effective.message;

        // Push live to SmartThings
        if (smartThingsManager != null)
        {
            smartThingsManager.co2 = effective.co2;
            smartThingsManager.traffic = effective.traffic;
            smartThingsManager.UpdateStatusText();
        }

        UpdateIndicatorColors(effective);
        UpdateVisualFeedback(effective);

        // Scenario progress
        if (scenarioManager != null)
            scenarioManager.UpdateScenarioProgress(effective.co2);

        // Success feedback
        if (effective.co2 <= 60 && effective.traffic <= 40 && effective.energy <= 100)
        {
            if (messageText != null)
            {
                messageText.text = "Green Mandate Achieved!";
                messageText.color = Color.green;
            }
        }
        else if (messageText != null)
        {
            messageText.color = Color.white;
        }

        StartCoroutine(SmoothUpdateUI(effective));

        Debug.Log($"Simulation Complete! Traffic: {effective.traffic}%, CO2: {effective.co2}%, Energy: {effective.energy}MW");
        Debug.Log($"SmartThings Mode: {currentSmartThingsMode}");
    }

    // Local fallback (kept intact)
    private SimulationResponse ComputeLocalFromGrid()
    {
        if (cityGrid == null)
            return new SimulationResponse { traffic = 25, co2 = 20, energy = 30, message = "No grid found" };

        CityGridManager.CityLayout layout = cityGrid.GetCityLayout();

        int residential = 0, commercial = 0, industrial = 0;
        foreach (var b in layout.buildings)
        {
            switch (b.type)
            {
                case "residential": residential += b.count; break;
                case "commercial": commercial += b.count; break;
                case "industrial": industrial += b.count; break;
            }
        }

        int total = residential + commercial + industrial;

        int traffic, co2, energy;
        if (total == 0)
        {
            traffic = 25; co2 = 20; energy = 30;
        }
        else
        {
            traffic = Mathf.Min(95, 30 + (commercial * 3) + (industrial * 5));
            co2 = Mathf.Min(90, 20 + (industrial * 8) + (commercial * 3));
            energy = Mathf.Min(85, 25 + (residential * 2) + (commercial * 4) + (industrial * 10));
        }

        string msg = $"City Analyzed: {residential}R {commercial}C {industrial}I";
        return new SimulationResponse { traffic = traffic, co2 = co2, energy = energy, message = msg };
    }

    private void ApplySmartThingsMode(ref SimulationResponse r, string mode)
    {
        if (string.IsNullOrEmpty(mode)) return;

        if (mode == "eco")
        {
            r.co2 = Mathf.Max(10, r.co2 - 15);
            r.energy = Mathf.Max(15, r.energy - 10);
            r.message = (r.message ?? "") + " | Eco Mode Active";
        }
        else if (mode == "traffic_control")
        {
            r.traffic = Mathf.Max(20, r.traffic - 25);
            r.energy = Mathf.RoundToInt(r.energy * 1.05f);
            r.message = (r.message ?? "") + " | Traffic Control Active";
        }
        else if (mode == "alert")
        {
            r.message = (r.traffic > 70 || r.co2 > 60) ? "ALERT: High Pollution/Traffic!" : "City Status Normal";
        }
    }

    public void UpdateIndicatorColors(SimulationResponse response)
    {
        if (trafficIndicator != null) trafficIndicator.color = GetColorForValue(response.traffic, true);
        if (co2Indicator != null) co2Indicator.color = GetColorForValue(response.co2, true);
        if (energyIndicator != null) energyIndicator.color = GetColorForValue(response.energy, false);
    }

    Color GetColorForValue(int value, bool lowerIsBetter)
    {
        if (lowerIsBetter)
            return value < 50 ? Color.green : (value < 75 ? Color.yellow : Color.red);
        else
            return value > 80 ? Color.green : (value > 50 ? Color.yellow : Color.red);
    }

    void UpdateVisualFeedback(SimulationResponse response)
    {
        UpdateTilemapTinting(response.traffic);
        UpdateFogOverlay(response.co2);
        UpdateBuildingTints(response);
    }

    void UpdateTilemapTinting(int trafficLevel)
    {
        if (groundTilemap == null) return;

        if (trafficLevel > 70)
            groundTilemap.color = Color.Lerp(Color.white, new Color(1f, 0.6f, 0.6f), 0.4f);
        else if (trafficLevel > 40)
            groundTilemap.color = Color.Lerp(Color.white, new Color(1f, 1f, 0.6f), 0.3f);
        else
            groundTilemap.color = Color.white;
    }

    void UpdateFogOverlay(int co2Level)
    {
        if (fogOverlay != null)
        {
            float alpha = Mathf.InverseLerp(60f, 120f, co2Level);
            alpha = Mathf.Clamp01(alpha) * 0.6f;
            Color targetColor = GetFogColorByTheme(alpha);

            if (fogCoroutine != null) StopCoroutine(fogCoroutine);
            fogCoroutine = StartCoroutine(LerpFogColor(targetColor, 0.8f));
        }
        else if (useFog)
        {
            if (co2Level > 60)
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = new Color(0.5f, 0.5f, 0.55f, 1f);
                RenderSettings.fogDensity = 0.015f + (co2Level - 60) * 0.0005f;
            }
            else RenderSettings.fog = false;
        }
    }

    Color GetFogColorByTheme(float alpha)
    {
        switch (fogTheme)
        {
            case FogColorTheme.LightBlueHaze: return new Color(0.7f, 0.7f, 0.8f, alpha);
            case FogColorTheme.BrownPollution: return new Color(0.4f, 0.3f, 0.3f, alpha);
            case FogColorTheme.GreenSmog: return new Color(0.3f, 0.5f, 0.3f, alpha);
            case FogColorTheme.YellowSmog: return new Color(0.6f, 0.5f, 0.3f, alpha);
            case FogColorTheme.StandardGray:
            default: return new Color(0.5f, 0.5f, 0.55f, alpha);
        }
    }

    void UpdateBuildingTints(SimulationResponse response)
    {
        if (buildingTilemap == null) return;
        buildingTilemap.color = (response.co2 > 70)
            ? Color.Lerp(Color.white, new Color(1f, 0.7f, 0.7f), 0.2f)
            : Color.white;
    }

    IEnumerator LerpFogColor(Color targetColor, float duration = 0.8f)
    {
        if (fogOverlay == null) yield break;

        Color startColor = fogOverlay.color;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            fogOverlay.color = Color.Lerp(startColor, targetColor, t / duration);
            yield return null;
        }
        fogOverlay.color = targetColor;
    }

    IEnumerator SmoothUpdateUI(SimulationResponse response)
    {
        float duration = 1.5f;
        float t = 0f;

        float.TryParse(trafficText.text.Replace("Traffic:", "").Replace("%", "").Trim(), out float startTraffic);
        float.TryParse(co2Text.text.Replace("CO2:", "").Replace("%", "").Trim(), out float startCO2);
        float.TryParse(energyText.text.Replace("Energy:", "").Replace("MW", "").Trim(), out float startEnergy);

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerpTraffic = Mathf.Lerp(startTraffic, response.traffic, t / duration);
            float lerpCO2 = Mathf.Lerp(startCO2, response.co2, t / duration);
            float lerpEnergy = Mathf.Lerp(startEnergy, response.energy, t / duration);

            if (trafficText != null) trafficText.text = $"Traffic: {lerpTraffic:F1}%";
            if (co2Text != null) co2Text.text = $"CO2: {lerpCO2:F1}%";
            if (energyText != null) energyText.text = $"Energy: {lerpEnergy:F1}MW";
            yield return null;
        }
    }

    void HandleErrorResponse(string error)
    {
        Debug.LogError($"API Error: {error}");
        if (messageText != null) messageText.text = "Connection Failed!";
    }
}

// ---- Data Models ----
[System.Serializable] public class APIBuildingData { public string type; public int count; }
[System.Serializable] public class SimulationRequest { public string layout; public List<APIBuildingData> buildings; public string smartthings_mode; }
[System.Serializable] public class SimulationResponse { public int traffic; public int co2; public int energy; public string message; }
[System.Serializable] public class SimulationResult { public float traffic; public float co2; public float energy; }

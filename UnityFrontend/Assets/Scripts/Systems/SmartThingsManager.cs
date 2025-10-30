using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SmartThingsManager : MonoBehaviour
{
    [Header("States (runtime)")]
    public bool ecoMode = false;
    public bool trafficControl = false;

    [Header("Live metrics (from APIManager)")]
    public float co2 = 0f;
    public float traffic = 0f;

    [Header("UI References")]
    public TMP_Text statusText;
    public Button ecoModeButton;
    public Button trafficControlButton;
    public Button alertButton;

    [Header("Button Visual Feedback")]
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.white;

    private APIManager apiManager;

    void Start()
    {
        apiManager = FindFirstObjectByType<APIManager>();

        if (ecoModeButton != null) ecoModeButton.onClick.AddListener(ToggleEcoMode);
        if (trafficControlButton != null) trafficControlButton.onClick.AddListener(ToggleTrafficControl);
        if (alertButton != null) alertButton.onClick.AddListener(TriggerAlert);

        UpdateButtonAppearance();
        UpdateStatusText();
    }

    public void ToggleEcoMode()
    {
        ecoMode = !ecoMode;

        if (apiManager != null)
        {
            if (ecoMode)
            {
                apiManager.currentSmartThingsMode = "eco";
                if (trafficControl) { trafficControl = false; UpdateButtonAppearance(); }
            }
            else
            {
                apiManager.currentSmartThingsMode = "";
            }

            apiManager.SimulateCurrentCity();
            ApplySmartThingsEffects(apiManager); // immediate visual nudge
        }

        UpdateButtonAppearance();
        UpdateStatusText();
        Debug.Log($"Eco Mode: {ecoMode}");
    }

    public void ToggleTrafficControl()
    {
        trafficControl = !trafficControl;

        if (apiManager != null)
        {
            if (trafficControl)
            {
                apiManager.currentSmartThingsMode = "traffic_control";
                if (ecoMode) { ecoMode = false; UpdateButtonAppearance(); }
            }
            else
            {
                apiManager.currentSmartThingsMode = "";
            }

            apiManager.SimulateCurrentCity();
            ApplySmartThingsEffects(apiManager); // immediate visual nudge
        }

        UpdateButtonAppearance();
        UpdateStatusText();
        Debug.Log($"Traffic Control: {trafficControl}");
    }

    public void TriggerAlert()
    {
        if (apiManager != null)
        {
            apiManager.currentSmartThingsMode = "alert";
            apiManager.SimulateCurrentCity();
            StartCoroutine(ResetAlertMode());
        }

        if (statusText != null) statusText.text = "ALERT: City-wide notification sent!";
        Debug.Log("SmartThings: Alert triggered!");
    }

    private System.Collections.IEnumerator ResetAlertMode()
    {
        yield return new WaitForSeconds(2f);
        if (apiManager != null) apiManager.currentSmartThingsMode = "";
        UpdateStatusText();
    }

    // Light, local-only UI effect; backend result still drives the true state via APIManager
    public void ApplySmartThingsEffects(APIManager api)
    {
        if (api == null) return;

        float c = co2;
        float t = traffic;
        float e = api.simulationResult.energy;

        if (ecoMode) { c *= 0.8f; e *= 0.9f; }
        if (trafficControl) { t *= 0.75f; e *= 1.05f; }

        c = Mathf.Clamp(c, 0f, 100f);
        t = Mathf.Clamp(t, 0f, 100f);
        e = Mathf.Clamp(e, 0f, 999f);

        if (api.co2Text != null) api.co2Text.text = $"CO2: {c:F1}%";
        if (api.trafficText != null) api.trafficText.text = $"Traffic: {t:F1}%";
        if (api.energyText != null) api.energyText.text = $"Energy: {e:F1}MW";

        api.UpdateIndicatorColors(new SimulationResponse
        {
            co2 = Mathf.RoundToInt(c),
            traffic = Mathf.RoundToInt(t),
            energy = Mathf.RoundToInt(e)
        });

        UpdateStatusText();
    }

    void UpdateButtonAppearance()
    {
        if (ecoModeButton != null)
        {
            Image ecoImage = ecoModeButton.GetComponent<Image>();
            if (ecoImage != null) ecoImage.color = ecoMode ? activeColor : inactiveColor;

            TMP_Text ecoText = ecoModeButton.GetComponentInChildren<TMP_Text>();
            if (ecoText != null) ecoText.text = ecoMode ? "ECO ON" : "ECO OFF";
        }

        if (trafficControlButton != null)
        {
            Image trafficImage = trafficControlButton.GetComponent<Image>();
            if (trafficImage != null) trafficImage.color = trafficControl ? activeColor : inactiveColor;

            TMP_Text trafficTextComp = trafficControlButton.GetComponentInChildren<TMP_Text>();
            if (trafficTextComp != null) trafficTextComp.text = trafficControl ? "TRAFFIC ON" : "TRAFFIC OFF";
        }
    }

    public void UpdateStatusText()
    {
        if (statusText == null) return;

        string status =
            $"Eco: {(ecoMode ? "ON" : "OFF")}\n" +
            $"Traffic: {(trafficControl ? "ON" : "OFF")}\n" +
            $"Live CO2: {co2}%\n" +
            $"Live Traffic: {traffic}%";

        statusText.text = status;
    }
}

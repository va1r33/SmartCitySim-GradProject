using UnityEngine;
using TMPro;

public class SmartThingsManager : MonoBehaviour
{
    [Header("SmartThings States")]
    public bool ecoMode = false;
    public bool trafficControl = false;

    [Header("Optional UI Feedback (drag TMP text here if you want)")]
    public TMP_Text statusText;

    public void ToggleEcoMode()
    {
        ecoMode = !ecoMode;
        Debug.Log($"Eco Mode: {(ecoMode ? "ON" : "OFF")}");
        UpdateStatusText();
    }

    public void ToggleTrafficControl()
    {
        trafficControl = !trafficControl;
        Debug.Log($"Traffic Control: {(trafficControl ? "ON" : "OFF")}");
        UpdateStatusText();
    }

    public void TriggerAlert()
    {
        Debug.Log("SmartThings Alert Triggered!");
        if (statusText != null)
            statusText.text = "Alert Triggered!";
    }

    private void UpdateStatusText()
    {
        if (statusText != null)
            statusText.text = $"EcoMode: {(ecoMode ? "ON" : "OFF")} | TrafficControl: {(trafficControl ? "ON" : "OFF")}";
    }
}

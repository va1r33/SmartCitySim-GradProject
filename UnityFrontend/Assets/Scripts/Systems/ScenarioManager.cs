using UnityEngine;

public class ScenarioManager : MonoBehaviour
{
    public enum ScenarioType { None, GreenMandate }

    public ScenarioType currentScenario = ScenarioType.None;
    public MissionManager missionManager;
    public APIManager apiManager;  // connect to API

    [Header("Green Mandate Settings")]
    public float goalCO2 = 60f;
    public float goalTraffic = 40f;
    public float goalEnergy = 100f;
    public float missionStartCO2 = 100f;

    void Start()
    {
        if (missionManager == null) missionManager = FindFirstObjectByType<MissionManager>();
        if (apiManager == null) apiManager = FindFirstObjectByType<APIManager>();
        StartScenario(ScenarioType.GreenMandate);
    }

    public void StartScenario(ScenarioType type)
    {
        currentScenario = type;
        switch (type)
        {
            case ScenarioType.GreenMandate:
                if (missionManager != null)
                {
                    missionManager.SetMission(
                        "GREEN MANDATE",
                        $"Reduce CO2 to ≤ {goalCO2}%, Traffic to ≤ {goalTraffic}%, Energy to ≤ {goalEnergy} MW",
                        100f
                    );
                }
                break;
        }
    }

    // Called by APIManager whenever new data arrives
    public void UpdateScenarioProgress(float currentCO2)
    {
        if (currentScenario != ScenarioType.GreenMandate || missionManager == null || apiManager == null) return;

        // Progress: average of the three-goal achievements (0..100)
        float pCO2 = Mathf.InverseLerp(missionStartCO2, goalCO2, Mathf.Clamp(currentCO2, goalCO2, missionStartCO2));
        float pTraffic = apiManager.simulationResult.traffic <= goalTraffic ? 1f : 0f;
        float pEnergy = apiManager.simulationResult.energy <= goalEnergy ? 1f : 0f;

        float composite = Mathf.Clamp01((pCO2 + pTraffic + pEnergy) / 3f) * 100f;

        // Set absolute progress to composite (not additive)
        missionManager.targetProgress = 100f;
        // overwrite internal progress via delta = desired - current
        float desired = composite;
        float delta = desired - (missionManager.IsComplete() ? 100f : (missionManager.progressBar != null ? missionManager.progressBar.value * 100f : 0f));
        missionManager.UpdateProgress(delta);

        // Completion condition
        if (currentCO2 <= goalCO2 &&
            apiManager.simulationResult.traffic <= goalTraffic &&
            apiManager.simulationResult.energy <= goalEnergy)
        {
            OnScenarioComplete();
        }
    }

    void OnScenarioComplete()
    {
        Debug.Log("SCENARIO COMPLETE: Green Mandate Achieved!");
        currentScenario = ScenarioType.None;
    }
}

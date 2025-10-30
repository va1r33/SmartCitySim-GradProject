using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject missionPanel;
    public TMP_Text missionTitle;
    public TMP_Text missionDesc;
    public Slider progressBar;
    public TMP_Text progressPercentText;

    private float progress = 0f;
    public float targetProgress = 100f;
    public float progressClampMin = 0f;

    void Start()
    {
        if (missionPanel != null) missionPanel.SetActive(false);
        UpdateUI();
    }

    public void ShowMission()
    {
        if (missionPanel != null) missionPanel.SetActive(true);
    }

    public void HideMission()
    {
        if (missionPanel != null) missionPanel.SetActive(false);
    }

    public void SetMission(string title, string description, float target = 100f)
    {
        if (missionTitle != null) missionTitle.text = title;
        if (missionDesc != null) missionDesc.text = description;
        targetProgress = Mathf.Max(1f, target);
        progress = 0f;
        UpdateUI();
        ShowMission();
    }

    // value is absolute increment (not normalized)
    public void UpdateProgress(float delta)
    {
        progress = Mathf.Clamp(progress + delta, progressClampMin, targetProgress);
        UpdateUI();
    }

    public void SetProgress01(float normalized)
    {
        normalized = Mathf.Clamp01(normalized);
        progress = normalized * targetProgress;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (progressBar != null) progressBar.value = progress / targetProgress;
        if (progressPercentText != null) progressPercentText.text = $"{Mathf.RoundToInt((progress / targetProgress) * 100)}%";
    }

    public bool IsComplete() => progress >= targetProgress;
}

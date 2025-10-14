// Assets/Scripts/UI/UIManager.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    private APIManager apiManager;
    private Button simulateButton;
    private Image buttonImage;
    private TMP_Text buttonText;

    [Header("Button Colors")]
    public Color normalColor = new Color(0.294f, 0.773f, 0.737f); // #4BC5BC
    public Color highlightedColor = new Color(1f, 1f, 1f); // #FFFFFF
    public Color pressedColor = new Color(0.137f, 0.820f, 0.694f); // #23D1B1
    public Color selectedColor = new Color(0.961f, 0.961f, 0.961f); // #F5F5F5
    public Color disabledColor = new Color(0.584f, 0.647f, 0.651f); // #95A5A6

    void Start()
    {
        // Find the APIManager automatically
        apiManager = FindFirstObjectByType<APIManager>();

        // Get button references
        simulateButton = FindFirstObjectByType<Button>();
        if (simulateButton != null)
        {
            buttonImage = simulateButton.GetComponent<Image>();
            buttonText = simulateButton.GetComponentInChildren<TMP_Text>();

            // Setup button colors
            SetupButtonColors();
        }

        if (apiManager == null)
        {
            Debug.LogError("APIManager not found in scene!");
        }
    }

    void SetupButtonColors()
    {
        ColorBlock colors = simulateButton.colors;
        colors.normalColor = normalColor;
        colors.highlightedColor = highlightedColor;
        colors.pressedColor = pressedColor;
        colors.disabledColor = disabledColor;
        colors.fadeDuration = 0.1f;
        simulateButton.colors = colors;
    }

    public void RunSimulation()
    {
        if (apiManager != null)
        {
            apiManager.SimulateCurrentCity();
            Debug.Log("Simulation running...");

            // Visual feedback
            StartCoroutine(AnimateButton());
        }
        else
        {
            Debug.LogError("Cannot run simulation - APIManager is missing!");
        }
    }

    IEnumerator AnimateButton()
    {
        if (buttonImage != null)
        {
            // Store original color
            Color originalColor = buttonImage.color;

            // Pulse effect
            buttonImage.color = highlightedColor;
            yield return new WaitForSeconds(0.2f);
            buttonImage.color = originalColor;
        }
    }
}
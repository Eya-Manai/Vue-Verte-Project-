using UnityEngine;
using TMPro; // Remove this line if you're not using TextMeshPro for the score text
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Fires every time the score changes, so any label anywhere can listen and update itself
    // Passes (currentScore, totalToCollect)
    public static event Action<int, int> OnScoreChanged;

    [Header("Score Settings")]
    public int totalToCollect = 7;
    private int currentScore = 0;
    public int CurrentScore => currentScore; // read-only access for other scripts

    [Header("UI References")]
    public TextMeshProUGUI scoreText; // Drag your on-screen score text here (optional)
    public GameObject winPopupPanel;  // Drag your finished info-card / thank-you Panel here (keep it disabled by default)

    [Header("Popup Audio")]
    public AudioSource popupAudioSource; // Drag an AudioSource component here
    public AudioClip popupVoiceClip;     // Drag your narration/voice clip here

    private PopupFollowPlayer popupFollowScript;

    void Awake()
    {
        // Simple singleton so any collectible can access this manager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreUI();
        if (winPopupPanel != null)
        {
            popupFollowScript = winPopupPanel.GetComponentInParent<PopupFollowPlayer>();
            winPopupPanel.SetActive(false);
        }
    }

    public void CollectItem()
    {
        currentScore++;
        UpdateScoreUI();

        if (currentScore >= totalToCollect)
        {
            ShowWinPopup();
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = currentScore + " / " + totalToCollect;

        // Let any listening label (e.g. on collectible items) update itself
        OnScoreChanged?.Invoke(currentScore, totalToCollect);
    }

    void ShowWinPopup()
    {
        if (winPopupPanel == null) return;

        winPopupPanel.SetActive(true);

        // Position the popup right in front of the player and have it face them
        if (popupFollowScript != null)
            popupFollowScript.PlaceInFrontOfPlayer();

        // Play the voice narration
        if (popupAudioSource != null && popupVoiceClip != null)
        {
            popupAudioSource.clip = popupVoiceClip;
            popupAudioSource.Play();
        }

        // Optional: pause the game while popup is showing
        // Time.timeScale = 0f;
    }
}
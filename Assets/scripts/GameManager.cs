using UnityEngine;
using TMPro; // Remove this line if you're not using TextMeshPro for the score text

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Score Settings")]
    public int totalToCollect = 7;
    private int currentScore = 0;

    [Header("UI References")]
    public TextMeshProUGUI scoreText; // Drag your on-screen score text here (optional)
    public GameObject winPopupPanel;  // Drag your finished info-card / thank-you Panel here (keep it disabled by default)

    [Header("Air Timer Settings")]
    public float airDuration = 90f;       // seconds of air per tank
    private float currentAir;
    private bool airDepleted = false;
    public GameObject airWarningPanel;    // your "refill air" card (separate from winPopupPanel, keep disabled by default)
    public PlayerMovement playerMovement; // drag the GameObject holding your locomotion script here

    private PopupFollowPlayer popupFollowScript;
    private PopupFollowPlayer airWarningFollowScript;

    private GameHUB hud;

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
        hud = FindObjectOfType<GameHUB>();
    }

    void Start()
    {
        UpdateScoreUI();
        if (winPopupPanel != null)
        {
            popupFollowScript = winPopupPanel.GetComponentInParent<PopupFollowPlayer>();
            winPopupPanel.SetActive(false);
        }
        if (airWarningPanel != null)
        {
            airWarningFollowScript = airWarningPanel.GetComponentInParent<PopupFollowPlayer>();
            //airWarningPanel.SetActive(false);
        }

        currentAir = airDuration;
    }

    void Update()
    {
        if (airDepleted) return;

        currentAir -= Time.deltaTime;

        if (currentAir <= 0)
        {
            currentAir = 0;
        }

        if (hud != null)
            hud.UpdateTime(currentAir);

        if (currentAir <= 0 && !airDepleted)
        {
            airDepleted = true;
            OnAirDepleted();
        }
    }

    void OnAirDepleted()
    {
        Debug.Log("playerMovement is " + (playerMovement == null ? "NULL" : "assigned"));

        if (playerMovement != null)
        {
            Debug.Log("Calling canMove = false");
            playerMovement.canMove = false;
        }
        else
        {
            Debug.Log("playerMovement is NULL");
        }

        if (airWarningPanel != null)
        {
            airWarningPanel.SetActive(true);

            if (airWarningFollowScript != null)
                airWarningFollowScript.PlaceInFrontOfPlayer();
        }
    }

    // Hook this to the air-warning card's OK button (OnClick)
    public void OnAirRefillConfirmed()
    {
        Debug.Log("Refill confirmed");
        if (airWarningPanel != null)
            airWarningPanel.SetActive(false);

        currentAir = airDuration;
        airDepleted = false;

        if (playerMovement != null)
            playerMovement.canMove = true;
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
    }

    void ShowWinPopup()
    {
        if (winPopupPanel == null) return;

        winPopupPanel.SetActive(true);

        // Position the popup right in front of the player and have it face them
        if (popupFollowScript != null)
            popupFollowScript.PlaceInFrontOfPlayer();

        // Optional: pause the game while popup is showing
        // Time.timeScale = 0f;
    }
}
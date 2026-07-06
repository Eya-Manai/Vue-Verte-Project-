using UnityEngine;
using UnityEngine.UI;

public class ShowInfoCard : MonoBehaviour
{
    [SerializeField]
    private GameObject infoCard;

    [Header("Objet Posidonie à cacher pendant l'affichage")]
    [SerializeField]
    private GameObject posidoniaObject; // <-- assign the posidonia GameObject here in the Inspector

    [Header("Billboard (optionnel)")]
    [SerializeField]
    private bool faceCamera = true;
    private Transform cam;

    [Header("Bouton Fermer (optionnel)")]
    [SerializeField]
    private Button closeButton;

    void Start()
    {
        Debug.Log("ShowInfoCard script charge et pret sur : " + gameObject.name);
        if (Camera.main != null)
        {
            cam = Camera.main.transform;
        }
        if (infoCard != null)
        {
            infoCard.SetActive(false); // caché au démarrage
        }
        // Connecte automatiquement le bouton Fermer à Hide()
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }
    }

    void LateUpdate()
    {
        if (faceCamera && infoCard != null && infoCard.activeSelf && cam != null)
        {
            infoCard.transform.LookAt(infoCard.transform.position + cam.forward);
        }
    }

    public void Show()
    {
        Debug.Log("GRAB detected - Show() called !");
        if (infoCard != null)
        {
            infoCard.SetActive(true);
            Debug.Log("Infocard active !");
        }
        else
        {
            Debug.LogWarning("infoCard is not assigned in the Inspector !");
        }

        // Hide the posidonia object while the card is up
        if (posidoniaObject != null)
        {
            posidoniaObject.SetActive(false);
        }
    }

    public void Hide() // <-- THIS makes the popup disappear
    {
        Debug.Log("Hide() called - Hide detected");
        if (infoCard != null)
        {
            infoCard.SetActive(false); // <-- popup hidden here
        }

        // Show the posidonia object again once the card is closed
        if (posidoniaObject != null)
        {
            posidoniaObject.SetActive(true);
        }
    }

    void OnMouseDown()
    {
        Debug.Log("Standard mouse click detected!");
        if (infoCard != null && !infoCard.activeSelf)
        {
            Show();
        }
        else if (infoCard != null && infoCard.activeSelf)
        {
            Hide();
        }
    }
}
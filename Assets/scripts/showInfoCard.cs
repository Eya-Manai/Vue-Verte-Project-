using UnityEngine;
using UnityEngine.UI;

public class ShowInfoCard : MonoBehaviour
{
    [SerializeField]
    private GameObject infoCard;

    [Header("Objet Posidonie à cacher pendant l'affichage")]
    [SerializeField]
    private GameObject posidoniaObject;

    [Header("Indicateur (Particle System) à cacher pendant l'affichage")]
    [SerializeField]
    private ParticleSystem indicatorParticles; // <-- drag your Particle System here

    [Header("Billboard (optionnel)")]
    [SerializeField]
    private bool faceCamera = true;
    private Transform cam;

    [Header("Bouton Fermer (optionnel)")]
    [SerializeField]
    private Button closeButton;

    [Header("Fermeture automatique (optionnel)")]
    [SerializeField]
    private bool autoCloseEnabled = true;
    [SerializeField]
    private float autoCloseDelay = 8f; // seconds
    private float autoCloseTimer;

    private Renderer[] posidoniaRenderers;
    private Collider[] posidoniaColliders;
    private Rigidbody posidoniaRigidbody;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        if (Camera.main != null) cam = Camera.main.transform;
        if (infoCard != null) infoCard.SetActive(false);
        if (closeButton != null) closeButton.onClick.AddListener(Hide);

        if (posidoniaObject != null)
        {
            posidoniaRenderers = posidoniaObject.GetComponentsInChildren<Renderer>(true);
            posidoniaColliders = posidoniaObject.GetComponentsInChildren<Collider>(true);
            posidoniaRigidbody = posidoniaObject.GetComponent<Rigidbody>();
            originalPosition = posidoniaObject.transform.position;
            originalRotation = posidoniaObject.transform.rotation;
        }
    }

    void Update()
    {
        if (autoCloseEnabled && infoCard != null && infoCard.activeSelf)
        {
            autoCloseTimer -= Time.deltaTime;
            if (autoCloseTimer <= 0f)
            {
                Debug.Log("Auto-close triggered after timeout.");
                Hide();
            }
        }
    }

    void LateUpdate()
    {
        if (faceCamera && infoCard != null && infoCard.activeSelf && cam != null)
        {
            Vector3 directionAwayFromCamera = infoCard.transform.position - cam.position;
            if (directionAwayFromCamera.sqrMagnitude > 0.0001f)
            {
                infoCard.transform.rotation = Quaternion.LookRotation(directionAwayFromCamera);
            }
        }
    }

    public void Show()
    {
        if (infoCard != null) infoCard.SetActive(true);
        SetPosidoniaVisible(false);
        SetIndicatorVisible(false);
        autoCloseTimer = autoCloseDelay;
    }

    public void Hide()
    {
        if (infoCard != null) infoCard.SetActive(false);
        if (posidoniaObject != null)
        {
            posidoniaObject.transform.position = originalPosition;
            posidoniaObject.transform.rotation = originalRotation;
            if (posidoniaRigidbody != null)
            {
                posidoniaRigidbody.velocity = Vector3.zero;
                posidoniaRigidbody.angularVelocity = Vector3.zero;
            }
        }
        SetPosidoniaVisible(true);
        SetIndicatorVisible(true);
    }

    private void SetPosidoniaVisible(bool visible)
    {
        if (posidoniaRenderers != null)
            foreach (var r in posidoniaRenderers)
                if (r != null) r.enabled = visible;
        if (posidoniaColliders != null)
            foreach (var c in posidoniaColliders)
                if (c != null) c.enabled = visible;
    }

    private void SetIndicatorVisible(bool visible)
    {
        if (indicatorParticles == null) return;

        if (visible)
        {
            indicatorParticles.gameObject.SetActive(true);
            indicatorParticles.Play(true); // true = include children
        }
        else
        {
            // Stop emitting AND clear existing particles instantly
            indicatorParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
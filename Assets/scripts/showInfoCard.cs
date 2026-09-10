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
    [Tooltip("Limite l'inclinaison verticale (axe X) en degrés, dans les deux sens.")]
    [SerializeField]
    private float maxPitchAngle = 60f;
    [Tooltip("Empêche la carte de se déplacer (utile si l'objet parent bouge, ex: quand on l'attrape). Optionnel: assigner un Transform fixe, sinon la position de départ de la carte est utilisée.")]
    [SerializeField]
    private bool lockPosition = true;
    [SerializeField]
    private Transform positionAnchor; // optional - if empty, uses the card's own starting position
    private Transform cam;
    private float initialYRotation; // Y rotation is frozen; only X (pitch) is allowed to move
    private Vector3 initialInfoCardPosition;

    [Header("Bouton Fermer (optionnel)")]
    [SerializeField]
    private Button closeButton;

    [Header("Fermeture automatique")]
    [SerializeField]
    private bool autoCloseEnabled = true;
    [Tooltip("Utilisé seulement si ListenForVoiceEnd n'a pas été appelé (pas de voix assignée).")]
    [SerializeField]
    private float autoCloseDelay = 3f; // seconds
    private float autoCloseTimer;

    // Set at runtime via ListenForVoiceEnd(), called from the Select() event list.
    private AudioSource activeVoiceSource;
    private bool waitingForVoiceToStart;

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

        if (infoCard != null)
        {
            initialYRotation = infoCard.transform.eulerAngles.y;
            initialInfoCardPosition = infoCard.transform.position;
        }

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
        if (!autoCloseEnabled || infoCard == null || !infoCard.activeSelf) return;

        if (activeVoiceSource != null)
        {
            // Wait for the voice clip to actually start, then close as soon as it stops.
            if (activeVoiceSource.isPlaying)
            {
                waitingForVoiceToStart = false;
            }
            else if (!waitingForVoiceToStart)
            {
                Debug.Log("Voice clip finished, auto-closing info card.");
                Hide();
            }
        }
        else
        {
            // Fallback: fixed timer, used only if no voice source was registered this time.
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
        if (infoCard == null || !infoCard.activeSelf) return;

        // Pin the card's world position so it can't drift/fly if a parent
        // object (e.g. the thing being grabbed) moves underneath it.
        if (lockPosition)
        {
            Vector3 targetPos = positionAnchor != null ? positionAnchor.position : initialInfoCardPosition;
            infoCard.transform.position = targetPos;
        }

        if (faceCamera && cam != null)
        {
            Vector3 toCamera = cam.position - infoCard.transform.position;
            float horizontalDist = Mathf.Sqrt(toCamera.x * toCamera.x + toCamera.z * toCamera.z);

            if (horizontalDist > 0.0001f || Mathf.Abs(toCamera.y) > 0.0001f)
            {
                // Compute pitch directly via trigonometry instead of decomposing a
                // Quaternion.LookRotation - this avoids the erratic flips/jumps that
                // happen with eulerAngles extraction when the camera gets close to
                // directly above/below the card (near-vertical direction).
                float pitch = -Mathf.Atan2(toCamera.y, horizontalDist) * Mathf.Rad2Deg;
                pitch = Mathf.Clamp(pitch, -maxPitchAngle, maxPitchAngle);

                // Keep Y fixed (no left/right rotation), only apply X (up/down tilt)
                infoCard.transform.rotation = Quaternion.Euler(pitch, initialYRotation, 0f);
            }
        }
    }

    public void Show()
    {
        if (infoCard != null) infoCard.SetActive(true);
        SetPosidoniaVisible(false);
        SetIndicatorVisible(false);
        autoCloseTimer = autoCloseDelay;

        // Reset - if this Show() call isn't followed by a ListenForVoiceEnd() call,
        // we fall back to the fixed timer above.
        activeVoiceSource = null;
        waitingForVoiceToStart = false;
    }

    // Call this from the SAME When Select() event list, right after AudioSource.Play,
    // passing that same voice AudioSource. Works for any number of cards/objects
    // without touching this component's own Inspector fields.
    public void ListenForVoiceEnd(AudioSource source)
    {
        activeVoiceSource = source;
        waitingForVoiceToStart = source != null;
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
        activeVoiceSource = null;
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
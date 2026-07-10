using UnityEngine;
using Oculus.Interaction;

public class DisappearOnGrab : MonoBehaviour
{
    [Header("What happens when grabbed")]
    [SerializeField]
    private bool destroyObject = false;
    [SerializeField]
    private float delayBeforeDisappear = 0f;

    [Header("Optional: play something before disappearing")]
    [SerializeField]
    private ParticleSystem disappearEffect;
    [SerializeField]
    private AudioClip disappearClip; // <-- use AudioClip, not AudioSource
    [SerializeField]
    [Range(0f, 1f)]
    private float disappearVolume = 1f;

    private Grabbable grabbable;

    void Awake()
    {
        grabbable = GetComponent<Grabbable>();
        if (grabbable == null)
            grabbable = GetComponentInChildren<Grabbable>();

        if (grabbable != null)
            grabbable.WhenPointerEventRaised += HandlePointerEvent;
        else
            Debug.LogWarning("DisappearOnGrab: No Grabbable component found on " + gameObject.name);
    }

    void OnDestroy()
    {
        if (grabbable != null)
            grabbable.WhenPointerEventRaised -= HandlePointerEvent;
    }

    private void HandlePointerEvent(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Select)
        {
            if (delayBeforeDisappear > 0f)
                Invoke(nameof(Disappear), delayBeforeDisappear);
            else
                Disappear();
        }
    }

    private void Disappear()
    {
        if (disappearEffect != null)
        {
            disappearEffect.transform.parent = null;
            disappearEffect.Play();
            Destroy(disappearEffect.gameObject, 3f);
        }

        if (disappearClip != null)
        {
            // Spawns its own temporary audio object at this position, independent of "this" object
            AudioSource.PlayClipAtPoint(disappearClip, transform.position, disappearVolume);
        }

        if (destroyObject)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }
}
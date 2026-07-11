using UnityEngine;
using Oculus.Interaction;

public class CollectableItem : MonoBehaviour
{
    private Grabbable grabbable;
    private bool alreadyCollected = false;

    void Awake()
    {
        grabbable = GetComponent<Grabbable>();
        if (grabbable == null)
            grabbable = GetComponentInChildren<Grabbable>();

        if (grabbable != null)
            grabbable.WhenPointerEventRaised += HandlePointerEvent;
        else
            Debug.LogWarning("CollectibleItem: No Grabbable component found on " + gameObject.name);
    }

    void OnDestroy()
    {
        if (grabbable != null)
            grabbable.WhenPointerEventRaised -= HandlePointerEvent;
    }

    private void HandlePointerEvent(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Select && !alreadyCollected)
        {
            alreadyCollected = true; // prevents double-counting if grabbed again before it disappears
            GameManager.Instance.CollectItem();
        }
    }
}
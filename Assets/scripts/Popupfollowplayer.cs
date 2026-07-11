using UnityEngine;

public class PopupFollowPlayer : MonoBehaviour
{
    [Header("References")]
    public Transform vrCamera; // Drag your XR Origin's Main Camera here

    [Header("Placement")]
    public float distanceFromPlayer = 1.5f;
    public float heightOffset = 0f;

    // Call this when you want the popup to appear right in front of the player
    public void PlaceInFrontOfPlayer()
    {
        if (vrCamera == null) return;

        // Position: a fixed distance in front of the camera, at the camera's height + offset
        Vector3 forwardFlat = new Vector3(vrCamera.forward.x, 0f, vrCamera.forward.z).normalized;
        Vector3 targetPos = vrCamera.position + forwardFlat * distanceFromPlayer;
        targetPos.y = vrCamera.position.y + heightOffset;
        transform.position = targetPos;

        FaceCamera();
    }

    void LateUpdate()
    {
        // Keep facing the player every frame while visible (billboard effect)
        if (vrCamera != null)
            FaceCamera();
    }

    void FaceCamera()
    {
        Vector3 direction = transform.position - vrCamera.position;
        direction.y = 0f; // keep it upright, no tilting up/down
        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(direction);
    }
}
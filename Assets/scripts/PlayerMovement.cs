using UnityEngine;
using Oculus.Interaction.Locomotion;

public class PlayerMovement : MonoBehaviour
{
    public FirstPersonLocomotor locomotor;
    public MonoBehaviour bodyTeleportInteractor;
    public MonoBehaviour smoothMovementTunneling;

    public bool canMove
    {
        get => locomotor.enabled;
        set
        {
            locomotor.enabled = value;

            if (bodyTeleportInteractor != null)
                bodyTeleportInteractor.enabled = value;
            if (smoothMovementTunneling != null)
                smoothMovementTunneling.enabled = value;
        }
    }
}
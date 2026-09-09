using UnityEngine;

public class AirWarningCard : MonoBehaviour
{
    public void OnConfirmClicked()
    {
        Debug.Log("AirWarningCard: OnConfirmClicked() was called!");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnAirRefillConfirmed();
        }
        else
        {
            Debug.Log("GameManager.Instance is NULL");
        }
    }
}
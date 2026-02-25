using UnityEngine;

public class PlayerEventReciever : MonoBehaviour
{
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }
    public void AirSlamImpact()
    {
        if (playerController != null)
        {
            playerController.OnAirSlamImpact();
        }
    }

    // for future when we add charging effects
    public void AirAttackStart()
    {
        if (playerController != null)
        {
            playerController.OnAirAttackStart();
        }
    }
}

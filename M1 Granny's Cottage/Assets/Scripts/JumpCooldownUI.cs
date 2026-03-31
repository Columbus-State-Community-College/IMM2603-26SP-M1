using UnityEngine;
using UnityEngine.UI;

public class JumpCooldownUI : MonoBehaviour
{
    public PlayerController player;
    public Image cooldownRing;

    void Update()
    {
        if (player == null) return;

        float cooldown = player.GetSlamCooldownTimer();
        float maxCooldown = player.GetSlamCooldownDuration();

        if (cooldown <= 0)
        {
            cooldownRing.fillAmount = 1f;
        }
        else
        {
            cooldownRing.fillAmount = 1f - (cooldown / maxCooldown);
        }
    }
}

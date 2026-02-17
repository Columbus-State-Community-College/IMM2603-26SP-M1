using UnityEngine;

public class HammerEventReciever : MonoBehaviour
{
    // this allows the animator to call functions from hammer attack script
    public HammerAttack hammerAttack;

    public void EnableHitbox() => hammerAttack.EnableHitbox();
    public void DisableHitbox() => hammerAttack.DisableHitbox();
    public void PlaySwooshSound() => hammerAttack.PlaySwooshSound();
}

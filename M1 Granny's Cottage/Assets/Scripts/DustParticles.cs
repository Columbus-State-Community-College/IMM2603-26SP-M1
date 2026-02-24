using UnityEngine;
using System.Collections;

public class DustParticles : MonoBehaviour
{
    public ParticleSystem bigDust;
    public ParticleSystem smallDust;

    public Transform leftFoot;
    public Transform rightFoot;

    public void LeftStep()
    {
        SpawnDust(leftFoot.position);
    }

    public void RightStep()
    {
        SpawnDust(rightFoot.position);
    }

    void SpawnDust(Vector3 position)
    {
        ParticleSystem big = Instantiate(bigDust, position, Quaternion.identity);
        big.Play();
        Destroy(big.gameObject, 1f);

        StartCoroutine(SmallDustDelay(position));
    }

    IEnumerator SmallDustDelay(Vector3 position)
    {
        yield return new WaitForSeconds(0.1f);

        ParticleSystem small = Instantiate(smallDust, position, Quaternion.identity);
        small.Play();
        Destroy(small.gameObject, 1f);
    }
}

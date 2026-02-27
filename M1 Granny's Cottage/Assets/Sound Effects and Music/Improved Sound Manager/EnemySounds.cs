using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemySounds : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("SFX Names (must match SoundManager list)")]
    public string monsterScreamSFX;
    public string monsterSpawnSFX;

    [Header("Local Volume Settings (can exceed normal volume)")]
    [Range(0f, 5f)]
    public float monsterScreamVolume = 1f;

    [Range(0f, 5f)]
    public float monsterSpawnVolume = 1f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    private AudioClip GetClip(string clipName)
    {
        var item = SoundManager.instance.sfxClips.Find(s => s.name == clipName);
        if (item == null)
        {
            Debug.LogWarning("[Grannys Sounds] Missing clip: " + clipName);
            return null;
        }
        return item.clip;
    }

    private void PlayLocalClip(string clipName, float localVolume)
    {
        AudioClip clip = GetClip(clipName);
        if (clip == null) return;

        float finalVolume = SoundManager.instance.sfxVolume * localVolume;

        // allow up to 300% volume without clipping too hard
        finalVolume = Mathf.Clamp(finalVolume, 0f, 3f);

        audioSource.PlayOneShot(clip, finalVolume);
    }

    public void PlayMonsterScream()
    {
        PlayLocalClip(monsterScreamSFX, monsterScreamVolume);
    }

    public void PlayMonsterSpawn()
    {
        PlayLocalClip(monsterSpawnSFX, monsterSpawnVolume);
    }
}

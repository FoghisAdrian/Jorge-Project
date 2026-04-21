using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("Jump Settings")]
    public AudioClip jumpSound;

    [Header("Hurt Settings")]
    public AudioClip[] hurtSounds;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayJumpSound()
    {
        if (audioSource != null && jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }

    public void PlayRandomHurtSound()
    {
        if (audioSource != null && hurtSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, hurtSounds.Length);
            audioSource.PlayOneShot(hurtSounds[randomIndex]);
        }
    }
}

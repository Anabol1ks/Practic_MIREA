using UnityEngine;

public class WhistleExplosion : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip whistleExplosionClip;

    public void PlayExplosionSound()
    {
        audioSource.PlayOneShot(whistleExplosionClip);
    }
}

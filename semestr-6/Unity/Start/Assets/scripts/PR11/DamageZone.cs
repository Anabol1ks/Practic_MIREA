using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public AudioSource damageAudio;
    public AudioClip damageClip;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            damageAudio.PlayOneShot(damageClip);
            Debug.Log("Игрок получил урон!");
        }
    }
}

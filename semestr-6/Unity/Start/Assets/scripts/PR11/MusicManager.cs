using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource track1;
    public AudioSource track2;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (track1.isPlaying)
            {
                track1.Stop();
                track2.Play();
            }
            else
            {
                track2.Stop();
                track1.Play();
            }
        }
    }
}
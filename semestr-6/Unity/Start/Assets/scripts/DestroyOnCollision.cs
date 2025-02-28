using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    // private void OnCollisionEnter(Collision collision)
    // {
    //     Destroy(gameObject);
    // }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Cube")
            Destroy(gameObject);
    }

}

using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] private GameObject destroyedVersion;
    [SerializeField] private float destructionForce = 10f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.relativeVelocity.magnitude > 5f)
        {
            DestroyBuilding();
        }
    }

    public void DestroyBuilding()
    {
        // Спауним разрушенную версию
        GameObject destroyed = Instantiate(destroyedVersion, transform.position, transform.rotation);
        // Применяем силу ко всем частям
        foreach (Rigidbody rb in destroyed.GetComponentsInChildren<Rigidbody>())
        {
            rb.AddExplosionForce(destructionForce, transform.position, 5f);
        }
        Destroy(gameObject);
    }
}
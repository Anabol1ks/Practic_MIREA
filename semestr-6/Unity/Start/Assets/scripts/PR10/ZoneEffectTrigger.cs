using UnityEngine;

public class ZoneEffectTrigger : MonoBehaviour
{
    [Tooltip("Префаб системы частиц")]
    public GameObject effectPrefab;

    [Tooltip("Минимальное количество эффектов")]
    public int minEffects = 1;
    [Tooltip("Максимальное количество эффектов")]
    public int maxEffects = 3;

    private bool playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playerInside)
        {
            playerInside = true;

            BoxCollider box = GetComponent<BoxCollider>();
            if (box == null)
            {
                Debug.LogWarning("ZoneEffectTrigger: Требуется BoxCollider на объекте.");
                return;
            }

            Vector3 size = box.size;
            Vector3 center = box.center;
            Vector3 worldCenter = transform.TransformPoint(center);
            Vector3 worldSize = Vector3.Scale(size, transform.lossyScale);

            float minX = worldCenter.x - worldSize.x / 2f;
            float maxX = worldCenter.x + worldSize.x / 2f;
            float minZ = worldCenter.z - worldSize.z / 2f;
            float maxZ = worldCenter.z + worldSize.z / 2f;
            float y = worldCenter.y + worldSize.y / 2f;

            int effectCount = Random.Range(minEffects, maxEffects + 1);
            for (int i = 0; i < effectCount; i++)
            {
                float x = Random.Range(minX, maxX);
                float z = Random.Range(minZ, maxZ);
                Vector3 pos = new Vector3(x, y, z);
                GameObject inst = Instantiate(effectPrefab, pos, Quaternion.identity);

                ParticleSystem ps = inst.GetComponent<ParticleSystem>();
                if (ps != null)
                    Destroy(inst, ps.main.duration + ps.main.startLifetime.constantMax);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }
}
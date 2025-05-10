using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 60f;
    public float fireRate = 0.2f; // Уменьшаем задержку между выстрелами
    private float nextFireTime = 0f;
    private bool canShoot = true;

    void Start()
    {
        // Проверяем наличие необходимых компонентов
        if (firePoint == null)
        {
            Debug.LogError("FirePoint не назначен в PlayerShooting!");
            enabled = false;
            return;
        }

        if (bulletPrefab == null)
        {
            Debug.LogError("BulletPrefab не назначен в PlayerShooting!");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        // Проверяем возможность стрельбы
        if (Time.time >= nextFireTime)
        {
            canShoot = true;
        }

        // Стрельба по левой кнопке мыши или пробелу
        if ((Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space)) && canShoot)
        {
            Shoot();
            canShoot = false;
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        try
        {
            // Создаём пулю с правильной позицией и поворотом
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            
            // Получаем компонент Rigidbody пули
            if (bullet.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                // Настраиваем физику пули
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
                rb.useGravity = false;
                
                // Устанавливаем скорость пули
                rb.linearVelocity = firePoint.forward * bulletSpeed;
            }
            else
            {
                Debug.LogWarning("Пуля не имеет компонента Rigidbody!");
                Destroy(bullet);
                return;
            }

            // Уничтожаем пулю через 2 секунды
            Destroy(bullet, 2f);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка при стрельбе: {e.Message}");
        }
    }
}

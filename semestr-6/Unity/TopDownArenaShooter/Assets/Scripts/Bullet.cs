using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 1f;

    private void OnCollisionEnter(Collision collision)
    {
        // Если попали во врага — наносим урон и уничтожаем пулю
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        // Если попали в стену — уничтожаем пулю
        else if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
        // Если попали в игрока — ничего не делаем (или можно игнорировать)
    }
}

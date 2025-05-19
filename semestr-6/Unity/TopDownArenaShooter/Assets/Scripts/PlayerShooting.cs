using UnityEngine;
using TMPro;

public class PlayerShooting : MonoBehaviour
{
    [Header("Pistol Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float pistolFireRate = 0.5f;
    public float pistolDamage = 1f;

    [Header("Shotgun Settings")]
    public bool isShotgunUnlocked = false;
    public Transform shotgunFirePoint;
    public GameObject shotgunPelletPrefab;
    public int pelletCount = 8;             // сколько пеллет вылетает
    public float shotgunSpreadAngle = 30f;  // угол разброса в градусах
    public float shotgunFireRate = 1.5f;    // задержка между выстрелами дробовиком
    public float shotgunDamage = 0.85f;     // базовый урон дробовика (85% от пистолета)
    public AudioClip shotgunSound;

    [Header("UI")]
    public TextMeshProUGUI damageText;

    private bool useShotgun = false;
    private float nextFireTime = 0f;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        UpdateDamageUI();
    }

    void Update()
    {
        // Переключение оружия (C)
        if (isShotgunUnlocked && Input.GetKeyDown(KeyCode.C))
        {
            useShotgun = !useShotgun;
            UpdateDamageUI();
        }

        // Стрельба
        if (Time.time >= nextFireTime && (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space)))
        {
            if (useShotgun)
                ShootShotgun();
            else
                ShootPistol();
        }
    }

    void ShootPistol()
    {
        nextFireTime = Time.time + pistolFireRate;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        bulletComponent.damage = pistolDamage;
        bullet.GetComponent<Rigidbody>().linearVelocity = firePoint.forward * bulletSpeed;
        Destroy(bullet, 2f);
        audioSource.PlayOneShot(audioSource.clip);
    }

    void ShootShotgun()
    {
        nextFireTime = Time.time + shotgunFireRate;

        for (int i = 0; i < pelletCount; i++)
        {
            float angle = Random.Range(-shotgunSpreadAngle / 2f, shotgunSpreadAngle / 2f);
            Quaternion rot = shotgunFirePoint.rotation * Quaternion.Euler(0, angle, 0);

            GameObject pellet = Instantiate(shotgunPelletPrefab, shotgunFirePoint.position, rot);
            Bullet pelletComponent = pellet.GetComponent<Bullet>();
            pelletComponent.damage = shotgunDamage;
            pellet.GetComponent<Rigidbody>().linearVelocity = rot * Vector3.forward * bulletSpeed;

            Destroy(pellet, 1f);
        }

        if (shotgunSound != null)
            audioSource.PlayOneShot(shotgunSound);
    }

    public void IncreaseDamage(float amount)
    {
        pistolDamage += amount;
        shotgunDamage = pistolDamage * 0.85f; // Обновляем урон дробовика
        UpdateDamageUI();
    }

    void UpdateDamageUI()
    {
        if (damageText != null)
        {
            float currentDamage = useShotgun ? shotgunDamage : pistolDamage;
            string weaponName = useShotgun ? "Дробовик" : "Пистолет";
            damageText.text = $"{weaponName}\nУрон: {currentDamage:F1}";
        }
    }
}

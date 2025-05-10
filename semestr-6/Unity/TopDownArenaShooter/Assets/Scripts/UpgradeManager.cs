using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public GameObject upgradePanel;
    public Button speedButton, damageButton, healthButton;

    private PlayerMovement playerMovement;
    private PlayerShooting playerShooting;
    private PlayerHealth playerHealth;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerShooting = playerMovement.GetComponent<PlayerShooting>();
        playerHealth = playerMovement.GetComponent<PlayerHealth>();

        speedButton.onClick.AddListener(() => ApplyUpgrade("Speed"));
        damageButton.onClick.AddListener(() => ApplyUpgrade("Damage"));
        healthButton.onClick.AddListener(() => ApplyUpgrade("Health"));

        upgradePanel.SetActive(false);
    }

    public void ShowUpgradePanel()
    {
        Time.timeScale = 0f;
        upgradePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ApplyUpgrade(string upgrade)
    {
        switch (upgrade)
        {
            case "Speed":
                playerMovement.moveSpeed += 1f;
                break;
            case "Damage":
                playerShooting.bulletPrefab.GetComponent<Bullet>().damage += 1f;
                break;
            case "Health":
                playerHealth.TakeDamage(-2f); // "Отхил"
                break;
        }

        Time.timeScale = 1f;
        upgradePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public GameObject upgradePanel;
    public Button speedButton, damageButton, healthButton;
    public TextMeshProUGUI speedPriceText, damagePriceText, healthPriceText;

    private PlayerMovement playerMovement;
    private PlayerShooting playerShooting;
    private PlayerHealth playerHealth;

    // Цены и уровни
    private int speedLevel = 0, damageLevel = 0, healthLevel = 0;
    private int speedPrice = 10, damagePrice = 10, healthPrice = 10;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerShooting = playerMovement.GetComponent<PlayerShooting>();
        playerHealth = playerMovement.GetComponent<PlayerHealth>();

        speedButton.onClick.AddListener(() => BuyUpgrade("Speed"));
        damageButton.onClick.AddListener(() => BuyUpgrade("Damage"));
        healthButton.onClick.AddListener(() => BuyUpgrade("Health"));

        upgradePanel.SetActive(false);
        UpdatePriceTexts();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (upgradePanel.activeSelf)
                CloseUpgradePanel();
            else
                OpenUpgradePanel();
        }
    }

    void OpenUpgradePanel()
    {
        Time.timeScale = 0f;
        upgradePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UpdatePriceTexts();
    }

    void CloseUpgradePanel()
    {
        Time.timeScale = 1f;
        upgradePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void BuyUpgrade(string upgrade)
    {
        switch (upgrade)
        {
            case "Speed":
                if (ScoreManager.Instance.SpendScore(speedPrice))
                {
                    playerMovement.moveSpeed += 1f;
                    speedLevel++;
                    speedPrice = Mathf.CeilToInt(speedPrice * 1.2f);
                }
                break;
            case "Damage":
                if (ScoreManager.Instance.SpendScore(damagePrice))
                {
                    playerShooting.bulletPrefab.GetComponent<Bullet>().damage += 1f;
                    damageLevel++;
                    damagePrice = Mathf.CeilToInt(damagePrice * 1.2f);
                }
                break;
            case "Health":
                if (ScoreManager.Instance.SpendScore(healthPrice))
                {
                    playerHealth.TakeDamage(-2f); // "Отхил"
                    healthLevel++;
                    healthPrice = Mathf.CeilToInt(healthPrice * 1.2f);
                }
                break;
        }
        UpdatePriceTexts();
    }

    void UpdatePriceTexts()
    {
        speedPriceText.text = $"Цена: {speedPrice}";
        damagePriceText.text = $"Цена: {damagePrice}";
        healthPriceText.text = $"Цена: {healthPrice}";
    }
}
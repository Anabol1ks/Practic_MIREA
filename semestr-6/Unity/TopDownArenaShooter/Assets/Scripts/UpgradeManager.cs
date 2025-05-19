using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public GameObject upgradePanel;
    public Button speedButton, damageButton, healthButton, shotgunButton;
    public Button flashlightRangeButton, flashlightWidthButton;
    public Button streetLightButton; // Кнопка покупки фонаря
    public TextMeshProUGUI speedPriceText, damagePriceText, healthPriceText, shotgunPriceText;
    public TextMeshProUGUI flashlightRangePriceText, flashlightWidthPriceText;
    public TextMeshProUGUI streetLightPriceText; // Текст цены фонаря

    private PlayerMovement playerMovement;
    private PlayerShooting playerShooting;
    private PlayerHealth playerHealth;
    private PlayerFlashlight playerFlashlight;

    // Цены и уровни
    private int speedLevel = 0, damageLevel = 0, healthLevel = 0;
    private int flashlightRangeLevel = 0, flashlightWidthLevel = 0;
    private int speedPrice = 10, damagePrice = 10, healthPrice = 10, shotgunPrice = 150;
    private int flashlightRangePrice = 15, flashlightWidthPrice = 15;
    private int streetLightPrice = 50; // Цена фонаря

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        playerShooting = playerMovement.GetComponent<PlayerShooting>();
        playerHealth = playerMovement.GetComponent<PlayerHealth>();
        playerFlashlight = playerMovement.GetComponent<PlayerFlashlight>();

        if (playerFlashlight == null)
        {
            playerFlashlight = playerMovement.gameObject.AddComponent<PlayerFlashlight>();
        }

        speedButton.onClick.AddListener(() => BuyUpgrade("Speed"));
        damageButton.onClick.AddListener(() => BuyUpgrade("Damage"));
        healthButton.onClick.AddListener(() => BuyUpgrade("Health"));
        shotgunButton.onClick.AddListener(() => BuyUpgrade("Shotgun"));
        
        // Добавляем обработчики для кнопок улучшения фонарика
        if (flashlightRangeButton != null)
            flashlightRangeButton.onClick.AddListener(() => BuyUpgrade("FlashlightRange"));
        if (flashlightWidthButton != null)
            flashlightWidthButton.onClick.AddListener(() => BuyUpgrade("FlashlightWidth"));

        upgradePanel.SetActive(false);
        UpdatePriceTexts();
        UpdateShotgunButtonState();
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
        UpdateShotgunButtonState();
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
                    playerShooting.IncreaseDamage(2f);
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
            case "Shotgun":
                if (ScoreManager.Instance.SpendScore(shotgunPrice))
                {
                    playerShooting.isShotgunUnlocked = true;
                    shotgunButton.interactable = false;
                    shotgunPriceText.text = "Куплено";
                }
                break;
            case "FlashlightRange":
                if (ScoreManager.Instance.SpendScore(flashlightRangePrice) && playerFlashlight != null)
                {
                    playerFlashlight.maxLightDistance += 2f;
                    flashlightRangeLevel++;
                    flashlightRangePrice = Mathf.CeilToInt(flashlightRangePrice * 1.2f);
                }
                break;
            case "FlashlightWidth":
                if (ScoreManager.Instance.SpendScore(flashlightWidthPrice) && playerFlashlight != null)
                {
                    playerFlashlight.lightAngle += 10f;
                    flashlightWidthLevel++;
                    flashlightWidthPrice = Mathf.CeilToInt(flashlightWidthPrice * 1.2f);
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
        if (!playerShooting.isShotgunUnlocked)
            shotgunPriceText.text = $"Цена: {shotgunPrice}";
            
        // Обновляем тексты цен для улучшений фонарика
        if (flashlightRangePriceText != null)
            flashlightRangePriceText.text = $"Цена: {flashlightRangePrice}";
        if (flashlightWidthPriceText != null)
            flashlightWidthPriceText.text = $"Цена: {flashlightWidthPrice}";
    }

    void UpdateShotgunButtonState()
    {
        if (playerShooting.isShotgunUnlocked)
        {
            shotgunButton.interactable = false;
            shotgunPriceText.text = "Куплено";
        }
    }
}
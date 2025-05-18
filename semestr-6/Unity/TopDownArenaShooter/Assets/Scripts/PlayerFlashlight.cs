using UnityEngine;

public class PlayerFlashlight : MonoBehaviour
{
    public Light spotLight;                    // Компонент Light для фонарика
    public float maxLightDistance = 10f;       // Максимальная дальность света
    public float lightAngle = 60f;             // Угол конуса света
    public KeyCode flashlightToggleKey = KeyCode.F;  // Клавиша для включения/выключения
    public Color lightColor = Color.white;     // Цвет света
    public float lightIntensity = 2f;          // Интенсивность света

    private bool isFlashlightOn = true;        // Состояние фонарика (включен/выключен)

    void Start()
    {
        if (spotLight == null)
        {
            Debug.LogError("SpotLight не назначен! Пожалуйста, назначьте объект Light в инспекторе.");
            return;
        }

        // Настройка света фонарика
        ConfigureLight();
    }

    void Update()
    {
        // Переключение состояния фонарика
        if (Input.GetKeyDown(flashlightToggleKey))
        {
            isFlashlightOn = !isFlashlightOn;
            spotLight.enabled = isFlashlightOn;
        }
    }

    void ConfigureLight()
    {
        if (spotLight != null)
        {
            spotLight.type = LightType.Spot;
            spotLight.spotAngle = lightAngle;
            spotLight.range = maxLightDistance;
            spotLight.color = lightColor;
            spotLight.intensity = lightIntensity;
            spotLight.enabled = isFlashlightOn;

            // Дополнительные настройки для лучшего качества света
            spotLight.shadows = LightShadows.Hard;
            spotLight.renderMode = LightRenderMode.ForcePixel;
        }
    }
}
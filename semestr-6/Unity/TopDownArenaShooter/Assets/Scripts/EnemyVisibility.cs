using UnityEngine;

public class EnemyVisibility : MonoBehaviour
{
    private Renderer[] renderers;
    private FogOfWar fogOfWar;
    private StreetLightManager streetLightManager;
    private bool wasVisible = false;
    
    // Визуальный эффект при обнаружении
    public float fadeSpeed = 2f;
    private float currentAlpha = 0f;
    private Color originalColor;
    
    [System.Obsolete]
    void Start()
    {
        // Получаем все рендереры объекта
        renderers = GetComponentsInChildren<Renderer>();
        
        // Сохраняем оригинальный цвет
        if (renderers.Length > 0)
        {
            originalColor = renderers[0].material.color;
        }
        
        // Получаем ссылку на систему тумана войны
        fogOfWar = FogOfWar.Instance;
        if (fogOfWar == null)
        {
            fogOfWar = FindObjectOfType<FogOfWar>();
        }
        
        // Получаем ссылку на менеджер уличных фонарей
        streetLightManager = StreetLightManager.Instance;
        if (streetLightManager == null)
        {
            streetLightManager = FindObjectOfType<StreetLightManager>();
        }
        
        // Делаем врага изначально невидимым
        SetVisibility(false);
    }
    
    [System.Obsolete]
    void Update()
    {
        if (fogOfWar == null)
        {
            fogOfWar = FindObjectOfType<FogOfWar>();
            if (fogOfWar == null) return;
        }
        
        // Проверяем, находится ли враг в зоне действия фонарика игрока
        bool isInPlayerLight = fogOfWar.IsInLightArea(transform.position);
        
        // Проверяем, находится ли враг в зоне действия уличного фонаря
        bool isInStreetLight = false;
        if (streetLightManager != null && streetLightManager.affectEnemyVisibility)
        {
            isInStreetLight = streetLightManager.IsPositionInAnyLightArea(transform.position);
        }
        
        // Враг виден, если он в зоне действия фонарика игрока ИЛИ в зоне уличного фонаря
        bool isVisible = isInPlayerLight || isInStreetLight;
        
        // Если изменилось состояние видимости, обновляем отображение
        if (isVisible != wasVisible)
        {
            wasVisible = isVisible;
            
            // Если стал видимым, начинаем плавное появление
            if (isVisible)
            {
                // Сразу делаем его полупрозрачным
                SetVisibility(true, 0.2f);
            }
        }
        
        // Плавная анимация появления/исчезновения
        if (isVisible && currentAlpha < 1f)
        {
            currentAlpha += fadeSpeed * Time.deltaTime;
            if (currentAlpha > 1f) currentAlpha = 1f;
            UpdateAlpha(currentAlpha);
        }
        else if (!isVisible && currentAlpha > 0f)
        {
            currentAlpha -= fadeSpeed * Time.deltaTime;
            if (currentAlpha < 0f)
            {
                currentAlpha = 0f;
                SetVisibility(false);
            }
            else
            {
                UpdateAlpha(currentAlpha);
            }
        }
    }
    
    // Метод для установки видимости всех рендереров
    void SetVisibility(bool visible, float alpha = 0f)
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = visible;
            
            if (visible)
            {
                Material mat = renderer.material;
                Color color = mat.color;
                color.a = alpha;
                mat.color = color;
            }
        }
        
        currentAlpha = alpha;
    }
    
    // Метод для обновления прозрачности
    void UpdateAlpha(float alpha)
    {
        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            Color color = mat.color;
            color.a = alpha;
            mat.color = color;
        }
    }
} 
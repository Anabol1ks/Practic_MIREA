using UnityEngine;

public class StreetLight : MonoBehaviour
{
    [Header("Параметры света")]
    public Light spotLight;                    // Компонент Light для фонаря
    public float lightDistance = 15f;          // Максимальная дальность света
    public float lightAngle = 60f;             // Угол конуса света
    public Color lightColor = Color.yellow;    // Цвет света (желтый для отличия от фонарика игрока)
    public float lightIntensity = 2.5f;        // Интенсивность света
    
    [Header("Отображение в редакторе")]
    public float gizmoSize = 0.5f;             // Размер отображения в редакторе
    public Color gizmoColor = Color.yellow;    // Цвет отображения в редакторе
    
    private FogOfWar fogOfWar;                 // Ссылка на систему тумана войны
    
    void Start()
    {
        // Инициализация и настройка света
        InitializeLight();
        
        // Находим FogOfWar
        fogOfWar = FindObjectOfType<FogOfWar>();
        
        // Регистрируем фонарь в менеджере фонарей (если он есть)
        StreetLightManager manager = FindObjectOfType<StreetLightManager>();
        if (manager != null)
        {
            manager.RegisterStreetLight(this);
        }
    }
    
    // Инициализация и настройка света
    void InitializeLight()
    {
        // Если компонент Light не указан, попробуем найти его или создать
        if (spotLight == null)
        {
            spotLight = GetComponentInChildren<Light>();
            
            if (spotLight == null)
            {
                GameObject lightObj = new GameObject("StreetLightSpot");
                lightObj.transform.SetParent(transform);
                lightObj.transform.localPosition = Vector3.zero; // Центрируем
                lightObj.transform.localRotation = Quaternion.identity;
                
                spotLight = lightObj.AddComponent<Light>();
            }
        }
        
        // Настройка параметров света
        ConfigureLight();
    }
    
    // Настройка параметров света
    void ConfigureLight()
    {
        if (spotLight != null)
        {
            spotLight.type = LightType.Spot;
            spotLight.spotAngle = lightAngle;
            spotLight.range = lightDistance;
            spotLight.color = lightColor;
            spotLight.intensity = lightIntensity;
            spotLight.enabled = true;
            
            // Дополнительные настройки для качества света
            spotLight.shadows = LightShadows.Hard;
            spotLight.renderMode = LightRenderMode.ForcePixel;
        }
    }
    
    // Метод для определения, находится ли точка в конусе света фонаря
    public bool IsInLightArea(Vector3 position)
    {
        if (spotLight == null)
            return false;
            
        // Направление от фонаря к указанной позиции
        Vector3 dirToPos = (position - transform.position).normalized;
        
        // Направление света фонаря
        Vector3 lightDirection = spotLight.transform.forward;
        
        // Угол между направлением света и направлением к позиции
        float angle = Vector3.Angle(lightDirection, dirToPos);
        
        // Расстояние от фонаря до позиции
        float distance = Vector3.Distance(transform.position, position);
        
        // Проверяем, находится ли точка в конусе света и в пределах дальности
        return angle <= lightAngle / 2 && distance <= lightDistance;
    }
    
    // Отображение в редакторе
    void OnDrawGizmos()
    {
        // Сохраняем текущий цвет
        Color prevColor = Gizmos.color;
        Gizmos.color = gizmoColor;
        
        // Рисуем сферу для обозначения позиции фонаря
        Gizmos.DrawSphere(transform.position, gizmoSize);
        
        // Если есть компонент Light, отображаем направление света
        if (spotLight != null)
        {
            Vector3 direction = spotLight.transform.forward;
            
            // Рисуем линию в направлении света
            Gizmos.DrawRay(transform.position, direction * 2f);
            
            // Рисуем конус света (упрощенно, через несколько линий)
            float halfAngle = lightAngle * 0.5f * Mathf.Deg2Rad;
            Vector3 coneDirection = direction.normalized * lightDistance;
            
            // Вычисляем перпендикулярные векторы для построения конуса
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;
            if (perpendicular.magnitude < 0.001f)
                perpendicular = Vector3.Cross(direction, Vector3.right).normalized;
            
            Vector3 up = Vector3.Cross(perpendicular, direction).normalized;
            
            // Рисуем линии, изображающие конус
            for (int i = 0; i < 8; i++)
            {
                float angle = i * Mathf.PI / 4f;
                Vector3 dir = Quaternion.AngleAxis(lightAngle / 2, perpendicular * Mathf.Cos(angle) + up * Mathf.Sin(angle)) * direction;
                Gizmos.DrawRay(transform.position, dir.normalized * lightDistance);
            }
        }
        
        // Восстанавливаем предыдущий цвет
        Gizmos.color = prevColor;
    }
} 
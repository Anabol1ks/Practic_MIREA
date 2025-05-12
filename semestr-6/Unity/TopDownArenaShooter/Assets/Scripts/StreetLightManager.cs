using UnityEngine;
using System.Collections.Generic;

public class StreetLightManager : MonoBehaviour
{
    public static StreetLightManager Instance;
    
    [Header("Настройки")]
    public bool findLightsAtStart = true;        // Найти все фонари при старте
    public bool affectEnemyVisibility = true;    // Влияют ли фонари на видимость врагов
    
    // Список всех уличных фонарей на сцене
    private List<StreetLight> streetLights = new List<StreetLight>();
    private FogOfWar fogOfWar;
    
    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        // Поиск системы тумана войны
        fogOfWar = FindObjectOfType<FogOfWar>();
        
        // Если нужно, находим все уличные фонари на сцене
        if (findLightsAtStart)
        {
            FindAllStreetLights();
        }
    }
    
    // Поиск всех фонарей на сцене
    public void FindAllStreetLights()
    {
        streetLights.Clear();
        StreetLight[] lights = FindObjectsOfType<StreetLight>();
        
        foreach (StreetLight light in lights)
        {
            RegisterStreetLight(light);
        }
        
        Debug.Log($"Найдено уличных фонарей: {streetLights.Count}");
    }
    
    // Регистрация уличного фонаря
    public void RegisterStreetLight(StreetLight streetLight)
    {
        if (!streetLights.Contains(streetLight))
        {
            streetLights.Add(streetLight);
        }
    }
    
    // Удаление уличного фонаря из системы
    public void UnregisterStreetLight(StreetLight streetLight)
    {
        if (streetLights.Contains(streetLight))
        {
            streetLights.Remove(streetLight);
        }
    }
    
    // Проверка, находится ли позиция в зоне освещения любого уличного фонаря
    public bool IsPositionInAnyLightArea(Vector3 position)
    {
        foreach (StreetLight light in streetLights)
        {
            if (light.IsInLightArea(position))
            {
                return true;
            }
        }
        
        return false;
    }
    
    // Включение всех фонарей
    public void EnableAllLights()
    {
        foreach (StreetLight light in streetLights)
        {
            if (light.spotLight != null)
            {
                light.spotLight.enabled = true;
            }
        }
    }
    
    // Выключение всех фонарей
    public void DisableAllLights()
    {
        foreach (StreetLight light in streetLights)
        {
            if (light.spotLight != null)
            {
                light.spotLight.enabled = false;
            }
        }
    }
    
    // Получение ближайшего фонаря к указанной позиции
    public StreetLight GetNearestStreetLight(Vector3 position)
    {
        StreetLight nearest = null;
        float minDistance = float.MaxValue;
        
        foreach (StreetLight light in streetLights)
        {
            float distance = Vector3.Distance(position, light.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = light;
            }
        }
        
        return nearest;
    }
} 
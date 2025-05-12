using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SpawnZoneManager : MonoBehaviour
{
    public static SpawnZoneManager Instance;
    
    [Header("Настройки зон")]
    public List<SpawnZone> allSpawnZones = new List<SpawnZone>();
    public bool activateAllZonesAtStart = true;
    
    [Header("Прогрессивная активация зон")]
    public bool useProgressiveActivation = false;
    public float timeToActivateAllZones = 300f; // Время в секундах до активации всех зон
    
    private Dictionary<SpawnZone, bool> initialStateByZone = new Dictionary<SpawnZone, bool>();
    private float gameTime = 0f;
    
    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        // Если не заданы зоны, находим их автоматически
        if (allSpawnZones.Count == 0)
        {
            allSpawnZones.AddRange(FindObjectsOfType<SpawnZone>());
            Debug.Log($"Автоматически найдено зон спавна: {allSpawnZones.Count}");
        }
        
        // Сохраняем начальное состояние всех зон
        foreach (var zone in allSpawnZones)
        {
            initialStateByZone[zone] = zone.isActiveZone;
            
            // Если нужно активировать все зоны при старте
            if (activateAllZonesAtStart)
            {
                zone.isActiveZone = true;
            }
        }
    }
    
    void Update()
    {
        if (useProgressiveActivation)
        {
            gameTime += Time.deltaTime;
            UpdateZonesByProgress();
        }
    }
    
    // Активирует или деактивирует зоны в зависимости от прогресса игры
    void UpdateZonesByProgress()
    {
        if (allSpawnZones.Count == 0) return;
        
        // Рассчитываем прогресс (от 0 до 1)
        float progress = Mathf.Clamp01(gameTime / timeToActivateAllZones);
        
        // Рассчитываем, сколько зон должно быть активно
        int zonesToActivate = Mathf.CeilToInt(progress * allSpawnZones.Count);
        
        // Сортируем зоны по расстоянию от базы (чтобы сначала активировать дальние)
        var sortedZones = allSpawnZones.OrderBy(z => {
            GameObject baseObj = GameObject.FindGameObjectWithTag("PlayerBase");
            if (baseObj != null)
                return Vector3.Distance(z.transform.position, baseObj.transform.position);
            return 0f;
        }).ToList();
        
        // Активируем нужные зоны
        for (int i = 0; i < sortedZones.Count; i++)
        {
            sortedZones[i].isActiveZone = (i < zonesToActivate);
        }
    }
    
    // Активирует все зоны
    public void ActivateAllZones()
    {
        foreach (var zone in allSpawnZones)
        {
            zone.isActiveZone = true;
        }
    }
    
    // Деактивирует все зоны
    public void DeactivateAllZones()
    {
        foreach (var zone in allSpawnZones)
        {
            zone.isActiveZone = false;
        }
    }
    
    // Возвращает список активных зон
    public List<SpawnZone> GetActiveZones()
    {
        return allSpawnZones.Where(z => z.isActiveZone).ToList();
    }
    
    // Сбрасывает зоны к исходному состоянию
    public void ResetToInitialState()
    {
        foreach (var zone in allSpawnZones)
        {
            if (initialStateByZone.ContainsKey(zone))
                zone.isActiveZone = initialStateByZone[zone];
        }
    }
    
    // Активирует указанную зону
    public void ActivateZone(SpawnZone zone)
    {
        if (allSpawnZones.Contains(zone))
            zone.isActiveZone = true;
    }
    
    // Деактивирует указанную зону
    public void DeactivateZone(SpawnZone zone)
    {
        if (allSpawnZones.Contains(zone))
            zone.isActiveZone = false;
    }
} 
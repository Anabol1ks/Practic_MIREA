using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    public static FogOfWar Instance;

    public Material fogOfWarMaterial;           // Материал для тумана войны
    public Transform player;                   // Трансформ игрока
    public PlayerFlashlight playerFlashlight;   // Ссылка на компонент фонарика игрока
    public LayerMask fogOfWarLayer;            // Слой для объектов, затрагиваемых туманом войны
    
    [Range(0.0f, 1.0f)]
    public float globalDarkness = 0.9f;         // Глобальное затемнение (0 - полная прозрачность, 1 - полная темнота)
    
    private Camera mainCamera;
    
    void Awake()
    {
        Instance = this;
        mainCamera = Camera.main;
        
        // Если игрок не задан, найдем его
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerFlashlight = playerObj.GetComponent<PlayerFlashlight>();
                
                // Если на игроке нет компонента фонарика, добавим его
                if (playerFlashlight == null)
                {
                    playerFlashlight = playerObj.AddComponent<PlayerFlashlight>();
                }
            }
        }
        
        // Проверяем материал для тумана войны
        if (fogOfWarMaterial == null)
        {
            Debug.LogError("Материал для тумана войны не назначен!");
        }
        
        // Применяем параметры к материалу
        UpdateFogOfWarMaterial();
    }
    
    void Update()
    {
        UpdateFogOfWarMaterial();
    }
    
    void UpdateFogOfWarMaterial()
    {
        if (fogOfWarMaterial != null && player != null && playerFlashlight != null && playerFlashlight.spotLight != null)
        {
            // Передаем позицию игрока в материал
            fogOfWarMaterial.SetVector("_PlayerPosition", player.position);
            
            // Передаем направление взгляда игрока (фонарика)
            fogOfWarMaterial.SetVector("_PlayerForward", player.forward);
            
            // Передаем параметры фонарика
            fogOfWarMaterial.SetFloat("_LightAngle", playerFlashlight.lightAngle);
            fogOfWarMaterial.SetFloat("_LightDistance", playerFlashlight.maxLightDistance);
            fogOfWarMaterial.SetFloat("_GlobalDarkness", globalDarkness);
            fogOfWarMaterial.SetFloat("_LightIntensity", playerFlashlight.spotLight.enabled ? playerFlashlight.lightIntensity : 0);
        }
    }
    
    // Метод для определения, находится ли объект в видимой зоне фонарика
    public bool IsInLightArea(Vector3 position)
    {
        if (player == null || playerFlashlight == null || !playerFlashlight.spotLight.enabled)
            return false;
            
        Vector3 dirToPos = (position - player.position).normalized;
        float angle = Vector3.Angle(player.forward, dirToPos);
        float distance = Vector3.Distance(player.position, position);
        
        return angle <= playerFlashlight.lightAngle / 2 && distance <= playerFlashlight.maxLightDistance;
    }
} 
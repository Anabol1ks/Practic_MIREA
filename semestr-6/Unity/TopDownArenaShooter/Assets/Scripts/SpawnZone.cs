using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    public float gizmoSize = 1f;                  // Размер отображения зоны в редакторе
    public Color gizmoColor = Color.red;          // Цвет зоны в редакторе
    public bool isActiveZone = true;              // Активна ли эта зона
    public float minDistanceFromBase = 15f;       // Минимальное расстояние от базы для спавна врагов
    
    private Transform playerBase;                 // Ссылка на базу игрока
    
    void Start()
    {
        // Находим базу игрока
        GameObject baseObj = GameObject.FindGameObjectWithTag("PlayerBase");
        if (baseObj != null)
        {
            playerBase = baseObj.transform;
        }
    }
    
    // Получить случайную позицию внутри этой зоны
    public Vector3 GetRandomPosition()
    {
        // Используем коллайдер для определения размеров зоны
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError("Зона спавна должна иметь коллайдер!", this);
            return transform.position;
        }
        
        // Получаем границы коллайдера
        Bounds bounds = collider.bounds;
        
        // Генерируем случайную позицию внутри границ
        Vector3 randomPos = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            0f, // Устанавливаем Y = 0 для плоской поверхности
            Random.Range(bounds.min.z, bounds.max.z)
        );
        
        // Проверяем расстояние до базы, если она существует
        if (playerBase != null && minDistanceFromBase > 0)
        {
            int attempts = 20; // Максимальное количество попыток найти подходящую позицию
            
            while (attempts > 0 && Vector3.Distance(randomPos, playerBase.position) < minDistanceFromBase)
            {
                // Пробуем другую позицию
                randomPos = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    0f,
                    Random.Range(bounds.min.z, bounds.max.z)
                );
                attempts--;
            }
            
            // Если не смогли найти подходящую позицию, возвращаем крайнюю точку за пределами minDistanceFromBase
            if (attempts == 0)
            {
                Vector3 dirFromBase = (randomPos - playerBase.position).normalized;
                randomPos = playerBase.position + dirFromBase * minDistanceFromBase;
            }
        }
        
        return randomPos;
    }
    
    // Отображение зоны в редакторе Unity
    void OnDrawGizmos()
    {
        // Сохраняем предыдущий цвет и меняем на наш
        Color prevColor = Gizmos.color;
        Gizmos.color = gizmoColor;
        
        // Получаем коллайдер
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            // Отображаем провязку с коллайдером
            Bounds bounds = collider.bounds;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
            
            // Отображаем метку в центре
            Gizmos.DrawSphere(transform.position, gizmoSize);
        }
        else
        {
            // Если коллайдера нет, просто рисуем сферу
            Gizmos.DrawSphere(transform.position, gizmoSize);
        }
        
        // Восстанавливаем предыдущий цвет
        Gizmos.color = prevColor;
    }
} 
using UnityEngine;

public class PR2 : MonoBehaviour
{
    public float RandomNumber = 7;
    public GameObject SomeObject;
    public Transform[] waypoints;       // Массив точек перемещения
    public float movementSpeed = 5f;    // Скорость перемещения

    private int currentWaypointIndex = 0;
    private bool isMoving = false;

    void Update()
    {
        Randomizer();

        if(isMoving)
        {
            MoveObject();
        }
    }

    void Randomizer()
    {
        RandomNumber = Random.Range(1, 10);
        Debug.Log(RandomNumber);

        // Активируем движение только если не движемся и выпало 5
        if(RandomNumber == 5 && !isMoving)
        {
            Debug.Log("Начинаем движение");
            isMoving = true;
            currentWaypointIndex = 0; // Сброс к начальной точке
        }
    }

    void MoveObject()
    {
        // Проверка наличия объекта и точек
        if(SomeObject == null || waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError("Настройки перемещения не верны!");
            isMoving = false;
            return;
        }

        // Получаем текущую целевую позицию
        Vector3 targetPosition = waypoints[currentWaypointIndex].position;
        
        // Перемещаем объект
        SomeObject.transform.position = Vector3.MoveTowards(
            SomeObject.transform.position,
            targetPosition,
            movementSpeed * Time.deltaTime
        );

        // Проверяем достижение точки
        if(Vector3.Distance(SomeObject.transform.position, targetPosition) < 0.001f)
        {
            currentWaypointIndex++;
            
            // Проверяем завершение маршрута
            if(currentWaypointIndex >= waypoints.Length)
            {
                isMoving = false;
                Debug.Log("Маршрут завершен");
            }
        }
    }
}
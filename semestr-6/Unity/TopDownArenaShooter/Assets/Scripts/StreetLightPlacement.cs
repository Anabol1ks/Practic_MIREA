using UnityEngine;

public class StreetLightPlacement : MonoBehaviour
{
    public static StreetLightPlacement Instance;

    public GameObject streetLightPrefab; // Префаб уличного фонаря
    private GameObject currentStreetLight; // Текущий фонарь для установки
    private bool isPlacing = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isPlacing && currentStreetLight != null)
        {
            // Перемещаем фонарь за курсором мыши
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                currentStreetLight.transform.position = hit.point;
            }

            // Вращение фонаря
            if (Input.GetKey(KeyCode.Q))
                currentStreetLight.transform.Rotate(Vector3.up, -1f);
            if (Input.GetKey(KeyCode.E))
                currentStreetLight.transform.Rotate(Vector3.up, 1f);

            // Установка фонаря
            if (Input.GetMouseButtonDown(0)) // ЛКМ для установки
            {
                isPlacing = false;
                currentStreetLight = null;
            }

            // Отмена установки
            if (Input.GetMouseButtonDown(1)) // ПКМ для отмены
            {
                Destroy(currentStreetLight);
                isPlacing = false;
            }
        }
    }

    public void StartPlacement()
    {
        if (streetLightPrefab != null)
        {
            isPlacing = true;
            currentStreetLight = Instantiate(streetLightPrefab);
        }
    }
}
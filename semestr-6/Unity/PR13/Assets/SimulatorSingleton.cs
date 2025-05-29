using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

public class SimulatorSingleton : MonoBehaviour
{
    void Awake()
    {
        // Используем новый метод с параметрами
        var sims = FindObjectsByType<XRDeviceSimulator>(
            FindObjectsInactive.Exclude, // Игнорируем неактивные объекты
            FindObjectsSortMode.None     // Отключаем сортировку для скорости
        );

        if (sims.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
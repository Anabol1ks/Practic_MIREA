using UnityEngine;


public class PedestalChecker : MonoBehaviour
{
    public GameObject correctObject; // Объект, который должен быть размещен
    public GameObject door; // Дверь для открытия
    private static int correctPlacements = 0;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == correctObject)
        {
            correctPlacements++;
        
            if (correctPlacements >= 3)
            {
                door.SetActive(false); // Открыть дверь
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == correctObject)
        {
            correctPlacements--;

            if (correctPlacements < 3)
            {
                door.SetActive(true); // Закрыть дверь
            }
        }
    }
}
using UnityEngine;
public class LevelComplete : MonoBehaviour
{
    public GameObject completionIndicator;

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FinalItem"))
        {
            completionIndicator.SetActive(false);
        }
    }
}
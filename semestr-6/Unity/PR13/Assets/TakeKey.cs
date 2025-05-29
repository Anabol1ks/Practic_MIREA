using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TakeKey : MonoBehaviour
{
    private void Start()
    {
        GetComponent<XRSimpleInteractable>().selectEntered.AddListener(args => 
        {
            // Выключаем ключ
            gameObject.SetActive(false);
            Debug.Log("ЗАБРАЛ КЛЮЧ");
        });
    }
}
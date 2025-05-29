using UnityEngine;

public class ChestOpen : MonoBehaviour
{
    public GameObject itemInside;
    public GameObject chest;
    private bool isOpened = false;


    private void Start()
    {
        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        interactable.selectEntered.AddListener((args) => OpenChest());
    }


    private void OpenChest()
    {
        if (!isOpened)
        {
            itemInside.SetActive(true);
            chest.SetActive(false);
            isOpened = true;
        }
    }
}
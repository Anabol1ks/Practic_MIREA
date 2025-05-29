using UnityEngine;


public class ButtonPress : MonoBehaviour
{
    public int buttonId;
    public ButtonSequence sequenceManager;

    private void Start()
    {
        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        interactable.selectEntered.AddListener((args) => sequenceManager.OnButtonPressed(buttonId));
    }
}

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class ButtonSequence : MonoBehaviour
{
    public GameObject door;
    private int[] correctSequence = { 0, 1, 2 }; // Красная, синяя, зеленая
    private int[] currentSequence = new int[3];
    private int currentIndex = 0;


    public void OnButtonPressed(int buttonId)
    {
        currentSequence[currentIndex] = buttonId;
        currentIndex++;
        Debug.Log("НАЖАЛ КНОПКУ");
        if (currentIndex == 3)
        {
            bool isCorrect = true;
            for (int i = 0; i < 3; i++)
            {
                if (currentSequence[i] != correctSequence[i])
                {
                    isCorrect = false;
                    break;
                }
            }
            if (isCorrect)
            {
                door.SetActive(false);
            }
            else
            {
                currentIndex = 0; // Сброс при ошибке
            }
        }
    }
}

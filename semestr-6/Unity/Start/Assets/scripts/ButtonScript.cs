using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ButtonScript : MonoBehaviour
{
    public GameObject gameObjectPanel;
    public GameObject gameObjectImage;
    public Sprite Newsprite;

    public void Close(){
        gameObjectPanel.SetActive(false);  
    }

    public void ChangeImage(){
        gameObjectImage.GetComponent<Image>().sprite = Newsprite;
    }
}

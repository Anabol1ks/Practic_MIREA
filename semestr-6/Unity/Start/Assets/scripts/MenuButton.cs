using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public void StartScene(){
        Application.LoadLevel("PR3");
    }

    public void Options(GameObject window){
        window.SetActive(true);
    }

    public void Quit(){
        Application.Quit();
    }
}

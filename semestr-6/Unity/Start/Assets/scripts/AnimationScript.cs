using UnityEngine;

public class AnimationScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            anim.SetTrigger("JumpTrigger");
        }
    }
}

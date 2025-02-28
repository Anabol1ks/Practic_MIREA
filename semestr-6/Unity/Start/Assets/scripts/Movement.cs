using UnityEngine;

public class Movement : MonoBehaviour
{   

    public CharacterController characterController;

    public float speed = 12f;

    public float gravity = -2f;

    Vector3 velocity;


    public Transform GroundCheck;

    public float groundDistance = 0.4f;

    public LayerMask groundMask;

    bool isGrounded;

    public float jumpForse = 1000;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        characterController.Move(move * speed * Time.deltaTime);

        
        velocity.y += gravity + Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime/2);


        isGrounded = Physics.CheckSphere(GroundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0){
            velocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded) {
            velocity.y = Mathf.Sqrt(jumpForse * -2f * gravity);
        }
    }
}

using Unity.Netcode; 
using UnityEngine; 
public class PlayerNetwork : NetworkBehaviour 
{ 
    void Update() 
    { 
    if (!IsOwner) return;
        Move(); 
    } 
    private void Move(){ 
        Vector3 moveDirection = new Vector3(0,0,0); 
        if (Input.GetKey(KeyCode.W)) moveDirection.z= +1f; 
        if (Input.GetKey(KeyCode.A)) moveDirection.x= -1f; 
        if (Input.GetKey(KeyCode.S)) moveDirection.z= -1f; 
        if (Input.GetKey(KeyCode.D)) moveDirection.x= +1f; 
        float moveSpeed = 10; 
        transform.position += moveSpeed * Time.deltaTime * moveDirection; 
    } 
}
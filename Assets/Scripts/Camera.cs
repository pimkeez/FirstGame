using UnityEngine;
using UnityEngine.InputSystem;

public class Camera : MonoBehaviour
{
    public float boundX = 0.15f;
    private float ahead = 0.5f;
    private float total = 0f; 
    private static GameObject otherObj;
    private float input;
    private Transform playerTransform;
    
    
   
    void Awake()
    {
        otherObj = GameObject.Find("Ayla"); 
       // input = otherObj.GetComponent<PlayerMovement>().input; 
        playerTransform = GameObject.FindWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        Vector3 delta = Vector3.zero;

        float deltaX = playerTransform.position.x - transform.position.x;
        if (deltaX > boundX || deltaX < -boundX)
        {
            if (transform.position.x < playerTransform.position.x)
            {
                delta.x = deltaX - boundX;
            }
            else
            {
                delta.x = deltaX + boundX;
            }
        }
        // if (input < 0)
        // {
        //     total = playerTransform.position.x + delta.x - ahead; 
        // }
        // else
        // {
        //     total = playerTransform.position.x + delta.x + ahead; 
        // }
        
        transform.position += new Vector3(delta.x, 0, 0);
    }
}
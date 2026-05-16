using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Animator _animator;
    private Vector2 movement; 
    float input; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float input = Input.GetAxisRaw("Horizontal");
        movement.x = input * speed * Time.deltaTime; 
        transform.Translate(movement);
        if (input != 0)
        {
            _animator.SetBool("isRunning", true); 
        }
        else {_animator.SetBool("isRunning", false);}
    }
    
}

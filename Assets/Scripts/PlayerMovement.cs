using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    void Awake()
    {
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private UnityEngine.Vector2 movement; 
    public bool isLeftFacing = true; 
    public float input; 

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
        if (input <= 0)
        {
            isLeftFacing = true;
        }
        else
        {
            isLeftFacing = false;
        }
        flipSprite();
    }

    void flipSprite()
    {
        // if (input==0)
        // {
        //     _spriteRenderer.flipX = false; 
        //     return; 
        // }
        if (isLeftFacing)
        {
            _spriteRenderer.flipX = false; 
        }
        else
        {
            _spriteRenderer.flipX = true; 
        }
    }
    
}

using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    GameProgression GameProgressionInstance;
    private Transform _transform;
    private RuntimeAnimatorController originalController;
    private AnimatorOverrideController casualOverride;
    private AnimationClip casualIdol;
    private AnimationClip casualWalk;

    void Awake()
    {
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_animator == null)
            _animator = GetComponent<Animator>();

        _transform = transform;
        GameProgressionInstance = Object.FindFirstObjectByType<GameProgression>();
        casualIdol = Resources.Load<AnimationClip>("Animation/_CasualPlayerIdol");
        casualWalk = Resources.Load<AnimationClip>("Animation/_CasualPlayerWalk");
        originalController = _animator != null ? _animator.runtimeAnimatorController : null;
        if (originalController != null && casualIdol != null && casualWalk != null)
        {   
            Debug.Log("Creating animator override controller for casual animations");
            casualOverride = new AnimatorOverrideController(originalController);
            casualOverride["_PJplayerIdol"] = casualIdol; // replace "Idle" with the original clip name from your controller
            casualOverride["_PJplayerWalk"] = casualWalk; // replace "Walk" with the original clip name from your controller
        }
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
        input = Input.GetAxisRaw("Horizontal");
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
        FlipSprite();
    }

    public void SceneMovement(Vector2 offset, string locationName)
    {
        
        _transform.position += (Vector3)offset;
        GameProgressionInstance.changeScene(locationName);

        if (_animator == null)
            return;

        if (GameProgression.currentScene == "SchoolHallway")
        {
            if (casualOverride != null)
            {
                Debug.Log("Switching to casual animator controller for SchoolHallway");
                _animator.runtimeAnimatorController = casualOverride;
                _animator.Rebind();
                _animator.SetBool("isRunning", false);
                _animator.Play("_playerIdol", 0, 0f); // make sure this state exists in the controller
            }
        }
        else
        {
            if (originalController != null)
            {
                _animator.runtimeAnimatorController = originalController;
                _animator.Rebind();
                _animator.SetBool("isRunning", false);
                _animator.Play("_playerIdol", 0, 0f);
            }
        }
    }

    public void FlipSprite()
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

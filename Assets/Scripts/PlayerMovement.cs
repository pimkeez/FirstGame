using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private static WaitForSeconds waitForSeconds0_2 = new WaitForSeconds(0.2f);
    [SerializeField] private float speed = 5f;
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    GameProgression GameProgressionInstance;
    private Transform _transform;
    private RuntimeAnimatorController originalController;
    private AnimatorOverrideController casualOverride;
    private AnimationClip casualIdol;
    private AnimationClip casualWalk;

    private Vector3 schoolHallwayDoor; 
    private Vector3 bedroomDoor; 
    private Vector3 offset;


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

        schoolHallwayDoor = GameObject.Find("SchoolHallwayDoor").transform.position;
        bedroomDoor = GameObject.Find("BedroomDoor").transform.position;

        offset.x = 10; 
    }
    private Vector2 movement; 
    public bool isLeftFacing = true; 
    public float input; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameData.dialogueActive && !GameData.dialogueOptionActive) {
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
    }

    // SceneMovement for walking into door
    public void SceneMovement(Vector2 offset, string locationName)
    {
        
        _transform.position += (Vector3)offset;
        // Ensure the coroutine is started on the GameProgression instance
        if (GameProgressionInstance != null)
        {
            GameProgressionInstance.StartCoroutine(GameProgressionInstance.ChangeScene(locationName));
        }
        AnimationChange();
    }

    // SceneMovement for pressing on things 
    public void SceneMovementClick(string locationName) {
        if (locationName == "SchoolHallway") {
            // float yOffset = schoolHallwayDoor.y - movement.y;
            
            Vector3 schoolOffset = new Vector3(0f,2f,0f);
            _transform.position = schoolHallwayDoor - offset - schoolOffset; 
        }
        if (locationName == "Bedroom") {
            _transform.position = bedroomDoor + offset; 
        }
        AnimationChange();
    }

    public void TimeTransition(string locationName) {
        SceneMovementClick(locationName);
    }

    public void AnimationChange() {
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

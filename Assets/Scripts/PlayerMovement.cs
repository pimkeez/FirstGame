
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private static WaitForSeconds waitForSeconds0_2 = new WaitForSeconds(0.2f);
    [SerializeField] private float speed = 5f; //it was 5
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private BlackScreenR blackScreenRInstance;
    GameProgression GameProgressionInstance;
    private Transform _transform;
    private RuntimeAnimatorController originalController;
    private AnimatorOverrideController casualOverride;
    private AnimationClip casualIdol;
    private AnimationClip casualTransTo;
    private AnimationClip casualWalk;
    private AnimationClip casualWalk2;
    private AnimationClip casualTransFrom;

    private UnityEngine.Vector3 schoolHallwayDoor; 
    private UnityEngine.Vector3 bedroomDoor; 
    private UnityEngine.Vector3 schoolClassroomPoint; 
    private UnityEngine.Vector3 schoolOutsidePoint;
    private UnityEngine.Vector3 kitchenPoint;
    private UnityEngine.Vector3 stairwayPoint;
    private UnityEngine.Vector3 carPoint;
    private UnityEngine.Vector3 schoolClassroom2Point;
    private UnityEngine.Vector3 bobaShopPoint;
    private UnityEngine.Vector3 forestPoint;
    private UnityEngine.Vector3 inBedPoint;
    private UnityEngine.Vector3 parallaxDoor; 

    private UnityEngine.Vector3 offset;
    
    private UnityEngine.Vector2 movement; 
    private Color currentTint;
    public bool isLeftFacing = true; 
    public float input; 

    private float stepInterval = 1f;
    private float stepTimer = 0f; 


    void Awake()
    {
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_animator == null)
            _animator = GetComponent<Animator>();

        _transform = transform;
        GameProgressionInstance = Object.FindFirstObjectByType<GameProgression>();
        casualIdol = Resources.Load<AnimationClip>("Animation/_CasualPlayerIdol");
        casualTransTo = Resources.Load<AnimationClip>("Animation/_CasualPlayerTransTo");
        casualWalk = Resources.Load<AnimationClip>("Animation/_CasualPlayerWalk");
        casualWalk2 = Resources.Load<AnimationClip>("Animation/_CasualPlayerWalk2");
        casualTransFrom = Resources.Load<AnimationClip>("Animation/_CasualPlayerTransFrom");
        originalController = _animator != null ? _animator.runtimeAnimatorController : null;
        if (originalController != null && casualIdol != null && casualWalk != null)
        {   
            Debug.Log("Creating animator override controller for casual animations");
            casualOverride = new AnimatorOverrideController(originalController);
            casualOverride["_PJplayerIdol"] = casualIdol; // replace "Idle" with the original clip name from your controller
            casualOverride["_PJplayerTransTo"] = casualTransTo;
            casualOverride["_PJplayerWalk"] = casualWalk; // replace "Walk" with the original clip name from your controller
            casualOverride["_PJplayerWalk2"] = casualWalk2;
            casualOverride["_PJplayerTransFrom"] = casualTransFrom;
        }

        schoolHallwayDoor = GameObject.Find("SchoolHallwayDoor").transform.position;
        bedroomDoor = GameObject.Find("BedroomDoor").transform.position;
        schoolClassroomPoint = GameObject.Find("SchoolClassroom").transform.position; 
        schoolOutsidePoint = GameObject.Find("SchoolOutside").transform.position; 
        kitchenPoint = GameObject.Find("Kitchen").transform.position; 
        stairwayPoint = GameObject.Find("Stairway").transform.position;
        carPoint = GameObject.Find("Car").transform.position;
        schoolClassroom2Point = GameObject.Find("SchoolClassroom2").transform.position; 
        bobaShopPoint = GameObject.Find("BobaShop").transform.position;
        forestPoint =GameObject.Find("Forest").transform.position;
        inBedPoint = GameObject.Find("InBed").transform.position;
        parallaxDoor = GameObject.Find("ParallaxDoor").transform.position;

        offset.x = 10; 
        _animator.SetBool("isRunning", false);

    }
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChangeSceneTint(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (GameProgression.currentScene!="SchoolClassroom" || GameProgression.currentScene!="SchoolOutside") {
        if (!GameData.dialogueActive && !GameData.dialogueOptionActive) {
            input = Input.GetAxisRaw("Horizontal");
            movement.x = input * speed * Time.deltaTime; //temp
            transform.Translate(movement);
            if (input != 0)
            {
                _animator.SetBool("isRunning", true); 
                stepTimer -= Time.deltaTime;

                if (stepTimer <= 0f)
                {
                    AudioManager.audioManagerInstance.PlayWalkSound();
                    stepTimer = stepInterval; // Reset the timer
                }
            }
            else {
                _animator.SetBool("isRunning", false);
                stepTimer = stepInterval; 
                }
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
        else
        {
            _animator.SetBool("isRunning", false);
        }
        }
    }

    void ChangeSceneTint()
    {
        if (!DialogueManager.colorTint.TryGetValue(GameProgression.currentScene, out currentTint)) {
            currentTint = Color.white;
        }
        _spriteRenderer.color = currentTint; 
    }

    // SceneMovement for walking into door
    public void SceneMovement(UnityEngine.Vector2 offset, string locationName)
    {
        
        _transform.position += (UnityEngine.Vector3)offset;
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
            
            UnityEngine.Vector3 schoolOffset = new UnityEngine.Vector3(0f,2f,0f);
            _transform.position = schoolHallwayDoor - offset - schoolOffset; 
            GameProgressionInstance.DialoguePrompt();
        }
        if (locationName == "Bedroom") {
            UnityEngine.Vector3 schoolOffset = new UnityEngine.Vector3(0f,2f,0f);
            _transform.position = bedroomDoor + offset - schoolOffset; 
            GameProgressionInstance.DialoguePrompt();
        }
        if (locationName == "SchoolClassroom")
        {
            _transform.position = schoolClassroomPoint; 
            GameProgressionInstance.DialoguePrompt();
        }
        if (locationName == "SchoolOutside")
        {
            _transform.position = schoolOutsidePoint; 
            GameProgressionInstance.DialoguePrompt();
        }
        if (locationName == "RealityTransition")
        {
            GameData.dialogueActive = true;
            blackScreenRInstance.gameObject.SetActive(true);
            GameProgressionInstance.DialoguePrompt();
        }
        if (locationName == "InBed")
        {
            _transform.position = inBedPoint;
            GameProgressionInstance.DialoguePrompt();
        }
        if (locationName == "Kitchen")
        {
            _transform.position = kitchenPoint;
            GameProgressionInstance.DialoguePrompt();
        }
        if (locationName == "Stairway")
        {
            _transform.position = stairwayPoint;
            GameProgressionInstance.DialoguePrompt();
        }
        if (locationName == "Car")
        {
            _transform.position = carPoint;
            GameProgressionInstance.DialoguePrompt();
        }
        if (locationName == "SchoolClassroom2")
        {
            _transform.position = schoolClassroom2Point;
            GameProgressionInstance.DialoguePrompt();
        }
        if (locationName == "BobaShop")
        {
            _transform.position = bobaShopPoint;
            GameProgressionInstance.DialoguePrompt();
        }
        if (locationName == "Forest")
        {
            _transform.position = forestPoint;
            GameProgressionInstance.DialoguePrompt();
        }
        if (locationName == "Parallax")
        {
            UnityEngine.Vector3 parallaxOffset = new UnityEngine.Vector3(0f,0f,0f);
            _transform.position = parallaxDoor - offset - parallaxOffset; 
            GameProgressionInstance.DialoguePrompt();
        }
        ChangeSceneTint(); 
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

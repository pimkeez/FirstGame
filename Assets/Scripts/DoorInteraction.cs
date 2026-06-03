using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    [Header("[State]")]
    public GameProgression GameProgressionInstance; // manager or no manager?
    public PlayerMovement PlayerMovementInstance; // manager or no manager?

    [Header("[Doors]")]
    public Vector2 doorA;
    public Vector2 offsetDoorA;
    public Vector2 doorB;
    public Vector2 offsetDoorB;

    [Header("[Location]")]
    [SerializeField]
    public GameObject doorALocation;
    [SerializeField]
    public GameObject doorBLocation;
    [SerializeField] string locationName;

    public Collider2D col;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // state
        GameProgressionInstance = Object.FindFirstObjectByType<GameProgression>();
        PlayerMovementInstance = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        col = GetComponent<Collider2D>();

        // doors
        if (doorALocation != null) {
        doorA = (Vector2)doorALocation.transform.position;
        }
        if (doorBLocation != null) {
        doorB = (Vector2)doorBLocation.transform.position;
        }
    }

    // Trigger callback when a 2D collider enters this trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        if (col == null) col = GetComponent<Collider2D>();
        if (col == null) return;
        if (doorALocation == null || doorBLocation == null) return;
        
        Debug.Log("DoorInteraction triggered by Player");
        Vector2 offset = doorB + offsetDoorB - (doorA + offsetDoorA);
        if (PlayerMovementInstance == null)
        {
            PlayerMovementInstance = GameObject.FindWithTag("Player")?.GetComponent<PlayerMovement>();
        }
        PlayerMovementInstance.SceneMovement(offset, locationName);
        
    }
}

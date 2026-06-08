using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;


public class Click : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("[DIALOGUE]")]
    [SerializeField] private int dialoguesIndex;
    [SerializeField] private List<TextAsset> interactionDialogues;
    public bool beenClicked = false;
    UnityEngine.Camera mainCamera;
    // PUT COLLIDERS ON ALL THE OBJECTS
    
    
    void Start()
    {
        mainCamera = UnityEngine.Camera.main;
    }
    bool IsMouseWithinBounds()
    {
         if (mainCamera == null)
    {
            mainCamera = UnityEngine.Camera.main;
            if (mainCamera == null) {
                Debug.Log("Main camera not found in WithinBounds check.");
                return false;
            }
    }
        // 1. Get mouse position and assign a positive Z depth for the camera
       Vector3 mouseScreenPos = Input.mousePosition;
       mouseScreenPos.z = 10f; 

     // 2. Convert screen coordinates to 2D world coordinates
     Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

     // 3. Check if the world point overlaps with any 2D physics collider
     Collider2D col = GetComponent<Collider2D>();
     return col != null && col.OverlapPoint(mouseWorldPos);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !beenClicked) 
        {   
            beenClicked = true; // bug prone later check if this is the issue
            if (!GameData.dialogueActive && IsMouseWithinBounds()) {
                GameProgression.GameProgressionInstance.ShowDialogue(interactionDialogues[dialoguesIndex]);
                Debug.Log("Mouse Clicked");
            }
            StartCoroutine(ClickCooldown());
            
        }
    }

    IEnumerator ClickCooldown() {
        yield return new WaitForSeconds(0.2f);
        beenClicked = false;
    }
}

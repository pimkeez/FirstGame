using Unity.VisualScripting;
using UnityEngine;

public class ParallaxBG : MonoBehaviour
{

    
    public UnityEngine.Camera Camera;
    private float length;
    private float startPos;
    public float parallaxEffect;


    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        Camera = UnityEngine.Camera.main;
    }

    void FixedUpdate()
    {
        if (GameProgression.currentScene != "Parallax")
            return;
        float dist = (Camera.transform.position.x * parallaxEffect);
        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);
    }
}
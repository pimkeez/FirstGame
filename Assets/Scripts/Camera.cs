using UnityEngine;

public class Camera : MonoBehaviour
{
    private Transform playerTransform;
    public float boundX = 0.15f;
   
    void Awake()
    {
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

        transform.position += new Vector3(delta.x, 0, 0);
    }
}
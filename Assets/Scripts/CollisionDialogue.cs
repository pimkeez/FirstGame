using System.Collections.Generic;
using UnityEngine;

public class CollisionDialogue : MonoBehaviour
{
    [Header("[DIALOGUE]")]
    [SerializeField] private int dialoguesIndex;
    [SerializeField] private List<TextAsset> interactionDialogues;
    private Collider2D thisCol;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thisCol = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Collision triggered");
        if (!col.gameObject.CompareTag("Player"))
        {
            return; 
        }
        GameProgression.GameProgressionInstance.ShowDialogue(interactionDialogues[dialoguesIndex]);
    }
}

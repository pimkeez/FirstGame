using System.Collections.Generic;
using UnityEngine;

public class IntroductionDialogue : MonoBehaviour
{
    [Header("[DIALOGUE]")]
    [SerializeField] private int dialoguesIndex;
    [SerializeField] private List<TextAsset> interactionDialogues;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    public void Start()
    {
        Debug.Log("Running intro");
        GameProgression.GameProgressionInstance.ShowDialogue(interactionDialogues[dialoguesIndex]);
    }
    
}

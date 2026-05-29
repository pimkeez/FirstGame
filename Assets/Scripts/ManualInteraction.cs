using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ManualInteraction : MonoBehaviour
{
    [Header("[DIALOGUE]")]
    [SerializeField] private int dialoguesIndex;
    [SerializeField] private List<TextAsset> interactionDialogues;

    public void Interact()
    {
        GameProgression.GameProgressionInstance.ShowDialogue(interactionDialogues[dialoguesIndex]);
    }
}
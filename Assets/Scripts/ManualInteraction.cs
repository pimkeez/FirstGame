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
        if (!GameData.dialogueActive) GameProgression.GameProgressionInstance.ShowDialogue(interactionDialogues[dialoguesIndex]);
    }
}
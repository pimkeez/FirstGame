using UnityEngine;

public class OptionButton : MonoBehaviour
{
    private OptionManager optionManagerInstance;

    void Awake()
    {
        optionManagerInstance = OptionManager.optionManagerInstance;
    }

    public void Interact()
    {
        AudioManager.audioManagerInstance.PlayButtonSound(); 
        optionManagerInstance.InteractOption(this);
    }

}
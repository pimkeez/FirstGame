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
        optionManagerInstance.InteractOption(this);
    }

}
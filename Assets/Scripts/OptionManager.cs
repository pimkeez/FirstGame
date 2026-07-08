using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class OptionManager : MonoBehaviour
{
    public static OptionManager optionManagerInstance; 
    private DialogueManager.Option option1; 
    private DialogueManager.Option option2; 
    private TextMeshProUGUI option1Text;
    private TextMeshProUGUI option2Text;
    private OptionButton option1Button;
    private OptionButton option2Button;
    private DialogueManager dialogueManagerInstance; // reference to the DialogueManager script
    private GameProgression gameProgressionInstance; // reference to the GameProgression script
    private PlayerMovement playerMovementInstance; 
    [SerializeField] private BlackScreen blackScreenInstance; 
    private int dialogueIndex; // to track the current dialogue index for options
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (optionManagerInstance == null)
        {
            optionManagerInstance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        option1Button = GameObject.Find("Canvas/DialogueBox/OptionManager/OptionBoxOne").GetComponent<OptionButton>();
        option2Button = GameObject.Find("Canvas/DialogueBox/OptionManager/OptionBoxTwo").GetComponent<OptionButton>();

        option1Text = GameObject.Find("Canvas/DialogueBox/OptionManager/OptionBoxOne/OptionTextOne").GetComponent<TextMeshProUGUI>();
        option2Text = GameObject.Find("Canvas/DialogueBox/OptionManager/OptionBoxTwo/OptionTextTwo").GetComponent<TextMeshProUGUI>();

        dialogueManagerInstance = GameObject.Find("Canvas/DialogueBox").GetComponent<DialogueManager>();
        gameProgressionInstance = GameProgression.GameProgressionInstance;
        playerMovementInstance = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>(); 

        optionManagerInstance.gameObject.SetActive(false);

        // ensure blackScreenInstance is assigned (allow inspector override)
        if (blackScreenInstance == null)
        {
            var bsObj = GameObject.Find("Canvas/BlackScreen");
            if (bsObj != null)
                blackScreenInstance = bsObj.GetComponent<BlackScreen>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadOptions(DialogueManager.Option option1, DialogueManager.Option option2, int dialogueIndex)
    {
        this.dialogueIndex = dialogueIndex;
        dialogueManagerInstance.enabled = false; 
    
        GameData.dialogueOptionActive = true;
        
        this.option1 = option1;
        this.option2 = option2;

        option1Text.text = option1.optionText;
        option2Text.text = option2.optionText;
    }

    public void InteractOption(OptionButton optionInstance)
    {
        DialogueManager.Option selectedOption = null;
        if (optionInstance == option1Button)
        {
            selectedOption = option1;
        }
        else if (optionInstance == option2Button)
        {
            selectedOption = option2;
        }
        dialogueManagerInstance.enabled = true; 
        gameProgressionInstance.enabled = true; 

        dialogueManagerInstance.GameProgressionDialogueAdvance(selectedOption); // Pass the selected option to the method

        if (selectedOption.nextScene != null)
        {
            StartCoroutine(gameProgressionInstance.ChangeScene(selectedOption.nextScene));
        
            return;
        }
        
        if (selectedOption.nextDialogueIndex != -1)
        {
            dialogueManagerInstance.OptionIntegrator(selectedOption.nextDialogueIndex);
        }
        else {
            dialogueManagerInstance.OptionIntegrator(dialogueIndex);
        }
        CloseOptions();
        // Handle option 1 selection logic here
    }

    public void CloseOptions()
    {
        GameData.dialogueOptionActive = false;
        optionManagerInstance.gameObject.SetActive(false);
        // i think this isn't running
        
        // add more
    }
}
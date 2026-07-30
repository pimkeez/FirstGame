using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class DialogueManager : MonoBehaviour
{   
    [Serializable]
    public class DialogueEntry
    {
        // skipId needs to exist because of future flags that will skip without dialogueOptions present
        //meanwhile, nextDialogueIndex needs to exist to jump around BECAUSE of dialogueOptions

        // nextScene and Option.nextScene for the same exact reasons
        // Mira if you ever read this and it's bad I just wanna say that I am sorry D: 

        public int skipId = -1; // default to -1 for no skip
        public string speaker;
        public string text;
        public string portrait;
        public string nextScene; // for future scene transition implementation
        public string photo; 

        public string flag;
        public bool flagValue;
        public bool hasFlagValue;
        public bool condition;
        public bool hasCondition;
        public string audio; 
        public Option[] options;
    }
    
    [Serializable]
    public class DialogueDataWrapper {
        public DialogueEntry[] dialogueEntries;
    }

    [Serializable]
    public class Option {
        public string optionText;
        public int nextDialogueIndex = -1; // index of the dialogue entry to jump to if this option is selected
        public string nextScene; // for future scene transition implementation
        public int gameProgressionDialogue = 0;
    }; // for future dialogue options implementation

    public DialogueDataWrapper dialogueDataWrapper;

    public DialogueEntry currentEntry;
    private int dialogueIndex = 0;

    private TextMeshProUGUI dialogueTMP; 
    private TextMeshProUGUI speakerTMP;
    private Image portraitImage;
    private Coroutine typingCoroutine;
    private OptionManager optionManagerInstance; // reference to the OptionManager instance
    private GameProgression gameProgressionInstance; 
    private Image objectImage; 
    private Sprite photoSprite; 
    private static Dictionary<string, bool> flagList; 
    public static Dictionary<string, Color> colorTint; 
    private bool isTyping = false;
    private bool skipTyping = false;
    public float typingSpeed = 0.05f;

    void Awake()
    {
        colorTint = new Dictionary<string, Color>();

        Color bedroomColor = new Color(0.8117251f, 0.7603773f, 1f, 1f); colorTint.Add("Bedroom", bedroomColor);
        Color schoolHallwayColor = new Color(0.8943396f, 0.7992896f, 0.720534f, 1f); colorTint.Add("SchoolHallway", schoolHallwayColor);
        Color schoolClassroomColor = new Color(0.9245283f, 0.8658025f, 0.8285867f, 1f); colorTint.Add("SchoolClassroom", schoolClassroomColor); 
        Color schoolOutsideColor = new Color(0.8943396f, 0.745739f, 0.6597863f, 1f); colorTint.Add("SchoolOutside", schoolOutsideColor);
        Color kitchenColor = new Color(0.9169811f, 0.811456f, 0.6972516f, 1f); colorTint.Add("Kitchen", kitchenColor);
        Color stairwayColor = new Color(0.8867924f, 0.6676041f, 0.7435741f, 1f); colorTint.Add("Stairway", stairwayColor); 
        Color carColor = new Color (0.9245283f, 0.8629522f, 0.7657886f, 1f); colorTint.Add("Car", carColor);
        Color schoolClassroomColor2 = new Color(0.9245283f, 0.8658025f, 0.8285867f, 1f); colorTint.Add("SchoolClassroom2", schoolClassroomColor2);
        Color bobaShopColor = new Color(0.8117251f, 0.7603773f, 1f, 1f); colorTint.Add("BobaShop", bobaShopColor); 
        Color forestColor = new Color(0.9471698f, 0.831166f, 0.6987611f, 1f); colorTint.Add("Forest", forestColor);
        Color inBedColor = new Color(0.8117251f, 0.7603773f, 1f, 1f); colorTint.Add("InBed", inBedColor);
        Color parallaxColor = new Color(0.4974083f, 0.5712773f, 0.8264151f, 1f); colorTint.Add("Parallax", parallaxColor);


        speakerTMP = GameObject.Find("Canvas/DialogueBox/SpeakerText").GetComponent<TextMeshProUGUI>();
        dialogueTMP = GameObject.Find("Canvas/DialogueBox/DialogueText").GetComponentInChildren<TextMeshProUGUI>();
        portraitImage = GameObject.Find("Canvas/DialogueBox/PortraitImage").GetComponent<Image>();
        objectImage = GameObject.Find("Canvas/DialogueBox/ObjectImage").GetComponent<Image>(); 

        optionManagerInstance = GameObject.Find("Canvas/DialogueBox/OptionManager").GetComponent<OptionManager>();
        gameProgressionInstance = GameObject.Find("GameProgressionManager").GetComponent<GameProgression>(); 

        gameObject.SetActive(false);
    }

    void Start()
    {
        if (GameProgression.flagListGp == null)
        {
            GameProgression.flagListGp = new Dictionary<string, bool>();
        }

        flagList = GameProgression.flagListGp; // reference the flagList from GameProgression
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameData.dialogueActive)
        {
            return; 
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                skipTyping = true; 
            }
            else
            {
                OnNext(); 
            }  
        }
    }

    public void SetVisualNovelJSONFile(TextAsset jsonFile)
    {
        Debug.Log("Loading dialogue JSON file: " + jsonFile.name);
        dialogueDataWrapper = JsonUtility.FromJson<DialogueDataWrapper>(jsonFile.text);
        GameData.dialogueActive = true;
        GameData.dialogueOptionActive = false;
        OnNext();
    }

    void OnNext()
    {
        // currently going through dialogue
        if (dialogueIndex < dialogueDataWrapper.dialogueEntries.Length)
        {
            currentEntry = dialogueDataWrapper.dialogueEntries[dialogueIndex];
            FlagChange();
            if (!String.IsNullOrEmpty(currentEntry.audio))
            {
                LoadAudio(); 
            }

            if (currentEntry.options != null)
            {
                optionManagerInstance.enabled = true;
                optionManagerInstance.gameObject.SetActive(true);
                optionManagerInstance.LoadOptions(currentEntry.options[0], currentEntry.options[1], dialogueIndex);
            }
            if (currentEntry.nextScene != null)
            {
                StartCoroutine(gameProgressionInstance.ChangeScene(currentEntry.nextScene));
                return; 
            }
            else {ShowDialogue();}
        }
        else
        {
            EndDialogue();
        }
        AudioManager.audioManagerInstance.PlayButtonSound();
    }

    public void GameProgressionDialogueAdvance(Option selectedOption) {
        if (selectedOption.gameProgressionDialogue != 0) {
            int a = selectedOption.gameProgressionDialogue;
            gameProgressionInstance.SkipDialogueIndex(a); 
        }
    }

    void FlagChange() {
        if (currentEntry == null || string.IsNullOrEmpty(currentEntry.flag) || !currentEntry.hasFlagValue)
        {
            Debug.Log("FlagChange: No flag to set.");
            return;
        }

        gameProgressionInstance.SetFlag(currentEntry.flag, currentEntry.flagValue);

        Debug.Log($"Flag '{currentEntry.flag}' set to {currentEntry.flagValue}");
    }

    bool FlagCheck() {
        if (currentEntry == null || string.IsNullOrEmpty(currentEntry.flag))
        {
            Debug.Log("FlagCheck: No flag to check.");
            return false;
        }

        if (!flagList.TryGetValue(currentEntry.flag, out bool storedFlag))
        {
            Debug.LogWarning($"Flag '{currentEntry.flag}' was not found in the flag list.");
            return false;
        }

        if (!currentEntry.hasCondition)
        {
            return false;
        }

        return storedFlag == currentEntry.condition;
    }

    public void ShowDialogue()
    {
        // SET: speaker
        speakerTMP.text = currentEntry.speaker;

        if (typingCoroutine != null) {
            StopCoroutine(typingCoroutine);
        }

        // SET: text (with typing effect)
        if (currentEntry.options != null)
        {
            dialogueTMP.text = currentEntry.text; // show full text immediately if options are present
        }
        else {
            dialogueTMP.text = ""; // Clear text before typing new line
            typingCoroutine = StartCoroutine(TypeLine(currentEntry.text));
        }

        if (currentEntry.photo != null) {
            photoSprite = Resources.Load<Sprite>($"Art/{currentEntry.photo}");
        }
        else
        {
            photoSprite = null; 
        }
        StartCoroutine(FadeInPhoto(photoSprite,0.5f));

        // SET: portrait
        LoadPortrait(); 

        // increment to the next dialogue entry
        if (currentEntry.skipId == -1) {
            dialogueIndex++;
            return;
        }

        if (currentEntry.hasCondition) {
            if (FlagCheck()) {
                dialogueIndex++;
                Debug.Log("Flag condition met, proceeding normally to next dialogue entry.");
                return;
            }
        }
        else {
            dialogueIndex = currentEntry.skipId + 1;
            return;
        }

        dialogueIndex = currentEntry.skipId + 1;
    }

    // literally just OnNext but for options, not sure if this is the best way to do it... but I highk don't wanna deal with parameters for EVERYTHING else that rlly doesnt need it
    public void OptionIntegrator(int dialogueIndex)
    {
        if (dialogueIndex < dialogueDataWrapper.dialogueEntries.Length - 1)
        {
            currentEntry = dialogueDataWrapper.dialogueEntries[dialogueIndex + 1]; //ALWAYS when skipping do 1 before index!!! BE CAREFUL WITH THISSSSSS
            this.dialogueIndex = dialogueIndex + 1; 
            FlagChange();
            ShowDialogue();
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine(string line) 
    {
        isTyping = true;
        skipTyping = false;
        dialogueTMP.text = "";

        foreach (char letter in line.ToCharArray())
        {
            if (skipTyping)
            {
                dialogueTMP.text = line;
                break;
            }
            dialogueTMP.text += letter;
            AudioManager.audioManagerInstance.PlayDialogueBlip();
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    IEnumerator FadeInPhoto(Sprite photo, float duration)
    {
        
        if (objectImage == null)
        {
            yield break;
        }

        Debug.Log("objectImage not null, coroutine ran");
        if (objectImage.sprite == photo)
        {
            objectImage.sprite = photo;
            objectImage.color = new Color(1f, 1f, 1f, photo == null ? 0f : 1f);
            yield break;
        }

        float elapsed = 0f;
        
        if (objectImage.sprite != null)
        {
            while (elapsed < duration)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                objectImage.color = new Color(1f, 1f, 1f, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        objectImage.color = new Color(1f, 1f, 1f, 0f);
        objectImage.sprite = photo;
        elapsed = 0f;

        if (photo != null)
        {
            while (elapsed < duration)
            {
                float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
                objectImage.color = new Color(1f, 1f, 1f, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        objectImage.color = new Color(1f, 1f, 1f, photo == null ? 0f : 1f);
    }

    Color GetSceneTint()
    {
        Color currentTint;
        if (!colorTint.TryGetValue(GameProgression.currentScene, out currentTint)) {
            currentTint = Color.white;
        }
        return currentTint;
    }

    IEnumerator FadePortrait(Sprite newPortrait, float duration)
    {
        Color currentTint; 
        if (portraitImage == null)
            yield break;
        
        else
        {
            currentTint = GetSceneTint(); 
        }
        if (portraitImage.sprite == newPortrait)
        {
            portraitImage.sprite = newPortrait;
            portraitImage.color = new Color(currentTint.r, currentTint.g, currentTint.b, newPortrait == null ? 0f : 1f);
            yield break;
        }

        float elapsed = 0f;
        if (portraitImage.sprite != null)
        {
            while (elapsed < duration)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                portraitImage.color = new Color(currentTint.r, currentTint.g, currentTint.b, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        portraitImage.color = new Color(currentTint.r, currentTint.g, currentTint.b, 0f);
        portraitImage.sprite = newPortrait;
        elapsed = 0f;

        if (newPortrait != null)
        {
            while (elapsed < duration)
            {
                float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
                portraitImage.color = new Color(currentTint.r, currentTint.g, currentTint.b, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        portraitImage.color = new Color(currentTint.r, currentTint.g, currentTint.b, newPortrait == null ? 0f : 1f);
    }

    public void StartDialogue() 
    {
        GameData.dialogueActive = true;
    }

    void LoadAudio()
    {
        AudioClip currentAudio = Resources.Load<AudioClip>($"Audio/{currentEntry.audio}");
        AudioManager.audioManagerInstance.dialogueCuedAudio(currentAudio); 
    }

    void LoadPortrait()
    {
        if (portraitImage == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(currentEntry.portrait))
        {
            StartCoroutine(FadePortrait(null, 0.5f));
            return;
        }

        Sprite newPortrait = Resources.Load<Sprite>($"Art/{currentEntry.speaker}/{currentEntry.portrait}");
        StartCoroutine(FadePortrait(newPortrait, 0.5f));
    }

    public void EndDialogue()
    {
        if (typingCoroutine != null) {
            StopCoroutine(typingCoroutine);
         }
        GameData.dialogueActive = false;
        currentEntry = null;
        dialogueIndex = 0;
        dialogueTMP.text = "";
        speakerTMP.text = "";
        isTyping = false; // these two by me so WATCH OUT BUDDYY
        skipTyping = false;
        if (!GameData.introductionOver) {
        GameData.introductionOver = true; 
        }

        if (portraitImage != null)
        {
            portraitImage.sprite = null;
            portraitImage.color = new Color(1f, 1f, 1f, 0f);
        }

        if (objectImage != null)
        {
            objectImage.sprite = null; 
            objectImage.color = new Color(1f, 1f, 1f, 0f);
        }

        gameObject.SetActive(false);
        // go reference hahckathon code 
    }
}
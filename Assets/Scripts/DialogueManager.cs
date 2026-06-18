using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

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
    }; // for future dialogue options implementation

    public DialogueDataWrapper dialogueDataWrapper;
    public AudioClip typeSound; 

    public DialogueEntry currentEntry;
    private int dialogueIndex = 0;
    public AudioSource audioSource;

    private TextMeshProUGUI dialogueTMP; 
    private TextMeshProUGUI speakerTMP;
    private Image portraitImage;
    private Coroutine typingCoroutine;
    private OptionManager optionManagerInstance; // reference to the OptionManager instance
    private bool isTyping = false;
    private bool skipTyping = false;
    public float typingSpeed = 0.05f;

    void Awake()
    {
        speakerTMP = GameObject.Find("Canvas/DialogueBox/SpeakerText").GetComponent<TextMeshProUGUI>();
        dialogueTMP = GameObject.Find("Canvas/DialogueBox/DialogueText").GetComponentInChildren<TextMeshProUGUI>();
        portraitImage = GameObject.Find("Canvas/DialogueBox/PortraitImage").GetComponent<Image>();

        optionManagerInstance = GameObject.Find("Canvas/DialogueBox/OptionManager").GetComponent<OptionManager>();

        gameObject.SetActive(false);
    }

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
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
            if (currentEntry.options != null)
            {
                optionManagerInstance.enabled = true;
                optionManagerInstance.gameObject.SetActive(true);
                optionManagerInstance.LoadOptions(currentEntry.options[0], currentEntry.options[1], dialogueIndex);
            }
            else {ShowDialogue();}
        }
        else
        {
            EndDialogue();
        }
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
    
        // SET: portrait
        LoadPortrait(); 

        // increment to the next dialogue entry
        if (currentEntry.skipId == -1) {
        dialogueIndex++;
        } else {
            dialogueIndex = Array.FindIndex(dialogueDataWrapper.dialogueEntries, entry => entry.skipId == currentEntry.skipId) + 1;
            if (dialogueIndex == 0) {
                Debug.LogWarning($"Skip ID {currentEntry.skipId} not found. Ending dialogue.");
                EndDialogue();
            }
            // ***PROLLY NEED ADJUSTMENTS LATER... what about options changing the skip id? You should probably have a METHOD for this
        }
    }

    // literally just OnNext but for options, not sure if this is the best way to do it... but I highk don't wanna deal with parameters for EVERYTHING else that rlly doesnt need it
    public void OptionIntegrator(int dialogueIndex)
    {
        if (dialogueIndex < dialogueDataWrapper.dialogueEntries.Length - 1)
        {
            currentEntry = dialogueDataWrapper.dialogueEntries[dialogueIndex + 1]; //ALWAYS when skipping do 1 before index!!! BE CAREFUL WITH THISSSSSS
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
            // audioSource.PlayOneShot(typeSound); WHEN YOU ADD AUDIO 
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    IEnumerator FadePortrait(Sprite newPortrait, float duration)
    {
        if (portraitImage == null)
            yield break;

        if (portraitImage.sprite == newPortrait)
        {
            portraitImage.sprite = newPortrait;
            portraitImage.color = new Color(1f, 1f, 1f, newPortrait == null ? 0f : 1f);
            yield break;
        }

        float elapsed = 0f;
        if (portraitImage.sprite != null)
        {
            while (elapsed < duration)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                portraitImage.color = new Color(1f, 1f, 1f, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        portraitImage.color = new Color(1f, 1f, 1f, 0f);
        portraitImage.sprite = newPortrait;
        elapsed = 0f;

        if (newPortrait != null)
        {
            while (elapsed < duration)
            {
                float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
                portraitImage.color = new Color(1f, 1f, 1f, alpha);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        portraitImage.color = new Color(1f, 1f, 1f, newPortrait == null ? 0f : 1f);
    }

    public void StartDialogue() 
    {
        GameData.dialogueActive = true;
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

        gameObject.SetActive(false);
        // go reference hahckathon code 
    }
}
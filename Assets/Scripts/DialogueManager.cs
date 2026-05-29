using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
using System.IO;

public class DialogueManager : MonoBehaviour
{   
    public class DialogueEntry
    {
        public int id; 
        public string speaker;
        public string text;
        public string portrait;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Serializable]
    public class DialogueDataWrapper {
        public DialogueEntry[] dialogueEntries;
    }
    public DialogueDataWrapper dialogueDataWrapper;
    public AudioClip typeSound; 

    public DialogueEntry currentEntry;
    private int dialogueIndex = 0;
    public AudioSource audioSource;

    private TextMeshProUGUI dialogueTMP; 
    private TextMeshProUGUI speakerTMP;
    private Image portraitImage;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool skipTyping = false;
    private bool dialogueActive = false;
    public float typingSpeed = 0.05f;

    void Awake()
    {
        speakerTMP = GameObject.Find("Canvas/DialogueBox/SpeakerText").GetComponent<TextMeshProUGUI>();
        dialogueTMP = GameObject.Find("Canvas/DialogueBox/DialogueText").GetComponentInChildren<TextMeshProUGUI>();
        portraitImage = GameObject.Find("Canvas/DialogueBox/PortraitImage").GetComponent<Image>();
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
        if (!dialogueActive)
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

    public void LoadDialogue(string filename)
    {
        string resourceName = Path.GetFileNameWithoutExtension(filename);
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(resourceName); // **FIX THIS TO BE FIT TO YOU
       
        if (jsonTextAsset != null)
        {
            dialogueDataWrapper = JsonUtility.FromJson<DialogueDataWrapper>(jsonTextAsset.text);
            currentEntry = dialogueDataWrapper.dialogueEntries[0]; // Start with the first entry
            dialogueActive = true;
        }
        else
        {
            Debug.LogError("JSON file not found: " + filename);
        }
    }

    void OnNext()
    {
        // at the end of dialogue, finished
        if (currentEntry == null)
        {
            return; 
        }
        // currently going through dialogue
        if (dialogueIndex < dialogueDataWrapper.dialogueEntries.Length - 1)
        {
            ShowDialogue();
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

        // SET: text (with typing effect)
        dialogueTMP.text = ""; // Clear text before typing new line
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(Typeline(currentEntry.text));
    
        // SET: portrait
        Sprite portraitSprite = Resources.Load<Sprite>(currentEntry.portrait); // takes portrait name, loads that sprite
        portraitImage.sprite = currentEntry.portrait != null ? portraitSprite : null;
        LoadPortrait(currentEntry.portrait); 

        // increment to the next dialogue entry
        dialogueIndex++;
        currentEntry = dialogueDataWrapper.dialogueEntries[dialogueIndex];
    }

    IEnumerator Typeline(string line) {
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
    }

    IEnumerator FadePortrait(Sprite newPortrait, float duration)
    {
        if ((portraitImage == null && newPortrait == null) || (portraitImage.sprite == newPortrait))
        {
            yield break; 
        }   
        float elapsed = 0f;
        if (portraitImage != null) {
            while (elapsed < duration)
        {
            
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            portraitImage.color = new Color(1f, 1f, 1f, alpha);
            elapsed += Time.deltaTime;
        }

        portraitImage.sprite = newPortrait;

        while (elapsed < duration)
            {
                float alpha = Mathf.Lerp(0f, 1f, duration / elapsed);
                portraitImage.color = new Color(1f, 1f, 1f, alpha);
                elapsed += Time.deltaTime;
            }
         }

    }

    public void StartDialogue() 
    {
        dialogueActive = true;
    }

    void LoadPortrait(string portraitName)
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
        Sprite newPortrait = Resources.Load<Sprite>(portraitName);
        StartCoroutine(FadePortrait(newPortrait, 0.5f));
    }

    void EndDialogue()
    {
        dialogueActive = false;
        currentEntry = null;
        dialogueIndex = 0;
        dialogueTMP.text = "";
        speakerTMP.text = "";
        portraitImage = null;
        isTyping = false; // these two by me so WATCH OUT BUDDYY
        skipTyping = false;
        // go reference hahckathon code 
    }
}

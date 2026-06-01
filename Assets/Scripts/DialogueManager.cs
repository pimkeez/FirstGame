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
        public int id; 
        public string speaker;
        public string text;
        public string portrait;
    }
    
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
    public float typingSpeed = 0.05f;

    void Awake()
    {
        speakerTMP = GameObject.Find("Canvas/DialogueBox/SpeakerText").GetComponent<TextMeshProUGUI>();
        dialogueTMP = GameObject.Find("Canvas/DialogueBox/DialogueText").GetComponentInChildren<TextMeshProUGUI>();
        portraitImage = GameObject.Find("Canvas/DialogueBox/PortraitImage").GetComponent<Image>();

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
        dialogueDataWrapper = JsonUtility.FromJson<DialogueDataWrapper>(jsonFile.text);
        GameData.dialogueActive = true;
        OnNext();
    }

    void OnNext()
    {
        // currently going through dialogue
        if (dialogueIndex < dialogueDataWrapper.dialogueEntries.Length)
        {
            currentEntry = dialogueDataWrapper.dialogueEntries[dialogueIndex];
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
        typingCoroutine = StartCoroutine(TypeLine(currentEntry.text));
    
        // SET: portrait
        LoadPortrait(); 

        // increment to the next dialogue entry
        dialogueIndex++;
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
        if ((portraitImage.sprite == null && newPortrait == null) || (portraitImage.sprite == newPortrait))
        {
            yield break; 
        }

        float elapsed = 0f;
        if (portraitImage != null && portraitImage.sprite != null) 
        {
            while (elapsed < duration)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                portraitImage.color = new Color(1f, 1f, 1f, alpha);
                elapsed += Time.deltaTime;

                yield return null;
            }
        }

        if (portraitImage != null)
        {
        portraitImage.color = new Color(1f, 1f, 1f, 0f);
        }

        portraitImage.sprite = newPortrait;
        elapsed = 0f; 

        if (portraitImage != null && portraitImage.sprite != null)
         {
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            portraitImage.color = new Color(1f, 1f, 1f, alpha);

            elapsed += Time.deltaTime;
            yield return null; // Pause for a frame
        }
        }

         // Ensure it hits exactly 1 at the end
        if (portraitImage != null)
        {   
           portraitImage.color = new Color(1f, 1f, 1f, 1f);
        }      
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

    void EndDialogue()
    {
        GameData.dialogueActive = false;
        currentEntry = null;
        dialogueIndex = 0;
        dialogueTMP.text = "";
        speakerTMP.text = "";
        portraitImage = null;
        isTyping = false; // these two by me so WATCH OUT BUDDYY
        skipTyping = false;

        gameObject.SetActive(false);
        // go reference hahckathon code 
    }
}
using UnityEngine;
using UnityEngine.UI; //required for sprite 
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System;
using System.Collections;
using System.Collections.Generic; // For Dictionary
using System.IO; // For Path
using NUnit.Framework;

public class DialogueManager : MonoBehaviour
{   
    public class DialogueEntry
    {
        public string id; 
        public string speaker;
        public string text;
        public string portrait;
        public string next;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Serializable]
    public class DialogueDataWrapper {
        public DialogueEntry[] dialogueEntries;
    }
    public TextMeshProUGUI textComponent; 
    public TextMeshProUGUI speakerComponent;
    public UnityEngine.UI.Image portraitComponent;
    public AudioClip typeSound; 

    private Dictionary<string, DialogueEntry> dialogueMap;

    public DialogueEntry currentEntry;
    private string currentDialogueRootId;
    public AudioSource audioSource;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool skipTyping = false;

    private bool dialogueActive = false;
    public float typingSpeed = 0.05f;

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
                onNext(); 
            }  
        }
    }

    public void loadDialogue(string filename)
    {
        string resourceName = Path.GetFileNameWithoutExtension(filename);
        TextAsset jsonTextAsset = Resources.Load<TextAsset>(resourceName); // **FIX THIS TO BE FIT TO YOU
       
        if (jsonTextAsset != null)
        {
            DialogueDataWrapper dataWrapper = JsonUtility.FromJson<DialogueDataWrapper>(jsonTextAsset.text);
            dialogueMap = new Dictionary<string, DialogueEntry>();
            foreach (var entry in dataWrapper.dialogueEntries)
            {
            dialogueMap[entry.id] = entry;
            }

        }
        else
        {
            Debug.LogError("JSON file not found: " + filename);
        }
    }

    public void showDialogue(string id)
    {
        if (dialogueMap != null && dialogueMap.ContainsKey(id))
        {
            currentEntry = dialogueMap[id];
            speakerComponent.text = currentEntry.speaker;
            // Load portrait sprite from Resources
            Sprite portraitSprite = Resources.Load<Sprite>(currentEntry.portrait); // takes portrait name, loads that sprite
            if (!dialogueActive || string.IsNullOrWhiteSpace(currentDialogueRootId))
            {
                currentDialogueRootId = id; 
            }
            
            currentEntry = dialogueMap[id];
            dialogueActive = true;
            portraitComponent.sprite = currentEntry.portrait != null ? portraitSprite : null;
            //speakerText.transform.parent.gameObject.SetActive(true); need to make speakerText object
            textComponent.text = ""; // Clear text before typing new line
            // speakerText.text = currentEntry.speaker;
            loadPortrait(currentEntry.portrait); 

            
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(typeLine(currentEntry.text));
        }
        else
        {
            Debug.LogError("Dialogue ID not found: " + id);
        }
    }

    IEnumerator typeLine(string line) {
        {
            isTyping = true;
            skipTyping = false;
            textComponent.text = "";
            foreach (char letter in line.ToCharArray())
            {
                if (skipTyping)
                {
                    textComponent.text = line;
                    break;
                }
                textComponent.text += letter;
                // audioSource.PlayOneShot(typeSound); WHEN YOU ADD AUDIO 
                yield return new WaitForSeconds(typingSpeed);
            }
            isTyping = false;
        }
    }

    IEnumerator fadePortrait(Sprite newPortrait, float duration)
    {
        if ((portraitComponent == null && newPortrait == null) || (portraitComponent.sprite == newPortrait))
        {
            yield break; 
        }   
        float elapsed = 0f;
        if (portraitComponent != null) {
            while (elapsed < duration)
        {
            
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            portraitComponent.color = new Color(1f, 1f, 1f, alpha);
            elapsed += Time.deltaTime;
        }

        portraitComponent.sprite = newPortrait;

        while (elapsed < duration)
            {
                float alpha = Mathf.Lerp(0f, 1f, duration / elapsed);
                portraitComponent.color = new Color(1f, 1f, 1f, alpha);
                elapsed += Time.deltaTime;
            }
         }

    }

    public void startDialogue() 
    {
        dialogueActive = true;

    }

    void onNext()
    {
        if (currentEntry == null)
        {
            return; 
        }
        if (!string.IsNullOrEmpty(currentEntry.next))
        {
            showDialogue(currentEntry.next);
        }
        else
        {
            endDialogue();
        }
    }

    void loadPortrait(string portraitName)
    {
        if (portraitComponent == null)
        {
            return; 
        }
        if (string.IsNullOrEmpty(dialogueMap[currentDialogueRootId].portrait))
        {
            StartCoroutine(fadePortrait(null, 0.5f));
            return;
        }
        Sprite newPortrait = Resources.Load<Sprite>(portraitName);
        StartCoroutine(fadePortrait(newPortrait, 0.5f));
    }

    void endDialogue()
    {
        dialogueActive = false;
        currentEntry = null;
        currentDialogueRootId = null;
        textComponent.text = "";
        speakerComponent.text = "";
        portraitComponent = null;
        isTyping = false; // these two by me so WATCH OUT BUDDYY
        skipTyping = false;
        // go reference hahckathon code 
    }
}

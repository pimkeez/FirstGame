using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameProgression : MonoBehaviour
{
    public static GameProgression GameProgressionInstance;
    public static string currentScene;
    public DialogueManager dialogueManager; 
    [SerializeField] private BlackScreen blackScreenInstance; 
    [SerializeField] private PlayerMovement playerMovementInstance; 
    [SerializeField] private DialogueManager dialogueManagerInstance; 
    [SerializeField] private OptionManager optionManagerInstance; 
    public static Dictionary<string, bool> flagListGp; 

    [Header("[DIALOGUE]")]
    [SerializeField] private int dialoguesIndex;
    [SerializeField] private List<TextAsset> interactionDialogues;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        if (GameProgressionInstance == null)
        {
            GameProgressionInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (flagListGp == null)
        {
            flagListGp = new Dictionary<string, bool>();
        }

        // audioSourceBGM = GetComponent<AudioSource>();
        // audioSourceSFX = transform.GetChild(0).GetComponent<AudioSource>();
    }

    void Start()
    {
        
    }

    public void SkipDialogueIndex(int a)
    {
        dialoguesIndex += a; 
    }

    public void SetFlag(string flagName, bool value)
    {
        if (flagListGp.ContainsKey(flagName))
        {
            flagListGp[flagName] = value;
        }
        else
        {
            flagListGp.Add(flagName, value);
        }
    }

    public void ShowDialogue(TextAsset dialogue)
    {
        dialogueManager.enabled = true;
        dialogueManager.gameObject.SetActive(true);
        dialogueManager.SetVisualNovelJSONFile(dialogue);
        
    }

    public void DialoguePrompt()
    {
        if (currentScene == "Bedroom")
        {
            GameData.gameStage++; 
        }
        GameProgressionInstance.ShowDialogue(interactionDialogues[dialoguesIndex]);
        dialoguesIndex++; 
    }

    public IEnumerator ChangeScene(string sceneName) // There's something fragile here about where I put stuff and idk why
    {
        Debug.Log($"ChangeScene started for {sceneName}");
        blackScreenInstance.gameObject.SetActive(true);
        yield return StartCoroutine(blackScreenInstance.FadeIn());
        Debug.Log("ChangeScene: FadeIn returned");
        dialogueManagerInstance.EndDialogue();

        currentScene = sceneName;
        playerMovementInstance.TimeTransition(sceneName);

        Debug.Log("ChangeScene: about to call FadeOut");
        yield return StartCoroutine(blackScreenInstance.FadeOut());
        Debug.Log("ChangeScene: FadeOut completed");
        
    }

    // **BUG LIST
    // when you click during transition fade transition fade BREAKS
    // portraits act up for some reason? I think it has to do with clicking too fast
    // not a bug but it's very easy to accidentally skip forward very short dialogue. think about if thats what u want
}
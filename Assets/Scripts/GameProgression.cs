using UnityEngine;
using System.Collections; 

public class GameProgression : MonoBehaviour
{
    public static GameProgression GameProgressionInstance;
    public static string currentScene;
    public DialogueManager dialogueManager; 
    [SerializeField] private BlackScreen blackScreenInstance; 
    [SerializeField] private PlayerMovement playerMovementInstance; 
    [SerializeField] private DialogueManager dialogueManagerInstance; 
    [SerializeField] private OptionManager optionManagerInstance; 

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
        }

        // audioSourceBGM = GetComponent<AudioSource>();
        // audioSourceSFX = transform.GetChild(0).GetComponent<AudioSource>();
    }

    public void ShowDialogue(TextAsset dialogue)
    {
        dialogueManager.enabled = true;
        dialogueManager.gameObject.SetActive(true);
        dialogueManager.SetVisualNovelJSONFile(dialogue);
        
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
}
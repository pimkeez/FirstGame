using UnityEngine;

public class GameProgression : MonoBehaviour
{
    public static GameProgression GameProgressionInstance;
    public DialogueManager dialogueManager; 

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
}
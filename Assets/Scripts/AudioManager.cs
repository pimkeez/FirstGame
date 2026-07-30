using UnityEngine;

public class AudioManager : MonoBehaviour
{
    
    public static AudioManager audioManagerInstance;

    [Header("AudioSource")]
    [SerializeField] private AudioSource music;
    
    [SerializeField] private AudioSource sfx; 
    [SerializeField] private AudioSource ambience; 
    [SerializeField] private AudioSource dialogue; 
    [SerializeField] private AudioSource walk; 

    [Header("AudioSource")]
    [SerializeField]
    public AudioClip talk; 
    public AudioClip walkBedroom; 
    public AudioClip walkSchool; 
    public AudioClip button;  

    private float pitchRandomness = 0.8f; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (audioManagerInstance == null)
        {
            audioManagerInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    public void PlayWalkSound()
    {
        if (GameProgression.currentScene == "Bedroom")
        {
            walk.volume = 1f; 
            walk.pitch = Random.Range(1f - pitchRandomness, 1f + pitchRandomness);
            walk.PlayOneShot(walkBedroom); 
        }
        else if (GameProgression.currentScene == "SchoolHallway")
        {
            walk.volume = 0.5f; 
            walk.pitch = Random.Range(1f - pitchRandomness, 1f + pitchRandomness);
            walk.PlayOneShot(walkSchool); 
        }
    }
    public void PlayDialogueBlip()
    {
        dialogue.PlayOneShot(talk); 
    }

    public void PlayButtonSound()
    {
        sfx.PlayOneShot(button); 
    }

    public void dialogueCuedAudio(AudioClip currentAudio)
    {
        sfx.PlayOneShot(currentAudio); 
    }
}

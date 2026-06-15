using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreen : MonoBehaviour
{
    // Sprite defaultSprite = GetComponent<SpriteRenderer>().sprite; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Image blackScreen;
    private OptionManager optionManagerInstance; 

    void Awake()
    {
        blackScreen = GetComponent<Image>();  
        optionManagerInstance = GameObject.Find("Canvas/DialogueBox/OptionManager").GetComponent<OptionManager>();
    }

    void Start()
    {
        gameObject.SetActive(false);
    }

    public IEnumerator FadeIn(float duration = 0.5f)
    {
        Debug.Log("BlackScreen: FadeIn started");
        blackScreen.color = new Color(0f, 0f, 0f, 0f);
        GameData.transitionActive = true;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            blackScreen.color = new Color(0f, 0f, 0f, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        blackScreen.color = new Color(0f, 0f, 0f, 1f);
        Debug.Log("BlackScreen: FadeIn completed");
    }

    public IEnumerator FadeOut(float duration = 0.5f)
    {
        Debug.Log("Fading out...");
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            blackScreen.color = new Color(0f, 0f, 0f, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        blackScreen.color = new Color(0f, 0f, 0f, 0f);
        GameData.transitionActive = false;
        optionManagerInstance.CloseOptions(); 
        gameObject.SetActive(false);
    }

    public IEnumerator FadeTransition()
    {
        yield return StartCoroutine(FadeIn());
        yield return StartCoroutine(FadeOut());
    }
}

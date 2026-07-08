using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreenR : MonoBehaviour
{
    // Sprite defaultSprite = GetComponent<SpriteRenderer>().sprite; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Image blackScreen;

    void Awake()
    {
        blackScreen = GetComponent<Image>();  
        gameObject.SetActive(false);
    }

    public void Update()
    {
        if (GameProgression.currentScene == "Bedroom") {
           StartCoroutine(FadeOut());
        }
    } 

    public IEnumerator FadeIn(float duration = 0.5f)
    {
        Debug.Log("BlackScreen: FadeIn started");
        blackScreen.color = new Color(0f, 0f, 0f, 0f);

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
        gameObject.SetActive(false);
    }

    public IEnumerator FadeTransition()
    {
        yield return StartCoroutine(FadeIn());
        yield return StartCoroutine(FadeOut());
    }
}

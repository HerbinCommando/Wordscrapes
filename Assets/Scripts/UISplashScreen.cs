using UnityEngine;

public class UISplashScreen : MonoBehaviour
{
    public GameObject leWord;
    public GameObject wordScrapes;

    // Hack.
    // Start() is called on both games, causing an initialization race condition
    // Reference and Activate the correct quit button here because GameStart()'s have already been called.
    public GameObject uiConfig;
    public GameObject uiConfigQuitLeWord;
    public GameObject uiConfigQuitWordScrapes;

    public void OnEnable()
    {
        leWord.SetActive(false);
        uiConfig.SetActive(false);
        wordScrapes.SetActive(false);
    }

    public void OnClickLeWord()
    {
        gameObject.SetActive(false);
        leWord.SetActive(true);
        uiConfigQuitLeWord.SetActive(true);
        uiConfigQuitWordScrapes.SetActive(false);
        wordScrapes.SetActive(false);
    }

    public void OnClickWordScrapes()
    {
        gameObject.SetActive(false);
        leWord.SetActive(false);
        uiConfigQuitLeWord.SetActive(false);
        uiConfigQuitWordScrapes.SetActive(true);
        wordScrapes.SetActive(true);
    }
}

using UnityEngine;

public class UISplashScreen : MonoBehaviour
{
    public GameObject leWord;
    public GameObject wordScrapes;

    public void OnEnable()
    {
        leWord.SetActive(false);
        wordScrapes.SetActive(false);
    }

    public void OnClickLeWord()
    {
        gameObject.SetActive(false);
        leWord.SetActive(true);
        wordScrapes.SetActive(false);
    }

    public void OnClickWordScrapes()
    {
        gameObject.SetActive(false);
        leWord.SetActive(false);
        wordScrapes.SetActive(true);
    }
}

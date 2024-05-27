using UnityEngine;

public class UISplashScreen : MonoBehaviour
{
    public GameObject borgle;
    public GameObject leWord;
    public GameObject wordScrapes;

    public void OnEnable()
    {
        borgle.SetActive(false);
        leWord.SetActive(false);
        wordScrapes.SetActive(false);
    }

    public void OnClickBorgle()
    {
        borgle.SetActive(true);
        gameObject.SetActive(false);
        leWord.SetActive(false);
        wordScrapes.SetActive(false);
    }

    public void OnClickLeWord()
    {
        borgle.SetActive(false);
        gameObject.SetActive(false);
        leWord.SetActive(true);
        wordScrapes.SetActive(false);
    }

    public void OnClickWordScrapes()
    {
        borgle.SetActive(false);
        gameObject.SetActive(false);
        leWord.SetActive(false);
        wordScrapes.SetActive(true);
    }
}

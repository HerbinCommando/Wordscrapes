using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    public GameObject LeWord;
    public GameObject WordScrapes;

    public void OnClickLeWord()
    {
        gameObject.SetActive(false);
        LeWord.SetActive(true);
        WordScrapes.SetActive(false);
    }

    public void OnClickWordScrapes()
    {
        gameObject.SetActive(false);
        LeWord.SetActive(false);
        WordScrapes.SetActive(true);
    }
}

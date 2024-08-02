using UnityEngine;

public class UISplashScreen : MonoBehaviour
{
    public GameObject borgle;
    public GameObject leWord;
    public GameObject sudooku;
    public GameObject wordScrapes;

    public void OnEnable()
    {
        borgle.SetActive(false);
        leWord.SetActive(false);
        sudooku.SetActive(false);
        wordScrapes.SetActive(false);
    }

    public void OnClickBorgle()
    {
        borgle.SetActive(true);
        gameObject.SetActive(false);
        leWord.SetActive(false);
        sudooku.SetActive(false);
        wordScrapes.SetActive(false);
    }

    public void OnClickLeWord()
    {
        borgle.SetActive(false);
        gameObject.SetActive(false);
        leWord.SetActive(true);
        sudooku.SetActive(false);
        wordScrapes.SetActive(false);
    }

    public void OnClickSudooku()
    {
        borgle.SetActive(false);
        gameObject.SetActive(false);
        leWord.SetActive(false);
        sudooku.SetActive(true);
        wordScrapes.SetActive(false);
    }

    public void OnClickWordScrapes()
    {
        borgle.SetActive(false);
        gameObject.SetActive(false);
        leWord.SetActive(false);
        sudooku.SetActive(false);
        wordScrapes.SetActive(true);
    }
}

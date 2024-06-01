using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Borgle : MonoBehaviour
{
    public static readonly string[][] BorgleClassic = new string[][] {
        new string[] { "A", "A", "C", "I", "O", "T", },
        new string[] { "A", "B", "I", "L", "T", "Y", },
        new string[] { "A", "B", "J", "M", "O", "Qu", },
        new string[] { "A", "C", "D", "E", "M", "P", },
        new string[] { "A", "C", "E", "L", "R", "S", },
        new string[] { "A", "D", "E", "N", "V", "Z", },
        new string[] { "A", "H", "M", "O", "R", "S", },
        new string[] { "B", "I", "F", "O", "R", "X", },
        new string[] { "D", "E", "N", "O", "S", "W", },
        new string[] { "D", "K", "N", "O", "T", "U", },
        new string[] { "E", "E", "F", "H", "I", "Y", },
        new string[] { "E", "G", "K", "L", "U", "Y", },
        new string[] { "E", "G", "I", "N", "T", "V", },
        new string[] { "E", "H", "I", "N", "P", "S", },
        new string[] { "E", "L", "P", "S", "T", "U", },
        new string[] { "G", "I", "L", "R", "U", "W", }
    };

    public static readonly string[][] BorgleModern = new string[][] {
        new string[] { "A", "A", "E", "E", "G", "N" },
        new string[] { "A", "B", "B", "J", "O", "O" },
        new string[] { "A", "C", "H", "O", "P", "S" },
        new string[] { "A", "F", "F", "K", "P", "S" },
        new string[] { "A", "O", "O", "T", "T", "W" },
        new string[] { "C", "I", "M", "O", "T", "U" },
        new string[] { "D", "E", "I", "L", "R", "X" },
        new string[] { "D", "E", "L", "R", "V", "Y" },
        new string[] { "D", "I", "S", "T", "T", "Y" },
        new string[] { "E", "E", "G", "H", "N", "W" },
        new string[] { "E", "E", "I", "N", "S", "U" },
        new string[] { "E", "H", "R", "T", "V", "W" },
        new string[] { "E", "I", "O", "S", "S", "T" },
        new string[] { "E", "L", "R", "T", "T", "Y" },
        new string[] { "H", "I", "M", "N", "U", "Qu" },
        new string[] { "H", "L", "N", "N", "R", "Z" }
    };

    public GameObject prefabUILine;
    public GameObject prefabUIWord;
    public RectTransform rectUIChars;
    public RectTransform rectUIWords;
    public TextMeshProUGUI textCurrentString;
    public UIBackgrounds uiBackgrounds;
    public UIConfig uiConfig;
    public GameObject uiGameOver;

    private string currentString = string.Empty;
    private bool touchDown = false;
    private List<UIChar> uiChars = new List<UIChar>();
    private List<RectTransform> uiCharsSelected = new List<RectTransform>();
    private List<GameObject> uiLines = new List<GameObject>();
    private List<UIWord> uiWords = new List<UIWord>();
    private List<string> wordHits = new List<string>();
    private List<string> wordSolutions = new List<string>();

    private void AppendCurrentString(UIChar uiChar)
    {
        if (uiChar.Selected)
            return;

        currentString += uiChar.Character;

        uiCharsSelected.Add(uiChar.transform as RectTransform);
        uiChar.SetState(UIChar.State.Green);

        if (uiCharsSelected.Count > 1)
            MakeUILine(uiCharsSelected[uiCharsSelected.Count - 2].transform as RectTransform, uiCharsSelected[uiCharsSelected.Count - 1].transform as RectTransform);

        if (Config.VibrateOnHighlight && wordSolutions.Contains(currentString) && !wordHits.Contains(currentString))
            Handheld.Vibrate();
    }

    private void CheckCurrentString()
    {
        if (Dictionary.Contains(currentString) && !wordHits.Contains(currentString))
        {
            GameObject instance = Instantiate(prefabUIWord);
            UIWord uiWord = instance.GetComponent<UIWord>();
            uiWord.onClick = OnClickUIWord;

            uiWord.Set(currentString);
            uiWord.SetState(UIWord.State.Default);
            uiWord.transform.SetParent(rectUIWords);
            uiWords.Add(uiWord);
            wordHits.Add(currentString);

            Handheld.Vibrate();
        }

        Deselect();
    }

    private void Deselect()
    {
        currentString = string.Empty;

        for (int i = uiLines.Count - 1; i >= 0; --i)
            Destroy(uiLines[i]);

        uiLines.Clear();
        uiCharsSelected.Clear();

        foreach (var tc in uiChars)
            tc.SetState(UIChar.State.Default);
    }

    private void DeselectOne(UIChar uiChar)
    {
        currentString = currentString.Substring(0, currentString.Length - 1);

        uiChar.SetState(UIChar.State.Default);
        uiCharsSelected.RemoveAt(uiCharsSelected.Count - 1);

        if (uiLines.Count > 0)
        {
            Destroy(uiLines[^1]);
            uiLines.RemoveAt(uiLines.Count - 1);
        }

        if (wordSolutions.Contains(currentString) && !wordHits.Contains(currentString))
            Handheld.Vibrate();
    }

    private void GameStart()
    {
        currentString = string.Empty;
        textCurrentString.text = string.Empty;

        uiBackgrounds.Shuffle();
        uiCharsSelected.Clear();
        uiLines.Clear();
        uiWords.Clear();
        wordHits.Clear();
        wordSolutions.Clear();

        for (int i = rectUIWords.childCount - 1; i >= 0; --i)
            Destroy(rectUIWords.GetChild(i).gameObject);

        for (int i = 0; i < uiChars.Count; ++i)
            uiChars[i].textChar.text = RollDie(i);
    }

    private void GameOver()
    {
        uiGameOver.SetActive(true);
    }

    private void MakeUILine(RectTransform a, RectTransform b)
    {
        GameObject instance = Instantiate(prefabUILine, transform.position, Quaternion.identity);
        RectTransform newImageRectTransform = instance.GetComponent<RectTransform>();
        Vector3 positionBetweenRects = (a.position + b.position) / 2f;
        newImageRectTransform.position = positionBetweenRects;

        float width = Vector3.Distance(a.position, b.position);
        Vector2 sizeDelta = newImageRectTransform.sizeDelta;
        sizeDelta.x = width;
        newImageRectTransform.sizeDelta = sizeDelta;

        Vector3 direction = a.position - b.position;
        float zRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        newImageRectTransform.rotation = Quaternion.Euler(0, 0, zRotation);

        newImageRectTransform.SetParent(a.parent);
        newImageRectTransform.SetAsFirstSibling();
        uiLines.Add(instance);
    }

    private string RollDie(int idx)
    {
        if (Config.BorgleClassic)
            return BorgleClassic[idx][Random.Range(0, 6)];
        else
            return BorgleModern[idx][Random.Range(0, 6)];
    }

    private void Start()
    {
        Config.Load();
        Dictionary.Load();
        Stats.Load();

        uiGameOver.SetActive(false);

        UIChar[] instances = rectUIChars.GetComponentsInChildren<UIChar>();
        for (int i = 0; i < instances.Length; ++i)
        {
            instances[i].onPointerDown += OnPointerDown;
            instances[i].onPointerUp += OnPointerUp;
            instances[i].onPointerEnter += OnPointerEnter;

            uiChars.Add(instances[i]);
        }

        GameStart();
    }

    private void Update()
    {
        if (textCurrentString.text != currentString)
            textCurrentString.text = currentString;

        /*
        foreach (var uiChar in uiChars)
        {
            if (!uiChar.Selected && Input.GetKeyDown(uiChar.KeyCode))
            {
                OnPointerDown(uiChar);
                break;
            }
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            ScreenOnPointerUp();

        if (Input.GetKeyDown(KeyCode.Escape))
            Deselect();

        if (Input.GetKeyDown(KeyCode.Delete))
            if (uiCharsSelected.Count >= 1)
                DeselectOne(uiCharsSelected[^1].GetComponent<UIChar>());
        */
    }

    public void OnClickQuitGame()
    {
        Config.Save();
        uiConfig.gameObject.SetActive(false);
        GameOver();
    }

    public void OnClickNewGame()
    {
        Config.Save();
        GameStart();
        uiGameOver.SetActive(false);
    }

    public void OnClickSettings()
    {
        Deselect();
        uiConfig.Activate(Config.Game.Borgle);
    }

    public void OnClickUIWord(UIWord uiWord)
    {
        if (uiWord.state == UIWord.State.Blacklist)
        {
            Config.Blacklist.Remove(uiWord.value);
            uiWord.SetState(UIWord.State.Default);
        }
        else if (uiWord.state != UIWord.State.Default)
        {
            Config.Blacklist.Add(uiWord.value);
            uiWord.SetState(UIWord.State.Blacklist);
        }

        if (Config.LogPointerEvents)
            Debug.Log($"OnClickUIWord {uiWord.value}");
    }

    public void OnPointerDown(UIChar uiChar)
    {
        touchDown = true;

        AppendCurrentString(uiChar);

        if (Config.LogPointerEvents)
            Debug.Log($"OnPointerDown {uiChar.Character}");
    }

    public void OnPointerUp(UIChar uiChar)
    {
        touchDown = false;

        CheckCurrentString();

        if (Config.LogPointerEvents)
            Debug.Log($"OnPointerUp {uiChar.Character}");
    }

    public void OnPointerEnter(UIChar uiChar)
    {
        if (touchDown)
        {
            if (uiChar.Selected && uiChar.gameObject == uiCharsSelected[^1].gameObject)
                DeselectOne(uiChar);
            else
            {
                int index1 = uiChars.IndexOf(uiCharsSelected[^1].GetComponent<UIChar>());
                int index2 = uiChars.IndexOf(uiChar);
                int row1 = index1 / 4;
                int col1 = index1 % 4;
                int row2 = index2 / 4;
                int col2 = index2 % 4;

                if (
                    (row1 == row2 && (col1 == col2 - 1 || col1 == col2 + 1)) ||     // Same row, left or right
                    (col1 == col2 && (row1 == row2 - 1 || row1 == row2 + 1)) ||     // Same column, above or below
                    (row1 == row2 - 1 && (col1 == col2 - 1 || col1 == col2 + 1)) || // Diagonal: top-left, top-right
                    (row1 == row2 + 1 && (col1 == col2 - 1 || col1 == col2 + 1))    // Diagonal: bottom-left, bottom-right
                ) AppendCurrentString(uiChar);
            }
        }

        if (Config.LogPointerEvents)
            Debug.Log($"OnPointerEnter {uiChar.Character}");
    }

    public void ScreenOnPointerUp()
    {
        touchDown = false;

        CheckCurrentString();

        if (Config.LogPointerEvents)
            Debug.Log($"ScreenOnPointerUp");
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordScrapes : MonoBehaviour
{
    public Image imageBackgroundA;
    public Image imageBackgroundB;
    public GameObject prefabUIChar;
    public GameObject prefabUILine;
    public GameObject prefabUIWord;
    public RectTransform rectUIChars;
    public RectTransform rectUIWords;
    public TextMeshProUGUI textCurrentString;
    public TextMeshProUGUI textFoundPct;
    public TextMeshProUGUI textFoundPctDelta;
    public TextMeshProUGUI textGameTime;
    public TextMeshProUGUI textGamesPlayed;
    public TextMeshProUGUI textGamesPlayedDelta;
    public TextMeshProUGUI textWordsFound;
    public TextMeshProUGUI textWordsFoundDelta;
    public TextMeshProUGUI textWordsTotal;
    public TextMeshProUGUI textWordsTotalDelta;
    public GameObject uiConfig;
    public GameObject uiGameOver;

    private string currentString = string.Empty;
    private float deltaTimeS = 0f;
    private int gameTimeS = 0;
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
        uiChar.SetState(UIChar.State.Selected);

        if (uiCharsSelected.Count > 1)
            MakeUILine(uiCharsSelected[uiCharsSelected.Count - 2].transform as RectTransform, uiCharsSelected[uiCharsSelected.Count - 1].transform as RectTransform);

        if (Config.VibrateOnHighlight && wordSolutions.Contains(currentString) && !wordHits.Contains(currentString))
            Handheld.Vibrate();
    }

    private void CheckCurrentString()
    {
        foreach (var sw in uiWords)
        {
            if (sw.word == currentString && !wordHits.Contains(currentString))
            {
                wordHits.Add(currentString);
                sw.SetState(UIWord.State.Hit);

                if (!Config.VibrateOnHighlight)
                    Handheld.Vibrate();

                if (wordHits.Count == wordSolutions.Count)
                    GameOver();
            }
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

    private void GameStart()
    {
        // Cleanup
        currentString = string.Empty;
        deltaTimeS = 0f;
        gameTimeS = Config.GameTimeSeconds;
        textCurrentString.text = string.Empty;
        textGameTime.text = Config.GameTimeSeconds.ToString();

        imageBackgroundA.gameObject.SetActive(UnityEngine.Random.value > 0.5f);
        imageBackgroundB.gameObject.SetActive(!imageBackgroundA.gameObject.activeSelf);
        textGameTime.gameObject.SetActive(Config.GameTimed);

        uiLines.Clear();
        wordHits.Clear();
        wordSolutions.Clear();
        uiChars.Clear();
        uiCharsSelected.Clear();

        for (int i = rectUIWords.childCount - 1; i >= 0; --i)
            Destroy(rectUIWords.GetChild(i).gameObject);

        for (int i = rectUIChars.childCount - 1; i >= 0; --i)
            Destroy(rectUIChars.GetChild(i).gameObject);
        
        // Generate Solution
        List<string> filteredStrings = Dictionary.lines.Where(s => s.Length == Config.WordLengthMax).ToList();
        string pickedWord = filteredStrings[UnityEngine.Random.Range(0, filteredStrings.Count)];
        List<string> permutations = GeneratePermutations(pickedWord);
        permutations = permutations.Distinct().ToList();

        foreach (string perm in permutations)
            if (Dictionary.lines.Contains(perm))
                wordSolutions.Add(perm);

        // Instance Prefabs
        foreach (string word in wordSolutions)
        {
            GameObject solutionWordGO = Instantiate(prefabUIWord);
            UIWord solutionWord = solutionWordGO.GetComponent<UIWord>();

            solutionWord.Set(word);
            solutionWord.transform.SetParent(rectUIWords);
            uiWords.Add(solutionWord);
        }

        float angleStep = 360f / Config.WordLengthMax;

        for (int i = 0; i < Config.WordLengthMax; ++i)
        {
            GameObject uiCharGO = Instantiate(prefabUIChar);
            UIChar uiChar = uiCharGO.GetComponent<UIChar>();
            uiChar.textChar.text = pickedWord[i].ToString();

            uiChar.onPointerDown += OnPointerDown;
            uiChar.onPointerUp += OnPointerUp;
            uiChar.onPointerEnter += OnPointerEnter;

            uiChar.transform.SetParent(rectUIChars);
            uiChars.Add(uiChar);

            float angle = i * (360f / Config.WordLengthMax) * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle + 45) * Config.ControlRadiusPx;
            float y = Mathf.Sin(angle + 45) * Config.ControlRadiusPx;
            (uiChar.transform as RectTransform).anchoredPosition = new Vector2(x, y);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectUIWords);

        float width = 200.0f + (rectUIWords.GetChild(rectUIWords.childCount - 1).transform as RectTransform).position.x;
        Vector2 sizeDelta = rectUIWords.sizeDelta;
        sizeDelta.x = width;
        rectUIWords.sizeDelta = sizeDelta;

        OnClickShuffle();

        // Logging
        if (Config.LogSolutionWords || Config.LogPermutations)
            Debug.Log($"Picked {Config.WordLengthMax} letter word: {pickedWord}");

        if (Config.LogPermutations)
            foreach (string permutation in permutations)
                Debug.Log(permutation);

        if (Config.LogSolutionWords)
            foreach (string word in wordSolutions)
                Debug.Log(word);
    }

    private void GameOver()
    {
        foreach (var sw in uiWords)
            if (!sw.Hit)
                sw.SetState(UIWord.State.Miss);

        IncrementStats(1, wordHits.Count, wordSolutions.Count);
        uiGameOver.SetActive(true);
    }

    private List<string> GeneratePermutations(string word)
    {
        List<string> permutations = new List<string>();
        bool[] used = new bool[word.Length]; // Array to track used letters

        for (int length = Config.WordLengthMin; length <= Config.WordLengthMax; length++)
            GeneratePermutationsRecursive(word, used, "", permutations, length);

        return permutations;
    }

    private void GeneratePermutationsRecursive(string word, bool[] used, string permutation, List<string> permutations, int length)
    {
        if (permutation.Length == length)
        {
            permutations.Add(permutation);
            return;
        }

        for (int i = 0; i < word.Length; ++i)
        {
            if (!used[i])
            {
                used[i] = true; // Mark the letter as used
                GeneratePermutationsRecursive(word, used, permutation + word[i], permutations, length);
                used[i] = false; // Backtrack: unmark the letter
            }
        }
    }

    private void IncrementStats(int gamesPlayedDelta, int wordsFoundDelta, int wordsTotalDelta)
    {
        float currentFoundPct = Stats.wordsTotal == 0 ? 0f : (float)Stats.wordsFound / Stats.wordsTotal * 100f;

        Stats.gamesPlayed += gamesPlayedDelta;
        Stats.wordsFound += wordsFoundDelta;
        Stats.wordsTotal += wordsTotalDelta;

        float foundPct = (float)Stats.wordsFound / Stats.wordsTotal * 100f;

        textFoundPct.text = $"{foundPct:F2}%";
        textFoundPctDelta.text = $"{foundPct - currentFoundPct:F2}%";
        textGamesPlayed.text = $"{Stats.gamesPlayed}";
        textGamesPlayedDelta.text = $"+{gamesPlayedDelta}";
        textWordsFound.text = $"{Stats.wordsFound}";
        textWordsFoundDelta.text = $"+{wordsFoundDelta}";
        textWordsTotal.text = $"{Stats.wordsTotal}";
        textWordsTotalDelta.text = $"+{wordsTotalDelta}";

        Stats.Save();
    }

    private void MakeUILine(RectTransform a, RectTransform b)
    {
        GameObject newImageObject = Instantiate(prefabUILine, transform.position, Quaternion.identity);
        RectTransform newImageRectTransform = newImageObject.GetComponent<RectTransform>();
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
        uiLines.Add(newImageObject);
    }

    private void Start()
    {
        Config.Load();
        Dictionary.Load();
        Stats.Load();

        GameStart();
    }

    private void Update()
    {
        deltaTimeS += Time.deltaTime;

        if (Config.GameTimed && gameTimeS > 0)
        {
            if (deltaTimeS > 1.0f)
            {
                gameTimeS -= Mathf.FloorToInt(deltaTimeS);
                deltaTimeS = 0f;

                if (gameTimeS <= 0)
                    GameOver();
            }
        }

        if (textCurrentString.text != currentString)
            textCurrentString.text = currentString;

        if (textGameTime.text != gameTimeS.ToString())
            textGameTime.text = gameTimeS.ToString();
    }

    public void OnClickQuitGame()
    {
        Config.Save();
        uiConfig.SetActive(false);
        GameOver();
    }

    public void OnClickRestart()
    {
        GameStart();
        uiGameOver.SetActive(false);
    }

    public void OnClickSettings()
    {
        uiConfig.SetActive(true);
    }

    public void OnClickShuffle()
    {
        Deselect();

        char[] characters = new char[uiChars.Count];

        for (int i = 0; i < uiChars.Count; i++)
            characters[i] = uiChars[i].Character[0];

        characters = characters.OrderBy(x => Guid.NewGuid()).ToArray();

        for (int i = 0; i < uiChars.Count; i++)
            uiChars[i].textChar.text = characters[i].ToString();
    }

    public void OnPointerDown(UIChar uiChar)
    {
        AppendCurrentString(uiChar);

        if (Config.LogPointerEvents)
            Debug.Log($"OnPointerDown {uiChar.Character}");
    }

    public void OnPointerUp(UIChar uiChar)
    {
        CheckCurrentString();

        if (Config.LogPointerEvents)
            Debug.Log($"OnPointerUp {uiChar.Character}");
    }

    public void OnPointerEnter(UIChar uiChar)
    {
        if (uiChar.Selected && uiChar.gameObject == uiCharsSelected[uiCharsSelected.Count - 1].gameObject)
        {
            currentString = currentString.Substring(0, currentString.Length - 1);
                
            uiChar.SetState(UIChar.State.Default);
            uiCharsSelected.RemoveAt(uiCharsSelected.Count - 1);

            if (uiLines.Count > 0)
            {
                Destroy(uiLines[uiLines.Count - 1]);
                uiLines.RemoveAt(uiLines.Count - 1);
            }

            if (wordSolutions.Contains(currentString) && !wordHits.Contains(currentString))
                Handheld.Vibrate();
        }
        else
        {
            AppendCurrentString(uiChar);
        }

        if (Config.LogPointerEvents)
            Debug.Log($"OnPointerEnter {uiChar.Character}");
    }

    public void SetControlRadius()
    {
        for (int i = 0; i < uiChars.Count; ++i)
        {
            float angle = i * (360f / Config.WordLengthMax) * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle + 45) * Config.ControlRadiusPx;
            float y = Mathf.Sin(angle + 45) * Config.ControlRadiusPx;
            (uiChars[i].transform as RectTransform).anchoredPosition = new Vector2(x, y);
        }
    }

    public void ScreenOnPointerUp()
    {
        CheckCurrentString();

        if (Config.LogPointerEvents)
            Debug.Log($"ScreenOnPointerUp");
    }
}

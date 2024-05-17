using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordScrapes : MonoBehaviour
{
    public static int GamesPlayed;
    public static int WordsFound;
    public static int WordsTotal;

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

    public static void Load()
    {
        GamesPlayed = PlayerPrefs.GetInt(nameof(GamesPlayed), 0);
        WordsFound = PlayerPrefs.GetInt(nameof(WordsFound), 0);
        WordsTotal = PlayerPrefs.GetInt(nameof(WordsTotal), 0);
    }

    public static void Save()
    {
        PlayerPrefs.SetInt(nameof(GamesPlayed), GamesPlayed);
        PlayerPrefs.SetInt(nameof(WordsFound), WordsFound);
        PlayerPrefs.SetInt(nameof(WordsTotal), WordsTotal);

        PlayerPrefs.Save();
    }

    private void AppendCurrentString(UIChar uiChar)
    {
        if (uiChar.Selected)
            return;

        currentString += uiChar.Character;

        uiCharsSelected.Add(uiChar.transform as RectTransform);
        uiChar.SetState(UIChar.State.Selected);

        if (uiCharsSelected.Count > 1)
            MakeUILine(uiCharsSelected[uiCharsSelected.Count - 2].transform as RectTransform, uiCharsSelected[uiCharsSelected.Count - 1].transform as RectTransform);

        if (UIConfig.VibrateOnHighlight && wordSolutions.Contains(currentString) && !wordHits.Contains(currentString))
            Handheld.Vibrate();
    }

    private void Awake()
    {
        uiGameOver.SetActive(false);
        uiConfig.SetActive(false);
    }

    private void CheckCurrentString()
    {
        foreach (var sw in uiWords)
        {
            if (sw.word == currentString && !wordHits.Contains(currentString))
            {
                wordHits.Add(currentString);
                sw.SetState(UIWord.State.Hit);

                if (!UIConfig.VibrateOnHighlight)
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

    private void DeselectOne(UIChar uiChar)
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

    private void GameStart()
    {
        // Cleanup
        currentString = string.Empty;
        deltaTimeS = 0f;
        gameTimeS = UIConfig.GameTimeSeconds;
        textCurrentString.text = string.Empty;
        textGameTime.text = UIConfig.GameTimeSeconds.ToString();

        imageBackgroundA.gameObject.SetActive(UnityEngine.Random.value > 0.5f);
        imageBackgroundB.gameObject.SetActive(!imageBackgroundA.gameObject.activeSelf);
        textGameTime.gameObject.SetActive(UIConfig.GameTimed);

        uiLines.Clear();
        wordHits.Clear();
        wordSolutions.Clear();
        uiChars.Clear();
        uiCharsSelected.Clear();
        uiWords.Clear();

        for (int i = rectUIWords.childCount - 1; i >= 0; --i)
            Destroy(rectUIWords.GetChild(i).gameObject);

        for (int i = rectUIChars.childCount - 1; i >= 0; --i)
            Destroy(rectUIChars.GetChild(i).gameObject);
        
        // Generate Solution
        List<string> filteredStrings = Dictionary.lines.Where(s => s.Length == UIConfig.WordLength).ToList();
        string pickedWord = filteredStrings[UnityEngine.Random.Range(0, filteredStrings.Count)];

        while (UIConfig.Blacklist.Contains(pickedWord))
            pickedWord = filteredStrings[UnityEngine.Random.Range(0, filteredStrings.Count)];

        Dictionary<string, int> dict2 = new Dictionary<string, int>();
        foreach (var w in Dictionary.lines)
            dict2[w] = 1;

        List<char> c = new List<char>();
        foreach (char ch in pickedWord)
            c.Add(ch);

        List<(string f, List<char> l)> s = new List<(string f, List<char> l)>();
        s.Add(("", c.ToList()));

        int maxlen = 0;
        var start = DateTime.Now;

        int iterations = 0;

        while (s.Count > 0)
        {
            var ar = s[s.Count - 1];
            s.RemoveAt(s.Count - 1);

            string word = ar.f;
            if (dict2.ContainsKey(word))
                if (word.Length > 1 && !wordSolutions.Contains(word))
                    wordSolutions.Add(word);

            for (int i = 0; i < ar.l.Count; i++)
            {
                List<char> newList = new List<char>(ar.l);
                newList.RemoveAt(i);
                s.Add((ar.f + ar.l[i], newList));
            }

            // Perf checking:
            if (s.Count > maxlen) maxlen = s.Count;
            iterations++;
            if (iterations % 1000 == 0)
                Console.WriteLine(iterations + " " + wordSolutions.Count + " " + maxlen);
        }

        // Instance Prefabs
        foreach (string word in wordSolutions)
        {
            GameObject solutionWordGO = Instantiate(prefabUIWord);
            UIWord solutionWord = solutionWordGO.GetComponent<UIWord>();
            solutionWord.onClick = OnClickUIWord;

            solutionWord.Set(word);
            solutionWord.transform.SetParent(rectUIWords);
            uiWords.Add(solutionWord);
        }

        float angleStep = 360f / UIConfig.WordLength;

        for (int i = 0; i < UIConfig.WordLength; ++i)
        {
            GameObject uiCharGO = Instantiate(prefabUIChar);
            UIChar uiChar = uiCharGO.GetComponent<UIChar>();
            uiChar.textChar.text = pickedWord[i].ToString();

            uiChar.onPointerDown += OnPointerDown;
            uiChar.onPointerUp += OnPointerUp;
            uiChar.onPointerEnter += OnPointerEnter;

            uiChar.transform.SetParent(rectUIChars);
            uiChars.Add(uiChar);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectUIWords);
        OnClickShuffle();
        SetControlRadius();
        SetControlScale();

        // Logging
        if (UIConfig.LogSolutionWords || UIConfig.LogPermutations)
            Debug.Log($"Picked {UIConfig.WordLengthMax} letter word: {pickedWord}");

        if (UIConfig.LogDictionary || UIConfig.LogSolutionWords)
            foreach (string word in UIConfig.Blacklist)
                Debug.Log($"Blacklisted {word}");

        /*
        if (UIConfig.LogPermutations)
            foreach (string permutation in permutations)
                Debug.Log(permutation);
        */

        if (UIConfig.LogSolutionWords)
            foreach (string word in wordSolutions)
                Debug.Log(word);
    }

    private void GameOver()
    {
        foreach (var uiWord in uiWords)
            if (!uiWord.Hit)
                uiWord.SetState(UIWord.State.Miss);

        IncrementStats(1, wordHits.Count, wordSolutions.Count);
        uiGameOver.SetActive(true);
    }

    private List<string> GeneratePermutations(string word)
    {
        List<string> permutations = new List<string>();
        bool[] used = new bool[word.Length]; // Array to track used letters

        for (int length = UIConfig.WordLengthMin; length <= UIConfig.WordLengthMax; length++)
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
        float currentFoundPct = WordsTotal == 0 ? 0f : (float)WordsFound / WordsTotal * 100f;

        GamesPlayed += gamesPlayedDelta;
        WordsFound += wordsFoundDelta;
        WordsTotal += wordsTotalDelta;

        float foundPct = (float)WordsFound / WordsTotal * 100f;

        textFoundPct.text = $"{foundPct:F2}%";
        textFoundPctDelta.text = $"{foundPct - currentFoundPct:F2}%";
        textGamesPlayed.text = $"{GamesPlayed}";
        textGamesPlayedDelta.text = $"+{gamesPlayedDelta}";
        textWordsFound.text = $"{WordsFound}";
        textWordsFoundDelta.text = $"+{wordsFoundDelta}";
        textWordsTotal.text = $"{WordsTotal}";
        textWordsTotalDelta.text = $"+{wordsTotalDelta}";

        Save();
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
        UIConfig.Load();
        Dictionary.Load();
        Load();

        GameStart();
    }

    private void Update()
    {
        if (uiConfig.gameObject.activeSelf)
            return;

        deltaTimeS += Time.deltaTime;

        if (UIConfig.GameTimed && gameTimeS > 0 && deltaTimeS > 1.0f)
        {
            gameTimeS -= Mathf.FloorToInt(deltaTimeS);
            deltaTimeS = 0f;

            if (gameTimeS <= 0)
                GameOver();
        }

        if (textCurrentString.text != currentString)
            textCurrentString.text = currentString;

        if (textGameTime.text != gameTimeS.ToString())
            textGameTime.text = gameTimeS.ToString();

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
    }

    public void OnClickQuitGame()
    {
        UIConfig.Save();
        uiConfig.SetActive(false);
        GameOver();
    }

    public void OnClickNewGame()
    {
        UIConfig.Save();
        GameStart();
        uiGameOver.SetActive(false);
    }

    public void OnClickSettings()
    {
        Deselect();
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

    public void OnClickUIWord(UIWord uiWord)
    {
        if (uiWord.state == UIWord.State.Blacklist)
        {
            UIConfig.Blacklist.Remove(uiWord.word);
            uiWord.SetState(wordHits.Contains(uiWord.word) ? UIWord.State.Hit : UIWord.State.Miss);
        }
        else if (uiWord.state != UIWord.State.Default)
        {
            UIConfig.Blacklist.Add(uiWord.word);
            uiWord.SetState(UIWord.State.Blacklist);
        }

        if (UIConfig.LogPointerEvents)
            Debug.Log($"OnClickUIWord {uiWord.word}");
    }

    public void OnPointerDown(UIChar uiChar)
    {
        AppendCurrentString(uiChar);

        if (UIConfig.LogPointerEvents)
            Debug.Log($"OnPointerDown {uiChar.Character}");
    }

    public void OnPointerUp(UIChar uiChar)
    {
        CheckCurrentString();

        if (UIConfig.LogPointerEvents)
            Debug.Log($"OnPointerUp {uiChar.Character}");
    }

    public void OnPointerEnter(UIChar uiChar)
    {
        if (uiChar.Selected && uiChar.gameObject == uiCharsSelected[^1].gameObject)
            DeselectOne(uiChar);
        else
            AppendCurrentString(uiChar);

        if (UIConfig.LogPointerEvents)
            Debug.Log($"OnPointerEnter {uiChar.Character}");
    }

    public void ScreenOnPointerUp()
    {
        CheckCurrentString();

        if (UIConfig.LogPointerEvents)
            Debug.Log($"ScreenOnPointerUp");
    }

    public void SetControlRadius()
    {
        for (int i = 0; i < uiChars.Count; ++i)
        {
            float angle = i * (360f / UIConfig.WordLength) * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle + 45) * UIConfig.ControlRadiusPx;
            float y = Mathf.Sin(angle + 45) * UIConfig.ControlRadiusPx;
            (uiChars[i].transform as RectTransform).anchoredPosition = new Vector2(x, y);
        }
    }

    public void SetControlScale()
    {
        for (int i = 0; i < uiChars.Count; ++i)
            (uiChars[i].transform as RectTransform).localScale = new Vector3(UIConfig.ControlScale, UIConfig.ControlScale, UIConfig.ControlScale);
    }
}

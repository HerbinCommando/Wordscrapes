using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordScrapes : MonoBehaviour
{
    public GameObject prefabUIChar;
    public GameObject prefabUILine;
    public GameObject prefabUIWord;
    public GridLayoutGroup gridLayoutGroup;
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
    public UIBackgrounds uiBackgrounds;
    public UIConfig uiConfig;
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
        uiChar.SetState(UIChar.State.Green);

        if (uiCharsSelected.Count > 1)
            MakeUILine(uiCharsSelected[uiCharsSelected.Count - 2].transform as RectTransform, uiCharsSelected[uiCharsSelected.Count - 1].transform as RectTransform);

        if (Config.VibrateOnHighlight && wordSolutions.Contains(currentString) && !wordHits.Contains(currentString))
            Handheld.Vibrate();
    }

    private void CheckCurrentString()
    {
        foreach (var sw in uiWords)
        {
            if (sw.value == currentString && !wordHits.Contains(currentString))
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
        // Cleanup
        currentString = string.Empty;
        deltaTimeS = 0f;
        gameTimeS = Config.GameTimeSeconds;
        gridLayoutGroup.cellSize = new Vector2(32 * Config.WordLength, gridLayoutGroup.cellSize.y);
        textCurrentString.text = string.Empty;
        textGameTime.text = $"{Config.GameTimeSeconds}";

        textGameTime.gameObject.SetActive(Config.GameTimed);
        uiBackgrounds.Shuffle();
        uiChars.Clear();
        uiCharsSelected.Clear();
        uiLines.Clear();
        uiWords.Clear();
        wordHits.Clear();
        wordSolutions.Clear();

        for (int i = rectUIWords.childCount - 1; i >= 0; --i)
            Destroy(rectUIWords.GetChild(i).gameObject);

        for (int i = rectUIChars.childCount - 1; i >= 0; --i)
            Destroy(rectUIChars.GetChild(i).gameObject);
        
        // Generate Solution
        List<string> filteredStrings = Dictionary.lines.Where(s => s.Length == Config.WordLength).ToList();
        string pickedWord = filteredStrings[UnityEngine.Random.Range(0, filteredStrings.Count)];

        while (Config.Blacklist.Contains(pickedWord))
            pickedWord = filteredStrings[UnityEngine.Random.Range(0, filteredStrings.Count)];

        //---- Ray's permutation algorithm.
        // TODO(hg): I think this was missing the permutation 'zen' in the solution word 'enzymatics'?
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
        //-----------------------

        wordSolutions = wordSolutions.OrderBy(s => s.Length).ThenBy(s => s).ToList();

        // Instance Prefabs
        foreach (string word in wordSolutions)
        {
            GameObject instance = Instantiate(prefabUIWord);
            UIWord uiWord = instance.GetComponent<UIWord>();
            uiWord.onClick = OnClickUIWord;

            uiWord.Set(word);
            uiWord.SetState(Config.ShowSolutions ? UIWord.State.Default : UIWord.State.Hidden);
            uiWord.transform.SetParent(rectUIWords);
            uiWords.Add(uiWord);
        }

        float angleStep = 360f / Config.WordLength;

        for (int i = 0; i < Config.WordLength; ++i)
        {
            GameObject instance = Instantiate(prefabUIChar);
            UIChar uiChar = instance.GetComponent<UIChar>();
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
        if (Config.LogSolutionWords || Config.LogPermutations)
            Debug.Log($"Picked {Config.WordLengthMax} letter word: {pickedWord}");

        /*
        if (UIConfig.LogPermutations)
            foreach (string permutation in permutations)
                Debug.Log(permutation);
        */

        if (Config.LogSolutionWords)
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

    private void IncrementStats(int gamesPlayedDelta, int wordsFoundDelta, int wordsTotalDelta)
    {
        float currentFoundPct = Stats.WordsTotal == 0 ? 0f : (float)Stats.WordsFound / Stats.WordsTotal * 100f;

        Stats.GamesPlayed += gamesPlayedDelta;
        Stats.WordsFound += wordsFoundDelta;
        Stats.WordsTotal += wordsTotalDelta;

        float foundPct = (float)Stats.WordsFound / Stats.WordsTotal * 100f;

        textFoundPct.text = $"{foundPct:F2}%";
        textFoundPctDelta.text = $"{foundPct - currentFoundPct:F2}%";
        textGamesPlayed.text = $"{Stats.GamesPlayed}";
        textGamesPlayedDelta.text = $"+{gamesPlayedDelta}";
        textWordsFound.text = $"{Stats.WordsFound}";
        textWordsFoundDelta.text = $"+{wordsFoundDelta}";
        textWordsTotal.text = $"{Stats.WordsTotal}";
        textWordsTotalDelta.text = $"+{wordsTotalDelta}";

        Stats.Save();
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

    private void Start()
    {
        Config.Load();
        Dictionary.Load();
        Stats.Load();

        uiGameOver.SetActive(false);

        GameStart();
    }

    private void Update()
    {
        if (uiConfig.gameObject.activeSelf)
            return;

        deltaTimeS += Time.deltaTime;

        if (Config.GameTimed && gameTimeS > 0 && deltaTimeS > 1.0f)
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
            OnClickSettings();

        if (Input.GetKeyDown(KeyCode.Delete))
            if (uiCharsSelected.Count >= 1)
                DeselectOne(uiCharsSelected[^1].GetComponent<UIChar>());
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
        uiConfig.Activate(Config.Game.WordScrapes);
    }

    public void OnClickShuffle()
    {
        Deselect();

        char[] characters = new char[uiChars.Count];

        for (int i = 0; i < uiChars.Count; i++)
            characters[i] = uiChars[i].Character[0];

        characters = characters.OrderBy(x => Guid.NewGuid()).ToArray();

        for (int i = 0; i < uiChars.Count; i++)
            uiChars[i].textChar.text = $"{characters[i]}";
    }

    public void OnClickUIWord(UIWord uiWord)
    {
        if (uiWord.state == UIWord.State.Blacklist)
        {
            Config.Blacklist.Remove(uiWord.value);
            uiWord.SetState(wordHits.Contains(uiWord.value) ? UIWord.State.Hit : UIWord.State.Miss);
        }
        else if (uiWord.state != UIWord.State.Hidden)
        {
            Config.Blacklist.Add(uiWord.value);
            uiWord.SetState(UIWord.State.Blacklist);
        }

        if (Config.LogPointerEvents)
            Debug.Log($"OnClickUIWord {uiWord.value}");
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
        if (uiChar.Selected && uiChar.gameObject == uiCharsSelected[^1].gameObject)
            DeselectOne(uiChar);
        else
            AppendCurrentString(uiChar);

        if (Config.LogPointerEvents)
            Debug.Log($"OnPointerEnter {uiChar.Character}");
    }

    public void ScreenOnPointerUp()
    {
        CheckCurrentString();

        if (Config.LogPointerEvents)
            Debug.Log($"ScreenOnPointerUp");
    }

    public void SetControlRadius()
    {
        for (int i = 0; i < uiChars.Count; ++i)
        {
            float angle = i * (360f / Config.WordLength) * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle + 45) * Config.ControlRadiusPx;
            float y = Mathf.Sin(angle + 45) * Config.ControlRadiusPx;
            (uiChars[i].transform as RectTransform).anchoredPosition = new Vector2(x, y);
        }
    }

    public void SetControlScale()
    {
        for (int i = 0; i < uiChars.Count; ++i)
            (uiChars[i].transform as RectTransform).localScale = new Vector3(Config.ControlScale, Config.ControlScale, Config.ControlScale);
    }
}

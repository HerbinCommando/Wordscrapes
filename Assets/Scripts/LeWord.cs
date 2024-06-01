using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LeWord : MonoBehaviour
{
    public const int WordCount = 6;
    public const int WordLength = 5;

    public List<List<UIChar>> gameBoard = new List<List<UIChar>>();
    public List<GuessDistribution> guessDistributions = new List<GuessDistribution>();
    public Transform panelStats;
    public Transform panelWords;
    public TextMeshProUGUI textAvgSolve;
    public TextMeshProUGUI textLeWordNumber;
    public TextMeshProUGUI textSolution;
    public TextMeshProUGUI textWinPct;
    public UIBackgrounds uiBackgrounds;
    public UIConfig uiConfig;
    public GameObject uiGameOver;
    public UIKeyboard uiKeyboard;

    bool[] locked = new bool[5];
    bool[] marked = new bool[5];
    string solution = string.Empty;
    string word = string.Empty;
    int wordIdx = 0;

    private void Append(char c)
    {
        if (word.Length == WordLength)
            return;

        word += c.ToString();
        gameBoard[wordIdx][word.Length - 1].textChar.text = c.ToString();
    }

    private void Deselect()
    {
        word = string.Empty;

        foreach (var uiChar in gameBoard[wordIdx])
        {
            uiChar.textChar.text = "_";

            uiChar.SetState(UIChar.State.Default);
        }

        for (int i = 0; i < marked.Length; ++i)
            marked[i] = false;
    }

    private void DeselectOne()
    {
        if (word.Length == 0)
            return;

        if (gameBoard[wordIdx][word.Length - 1].state == UIChar.State.Yellow)
        {
            for (int i = 0; i < WordLength; ++i)
            {
                if (marked[i] && solution[i] == gameBoard[wordIdx][word.Length - 1].Char)
                {
                    marked[i] = false;
                    break;
                }
            }
        }

        gameBoard[wordIdx][word.Length - 1].textChar.text = "_";

        gameBoard[wordIdx][word.Length - 1].SetState(UIChar.State.Default);

        word = word.Substring(0, word.Length - 1);
    }

    private void GameOver()
    {
        ++Stats.LeWordGuesses[wordIdx];

        int avgSolve = Stats.LeWordGuesses.ToList().IndexOf(Stats.LeWordGuesses.Max());
        int totalGames = Stats.LeWordGuesses.Sum();
        textAvgSolve.text = $"{(avgSolve == WordCount ? "NS" : avgSolve)}";
        textLeWordNumber.text = $"{totalGames}";
        textWinPct.text = $"{Mathf.FloorToInt((float)(totalGames - Stats.LeWordGuesses[WordCount]) / totalGames * 100)}";

        for (int i = 0; i < Stats.LeWordGuesses.Length; ++i)
            guessDistributions[i].Set(Stats.LeWordGuesses[i], (float)Stats.LeWordGuesses[i] / totalGames, avgSolve == i);

        /*
         * // Get the width of the parent RectTransform
        float parentWidth = parentRectTransform.rect.width;

        // Calculate the new width based on the percentage
        float newWidth = parentWidth * pct;

        // Set the width of the RectTransform by adjusting the 'right' offset
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newWidth);

        // Adjust the 'right' margin proportionally
        float rightMargin = parentWidth - newWidth;
        rectTransform.offsetMax = new Vector2(-rightMargin, rectTransform.offsetMax.y);
        */

        Stats.Save();
        uiGameOver.SetActive(true);
    }

    private void GameStart()
    {
        word = string.Empty;
        wordIdx = 0;

        uiBackgrounds.Shuffle();

        for (int i = 0; i < locked.Length; ++i)
            locked[i] = false;

        for (int i = 0; i < marked.Length; ++i)
            marked[i] = false;

        foreach (var uiChar in uiKeyboard.uiChars)
            uiChar.SetState(UIChar.State.Default);

        foreach (var uiCharList in gameBoard)
        {
            foreach (var uiChar in uiCharList)
            {
                uiChar.textChar.text = " ";

                uiChar.SetState(UIChar.State.Default);
            }
        }

        foreach (var uiChar in gameBoard[wordIdx])
            uiChar.textChar.text = "_";

        List<string> filteredStrings = Dictionary.lines.Where(s => s.Length == 5).ToList();
        solution = filteredStrings[Random.Range(0, filteredStrings.Count)];
        textSolution.text = $"Le Word: {solution}";
    }

    private void Start()
    {
        Config.Load();
        Dictionary.Load();
        Stats.Load();

        guessDistributions = panelStats.GetComponentsInChildren<GuessDistribution>().ToList();

        uiGameOver.SetActive(false);
        uiKeyboard.onBackspaceDown += (_) => { DeselectOne(); };
        uiKeyboard.onEnterDown += (_) => { Submit(); };
        uiKeyboard.onPointerDown += (UIChar uiChar) => {
            if (!uiChar.Disabled)
                Append(uiChar.Char);
        };

        for (int i = 0; i < panelWords.childCount; ++i)
        {
            var horizontalLayout = panelWords.GetChild(i).transform;
            gameBoard.Add(new List<UIChar>());

            for (int j = 0; j < horizontalLayout.childCount; ++j)
                gameBoard[i].Add(horizontalLayout.GetChild(j).GetComponent<UIChar>());
        }

        GameStart();
    }

    private void Update()
    {
        if (uiGameOver.activeSelf)
            return;

        foreach (var uiChar in uiKeyboard.uiChars)
            if (Input.GetKeyDown(uiChar.KeyCode) && !uiChar.Disabled)
                Append(uiChar.Char);

        if (Input.GetKeyDown(KeyCode.Escape))
            Deselect();

        if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Backspace))
            DeselectOne();

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            Submit();
    }

    private void Submit()
    {
        bool enteredLockedLetters = true;

        for (int i = 0; i < WordLength; ++i)
        {
            if (locked[i] && word[i] != solution[i])
            {
                enteredLockedLetters = false;
                break;
            }
        }

        if (enteredLockedLetters && word.Length == WordLength && Dictionary.Contains(word))
        {
            for (int i = 0; i < WordLength; ++i)
            {
                if (solution[i] == word[i])
                {
                    locked[i] = true;
                    marked[i] = true;

                    gameBoard[wordIdx][i].SetState(UIChar.State.Green);

                    foreach (var uiChar in uiKeyboard.uiChars)
                        if (uiChar.Char == solution[i])
                            uiChar.SetState(UIChar.State.Green);
                }
            }

            for (int i = 0; i < WordLength; ++i)
            {
                if (gameBoard[wordIdx][i].state == UIChar.State.Default)
                {
                    for (int j = 0; j < WordLength; ++j)
                    {
                        if (!marked[j] && solution[j] == gameBoard[wordIdx][i].Char)
                        {
                            marked[j] = true;

                            gameBoard[wordIdx][i].SetState(UIChar.State.Yellow);

                            foreach (var uiChar in uiKeyboard.uiChars)
                                if (uiChar.Char == solution[j])
                                    uiChar.SetState(UIChar.State.Yellow);
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < WordLength; ++i)
                if (gameBoard[wordIdx][i].state == UIChar.State.Default)
                    foreach (var uiChar in uiKeyboard.uiChars)
                        if (uiChar.Char == gameBoard[wordIdx][i].Char && !solution.Contains(uiChar.Char))
                            uiChar.SetState(UIChar.State.Disabled);

            if (word == solution)
            {
                GameOver();

                return;
            }

            word = string.Empty;

            for (int i = 0; i < marked.Length; ++i)
                marked[i] = false;

            if (++wordIdx == WordCount)
                GameOver();
            else
                foreach (var uiChar in gameBoard[wordIdx])
                    uiChar.textChar.text = "_";
        }
    }

    public void OnClickLeWord()
    {
        if (Config.Blacklist.Contains(solution))
        {
            textSolution.text = $"Le Word: {solution}";

            Config.Blacklist.Remove(solution);
            Config.Save();
        }
        else
        {
            textSolution.text = $"Le Word: <s><color=red>{solution}</color></s>";

            Config.Blacklist.Add(solution);
            Config.Save();
        }
    }

    public void OnClickNewGame()
    {
        GameStart();
        uiGameOver.SetActive(false);
    }

    public void OnClickQuitGame()
    {
        wordIdx = WordCount;

        GameOver();
    }

    public void OnClickSettings()
    {
        uiConfig.Activate(Config.Game.LeWord);
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeWord : MonoBehaviour
{
    public const int WordCount = 6;
    public const int WordLength = 5;

    public List<List<UIChar>> gameBoard;
    public Transform panelWords;
    public UIKeyboard uiKeyboard;

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

    private void GameStart()
    {
        word = string.Empty;
        wordIdx = 0;

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
    }

    private void Start()
    {
        Dictionary.Load();

        gameBoard = new List<List<UIChar>>();
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
        if (word.Length == WordLength && Dictionary.lines.Contains(word))
        {
            for (int i = 0; i < WordLength; ++i)
            {
                if (solution[i] == word[i])
                {
                    marked[i] = true;

                    gameBoard[wordIdx][i].SetState(UIChar.State.Selected);

                    foreach (var uiChar in uiKeyboard.uiChars)
                        if (uiChar.Char == solution[i])
                            uiChar.SetState(UIChar.State.Selected);
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
                GameStart(); // Game Over, you win!
                return;
            }

            word = string.Empty;

            for (int i = 0; i < marked.Length; ++i)
                marked[i] = false;

            if (++wordIdx == WordCount)
                GameStart();
            else
                foreach (var uiChar in gameBoard[wordIdx])
                    uiChar.textChar.text = "_";
        }
    }
}

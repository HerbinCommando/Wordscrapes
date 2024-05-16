using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeWord : MonoBehaviour
{
    public const int WordCount = 6;
    public const int WordLength = 5;

    public Transform VerticalLayout;
    public List<List<UIChar>> GameBoard;

    string solution = string.Empty;
    string word = string.Empty;
    int wordIdx = 0;

    private void GameStart()
    {
        word = string.Empty;
        wordIdx = 0;

        foreach (var uiCharList in GameBoard)
        {
            foreach (var uiChar in uiCharList)
            {
                uiChar.textChar.text = "_";

                uiChar.SetState(UIChar.State.Default);
            }
        }

        // Generate Solution
        List<string> filteredStrings = Dictionary.lines.Where(s => s.Length == 5).ToList();
        solution = filteredStrings[UnityEngine.Random.Range(0, filteredStrings.Count)];

        Debug.Log(solution);
    }

    private void Start()
    {
        Dictionary.Load();

        GameBoard = new List<List<UIChar>>();

        for (int i = 0; i < VerticalLayout.childCount; ++i)
        {
            var horizontalLayout = VerticalLayout.GetChild(i).transform;
            GameBoard.Add(new List<UIChar>());

            for (int j = 0; j < horizontalLayout.childCount; ++j)
            {
                GameBoard[i].Add(horizontalLayout.GetChild(j).GetComponent<UIChar>());
            }
        }

        GameStart();
    }

    private void Update()
    {
        for (KeyCode key = KeyCode.A; key <= KeyCode.Z; key++)
        {
            if (Input.GetKeyDown(key))
            {
                word += key.ToString();
                GameBoard[wordIdx][word.Length - 1].textChar.text = key.ToString();

                if (solution[word.Length - 1] == word[word.Length - 1])
                    GameBoard[wordIdx][word.Length - 1].SetState(UIChar.State.Selected);

                if (word.Length == WordLength)
                {
                    if (word == solution)
                    {
                        GameStart(); // Game Over, you win!
                        return;
                    }

                    word = string.Empty;

                    if (++wordIdx == WordCount)
                        GameStart(); // Game Over, you lose!
                }
            }
        }

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

}

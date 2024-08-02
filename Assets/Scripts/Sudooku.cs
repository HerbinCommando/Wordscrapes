﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class Sudooku : MonoBehaviour
{
    public RectTransform rectUIChars;
    public RectTransform rectUINumpad;
    public UIBackgrounds uiBackgrounds;
    public UIConfig uiConfig;
    public GameObject uiGameOver;

    private int[,] solution = null;
    private UIChar selected = null;
    private List<UIChar> uiChars = new List<UIChar>();
    private List<UIChar> uiNumpad = new List<UIChar>();

    private readonly Random random = new Random();

    private void CheckSolved()
    {
    }

    private void GameOver()
    {
        foreach (var uiChar in uiChars)
            uiChar.onPointerDown -= OnSudookuDown;

        uiGameOver.SetActive(true);
    }

    private void GameStart()
    {
        solution = GenerateSudoku();

        int[,] solvableSudoku = (int[,])solution.Clone();
        List<int> cells = Enumerable.Range(0, 81).OrderBy(c => random.Next()).ToList();

        NumpadEnabled(false);

        for (int i = 0; i < Config.Blanks; ++i)
        {
            int cell = cells[i];
            int row = cell / 9;
            int col = cell % 9;
            int temp = solvableSudoku[row, col];
            solvableSudoku[row, col] = 0;
        }

        int idx = 0;
        for (int row = 0; row < 9; ++row)
        {
            for (int col = 0; col < 9; ++col)
            {
                bool blank = solvableSudoku[row, col] == 0;
                UIChar uiChar = uiChars[idx++];

                uiChar.textChar.text = blank ? string.Empty : $"{solvableSudoku[row, col]}";

                uiChar.SetItalic(!blank);
                uiChar.SetState(UIChar.State.Default);

                if (blank)
                    uiChar.onPointerDown += OnSudookuDown;
            }
        }
    }

    private void NumpadEnabled(bool value)
    {
        foreach (var uiChar in uiNumpad)
            uiChar.SetState(value ? UIChar.State.Default : UIChar.State.Disabled);
    }

    private void OnSudookuDown(UIChar uiChar)
    {
        if (selected)
            selected.SetState(UIChar.State.Default);

        if (selected == uiChar)
        {
            selected = null;

            NumpadEnabled(false);

            return;
        }

        selected = uiChar;

        NumpadEnabled(true);
        selected.SetState(UIChar.State.DodgerBlue);
    }

    private void OnNumpadDown(UIChar uiChar)
    {
        if (selected)
            selected.textChar.text = uiChar.textChar.text;

        CheckSolved();
    }

    private void Start()
    {
        Config.Load();
        Stats.Load();

        uiGameOver.SetActive(false);

        UIChar[] instances = rectUIChars.GetComponentsInChildren<UIChar>();
        for (int i = 0; i < instances.Length; ++i)
            uiChars.Add(instances[i]);

        instances = rectUINumpad.GetComponentsInChildren<UIChar>();
        for (int i = 0; i < instances.Length; ++i)
        {
            instances[i].textChar.text = $"{i + 1}";

            instances[i].onPointerDown += OnNumpadDown;

            uiNumpad.Add(instances[i]);
        }

        GameStart();
    }

    public void OnClickNewGame()
    {
        GameStart();
        uiGameOver.SetActive(false);
    }

    public void OnClickQuitGame()
    {
        GameOver();
    }

    public void OnClickSettings()
    {
        uiConfig.Activate(Config.Game.Sudooku);
    }

    private int[,] GenerateSudoku()
    {
        var grid = new int[9, 9];
        FillGrid(grid);
        return grid;
    }

    private bool FillGrid(int[,] grid)
    {
        var cell = FindEmptyCell(grid);
        if (cell == null)
        {
            return true;
        }

        var (row, col) = cell.Value;
        var numbers = Enumerable.Range(1, 9).OrderBy(n => random.Next()).ToList();

        foreach (var number in numbers)
        {
            if (IsSafe(grid, row, col, number))
            {
                grid[row, col] = number;
                if (FillGrid(grid))
                {
                    return true;
                }
                grid[row, col] = 0;
            }
        }
        return false;
    }

    private (int, int)? FindEmptyCell(int[,] grid)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (grid[row, col] == 0)
                {
                    return (row, col);
                }
            }
        }
        return null;
    }

    private bool IsSafe(int[,] grid, int row, int col, int num)
    {
        return !IsInRow(grid, row, num) && !IsInCol(grid, col, num) && !IsInBox(grid, row - row % 3, col - col % 3, num);
    }

    private bool IsInRow(int[,] grid, int row, int num)
    {
        for (int col = 0; col < 9; col++)
        {
            if (grid[row, col] == num)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsInCol(int[,] grid, int col, int num)
    {
        for (int row = 0; row < 9; row++)
        {
            if (grid[row, col] == num)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsInBox(int[,] grid, int startRow, int startCol, int num)
    {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                if (grid[row + startRow, col + startCol] == num)
                {
                    return true;
                }
            }
        }
        return false;
    }
}

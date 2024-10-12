using System;
using System.Collections.Generic;
using UnityEngine;

public static class Config
{
    public enum Game
    {
        Borgle,
        LeWord,
        Sudooku,
        WordScrapes
    }

    public static class Palette
    {
        public static readonly Color BlackKyber = new Color(0.1f, 0.1f, 0.1f, 1.0f);
        public static readonly Color BlueKyber = new Color(0.1f, 0.6f, 1.0f, 1.0f);
        public static readonly Color DodgerBlue = new Color(0.118f, 0.565f, 1.0f, 1.0f);
        public static readonly Color Green = new Color(0, 1, 0, 1);
        public static readonly Color GreenKyber = new Color(0.65f, 1.0f, 0.14f, 1.0f);
        public static readonly Color PurpleKyber = new Color(0.5f, 0.1f, 0.9f, 1.0f);
        public static readonly Color RedKyber = new Color(0.9f, 0.1f, 0.1f, 1.0f);
        public static readonly Color White = new Color(1, 1, 1, 1);
        public static readonly Color WhiteKyber = new Color(0.9f, 0.9f, 0.9f, 1.0f);
        public static readonly Color YellowKyber = new Color(1.0f, 0.9f, 0.1f, 1.0f);
        public static readonly Color Yellow = new Color(1, 1, 0, 1);
    }

    public static readonly Color[] KyberColors = {
        Palette.BlackKyber,
        Palette.BlueKyber,
        Palette.GreenKyber,
        Palette.PurpleKyber,
        Palette.RedKyber,
        Palette.WhiteKyber,
        Palette.YellowKyber
    };

    public static bool LogDictionary = false;
    public static bool LogPermutations = false;
    public static bool LogPointerEvents = false;
    public static bool LogSolutionWords = false;

    public static List<string> Blacklist = new List<string>();
    public static int Blanks = 54;
    public static int BlanksMax = 55;
    public static int BlanksMin = 36;
    public static bool BorgleClassic = true;
    public static bool BorgleTimed = true;
    public static int BorgleTimeS = 180;
    public static bool BorgledLetters = true;
    public static int ControlRadiusPx = 275;
    public const int ControlRadiusPxMax = 500;
    public const int ControlRadiusPxMin = 200;
    public static float ControlScale = 1.3f;
    public const float ControlScaleMax = 2.0f;
    public const float ControlScaleMin = 0.75f;
    public static bool GameTimed = false;
    public static int GameTimeSeconds = 60;
    public const int GameTimeSecondsMax = 300;
    public const int GameTimeSecondsMin = 30;
    public static bool HardMode = true;
    public static int KyberColor = Array.IndexOf(KyberColors, Palette.WhiteKyber);
    public static bool ShowSolutions = false;
    public static bool UnfetteredAllegience = true;
    public static string UserSelectedWord = string.Empty;
    public static bool VibrateOnHighlight = false;
    public static int WordLength = 5;
    public const int WordLengthMax = 10;
    public const int WordLengthMin = 3;

    public static void Load()
    {
        Blanks = PlayerPrefs.GetInt(nameof(Blanks), Blanks);
        BorgleClassic = PlayerPrefs.GetInt(nameof(BorgleClassic), 1) == 1;
        BorgleTimed = PlayerPrefs.GetInt(nameof(BorgleTimed), 1) == 1;
        BorgledLetters = PlayerPrefs.GetInt(nameof(BorgledLetters), 1) == 1;
        ControlRadiusPx = PlayerPrefs.GetInt(nameof(ControlRadiusPx), ControlRadiusPx);
        ControlScale = PlayerPrefs.GetFloat(nameof(ControlScale), ControlScale);
        GameTimed = PlayerPrefs.GetInt(nameof(GameTimed), 0) == 1;
        GameTimeSeconds = PlayerPrefs.GetInt(nameof(GameTimeSeconds), GameTimeSeconds);
        HardMode = PlayerPrefs.GetInt(nameof(HardMode), 1) == 1;
        KyberColor = PlayerPrefs.GetInt(nameof(KyberColor), Array.IndexOf(KyberColors, Palette.WhiteKyber));
        ShowSolutions = PlayerPrefs.GetInt(nameof(ShowSolutions), 0) == 1;
        UnfetteredAllegience = PlayerPrefs.GetInt(nameof(UnfetteredAllegience), 0) == 1;
        VibrateOnHighlight = PlayerPrefs.GetInt(nameof(VibrateOnHighlight), 0) == 1;
        WordLength = PlayerPrefs.GetInt(nameof(WordLength), WordLength);

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(nameof(Blacklist))))
            Blacklist = new List<string>(PlayerPrefs.GetString(nameof(Blacklist)).Split(','));
    }

    public static void Save()
    {
        PlayerPrefs.SetString(nameof(Blacklist), string.Join(",", Blacklist.ToArray()));
        PlayerPrefs.SetInt(nameof(Blanks), Blanks);
        PlayerPrefs.SetInt(nameof(BorgleClassic), BorgleClassic ? 1 : 0);
        PlayerPrefs.SetInt(nameof(BorgleTimed), BorgleTimed ? 1 : 0);
        PlayerPrefs.SetInt(nameof(BorgledLetters), BorgledLetters ? 1 : 0);
        PlayerPrefs.SetFloat(nameof(ControlScale), ControlScale);
        PlayerPrefs.SetInt(nameof(ControlRadiusPx), ControlRadiusPx);
        PlayerPrefs.SetInt(nameof(GameTimed), GameTimed ? 1 : 0);
        PlayerPrefs.SetInt(nameof(GameTimeSeconds), GameTimeSeconds);
        PlayerPrefs.SetInt(nameof(HardMode), HardMode ? 1 : 0);
        PlayerPrefs.SetInt(nameof(KyberColor), KyberColor);
        PlayerPrefs.SetInt(nameof(ShowSolutions), ShowSolutions ? 1 : 0);
        PlayerPrefs.SetInt(nameof(ShowSolutions), ShowSolutions ? 1 : 0);
        PlayerPrefs.SetInt(nameof(UnfetteredAllegience), UnfetteredAllegience ? 1 : 0);
        PlayerPrefs.SetInt(nameof(WordLength), WordLength);

        PlayerPrefs.Save();
    }
}

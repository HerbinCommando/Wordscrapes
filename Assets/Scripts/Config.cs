using System.Collections.Generic;
using UnityEngine;

public static class Config
{
    public enum Game
    {
        Borgle,
        LeWord,
        WordScrapes
    }

    public static bool LogDictionary = false;
    public static bool LogPermutations = false;
    public static bool LogPointerEvents = false;
    public static bool LogSolutionWords = false;

    public static List<string> Blacklist = new List<string>();
    public static bool BorgleClassic = true;
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
    public static bool ShowSolutions = false;
    public static bool VibrateOnHighlight = false;
    public static int WordLength = 5;
    public const int WordLengthMax = 10;
    public const int WordLengthMin = 3;

    public static void Load()
    {
        BorgleClassic = PlayerPrefs.GetInt(nameof(BorgleClassic), 1) == 1;
        ControlRadiusPx = PlayerPrefs.GetInt(nameof(ControlRadiusPx), ControlRadiusPx);
        ControlScale = PlayerPrefs.GetFloat(nameof(ControlScale), ControlScale);
        GameTimed = PlayerPrefs.GetInt(nameof(GameTimed), 0) == 1;
        GameTimeSeconds = PlayerPrefs.GetInt(nameof(GameTimeSeconds), GameTimeSeconds);
        ShowSolutions = PlayerPrefs.GetInt(nameof(ShowSolutions), 0) == 1;
        VibrateOnHighlight = PlayerPrefs.GetInt(nameof(VibrateOnHighlight), 0) == 1;
        WordLength = PlayerPrefs.GetInt(nameof(WordLength), WordLength);

        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(nameof(Blacklist))))
            Blacklist = new List<string>(PlayerPrefs.GetString(nameof(Blacklist)).Split(','));
    }

    public static void Save()
    {
        PlayerPrefs.SetInt(nameof(BorgleClassic), BorgleClassic ? 1 : 0);
        PlayerPrefs.SetFloat(nameof(ControlScale), ControlScale);
        PlayerPrefs.SetInt(nameof(ControlRadiusPx), ControlRadiusPx);
        PlayerPrefs.SetInt(nameof(GameTimed), GameTimed ? 1 : 0);
        PlayerPrefs.SetInt(nameof(GameTimeSeconds), GameTimeSeconds);
        PlayerPrefs.SetInt(nameof(ShowSolutions), ShowSolutions ? 1 : 0);
        PlayerPrefs.SetInt(nameof(VibrateOnHighlight), VibrateOnHighlight ? 1 : 0);
        PlayerPrefs.SetInt(nameof(WordLength), WordLength);
        PlayerPrefs.SetString(nameof(Blacklist), string.Join(",", Blacklist.ToArray()));

        PlayerPrefs.Save();
    }
}

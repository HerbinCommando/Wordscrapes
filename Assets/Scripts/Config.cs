using System.Collections.Generic;
using UnityEngine;

public static class Config
{
    public static bool LogDictionary = false;
    public static bool LogPermutations = false;
    public static bool LogPointerEvents = false;
    public static bool LogSolutionWords = false;

    public static List<string> Blacklist = new List<string>();
    public static int ControlRadiusPx = 275;
    public static int ControlRadiusPxMax = 500;
    public static int ControlRadiusPxMin = 200;
    public static float ControlScale = 1.3f;
    public static float ControlScaleMax = 2.0f;
    public static float ControlScaleMin = 0.75f;
    public static bool GameTimed = false;
    public static int GameTimeSeconds = 60;
    public static int GameTimeSecondsMax = 300;
    public static int GameTimeSecondsMin = 30;
    public static bool ShowSolutions = false;
    public static bool VibrateOnHighlight = false;
    public static int WordLength = 5;
    public static int WordLengthMax = 10;
    public static int WordLengthMin = 3;

    public static void Load()
    {
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

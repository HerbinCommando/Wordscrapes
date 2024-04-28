using UnityEngine;

public static class Config
{
    public static bool LogDictionary = false;
    public static bool LogPermutations = false;
    public static bool LogPointerEvents = false;
    public static bool LogSolutionWords = false;

    public static int ControlRadiusPx = 275;
    public static bool GameTimed = true;
    public static int GameTimeSeconds = 60;
    public static int GameTimeSecondsMax = 300;
    public static int GameTimeSecondsMin = 30;
    public static bool ShowSolutions = false;
    public static bool VibrateOnHighlight = false;
    public static int WordLengthMax = 5;
    public static int WordLengthMin = 3;

    public static void Load()
    {
        GameTimed = PlayerPrefs.GetInt(nameof(GameTimed), 1) == 1;
        GameTimeSeconds = PlayerPrefs.GetInt(nameof(GameTimeSeconds), 60);
        ShowSolutions = PlayerPrefs.GetInt(nameof(ShowSolutions), 0) == 1;
        VibrateOnHighlight = PlayerPrefs.GetInt(nameof(VibrateOnHighlight), 0) == 1;
        WordLengthMax = PlayerPrefs.GetInt(nameof(WordLengthMax), 5);
    }

    public static void Save()
    {
        PlayerPrefs.SetInt(nameof(GameTimed), GameTimed ? 1 : 0);
        PlayerPrefs.SetInt(nameof(GameTimeSeconds), GameTimeSeconds);
        PlayerPrefs.SetInt(nameof(ShowSolutions), ShowSolutions ? 1 : 0);
        PlayerPrefs.SetInt(nameof(VibrateOnHighlight), VibrateOnHighlight ? 1 : 0);
        PlayerPrefs.SetInt(nameof(WordLengthMax), WordLengthMax);

        PlayerPrefs.Save();
    }
}

using UnityEngine;

public static class Stats
{
    public static int gamesPlayed;
    public static int wordsFound;
    public static int wordsTotal;

    public static void Load()
    {
        gamesPlayed = PlayerPrefs.GetInt(nameof(gamesPlayed), 0);
        wordsFound = PlayerPrefs.GetInt(nameof(wordsFound), 0);
        wordsTotal = PlayerPrefs.GetInt(nameof(wordsTotal), 0);
    }

    public static void Save()
    {
        PlayerPrefs.SetInt(nameof(gamesPlayed), gamesPlayed);
        PlayerPrefs.SetInt(nameof(wordsFound), wordsFound);
        PlayerPrefs.SetInt(nameof(wordsTotal), wordsTotal);

        PlayerPrefs.Save();
    }
}
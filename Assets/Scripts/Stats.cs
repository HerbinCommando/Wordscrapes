using UnityEngine;
using System.Collections;

public class Stats : MonoBehaviour
{
    public static int GamesPlayed;
    public static int WordsFound;
    public static int WordsTotal;

    public static void Load()
    {
        GamesPlayed = PlayerPrefs.GetInt(nameof(GamesPlayed), 0);
        WordsFound = PlayerPrefs.GetInt(nameof(WordsFound), 0);
        WordsTotal = PlayerPrefs.GetInt(nameof(WordsTotal), 0);
    }

    public static void Save()
    {
        PlayerPrefs.SetInt(nameof(GamesPlayed), GamesPlayed);
        PlayerPrefs.SetInt(nameof(WordsFound), WordsFound);
        PlayerPrefs.SetInt(nameof(WordsTotal), WordsTotal);

        PlayerPrefs.Save();
    }
}

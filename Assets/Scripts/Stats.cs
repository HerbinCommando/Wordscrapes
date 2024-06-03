using UnityEngine;

public class Stats : MonoBehaviour
{
    public static int GamesPlayed;
    public static int[] GuessDistribution = new int[7];
    public static int HighScore;
    public static int WordsFound;
    public static int WordsTotal;

    public static void Load()
    {
        GamesPlayed = PlayerPrefs.GetInt(nameof(GamesPlayed), 0);
        HighScore = PlayerPrefs.GetInt(nameof(HighScore), 0);
        WordsFound = PlayerPrefs.GetInt(nameof(WordsFound), 0);
        WordsTotal = PlayerPrefs.GetInt(nameof(WordsTotal), 0);

        if (PlayerPrefs.HasKey(nameof(GuessDistribution)))
        {
            string[] leWordGuesses = PlayerPrefs.GetString(nameof(GuessDistribution)).Split(',');

            for (int i = 0; i < leWordGuesses.Length; ++i)
                GuessDistribution[i] = int.Parse(leWordGuesses[i]);
        }
    }

    public static void Save()
    {
        PlayerPrefs.SetInt(nameof(GamesPlayed), GamesPlayed);
        PlayerPrefs.SetInt(nameof(HighScore), HighScore);
        PlayerPrefs.SetInt(nameof(WordsFound), WordsFound);
        PlayerPrefs.SetInt(nameof(WordsTotal), WordsTotal);
        PlayerPrefs.SetString(nameof(GuessDistribution), string.Join(",", GuessDistribution));

        PlayerPrefs.Save();
    }
}

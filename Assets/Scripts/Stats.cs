using UnityEngine;

public class Stats : MonoBehaviour
{
    public static int GamesPlayed;
    public static int WordsFound;
    public static int WordsTotal;
    public static int[] LeWordGuesses = new int[7];

    public static void Load()
    {
        GamesPlayed = PlayerPrefs.GetInt(nameof(GamesPlayed), 0);
        WordsFound = PlayerPrefs.GetInt(nameof(WordsFound), 0);
        WordsTotal = PlayerPrefs.GetInt(nameof(WordsTotal), 0);

        if (PlayerPrefs.HasKey(nameof(LeWordGuesses)))
        {
            string[] leWordGuesses = PlayerPrefs.GetString(nameof(LeWordGuesses)).Split(',');

            for (int i = 0; i < leWordGuesses.Length; ++i)
                LeWordGuesses[i] = int.Parse(leWordGuesses[i]);
        }
    }

    public static void Save()
    {
        PlayerPrefs.SetInt(nameof(GamesPlayed), GamesPlayed);
        PlayerPrefs.SetInt(nameof(WordsFound), WordsFound);
        PlayerPrefs.SetInt(nameof(WordsTotal), WordsTotal);
        PlayerPrefs.SetString(nameof(LeWordGuesses), string.Join(",", LeWordGuesses));

        PlayerPrefs.Save();
    }
}

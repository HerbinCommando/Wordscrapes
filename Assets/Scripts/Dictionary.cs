using UnityEngine;
using System.Collections.Generic;

public static class Dictionary
{
    public static string fileName = "scrabble_dictionary"; // File name without extension
    public static List<string> lines = new List<string>();

    public static void Load()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);
        string[] fileLines = textAsset.text.Split('\n');

        for (int i = 0; i < fileLines.Length; i++)
        {
            string line = fileLines[i].Trim();

            if (line.Length >= Config.WordLengthMin)
                lines.Add(line);
        }

        if (Config.LogDictionary)
            foreach (string line in lines)
                Debug.Log(line);
    }
}

using System;
using System.IO;
using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Networking;
#endif

public static class BuildNumber
{
    private const string FileName = "build_number.txt";
    private static readonly string FullPath = Path.Combine(Application.streamingAssetsPath, FileName);

    public static string Read()
    {
        try
        {
#if UNITY_EDITOR
            var utcNow = DateTime.UtcNow;
            string year = utcNow.Year.ToString();
            string month = utcNow.Month.ToString("00");
            string day = utcNow.Day.ToString("00");

            return $"{year}{month}{day}E";
#elif UNITY_ANDROID
            UnityWebRequest www = UnityWebRequest.Get(FullPath);

            www.SendWebRequest();
            while (!www.isDone);

            if (www.result == UnityWebRequest.Result.Success)
            return www.downloadHandler.text;
#else
            using StreamReader streamReader = new StreamReader(FullPath);
            return streamReader.ReadToEnd();
#endif
        }
        catch (Exception ex)
        {
            Debug.LogError($"[BuildNumber.Read] EXCEPTION {ex}");
        }

        return string.Empty;
    }

#if UNITY_EDITOR
    public static void Write(string buildNumber)
    {
        try
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
                Directory.CreateDirectory(Application.streamingAssetsPath);

            using StreamWriter streamWriter = new StreamWriter(FullPath);
            streamWriter.Write(buildNumber);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[BuildNumber.Write] EXCEPTION {ex}");
        }
    }
#endif
}
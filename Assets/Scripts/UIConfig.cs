using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIConfig : MonoBehaviour
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

    public GameObject blacklist;
    public GameObject prefabUIWord;
    public RectTransform rectUIWords;
    public Slider sliderControlRadius;
    public Slider sliderControlScale;
    public Slider sliderGameTime;
    public Slider sliderWordLength;
    public TextMeshProUGUI textControlRadiusPx;
    public TextMeshProUGUI textControlScale;
    public TextMeshProUGUI textGameTimeSeconds;
    public TextMeshProUGUI textWordLength;
    public Toggle toggleGameTimed;
    public Toggle toggleShowSolutions;
    public Toggle toggleVibrateOnHighlight;
    public Toggle toggleVibrateOnSubmit;
    public UISplashScreen uiSplashScreen;

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

    public void OnClickBlacklistClose()
    {
        blacklist.SetActive(false);
        UIConfig.Save();

        for (int i = rectUIWords.childCount - 1; i >= 0; --i)
            Destroy(rectUIWords.GetChild(i).gameObject);
    }

    public void OnClickBlacklistOpen()
    {
        blacklist.SetActive(true);

        foreach (string word in UIConfig.Blacklist)
        {
            GameObject solutionWordGO = Instantiate(prefabUIWord);
            UIWord solutionWord = solutionWordGO.GetComponent<UIWord>();
            solutionWord.onClick = OnClickUIWord;

            solutionWord.Set(word);
            solutionWord.SetState(UIWord.State.Miss);
            solutionWord.transform.SetParent(rectUIWords);
        }
    }

    public void OnClickClose()
    {
        gameObject.SetActive(false);
    }

    public void OnClickUIWord(UIWord uiWord)
    {
        if (uiWord.state == UIWord.State.Blacklist)
        {
            UIConfig.Blacklist.Add(uiWord.word);
            uiWord.SetState(UIWord.State.Miss);
        }
        else
        {
            UIConfig.Blacklist.Remove(uiWord.word);
            uiWord.SetState(UIWord.State.Blacklist);
        }

        if (UIConfig.LogPointerEvents)
            Debug.Log($"OnClickUIWord {uiWord.word}");
    }

    public void OnClickMainMenu()
    {
        gameObject.SetActive(false);
        uiSplashScreen.gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        sliderControlRadius.maxValue = UIConfig.ControlRadiusPxMax;
        sliderControlRadius.minValue = UIConfig.ControlRadiusPxMin;
        sliderControlRadius.value = UIConfig.ControlRadiusPx;
        sliderControlScale.maxValue = UIConfig.ControlScaleMax;
        sliderControlScale.minValue = UIConfig.ControlScaleMin;
        sliderControlScale.value = UIConfig.ControlScale;
        sliderWordLength.maxValue = UIConfig.WordLengthMax;
        sliderWordLength.minValue = UIConfig.WordLengthMin;
        sliderWordLength.value = UIConfig.WordLength;
        sliderGameTime.maxValue = UIConfig.GameTimeSecondsMax;
        sliderGameTime.minValue = UIConfig.GameTimeSecondsMin;
        sliderGameTime.value = UIConfig.GameTimeSeconds;
        textControlRadiusPx.text = $"CONTROL RADIUS: {UIConfig.ControlRadiusPx}px";
        textControlScale.text = $"CONTROL SCALE: {UIConfig.ControlScale}";
        textGameTimeSeconds.text = $"GAME TIME: {UIConfig.GameTimeSeconds}s";
        textWordLength.text = $"WORD LENGTH: {UIConfig.WordLength}";
        toggleGameTimed.isOn = UIConfig.GameTimed;
        toggleShowSolutions.isOn = UIConfig.ShowSolutions;
        toggleVibrateOnHighlight.isOn = UIConfig.VibrateOnHighlight;
        toggleVibrateOnSubmit.isOn = !UIConfig.VibrateOnHighlight;

        blacklist.SetActive(false);
    }

    public void SetControlRadius(int _)
    {
        UIConfig.ControlRadiusPx = (int)sliderControlRadius.value;
        textControlRadiusPx.text = $"CONTROL RADIUS: {UIConfig.ControlRadiusPx}px";
    }

    public void SetControlScale(float _)
    {
        UIConfig.ControlScale = sliderControlScale.value;
        textControlScale.text = $"CONTROL SCALE: {UIConfig.ControlScale:F2}";
    }

    public void SetGameTime(int _)
    {
        UIConfig.GameTimeSeconds = (int)sliderGameTime.value;
        textGameTimeSeconds.text = $"GAME TIME: {UIConfig.GameTimeSeconds}s";
    }

    public void SetGameTimed(bool _)
    {
        UIConfig.GameTimed = toggleGameTimed.isOn;
    }

    public void SetShowSolutions(bool _)
    {
        UIConfig.ShowSolutions = toggleShowSolutions.isOn;
    }

    public void SetVibrateOnHighlight(bool _)
    {
        UIConfig.VibrateOnHighlight = toggleVibrateOnHighlight.isOn;
        toggleVibrateOnSubmit.isOn = !toggleVibrateOnHighlight.isOn;
    }

    public void SetVibrateOnSubmit(bool _)
    {
        UIConfig.VibrateOnHighlight = !toggleVibrateOnSubmit.isOn;
        toggleVibrateOnHighlight.isOn = !toggleVibrateOnSubmit.isOn;
    }

    public void SetWordLength(int _)
    {
        UIConfig.WordLength = (int)sliderWordLength.value;
        textWordLength.text = $"WORD LENGTH: {sliderWordLength.value}";
    }
}

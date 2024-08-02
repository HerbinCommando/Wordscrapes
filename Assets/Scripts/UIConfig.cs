using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIConfig : MonoBehaviour
{
    public GameObject buttonQuitBorgle;
    public GameObject buttonQuitLeWord;
    public GameObject buttonQuitSudooku;
    public GameObject buttonQuitWordScrapes;
    public GameObject prefabUIWord;
    public RectTransform rectUIWords;
    public GameObject scrollPanelBorgleUIConfig;
    public GameObject scrollPanelLeWordUIConfig;
    public GameObject scrollPanelSudookuUIConfig;
    public GameObject scrollPanelWordScrapesUIConfig;
    public Slider sliderBlanks;
    public Slider sliderControlRadius;
    public Slider sliderControlScale;
    public Slider sliderGameTime;
    public Slider sliderWordLength;
    public TextMeshProUGUI textBlanks;
    public TextMeshProUGUI textBorgleClassic;
    public TextMeshProUGUI textBorgleModern;
    public TextMeshProUGUI textControlRadiusPx;
    public TextMeshProUGUI textControlScale;
    public TextMeshProUGUI textDifficulty;
    public TextMeshProUGUI textGameTimeSeconds;
    public TextMeshProUGUI textHardMode;
    public TextMeshProUGUI textHighScore;
    public TextMeshProUGUI textWordLength;
    public Toggle toggleBorgleClassic;
    public Toggle toggleBorgleModern;
    public Toggle toggleBorgleTimed;
    public Toggle toggleBorgledLetters;
    public Toggle toggleGameTimed;
    public Toggle toggleHardMode;
    public Toggle toggleShowSolutions;
    public Toggle toggleVibrateOnHighlight;
    public Toggle toggleVibrateOnSubmit;
    public GameObject uiBlacklist;
    public UISplashScreen uiSplashScreen;

    private readonly Func<string> Difficulty = () =>
    {
        int difficultyStep = (Config.BlanksMax - Config.BlanksMin) / 5;

        //An easy puzzle contains 35 - 45 given numbers, hardest contains 25 - 26.
        if (Config.Blanks <= Config.BlanksMin + difficultyStep)
        {
            return "EASY";
        }
        else if (Config.Blanks <= Config.BlanksMin + 2 * difficultyStep)
        {
            return "MODERATE";
        }
        else if (Config.Blanks <= Config.BlanksMin + 3 * difficultyStep)
        {
            return "HARD";
        }
        else if (Config.Blanks <= Config.BlanksMin + 4 * difficultyStep)
        {
            return "TOUGH";
        }
        else
            return "EXTREME";
    };

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!gameObject.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
            OnClickClose();
    }

    public void Activate(Config.Game game)
    {
        sliderBlanks.maxValue = Config.BlanksMax;
        sliderBlanks.minValue = Config.BlanksMin;
        sliderBlanks.value = Config.Blanks;
        sliderControlRadius.maxValue = Config.ControlRadiusPxMax;
        sliderControlRadius.minValue = Config.ControlRadiusPxMin;
        sliderControlRadius.value = Config.ControlRadiusPx;
        sliderControlScale.maxValue = Config.ControlScaleMax;
        sliderControlScale.minValue = Config.ControlScaleMin;
        sliderControlScale.value = Config.ControlScale;
        sliderWordLength.maxValue = Config.WordLengthMax;
        sliderWordLength.minValue = Config.WordLengthMin;
        sliderWordLength.value = Config.WordLength;
        sliderGameTime.maxValue = Config.GameTimeSecondsMax;
        sliderGameTime.minValue = Config.GameTimeSecondsMin;
        sliderGameTime.value = Config.GameTimeSeconds;
        textBlanks.text = $"BLANKS: {Config.Blanks}";
        textControlRadiusPx.text = $"CONTROL RADIUS: {Config.ControlRadiusPx}px";
        textControlScale.text = $"CONTROL SCALE: {Config.ControlScale}";
        textDifficulty.text = $"DIFFICULTY: {Difficulty()}";
        textGameTimeSeconds.text = $"GAME TIME: {Config.GameTimeSeconds}s";
        textHardMode.text = Config.HardMode ? LeWord.RulesHardOn : LeWord.RulesHardOff;
        textHighScore.text = $"HIGH SCORE: {Stats.HighScore}";
        textWordLength.text = $"WORD LENGTH: {Config.WordLength}";
        toggleBorgleClassic.isOn = Config.BorgleClassic;
        toggleBorgleModern.isOn = !Config.BorgleClassic;
        toggleBorgleTimed.isOn = Config.BorgleTimed;
        toggleBorgledLetters.isOn = Config.BorgledLetters;
        toggleGameTimed.isOn = Config.GameTimed;
        toggleHardMode.isOn = Config.HardMode;
        toggleShowSolutions.isOn = Config.ShowSolutions;
        toggleVibrateOnHighlight.isOn = Config.VibrateOnHighlight;
        toggleVibrateOnSubmit.isOn = !Config.VibrateOnHighlight;

        buttonQuitBorgle.SetActive(game == Config.Game.Borgle);
        buttonQuitLeWord.SetActive(game == Config.Game.LeWord);
        buttonQuitSudooku.SetActive(game == Config.Game.Sudooku);
        buttonQuitWordScrapes.SetActive(game == Config.Game.WordScrapes);
        scrollPanelBorgleUIConfig.SetActive(game == Config.Game.Borgle);
        scrollPanelLeWordUIConfig.SetActive(game == Config.Game.LeWord);
        scrollPanelSudookuUIConfig.SetActive(game == Config.Game.Sudooku);
        scrollPanelWordScrapesUIConfig.SetActive(game == Config.Game.WordScrapes);
        uiBlacklist.SetActive(false);

        gameObject.SetActive(true);
    }

    public void OnClickBlacklistClose()
    {
        uiBlacklist.SetActive(false);
        Config.Save();

        for (int i = rectUIWords.childCount - 1; i >= 0; --i)
            Destroy(rectUIWords.GetChild(i).gameObject);
    }

    public void OnClickBlacklistOpen()
    {
        uiBlacklist.SetActive(true);

        foreach (string word in Config.Blacklist)
        {
            GameObject instance = Instantiate(prefabUIWord);
            RectTransform rectTransform = instance.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(600, rectTransform.sizeDelta.y);
            UIWord solutionWord = instance.GetComponent<UIWord>();
            solutionWord.onClick = OnClickUIWord;
            solutionWord.textWord.fontSize = 66;

            solutionWord.Set(word);
            solutionWord.SetState(UIWord.State.Miss);
            solutionWord.transform.SetParent(rectUIWords);
        }
    }

    public void OnClickClose()
    {
        Config.Save();
        gameObject.SetActive(false);
    }

    public void OnClickUIWord(UIWord uiWord)
    {
        if (uiWord.state == UIWord.State.Blacklist)
        {
            Config.Blacklist.Add(uiWord.value);
            uiWord.SetState(UIWord.State.Miss);
        }
        else
        {
            Config.Blacklist.Remove(uiWord.value);
            uiWord.SetState(UIWord.State.Blacklist);
        }

        if (Config.LogPointerEvents)
            Debug.Log($"OnClickUIWord {uiWord.value}");
    }

    public void OnClickMainMenu()
    {
        gameObject.SetActive(false);
        uiSplashScreen.gameObject.SetActive(true);
    }

    public void SetBlanks(int _)
    {
        Config.Blanks = (int)sliderBlanks.value;
        textBlanks.text = $"BLANKS: {Config.Blanks}";
        textDifficulty.text = $"DIFFICULTY: {Difficulty()}";
    }

    public void SetBorgleClassic(bool _)
    {
        Config.BorgleClassic = toggleBorgleClassic.isOn;
        textBorgleClassic.alpha = 0.75f;
        textBorgleModern.alpha = 1.0f;
        toggleBorgleModern.isOn = !toggleBorgleClassic.isOn;
    }

    public void SetBorgleModern(bool _)
    {
        Config.BorgleClassic = toggleBorgleModern.isOn;
        textBorgleClassic.alpha = 1.0f;
        textBorgleModern.alpha = 0.75f;
        toggleBorgleClassic.isOn = !toggleBorgleModern.isOn;
    }

    public void SetBorgleTimed(bool _)
    {
        Config.BorgleTimed = toggleBorgleTimed.isOn;
    }

    public void SetBorgledLetters(bool _)
    {
        Config.BorgledLetters = toggleBorgledLetters.isOn;
    }

    public void SetControlRadius(int _)
    {
        Config.ControlRadiusPx = (int)sliderControlRadius.value;
        textControlRadiusPx.text = $"CONTROL RADIUS: {Config.ControlRadiusPx}px";
    }

    public void SetControlScale(float _)
    {
        Config.ControlScale = sliderControlScale.value;
        textControlScale.text = $"CONTROL SCALE: {Config.ControlScale:F2}";
    }

    public void SetGameTime(int _)
    {
        Config.GameTimeSeconds = (int)sliderGameTime.value;
        textGameTimeSeconds.text = $"GAME TIME: {Config.GameTimeSeconds}s";
    }

    public void SetGameTimed(bool _)
    {
        Config.GameTimed = toggleGameTimed.isOn;
    }

    public void SetHardMode(bool _)
    {
        Config.HardMode = toggleHardMode.isOn;
        textHardMode.text = Config.HardMode ? LeWord.RulesHardOn : LeWord.RulesHardOff;
    }

    public void SetShowSolutions(bool _)
    {
        Config.ShowSolutions = toggleShowSolutions.isOn;
    }

    public void SetVibrateOnHighlight(bool _)
    {
        Config.VibrateOnHighlight = toggleVibrateOnHighlight.isOn;
        toggleVibrateOnSubmit.isOn = !toggleVibrateOnHighlight.isOn;
    }

    public void SetVibrateOnSubmit(bool _)
    {
        Config.VibrateOnHighlight = !toggleVibrateOnSubmit.isOn;
        toggleVibrateOnHighlight.isOn = !toggleVibrateOnSubmit.isOn;
    }

    public void SetWordLength(int _)
    {
        Config.WordLength = (int)sliderWordLength.value;
        textWordLength.text = $"WORD LENGTH: {sliderWordLength.value}";
    }
}

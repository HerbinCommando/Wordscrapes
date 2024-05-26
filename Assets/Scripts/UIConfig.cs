using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIConfig : MonoBehaviour
{
    public GameObject buttonQuitLeWord;
    public GameObject buttonQuitWordScrapes;
    public GameObject prefabUIWord;
    public RectTransform rectUIWords;
    public GameObject scrollPanelLeWordUIConfig;
    public GameObject scrollPanelWordScrapesUIConfig;
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
    public GameObject uiBlacklist;
    public UISplashScreen uiSplashScreen;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Activate(Config.Game game)
    {
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
        textControlRadiusPx.text = $"CONTROL RADIUS: {Config.ControlRadiusPx}px";
        textControlScale.text = $"CONTROL SCALE: {Config.ControlScale}";
        textGameTimeSeconds.text = $"GAME TIME: {Config.GameTimeSeconds}s";
        textWordLength.text = $"WORD LENGTH: {Config.WordLength}";
        toggleGameTimed.isOn = Config.GameTimed;
        toggleShowSolutions.isOn = Config.ShowSolutions;
        toggleVibrateOnHighlight.isOn = Config.VibrateOnHighlight;
        toggleVibrateOnSubmit.isOn = !Config.VibrateOnHighlight;

        buttonQuitLeWord.SetActive(game == Config.Game.LeWord);
        buttonQuitWordScrapes.SetActive(game == Config.Game.WordScrapes);
        scrollPanelLeWordUIConfig.SetActive(game == Config.Game.LeWord);
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
        gameObject.SetActive(false);
    }

    public void OnClickUIWord(UIWord uiWord)
    {
        if (uiWord.state == UIWord.State.Blacklist)
        {
            Config.Blacklist.Add(uiWord.word);
            uiWord.SetState(UIWord.State.Miss);
        }
        else
        {
            Config.Blacklist.Remove(uiWord.word);
            uiWord.SetState(UIWord.State.Blacklist);
        }

        if (Config.LogPointerEvents)
            Debug.Log($"OnClickUIWord {uiWord.word}");
    }

    public void OnClickMainMenu()
    {
        gameObject.SetActive(false);
        uiSplashScreen.gameObject.SetActive(true);
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

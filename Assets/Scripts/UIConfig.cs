using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIConfig : MonoBehaviour
{
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

    public void OnClickBlacklistClose()
    {
        blacklist.SetActive(false);
        Config.Save();

        for (int i = rectUIWords.childCount - 1; i >= 0; --i)
            Destroy(rectUIWords.GetChild(i).gameObject);
    }

    public void OnClickBlacklistOpen()
    {
        blacklist.SetActive(true);

        foreach (string word in Config.Blacklist)
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

    private void OnEnable()
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

        blacklist.SetActive(false);
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

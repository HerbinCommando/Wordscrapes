using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIConfig : MonoBehaviour
{
    public Slider sliderGameTime;
    public Slider sliderWordLength;
    public TextMeshProUGUI textGameTimeSeconds;
    public TextMeshProUGUI textWordLengthMax;
    public Toggle toggleGameTimed;
    public Toggle toggleShowSolutions;

    public void OnClickClose()
    {
        Config.Save();
        gameObject.SetActive(false);
    }

    public void SetGameTime(int _)
    {
        Config.GameTimeSeconds = (int)sliderGameTime.value;
        textGameTimeSeconds.text = $"Game Time: {Config.GameTimeSeconds}s";
    }

    public void SetGameTimed(bool _)
    {
        Config.GameTimed = toggleGameTimed.isOn;
    }

    public void SetShowSolutions(bool _)
    {
        Config.ShowSolutions = toggleShowSolutions.isOn;
    }

    public void SetWordLengthMax(int _)
    {
        Config.WordLengthMax = (int)sliderWordLength.value;
        textWordLengthMax.text = $"Word Length: {sliderWordLength.value}";
    }

    private void Start()
    {
        sliderWordLength.maxValue = 7;
        sliderWordLength.minValue = Config.WordLengthMin;
        sliderWordLength.value = Config.WordLengthMax;
        sliderGameTime.maxValue = Config.GameTimeSecondsMax;
        sliderGameTime.minValue = Config.GameTimeSecondsMin;
        sliderGameTime.value = Config.GameTimeSeconds;
        textGameTimeSeconds.text = $"Game Time: {Config.GameTimeSeconds}s";
        textWordLengthMax.text = $"Word Length: {Config.WordLengthMax}";
        toggleGameTimed.isOn = Config.GameTimed;
        toggleShowSolutions.isOn = Config.ShowSolutions;
    }
}

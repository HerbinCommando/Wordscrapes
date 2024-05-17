using System;
using TMPro;
using UnityEngine;

public class UIWord : MonoBehaviour
{
    public enum State
    {
        Blacklist,
        Default,
        Hit,
        Miss
    }

    public TextMeshProUGUI textWord;

    [NonSerialized] public Action<UIWord> onClick;
    [NonSerialized] public State state;
    [NonSerialized] public string word;

    public bool Hit => state == State.Hit;

    private void Awake()
    {
        Set(string.Empty);
        SetState(State.Default);
    }

    public void Set(string _word)
    {
        textWord.text = string.Empty;
        word = _word;

        if (UIConfig.ShowSolutions)
            textWord.text = word;
        else
            for (int i = 0; i < _word.Length; ++i)
                textWord.text += '_';
    }

    public void SetState(State _state)
    {
        state = _state;
        textWord.fontStyle &= ~FontStyles.Strikethrough;

        switch (_state)
        {
            case State.Blacklist:
                textWord.color = Color.gray;
                textWord.fontStyle |= FontStyles.Strikethrough;
                break;
            case State.Default:
                textWord.color = Color.white;
                break;
            case State.Hit:
                textWord.color = Color.green;
                textWord.text = word;
                break;
            case State.Miss:
                textWord.color = Color.red;
                textWord.text = word;
                break;
        }
    }

    public void OnClick()
    {
        onClick?.Invoke(this);
    }
}

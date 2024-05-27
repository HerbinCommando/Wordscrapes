using System;
using TMPro;
using UnityEngine;

public class UIWord : MonoBehaviour
{
    public enum State
    {
        Blacklist,
        Hidden,
        Default,
        Hit,
        Miss
    }

    public TextMeshProUGUI textWord;

    [NonSerialized] public Action<UIWord> onClick;
    [NonSerialized] public State state;
    [NonSerialized] public string value;

    public bool Hit => state == State.Hit;

    private void Awake()
    {
        Set(string.Empty);
        SetState(State.Default);
    }

    public void Set(string _value)
    {
        value = _value;
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

            case State.Hidden:
                textWord.color = Color.white;
                textWord.text = string.Empty;

                for (int i = 0; i < value.Length; ++i)
                    textWord.text += '_';
                break;

            case State.Default:
                textWord.text = value;
                textWord.color = Color.white;
                break;

            case State.Hit:
                textWord.color = Color.green;
                textWord.text = value;
                break;

            case State.Miss:
                textWord.color = Color.red;
                textWord.text = value;
                break;
        }
    }

    public void OnClick()
    {
        onClick?.Invoke(this);
    }
}

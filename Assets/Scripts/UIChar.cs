using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIChar : MonoBehaviour
{
    public enum State
    {
        Default,
        Disabled,
        DodgerBlue,
        Green,
        Yellow
    }

    public Image imageButton;
    public TextMeshProUGUI textChar;

    public Action<UIChar> onPointerDown;
    public Action<UIChar> onPointerUp;
    public Action<UIChar> onPointerEnter;

    [NonSerialized] public State state;

    public char Char => textChar.text[0];
    public string Character => textChar.text;
    public bool Disabled => state == State.Disabled;
    public KeyCode KeyCode => (KeyCode)Enum.Parse(typeof(KeyCode), Character);
    public bool Selected => state == State.Green;

    private void Awake()
    {
        SetState(State.Default);
    }

    public void OnPointerDown()
    {
        onPointerDown?.Invoke(this);
    }

    public void OnPointerUp()
    {
        onPointerUp?.Invoke(this);
    }

    public void OnPointerEnter()
    {
        onPointerEnter?.Invoke(this);
    }

    public void SetBold(bool value)
    {
        if (value)
            textChar.fontStyle |= FontStyles.Bold;
        else
            textChar.fontStyle &= ~FontStyles.Bold;
    }

    public void SetItalic(bool value)
    {
        if (value)
            textChar.fontStyle |= FontStyles.Italic;
        else
            textChar.fontStyle &= ~FontStyles.Italic;
    }

    public void SetState(State _state)
    {
        state = _state;

        textChar.alpha = _state == State.Disabled ? 0.3f : 1.0f;

        switch (_state)
        {
            case State.Default:
                imageButton.color = new Color(1, 1, 1, 1);
                break;

            case State.Disabled:
                imageButton.color = new Color(1, 1, 1, 0.75f);
                break;

            case State.DodgerBlue:
                imageButton.color = new Color(0.118f, 0.565f, 1.0f);
                break;

            case State.Green:
                imageButton.color = new Color(0, 1, 0, 1);
                break;

            case State.Yellow:
                imageButton.color = new Color(1, 1, 0, 1);
                break;
        }
    }
}

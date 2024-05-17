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
        Selected,
        Yellow
    }

    public Image imageButton;
    public Image imageSelected;
    public Image imageYellow;
    public TextMeshProUGUI textChar;

    public Action<UIChar> onPointerDown;
    public Action<UIChar> onPointerUp;
    public Action<UIChar> onPointerEnter;

    [NonSerialized] public State state;

    public char Char => textChar.text[0];
    public string Character => textChar.text;
    public bool Disabled => state == State.Disabled;
    public KeyCode KeyCode => (KeyCode)Enum.Parse(typeof(KeyCode), Character);
    public bool Selected => state == State.Selected;

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

    public void SetState(State _state)
    {
        state = _state;

        textChar.alpha = _state == State.Disabled ? 0.3f : 1.0f;

        imageButton.gameObject.SetActive(_state == State.Default || _state == State.Disabled);
        imageSelected.gameObject.SetActive(_state == State.Selected);
        imageYellow.gameObject.SetActive(_state == State.Yellow);
    }
}

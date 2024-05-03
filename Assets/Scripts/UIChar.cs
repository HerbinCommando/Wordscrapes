using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIChar : MonoBehaviour
{
    public enum State
    {
        Default,
        Selected,
    }

    public Image imageButton;
    public Image imageSelected;
    public TextMeshProUGUI textChar;

    public Action<UIChar> onPointerDown;
    public Action<UIChar> onPointerUp;
    public Action<UIChar> onPointerEnter;

    [NonSerialized] public State state;

    public string Character => textChar.text;
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

        imageButton.gameObject.SetActive(_state == State.Default);
        imageSelected.gameObject.SetActive(_state == State.Selected);
    }
}

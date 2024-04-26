using System;
using TMPro;
using UnityEngine;

public class UIWord : MonoBehaviour
{
    public enum State
    {
        Default,
        Hit,
        Miss
    }

    public TextMeshProUGUI textWord;

    [NonSerialized] public State state;
    [NonSerialized] public string word;

    public bool Hit => state == State.Hit;

    private void Awake()
    {
        Set(string.Empty);
    }

    public void Set(string _word)
    {
        textWord.text = string.Empty;
        word = _word;

        if (Config.ShowSolutions)
            textWord.text = word;
        else
            for (int i = 0; i < _word.Length; ++i)
                textWord.text += '_';
    }

    public void SetState(State _state)
    {
        state = _state;

        switch(_state)
        {
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
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIKeyboard : MonoBehaviour
{
    readonly string[] Keyboard = new string[] { "QWERTYUIOP", "ASDFGHJKL", "ZXCVBNM" };

    public Action<UIChar> onBackspaceDown;
    public Action<UIChar> onEnterDown;
    public Action<UIChar> onPointerDown;
    public Transform panelKeyboard;
    public GameObject prefabUIChar;

    [NonSerialized] public List<UIChar> uiChars = new List<UIChar>();

    private void OnDestroy()
    {
        for (int i = uiChars.Count - 1; i > 0; --i)
            Destroy(uiChars[i]);

        uiChars.Clear();
    }

    private void Start()
    {
        GameObject instance;
        LayoutElement layoutElement;
        UIChar uiChar;

        for (int i = 0; i < Keyboard.Length; ++i)
        {
            foreach (char ch in Keyboard[i])
            {
                instance = Instantiate(prefabUIChar);
                layoutElement = instance.AddComponent<LayoutElement>();
                layoutElement.minHeight = 75;
                layoutElement.preferredHeight = 75;
                uiChar = instance.GetComponent<UIChar>();
                uiChar.textChar.fontSize = 75;
                uiChar.textChar.text = $"{ch}";

                uiChar.onPointerDown += OnPointerDown;

                uiChar.transform.SetParent(panelKeyboard.GetChild(i));
                uiChars.Add(uiChar);
            }
        }

        instance = Instantiate(prefabUIChar);
        layoutElement = instance.AddComponent<LayoutElement>();
        layoutElement.minHeight = 75;
        layoutElement.preferredHeight = 75;
        uiChar = instance.GetComponent<UIChar>();
        uiChar.textChar.text = "enter";
        uiChar.textChar.enableAutoSizing = true;

        uiChar.onPointerDown += OnEnterDown;

        uiChar.transform.SetParent(panelKeyboard.GetChild(2));
        uiChar.transform.SetAsFirstSibling();

        instance = Instantiate(prefabUIChar);
        layoutElement = instance.AddComponent<LayoutElement>();
        layoutElement.minHeight = 75;
        layoutElement.preferredHeight = 75;
        uiChar = instance.GetComponent<UIChar>();
        uiChar.textChar.text = "bsp";
        uiChar.textChar.enableAutoSizing = true;

        uiChar.onPointerDown += OnBackspaceDown;

        uiChar.transform.SetParent(panelKeyboard.GetChild(2));
    }

    public void OnBackspaceDown(UIChar uiChar)
    {
        onBackspaceDown?.Invoke(uiChar);
    }

    public void OnEnterDown(UIChar uiChar)
    {
        onEnterDown?.Invoke(uiChar);
    }

    public void OnPointerDown(UIChar uiChar)
    {
        onPointerDown?.Invoke(uiChar);
    }
}

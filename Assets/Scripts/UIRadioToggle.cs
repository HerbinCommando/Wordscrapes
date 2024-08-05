using System;
using UnityEngine;
using UnityEngine.UI;

public class UIRadioToggle : MonoBehaviour
{
    public Action<int> onClickRadioToggle;
    public Toggle[] toggles;

    private void OnDestroy()
    {
        foreach (var toggle in toggles)
            toggle.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(bool isOn)
    {
        Toggle toggle = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>();

        if (toggle != null)
            onClickRadioToggle?.Invoke(Array.IndexOf(toggles, toggle));

        foreach (var t in toggles)
            if (t.isOn && t != toggle)
                t.isOn = false;
    }

    private void Start()
    {
        toggles = GetComponentsInChildren<Toggle>();

        foreach (var toggle in toggles)
            toggle.onValueChanged.AddListener(OnValueChanged);
    }
}

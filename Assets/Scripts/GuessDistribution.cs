using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuessDistribution : MonoBehaviour
{
    public Image background;
    public TextMeshProUGUI textDistribution;

    public void Set(int distribution, float pct, bool color)
    {
        RectTransform rectParent = transform.parent as RectTransform;
        RectTransform rectTransform = transform as RectTransform;
        float parentWidth = rectParent.rect.width;
        float newWidth = parentWidth * Mathf.Max(0.12f, pct);
        float rightMargin = parentWidth - newWidth;
        background.color = color ? Color.green : Color.grey;
        rectTransform.offsetMax = new Vector2(-rightMargin, rectTransform.offsetMax.y);
        textDistribution.text = $"{distribution}";
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuessDistribution : MonoBehaviour
{
    public Image background;
    public TextMeshProUGUI textDistribution;

    public void Set(int distribution, float pct, bool colorDefault)
    {
        RectTransform rectParent = transform.parent as RectTransform;
        RectTransform rectTransform = transform as RectTransform;
        float parentWidth = rectParent.rect.width;
        float newWidth = Mathf.Min(200 + parentWidth * pct, parentWidth);
        float rightMargin = parentWidth - newWidth;
        background.color = colorDefault ? Color.grey : Color.green;
        rectTransform.offsetMax = new Vector2(-rightMargin, rectTransform.offsetMax.y);
        textDistribution.text = $"{distribution}";
    }
}

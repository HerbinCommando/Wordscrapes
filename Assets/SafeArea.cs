using UnityEngine;

public class SafeArea : MonoBehaviour
{
    public RectTransform rectTransform;

    void Start()
    {
        if (Screen.safeArea != Rect.zero)
        {
            Rect safeArea = Screen.safeArea;
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            // Convert anchor coordinates to canvas space
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PassTouchEvents : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    private bool isPointerDown;
    private bool isPointerOver;
    private ScrollRect scrollRect;

    private void Start()
    {
        scrollRect = GetComponentInParent<ScrollRect>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        scrollRect.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if ((isPointerOver || isPointerDown) && scrollRect != null)
            scrollRect.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        scrollRect.OnEndDrag(eventData);
    }

    public void OnPointerDown(PointerEventData _)
    {
        isPointerDown = true;
    }

    public void OnPointerEnter(PointerEventData _)
    {
        isPointerOver = true;
    }

    public void OnPointerExit(PointerEventData _)
    {
        isPointerOver = false;
    }

    public void OnPointerUp(PointerEventData _)
    {
        isPointerDown = false;
    }
}
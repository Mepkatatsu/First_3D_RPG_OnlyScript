using SingletonPattern;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotButtonHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private ScrollRect _scrollRect;

    private void Awake()
    {
        _scrollRect = transform.parent.parent.parent.parent.GetComponent<ScrollRect>();
    }

    public void OnBeginDrag(PointerEventData e)
    {
        _scrollRect.OnBeginDrag(e);
    }
    public void OnDrag(PointerEventData e)
    {
        _scrollRect.OnDrag(e);
    }
    public void OnEndDrag(PointerEventData e)
    {
        _scrollRect.OnEndDrag(e);
    }

    public void OnClickInventoryItemButton()
    {
        int index = transform.parent.GetSiblingIndex() * 5 + transform.GetSiblingIndex();

        UIManager.Instance.OnClickInventoryItemButton(index);
    }
}
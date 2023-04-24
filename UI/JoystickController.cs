using SingletonPattern;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region Variables

    [SerializeField] private RectTransform _joystick;
    [SerializeField] private RectTransform _handle;
    [SerializeField] private PlayerCharacterController _characterController;

    private Vector2 _centerPosition = new();
    private Vector2 _normalizedPosition = new();

    #endregion Variables

    #region Methods

    public void OnBeginDrag(PointerEventData eventData)
    {
        _centerPosition = eventData.position;
        _joystick.position = _centerPosition;
        _handle.position = _centerPosition;
        _joystick.gameObject.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _normalizedPosition = (eventData.position - _centerPosition).normalized;
        _characterController.SetJoystickInput(_normalizedPosition.x, _normalizedPosition.y);

        if (Vector2.Distance(eventData.position, _centerPosition) < 100)
        {
            _handle.position = eventData.position;
        }
        else
        {
            _handle.position = _centerPosition + (_normalizedPosition * 100);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _characterController.SetJoystickInput(0, 0);
        _joystick.gameObject.SetActive(false);
    }

    #endregion Methods
}

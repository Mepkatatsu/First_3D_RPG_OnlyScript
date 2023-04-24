using SingletonPattern;
using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    #region Variables

    public Transform target;

    public float wheelValue = 1;
    public float height = 15f;
    public float distance = 15f;
    public float lookAtHeight = 0f;
    private float _inventoryValue = 5;

    private Vector3 _cameraPosition = new();
    private Vector3 _finalTargetPosition = new();

    private UIManager _uiManager;

    #endregion Variables

    #region Unity Methods

    private void Start()
    {
        _uiManager = UIManager.Instance;
    }

    private void LateUpdate()
    {
        // ToDo: wheelInput�� �ܰ踦 ������ height, distance, angle ���� ���س��� ���� ���� ��. Ȥ�� ������ �Ȱ��� ���ߵ簡.
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");

        if (wheelInput > 0)
        {
            wheelValue -= 0.025f;
        }
        else if (wheelInput < 0)
        {
            wheelValue += 0.025f;
        }
        if (wheelValue < 0) wheelValue = 0;
        else if (wheelValue > 1) wheelValue = 1;

        height = 2 + 13 * wheelValue;
        distance = 6 + 9 * wheelValue;
        lookAtHeight = 1 - wheelValue;
        _inventoryValue = 1.5f + 3.5f * wheelValue;

        _cameraPosition = target.position;
        _cameraPosition.y += height;
        _cameraPosition.z -= distance;

        _finalTargetPosition = target.position + Vector3.up * lookAtHeight;

        if (_uiManager.isOpenInventory)
        {
            _cameraPosition.x += _inventoryValue;
            _finalTargetPosition.x += _inventoryValue;
        }

        Vector3 direction = _finalTargetPosition - _cameraPosition;
        Quaternion rotation = Quaternion.LookRotation(direction.normalized);

        Vector3 setPosition = Vector3.Slerp(transform.position, _cameraPosition, 0.1f);
        Quaternion setRotation = Quaternion.Slerp(transform.rotation, rotation, 0.1f);        

        transform.SetPositionAndRotation(setPosition, setRotation);
    }

    #endregion Unity Methods
}

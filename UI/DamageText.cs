using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    #region Variables
    private TextMeshProUGUI _textMeshPro;

    public float destroyDelayTime = 1.0f;

    #endregion Variables

    #region Properties

    public int Damage
    {
        get
        {
            if (_textMeshPro != null)
            {
                return int.Parse(_textMeshPro.text);
            }

            return 0;
        }
        set
        {
            if (_textMeshPro != null)
            {
                _textMeshPro.text = value.ToString();
            }
        }
    }

    #endregion Properties

    #region Unity Methods
    private void Awake()
    {
        _textMeshPro = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        Destroy(gameObject, destroyDelayTime);
    }
    #endregion Unity Methods
}
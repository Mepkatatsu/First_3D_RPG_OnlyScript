using System.Collections;
using UnityEngine;

public abstract class AttackBehaviour : MonoBehaviour
{
    #region Variables

#if UNITY_EDITOR
    [Multiline]
    public string description = "";
#endif // UNITY_EDITOR

    public int attackIndex = 0;
    public int attackAnimationIndex;
    public int priority;

    public DamageType damageType;
    public DamageCalculateType damageCalculateType;
    public float damageValue = 10;
    public float range = 2f;

    [SerializeField]
    protected float coolTime;
    protected float calcCoolTime = 0.0f;

    public GameObject effectPrefab;

    [HideInInspector]
    public LayerMask targetMask;

    public bool IsAvailable => calcCoolTime >= coolTime;

    #endregion Variables

    // Start is called before the first frame update
    private void Start()
    {
        ResetCooltime();
    }

    public void ResetCooltime()
    {
        calcCoolTime = coolTime;
    }

    public IEnumerator StartCooltimeCoroutine()
    {
        calcCoolTime = 0;
        while (calcCoolTime < coolTime)
        {
            yield return new WaitForSeconds(0.1f);
            calcCoolTime += 0.1f;
        }
    }

    public void StartCooltime()
    {
        StartCoroutine(StartCooltimeCoroutine());
    }

    public abstract void ExecuteAttack(GameObject target = null, Transform startPoint = null);
}

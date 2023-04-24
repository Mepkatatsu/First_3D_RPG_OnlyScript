using SingletonPattern;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController_Melee : EnemyController, IAttackable, IDamageable
{
    #region Variables
    [SerializeField] public Transform hitPoint;
    public ItemDatabaseObject itemDatabase;
    public InventoryObject inventoryObject;

    [SerializeField] private int _enemyLevel = 1;
    [SerializeField] private int _minItemLevel = 1;
    [SerializeField] private int _maxItemLevel = 1;
    [SerializeField] private float _itemDropChancePercent = 50;
    public float maxHP = 100f;
    public float currentHP;

    private readonly int _hitTriggerHash = Animator.StringToHash("HitTrigger");

    #endregion Variables

    #region Proeprties
    public override bool IsAvailableAttack
    {
        get
        {
            if (!Target)
            {
                return false;
            }

            float distance = Vector3.Distance(transform.position, Target.position);
            return (distance <= AttackRange);
        }
    }

    public int EnemyLevel => _enemyLevel;

    #endregion Properties

    #region Unity Methods

    protected override void Start()
    {
        base.Start();

        stateMachine.AddState(new MoveToTargetState());
        stateMachine.AddState(new AttackState());
        stateMachine.AddState(new DeadState());
        stateMachine.AddState(new MoveToWaypointState());

        currentHP = maxHP;

        InitAttackBehaviour();
    }

    protected override void Update()
    {
        CheckAttackBehaviour();

        base.Update();
    }

    #endregion Unity Methods

    #region Helper Methods

    private void InitAttackBehaviour()
    {
        foreach (AttackBehaviour behaviour in attackBehaviours)
        {
            if (CurrentAttackBehaviour == null)
            {
                CurrentAttackBehaviour = behaviour;
            }

            behaviour.targetMask = TargetMask;
        }
    }

    private void CheckAttackBehaviour()
    {
        if (CurrentAttackBehaviour == null || !CurrentAttackBehaviour.IsAvailable)
        {
            CurrentAttackBehaviour = null;

            foreach (AttackBehaviour behaviour in attackBehaviours)
            {
                if (behaviour.IsAvailable)
                {
                    if ((CurrentAttackBehaviour == null) || (CurrentAttackBehaviour.priority < behaviour.priority))
                    {
                        CurrentAttackBehaviour = behaviour;
                    }
                }
            }
        }
    }

    private void DropItem()
    {
        float randNum = Random.Range(0, 100f);

        if (randNum > _itemDropChancePercent)
        {
            return;
        }

        ItemObject itemObject = itemDatabase.GetRandomLevelItem(_minItemLevel, _maxItemLevel);
        ItemData itemData = new ItemData(itemObject);

        inventoryObject.AddItem(itemData, 1);
    }

    #endregion Helper Methods

    #region IDamagable interfaces

    public bool IsAlive => (currentHP > 0);

    public void TakeDamage(int damage, GameObject hitEffectPrefab, Transform attackFrom)
    {
        isInBattle = true;
        target = attackFrom.transform;

        if (!IsAlive)
        {
            return;
        }

        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;

        OnDrawEnemyOutline();
        PlayerCharacterController.Instance.target = gameObject;
        UIManager.Instance.UpdateEnemyUI(gameObject);
        UIManager.Instance.ShowDamageText(gameObject, damage);

        if (hitEffectPrefab)
        {
            Instantiate(hitEffectPrefab, hitPoint);
        }

        if (IsAlive)
        {
            if (stateMachine.CurrentState is not AttackState)
            {
                animator.SetTrigger(_hitTriggerHash);
            }
        }
        else
        {
            DropItem();
            PlayerCharacterController.Instance.target = null;
            UIManager.Instance.ResetEnemyUI();
            
            stateMachine.ChangeState<DeadState>();
        }
    }

    #endregion IDamagable interfaces

    #region IAttackable Interfaces

    public GameObject hitEffectPrefab = null;

    [SerializeField]
    private List<AttackBehaviour> attackBehaviours = new List<AttackBehaviour>();

    public AttackBehaviour CurrentAttackBehaviour
    {
        get;
        private set;
    }

    public void OnExecuteAttack(int attackIndex)
    {
        foreach (AttackBehaviour attackBehaviour in attackBehaviours)
        {
            if (attackBehaviour.attackAnimationIndex == attackIndex)
            {
                attackBehaviour.ExecuteAttack();
            }
        }
    }

    #endregion IAttackable Interfaces
}

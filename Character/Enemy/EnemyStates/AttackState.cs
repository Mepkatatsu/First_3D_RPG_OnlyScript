using UnityEngine;

public class AttackState : State<EnemyController>
{
    #region Variables

    private Animator _animator;
    private AttackStateController _attackStateController;
    private IAttackable _attackable;

    private readonly int _attackTriggerHash = Animator.StringToHash("AttackTrigger");
    private readonly int _attackIndexHash = Animator.StringToHash("AttackIndex");

    #endregion Variables

    #region Methods

    public override void OnInitialized()
    {
        _animator = context.GetComponent<Animator>();
        _attackStateController = context.GetComponent<AttackStateController>();
        _attackable = context.GetComponent<IAttackable>();
    }

    public override void OnEnter()
    {
        if (_attackable == null || _attackable.CurrentAttackBehaviour == null)
        {
            stateMachine.ChangeState<IdleState>();
            return;
        }

        _attackStateController.enterAttackHandler += OnEnterAttackState;
        _attackStateController.exitAttackHandler += OnExitAttackState;

        _animator.SetInteger(_attackIndexHash, _attackable.CurrentAttackBehaviour.attackAnimationIndex);
        _animator.SetTrigger(_attackTriggerHash);
        _attackable.CurrentAttackBehaviour.StartCooltime();
    }

    public override void Update(float deltaTime)
    {
    }

    public override void OnExit()
    {
        _attackStateController.enterAttackHandler -= OnEnterAttackState;
        _attackStateController.exitAttackHandler -= OnExitAttackState;
    }

    public void OnEnterAttackState()
    {
        
    }

    public void OnExitAttackState()
    {
        stateMachine.ChangeState<IdleState>();
    }

    #endregion Methods
}

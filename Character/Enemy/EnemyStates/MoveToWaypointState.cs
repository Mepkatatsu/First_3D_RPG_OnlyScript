using UnityEngine;
using UnityEngine.AI;

public class MoveToWaypointState : State<EnemyController>
{
    #region Variables

    private Animator _animator;
    private CharacterController _controller;
    private NavMeshAgent _agent;


    private readonly int _isMovingHash = Animator.StringToHash("IsMoving");
    private readonly int _moveSpeedHash = Animator.StringToHash("MoveSpeed");

    #endregion Variables

    #region Methods

    public override void Update(float deltaTime)
    {
        if (context.Target)
        {
            if (context.IsAvailableAttack)
            {
                // check attack cool time
                // and transition to attack state
                stateMachine.ChangeState<AttackState>();
            }
            else
            {
                stateMachine.ChangeState<MoveToTargetState>();
            }
        }
        else
        {

            if (!_agent.pathPending && (_agent.remainingDistance <= _agent.stoppingDistance))
            {
                stateMachine.ChangeState<IdleState>();
            }
            else
            {
                _controller.Move(_agent.velocity * Time.deltaTime);
                _animator.SetFloat(_moveSpeedHash, _agent.velocity.magnitude / _agent.speed, .1f, Time.deltaTime);
            }
        }
    }

    public override void OnInitialized()
    {
        _animator = context.GetComponent<Animator>();
        _controller = context.GetComponent<CharacterController>();
        _agent = context.GetComponent<NavMeshAgent>();
    }

    public override void OnEnter()
    {
        _agent.stoppingDistance = 0.2f;

        if (Vector3.Distance(context.transform.position, context.patrolPosition) > _agent.stoppingDistance)
        {
            _animator.SetBool(_isMovingHash, true);
            _agent.SetDestination(context.patrolPosition);
        }
        else
        {
            stateMachine.ChangeState<IdleState>();
        }
    }

    public override void OnExit()
    {
        _agent.stoppingDistance = context.AttackRange;
        _animator?.SetBool(_isMovingHash, false);
        _agent.ResetPath();
    }

    #endregion Methods
}

using UnityEngine;
using UnityEngine.AI;

public class MoveToTargetState : State<EnemyController>
{
    #region Variables

    private Animator _animator;
    private CharacterController _controller;
    private NavMeshAgent _agent;

    private readonly int _isMovehash = Animator.StringToHash("IsMoving");
    private readonly int _moveSpeedHash = Animator.StringToHash("MoveSpeed");

    #endregion Variables

    #region Methods

    public override void OnInitialized()
    {
        _animator = context.GetComponent<Animator>();
        _controller = context.GetComponent<CharacterController>();

        _agent = context.GetComponent<NavMeshAgent>();
    }

    public override void OnEnter()
    {
        _agent.stoppingDistance = context.AttackRange;
        _agent.SetDestination(context.Target.position);

        _animator.SetBool(_isMovehash, true);
    }

    public override void Update(float deltaTime)
    {
        if (context.Target)
        {
            _agent.SetDestination(context.Target.position);
        }
        else
        {
            stateMachine.ChangeState<IdleState>();
            context.isInBattle = false;
        }

        _controller.Move(_agent.velocity * Time.deltaTime);

        if (_agent.remainingDistance > _agent.stoppingDistance)
        {
            _animator.SetFloat(_moveSpeedHash, _agent.velocity.magnitude / _agent.speed, .1f, Time.deltaTime);
        }
        else
        {
            if (!_agent.pathPending)
            {
                _animator.SetFloat(_moveSpeedHash, 0);
                _animator.SetBool(_isMovehash, false);

                stateMachine.ChangeState<IdleState>();
            }
        }
    }

    public override void OnExit()
    {
        _agent.stoppingDistance = 0.2f;
        _agent.ResetPath();

        _animator.SetBool(_isMovehash, false);
        _animator.SetFloat(_moveSpeedHash, 0.0f);
    }

    #endregion Methods
}
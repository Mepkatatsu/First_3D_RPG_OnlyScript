using System;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class IdleState : State<EnemyController>
{
    #region Variables

    private bool _isPatrol = true;
    private float _minIdleTime = 10.0f;
    private float _maxIdleTime = 30.0f;
    private float _idleTime = 0.0f;

    private Animator _animator;
    private CharacterController _controller;
    private NavMeshAgent _agent;

    private readonly int _isMoveHash = Animator.StringToHash("IsMoving");
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
        _animator.SetBool(_isMoveHash, false);
        _animator.SetFloat(_moveSpeedHash, 0);
        _controller.Move(Vector3.zero);

        if (context.isPatrol)
        {
            _isPatrol = true;
            _idleTime = UnityEngine.Random.Range(_minIdleTime, _maxIdleTime);
        }
    }

    public override void Update(float deltaTime)
    {
        // if searched target
        // change to move state

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
        else if (_isPatrol)
        {
            if (stateMachine.ElapsedTimeInState > _idleTime)
            {
                context.SetPatrolPosition();
            }
            if (Vector3.Distance(context.transform.position, context.patrolPosition) > _agent.stoppingDistance)
            {
                stateMachine.ChangeState<MoveToWaypointState>();
            }            
        }
    }

    public override void OnExit()
    {
    }

    #endregion Methods
}

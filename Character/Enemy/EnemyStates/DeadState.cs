using System;
using UnityEngine;

[Serializable]
public class DeadState : State<EnemyController>
{
    private Animator _animator;

    private readonly int _isAliveHash = Animator.StringToHash("IsAlive");

    #region Methods
    public override void OnInitialized()
    {
        _animator = context.GetComponent<Animator>();
    }

    public override void OnEnter()
    {
        _animator?.SetBool(_isAliveHash, false);
    }

    public override void Update(float deltaTime)
    {
        if (stateMachine.ElapsedTimeInState > 3.0f)
        {
            GameObject.Destroy(context.gameObject);
        }
    }

    public override void OnExit()
    {
    }
    #endregion Methods
}

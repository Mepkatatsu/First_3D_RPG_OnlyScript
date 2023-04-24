using UnityEngine;

public class AttackStateMachineBehaviour : StateMachineBehaviour
{
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        animator.GetComponent<AttackStateController>().OnStartOfAttackState();
    }
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        animator.GetComponent<AttackStateController>().OnEndOfAttackState();
    }
}

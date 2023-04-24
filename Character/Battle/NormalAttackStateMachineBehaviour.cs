using SingletonPattern;
using UnityEngine;

public class NormalAttackStateMachineBehaviour : StateMachineBehaviour
{
    private readonly int _comboAttackTriggerHash = Animator.StringToHash("ComboAttackTrigger");

    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        animator.GetComponent<PlayerCharacterController>().IsInNornalAttackState = false;
        animator.ResetTrigger(_comboAttackTriggerHash);
    }
}

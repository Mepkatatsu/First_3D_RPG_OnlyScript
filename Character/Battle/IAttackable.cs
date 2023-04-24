public interface IAttackable
{
    AttackBehaviour CurrentAttackBehaviour
    {
        get;
    }

    void OnExecuteAttack(int attackIndex);
}
using SingletonPattern;
using UnityEngine;

public class AttackBehaviour_Melee_Single : AttackBehaviour
{
    public MeleeAttackCollision meleeAttackCollision;

    public override void ExecuteAttack(GameObject target = null, Transform startPoint = null)
    {
        Collider[] colliders = meleeAttackCollision.CheckOverlapBox(targetMask);

        if (colliders.Length == 0) return;

        Collider targetCollider = null;

        // �߽ɿ� ���� ����� ����ִ� �� Ž��
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<PlayerCharacterController>() != null)
            {
                if (colliders[i].GetComponent<PlayerCharacterController>().IsAlive)
                {
                    targetCollider = colliders[i];
                    break;
                }
            }
            else if (colliders[i].GetComponent<EnemyController_Melee>() != null)
            {
                if (colliders[i].GetComponent<EnemyController_Melee>().IsAlive)
                {
                    targetCollider = colliders[i];
                    break;
                }
            }
        }
        // TODO : ���� ����� ���� ������ ���� ���� �� �ڿ������� �� �ְ���.

        if (targetCollider == null) return;

        // ������ ���
        if (damageCalculateType == DamageCalculateType.Fixed)
        {
            targetCollider.gameObject.GetComponent<IDamageable>()?.TakeDamage(Mathf.RoundToInt(damageValue), effectPrefab, transform);
        }
        else if (damageCalculateType == DamageCalculateType.Physical)
        {
            foreach (Attribute attribute in GetComponent<PlayerStat>().playerStats.attributes)
            {
                if (attribute.type == AttributeType.PhysicalAttack)
                {
                    int damage = Mathf.RoundToInt(attribute.value.ModifiedValue * (damageValue / 100));
                    targetCollider.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage, effectPrefab, transform);
                    break;
                }
            }
        }
        else if (damageCalculateType == DamageCalculateType.Magical)
        {
            foreach (Attribute attribute in GetComponent<PlayerStat>().playerStats.attributes)
            {
                if (attribute.type == AttributeType.MagicalAttack)
                {
                    int damage = Mathf.RoundToInt(attribute.value.ModifiedValue * (damageValue / 100));
                    targetCollider.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage, effectPrefab, transform);
                    break;
                }
            }
        }
    }
}

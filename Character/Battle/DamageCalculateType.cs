// Fixed: 적의 공격 등 고정된 수치의 데미지를 입힐 때 사용
// Physical: 물리 공격력 기반 데미지를 입힐 때 사용
// Magical: 마법 공격력 기반 데미지를 입힐 때 사용
// 이외에 체력 비례 데미지 등이 추가될 수 있겠음.

public enum DamageCalculateType
{
    Fixed,
    Physical,
    Magical,
}

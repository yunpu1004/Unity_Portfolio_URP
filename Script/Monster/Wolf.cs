// 이 스크립트는 늑대 몬스터를 담당하며, 몬스터 클래스에서 상속되어 늑대 몬스터 유형의 초기화와 처음 상태를 설정합니다.
// 전략 패턴과 상태 패턴을 사용하여 몬스터의 초기화 전략과 상태를 설정합니다.
public class Wolf : Monster
{
    protected override void Attack()
    {
        weapon.ActivateWeapon();
    }

    protected override void StopAttack()
    {
        weapon.DeactivateWeapon();
    }

    // 늑대의 초기화 전략을 설정합니다.
    protected override IAwakeStrategy SetAwakeStrategy()
    {
        return new NormalMonsterAwakeStrategy();
    }

    // 늑대의 초기 상태를 설정합니다.
    protected override IMonsterState SetMonsterInitialState()
    {
        return new WolfIdleState(this);
    }
}
 
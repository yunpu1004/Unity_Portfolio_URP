using UnityEngine;

// 이 스크립트는 골렘 몬스터를 담당하며, 몬스터 클래스에서 상속되어 골렘 몬스터 유형의 초기화와 처음 상태를 설정합니다.
// 전략 패턴과 상태 패턴을 사용하여 몬스터의 초기화 전략과 상태를 설정합니다.
public class Golem : Monster
{
    // 골렘 몬스터가 대기 상태일때 서있는 위치입니다.
    public Transform idleWayPoint;

    // 골렘의 초기화 전략을 설정합니다.
    protected override IAwakeStrategy SetAwakeStrategy()
    {
        return new BossMonsterAwakeStrategy();
    }

    // 골렘의 초기 상태를 설정합니다.
    protected override IMonsterState SetMonsterInitialState()
    {
        return new GolemIdleState(this);
    }

    // 몬스터가 죽었을 때 UI를 숨깁니다.
    private void OnDisable() 
    {
        BossUI.instance.HideUI();
    }
}
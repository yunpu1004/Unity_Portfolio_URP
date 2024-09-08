using UnityEngine;
using UnityEngine.AI;

// 이 클래스는 전략 패턴을 사용해서 보스 몬스터의 초기화를 담당합니다.
public class BossMonsterAwakeStrategy : IAwakeStrategy
{
    public void OnAwake(GameObject gameObject)
    {
        // 이벤트 등록
        var monster = gameObject.GetComponent<Monster>();
        var playerStat = monster.playerTransform.GetComponent<PlayerStat>();
        var playerActivity = monster.playerTransform.GetComponent<PlayerActivity>();
        var navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        
        monster.monsterStat.OnDamageEvent += (damage) => TriggerHitAnimation(monster);
        monster.monsterStat.OnDeathEvent += () => playerStat.GainExp(monster.monsterStat.GetExp());
        monster.monsterStat.OnDeathEvent += () => playerActivity.SetRecentActivity(new PlayerActivityData{activityType = PlayerActivityData.ActivityType.Kill, activityTarget = gameObject.name});
        monster.monsterStat.OnHPChangedEvent += (hp, maxHp) => BossUI.instance.SetHPBar(hp, maxHp);
        navMeshAgent.updateUpAxis = false;
    }

    // 피격 애니메이션을 트리거합니다.
    // 몬스터의 현재 모션이 Idle이 아니거나, 애니메이션 전환중에는 피격 애니메이션을 트리거하지 않습니다.
    private void TriggerHitAnimation(Monster monster)
    {
        AnimatorStateInfo stateInfo = monster.animator.GetCurrentAnimatorStateInfo(0);
        if(monster.monsterStat.IsDead()) return;
        if(!stateInfo.IsName("Idle")) return;
        if(monster.animator.IsInTransition(0)) return;
        monster.animator.SetTrigger("Hit");
    }
}
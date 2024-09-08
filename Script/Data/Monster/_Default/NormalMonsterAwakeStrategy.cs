using UnityEngine;
using UnityEngine.AI;

// 이 클래스는 전략 패턴을 사용해서 일반 몬스터의 초기화를 담당합니다.
public class NormalMonsterAwakeStrategy : IAwakeStrategy
{
    public void OnAwake(GameObject gameObject)
    {
        // 이벤트 등록
        var monster = gameObject.GetComponent<Monster>();
        var playerStat = monster.playerTransform.GetComponent<PlayerStat>();
        var playerActivity = monster.playerTransform.GetComponent<PlayerActivity>();
        var monsterUI = gameObject.GetComponentInChildren<MonsterUI>();
        var navMeshAgent = gameObject.GetComponent<NavMeshAgent>();

        monster.monsterStat.OnDamageEvent += (damage) => monster.animator.SetTrigger("Hit");
        monster.monsterStat.OnDeathEvent += () => playerStat.GainExp(monster.monsterStat.GetExp());
        monster.monsterStat.OnDeathEvent += () => playerActivity.SetRecentActivity(new PlayerActivityData{activityType = PlayerActivityData.ActivityType.Kill, activityTarget = gameObject.name});
        monster.monsterStat.OnHPChangedEvent += monsterUI.SetHPBar;
        navMeshAgent.updateUpAxis = false;
    }
}

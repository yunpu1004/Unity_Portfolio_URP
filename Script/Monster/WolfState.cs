using UnityEngine;
using UnityEngine.AI;

// 이 클래스는 늑대 몬스터의 대기 상태를 담당합니다.
public class WolfIdleState : IMonsterState
{
    private Wolf wolf;
    private float idleTime = 0;
    private const float maxIdleTime = 3;

    public WolfIdleState(Monster monster)
    {
        wolf = monster as Wolf;
    }

    public void OnFixedUpdate()
    {
        if(wolf.monsterStat.IsDead())
        {
            wolf.SetMonsterState(new WolfDeadState(wolf));
        }
        else if(wolf.chaseTrigger.inSight)
        {
            wolf.SetMonsterState(new WolfFightState(wolf));
        }
        else if(idleTime > maxIdleTime)
        {
            wolf.SetMonsterState(new WolfWanderState(wolf));
        }
        else
        {
            idleTime += Time.deltaTime;
        }
    }

    public void OnEnterState()
    {
        wolf.navMeshAgent.ResetPath();
        wolf.navMeshAgent.speed = 0;
        wolf.animator.SetFloat("Speed", 0);
    }

    public void OnExitState()
    {

    }
}


// 이 클래스는 늑대 몬스터의 전투 상태를 담당합니다.
public class WolfFightState : IMonsterState
{
    private Wolf wolf;
    private float attackDelay = 0;
    private const float maxAttackDelay = 3;
    private float lostSightChaseDuration = 0;
    private const float maxLostSightChaseDuration = 5;
    private const float moveSpeed = 5;

    public WolfFightState(Monster monster)
    {
        wolf = monster as Wolf;
    }

    public void OnFixedUpdate()
    {
        wolf.animator.SetFloat("Speed", wolf.navMeshAgent.velocity.magnitude);

        if(wolf.monsterStat.IsDead())
        {
            wolf.SetMonsterState(new WolfDeadState(wolf));
        }

        else if(lostSightChaseDuration > maxLostSightChaseDuration)
        {
            wolf.SetMonsterState(new WolfIdleState(wolf));
        }
        
        else
        {
            // 플레이어를 추적
            wolf.navMeshAgent.SetDestination(wolf.playerTransform.position);

            // 플레이어가 시야에서 벗어나더라도 일정 시간 동안 추적
            if(wolf.chaseTrigger.inSight) lostSightChaseDuration = 0;
            else lostSightChaseDuration += Time.deltaTime;

            // 플레이어가 몬스터와 가까이 있으면
            if (wolf.navMeshAgent.remainingDistance <= wolf.navMeshAgent.stoppingDistance)
            {
                // 대상을 바라보는 방향 계산
                Vector3 direction = (wolf.playerTransform.position - wolf.transform.position).normalized;
                direction.y = 0;

                // 몬스터의 바라볼 방향으로 쿼터니언을 계산
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                // 회전을 부드럽게 조절
                wolf.transform.rotation = Quaternion.Slerp(wolf.transform.rotation, lookRotation, Time.deltaTime * 5f);

                // 공격 딜레이 갱신
                attackDelay += Time.deltaTime;
                if(attackDelay > maxAttackDelay)
                {
                    wolf.animator.SetTrigger("Attack");
                    attackDelay = 0;
                }
            }
        }
    }

    public void OnEnterState()
    {
        wolf.navMeshAgent.speed = moveSpeed;
    }

    public void OnExitState()
    {
        
    }
}


// 이 클래스는 늑대 몬스터의 방황 상태를 담당합니다.
public class WolfWanderState : IMonsterState
{
    private Wolf wolf;

    public WolfWanderState(Monster monster)
    {
        wolf = monster as Wolf;
    }

    public void OnFixedUpdate()
    {
        wolf.animator.SetFloat("Speed", wolf.navMeshAgent.velocity.magnitude);

        if(wolf.monsterStat.IsDead())
        {
            wolf.SetMonsterState(new WolfDeadState(wolf));
        }
        else if(wolf.chaseTrigger.inSight)
        {
            wolf.SetMonsterState(new WolfFightState(wolf));
        }
        else if(wolf.navMeshAgent.remainingDistance <= wolf.navMeshAgent.stoppingDistance)
        {
            wolf.SetMonsterState(new WolfIdleState(wolf));
        }
    }

    // 몬스터가 방황을 시작할때, 일정 범위 내의 랜덤한 위치로 목적지를 설정합니다.
    public void OnEnterState()
    {
        wolf.navMeshAgent.speed = 1;
        wolf.animator.SetFloat("Speed", wolf.navMeshAgent.velocity.magnitude);
        int maxTryCount = 10;
        float wanderRange = 20;

        // 적절한 목적지를 찾을 때까지 최대 10번 시도
        while(maxTryCount > 0)
        {
            maxTryCount--;
            wanderRange += 1f;

            // agent가 현재 위치로부터 일정거리 안에 있는 랜덤한 위치로 이동
            var randomPos = Random.insideUnitCircle * wanderRange;
            var destPos = wolf.transform.position + new Vector3(randomPos.x, 0, randomPos.y);

            // destPos가 NavMesh 위에 있는지 확인
            NavMeshHit hit;
            if (NavMesh.SamplePosition(destPos, out hit, 1.5f, NavMesh.AllAreas))
            {
                wolf.navMeshAgent.SetDestination(hit.position);
                return;
            }
        }
    }

    public void OnExitState()
    {
        
    }
}


// 이 클래스는 늑대 몬스터의 사망 상태를 담당합니다.
public class WolfDeadState : IMonsterState
{
    private Wolf wolf;
    private float deadStateDuration = 0;
    private const float maxDeadStateDuration = 5;

    public WolfDeadState(Monster monster)
    {
        wolf = monster as Wolf;
    }

    public void OnFixedUpdate()
    {
        deadStateDuration += Time.fixedDeltaTime;
        if(deadStateDuration > maxDeadStateDuration)
        {
            wolf.SetMonsterState(new WolfRespawnState(wolf));
        }
    }

    public void OnEnterState()
    {
        wolf.animator.SetFloat("Speed", wolf.navMeshAgent.velocity.magnitude);
        wolf.animator.SetTrigger("Die");
        wolf.navMeshAgent.ResetPath();
        wolf.navMeshAgent.speed = 1;

        // 몬스터가 죽었을 때, 드랍 테이블에 따라 아이템을 필드에 생성
        var dropTable = wolf.monsterStat.GetDropTable();
        foreach (var dropItem in dropTable)
        {
            if(Random.Range(0f, 1f) < dropItem.dropRate)
            {
                // 현재 몬스터의 위치에서 일정 범위 안에 랜덤한 위치에 아이템 생성
                var randomPos = Random.insideUnitCircle;
                var itemPos = wolf.transform.position + new Vector3(randomPos.x, 0, randomPos.y);
                var go = FieldItem.CreateFieldItem(dropItem.item, itemPos);
            }
        }
    }

    public void OnExitState()
    {
        wolf.animator.SetTrigger("DieComplete");
    }
}


// 이 클래스는 늑대 몬스터의 리스폰 상태를 담당합니다.
public class WolfRespawnState : IMonsterState
{
    private Monster wolf;
    private float respawnIdle = 0;
    private const float maxRespawnIdle = 1;
    private const float minRespawnDistance = 3;
    private const float maxRespawnDistance = 10;

    public WolfRespawnState(Monster monster)
    {
        wolf = monster as Wolf;
    }

    public void OnFixedUpdate()
    {
        if(wolf.monsterStat.IsDead())
        {
            wolf.SetMonsterState(new WolfDeadState(wolf));
        }
        else if(respawnIdle > maxRespawnIdle)
        {
            wolf.SetMonsterState(new WolfIdleState(wolf));
        }
        else
        {
            respawnIdle += Time.deltaTime;
        }
    }

    public void OnEnterState()
    {
        wolf.animator.SetFloat("Speed", wolf.navMeshAgent.velocity.magnitude);
        wolf.monsterStat.RestoreHP();
        wolf.navMeshAgent.ResetPath();

        // 몬스터가 리스폰될 때, 일정 범위 내의 랜덤한 위치로 몬스터를 이동
        while(true)
        {
            var center = new Vector3(Random.Range(-80, -10), 5, Random.Range(85, 125));
            NavMeshHit hit;
            if (NavMesh.SamplePosition(center, out hit, maxRespawnDistance, NavMesh.AllAreas))
            {
                // monster.rigidbody.position = hit.position;
                wolf.navMeshAgent.Warp(hit.position);
                break;
            }
        }
    }

    public void OnExitState()
    {

    }
}
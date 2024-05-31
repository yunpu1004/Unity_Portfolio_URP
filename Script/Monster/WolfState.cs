using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WolfIdleState : IMonsterState
{
    private Monster monster;
    private float idleTime = 0;
    private readonly float maxIdleTime = 3;

    public WolfIdleState(Monster monster)
    {
        this.monster = monster;
    }

    public void OnFixedUpdate()
    {
        if(monster.monsterStat.IsDead())
        {
            monster.SetMonsterState(new WolfDeadState(monster));
        }
        else if(monster.chaseTrigger.IsInSight())
        {
            monster.SetMonsterState(new WolfFightState(monster));
        }
        else if(idleTime > maxIdleTime)
        {
            monster.SetMonsterState(new WolfWanderState(monster));
        }
        else
        {
            idleTime += Time.deltaTime;
        }
    }

    public void OnEnterState()
    {
        monster.navMeshAgent.ResetPath();
        monster.navMeshAgent.speed = 0;
        monster.animator.SetFloat("Speed", 0);
    }

    public void OnExitState()
    {

    }
}



public class WolfFightState : IMonsterState
{
    private Monster monster;
    private float attackDelay = 0;
    private readonly float maxAttackDelay = 3;
    private float lostSightChaseDuration = 0;
    private float maxLostSightChaseDuration = 5;

    public WolfFightState(Monster monster)
    {
        this.monster = monster;
    }

    public void OnFixedUpdate()
    {
        monster.animator.SetFloat("Speed", monster.navMeshAgent.velocity.magnitude);

        if(monster.monsterStat.IsDead())
        {
            monster.SetMonsterState(new WolfDeadState(monster));
        }

        else if(lostSightChaseDuration > maxLostSightChaseDuration)
        {
            monster.SetMonsterState(new WolfIdleState(monster));
        }
        
        else
        {
            // 플레이어를 추적
            monster.navMeshAgent.SetDestination(monster.playerTransform.position);

            // 플레이어가 시야에서 벗어나더라도 일정 시간 동안 추적
            if(monster.chaseTrigger.IsInSight()) lostSightChaseDuration = 0;
            else lostSightChaseDuration += Time.deltaTime;

            // 플레이어가 몬스터와 가까이 있으면
            if (monster.navMeshAgent.remainingDistance <= monster.navMeshAgent.stoppingDistance)
            {
                // 대상을 바라보는 방향 계산
                Vector3 direction = (monster.playerTransform.position - monster.transform.position).normalized;
                direction.y = 0;

                // 몬스터의 바라볼 방향으로 쿼터니언을 계산
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                // 회전을 부드럽게 조절
                monster.transform.rotation = Quaternion.Slerp(monster.transform.rotation, lookRotation, Time.deltaTime * 5f);

                // 공격 딜레이 갱신
                attackDelay += Time.deltaTime;
                if(attackDelay > maxAttackDelay)
                {
                    monster.animator.SetTrigger("Attack");
                    attackDelay = 0;
                }
            }
        }
    }

    public void OnEnterState()
    {
        monster.navMeshAgent.speed = 5;
    }

    public void OnExitState()
    {
        
    }
}



public class WolfWanderState : IMonsterState
{
    private Monster monster;

    public WolfWanderState(Monster monster)
    {
        this.monster = monster;
    }

    public void OnFixedUpdate()
    {
        monster.animator.SetFloat("Speed", monster.navMeshAgent.velocity.magnitude);

        if(monster.monsterStat.IsDead())
        {
            monster.SetMonsterState(new WolfDeadState(monster));
        }
        else if(monster.chaseTrigger.IsInSight())
        {
            monster.SetMonsterState(new WolfFightState(monster));
        }
        else if(monster.navMeshAgent.remainingDistance <= monster.navMeshAgent.stoppingDistance)
        {
            monster.SetMonsterState(new WolfIdleState(monster));
        }
    }

    // 몬스터가 방황을 시작할때, 일정 범위 내의 랜덤한 위치로 목적지를 설정합니다.
    public void OnEnterState()
    {
        monster.navMeshAgent.speed = 1;
        monster.animator.SetFloat("Speed", monster.navMeshAgent.velocity.magnitude);
        int maxTryCount = 10;
        float wanderRange = 20;

        // 적절한 목적지를 찾을 때까지 최대 10번 시도
        while(maxTryCount > 0)
        {
            maxTryCount--;
            wanderRange += 1f;

            // agent가 현재 위치로부터 일정거리 안에 있는 랜덤한 위치로 이동
            var randomPos = Random.insideUnitCircle * wanderRange;
            var destPos = monster.transform.position + new Vector3(randomPos.x, 0, randomPos.y);

            // destPos가 NavMesh 위에 있는지 확인
            NavMeshHit hit;
            if (NavMesh.SamplePosition(destPos, out hit, 1.5f, NavMesh.AllAreas))
            {
                monster.navMeshAgent.SetDestination(hit.position);
                return;
            }
        }
    }

    public void OnExitState()
    {
        
    }
}



public class WolfDeadState : IMonsterState
{
    private Monster monster;

    public WolfDeadState(Monster monster)
    {
        this.monster = monster;
    }

    public void OnFixedUpdate()
    {
        
    }

    public void OnEnterState()
    {
        monster.animator.SetFloat("Speed", monster.navMeshAgent.velocity.magnitude);
        monster.navMeshAgent.ResetPath();
        monster.navMeshAgent.speed = 1;

        // 몬스터가 죽었을 때, 드랍 테이블에 따라 아이템을 필드에 생성합니다.
        var dropTable = monster.monsterStat.GetDropTable();
        foreach (var dropItem in dropTable)
        {
            if(Random.Range(0f, 1f) < dropItem.dropRate)
            {
                // 현재 몬스터의 위치에서 일정 범위 안에 랜덤한 위치에 아이템 생성
                var randomPos = Random.insideUnitCircle;
                var itemPos = monster.transform.position + new Vector3(randomPos.x, 0, randomPos.y);
                var go = FieldItem.CreateFieldItem(dropItem.item, itemPos);
            }
        }
    }

    public void OnExitState()
    {
        
    }
}



public class WolfRespawnState : IMonsterState
{
    private Monster monster;
    private float respawnIdle = 0;
    private readonly float maxRespawnIdle = 1;

    public WolfRespawnState(Monster monster)
    {
        this.monster = monster;
    }

    public void OnFixedUpdate()
    {
        if(monster.monsterStat.IsDead())
        {
            monster.SetMonsterState(new WolfDeadState(monster));
        }
        else if(respawnIdle > maxRespawnIdle)
        {
            monster.SetMonsterState(new WolfIdleState(monster));
        }
        else
        {
            respawnIdle += Time.deltaTime;
        }
    }

    public void OnEnterState()
    {
        monster.animator.SetFloat("Speed", monster.navMeshAgent.velocity.magnitude);
        monster.monsterStat.RestoreHP();
        monster.navMeshAgent.ResetPath();

        // 몬스터가 리스폰될 때, 일정 범위 내의 랜덤한 위치로 몬스터를 이동시킵니다.
        while(true)
        {
            var center = new Vector3(Random.Range(-80, -10), 5, Random.Range(85, 125));
            NavMeshHit hit;
            if (NavMesh.SamplePosition(center, out hit, 10, NavMesh.AllAreas))
            {
                monster.rigidbody.position = hit.position;
                return;
            }
        }
    }

    public void OnExitState()
    {
        
    }
}

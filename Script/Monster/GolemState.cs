using UnityEngine;


// 이 클래스는 골렘 몬스터의 대기 상태를 담당합니다.
public class GolemIdleState : IMonsterState
{
    private Golem golem;

    public GolemIdleState(Monster monster)
    {
        golem = monster as Golem;
    }

    public void OnFixedUpdate()
    {
        golem.animator.SetFloat("Speed", golem.navMeshAgent.velocity.magnitude);
        
        if(golem.monsterStat.IsDead())
        {
            golem.SetMonsterState(new GolemDeadState(golem));
        }
        else if(golem.chaseTrigger.inSight)
        {
            golem.SetMonsterState(new GolemFightState(golem));
        }
    }

    public void OnEnterState()
    {
        golem.navMeshAgent.SetDestination(golem.idleWayPoint.position);
    }

    public void OnExitState()
    {

    }
}


// 이 클래스는 골렘 몬스터의 전투 상태를 담당합니다.
public class GolemFightState : IMonsterState
{
    private Golem golem;
    private float attackDelay = 0;
    private readonly float maxAttackDelay = 3;
    private float lostSightChaseDuration = 0;
    private readonly float maxLostSightChaseDuration = 5;
    private readonly float moveSpeed = 2;

    public GolemFightState(Monster monster)
    {
        golem = monster as Golem;
    }

    public void OnFixedUpdate()
    {
        golem.animator.SetFloat("Speed", golem.navMeshAgent.velocity.magnitude);

        if(golem.monsterStat.IsDead())
        {
            golem.SetMonsterState(new GolemDeadState(golem));
        }

        else if(lostSightChaseDuration > maxLostSightChaseDuration)
        {
            golem.SetMonsterState(new GolemIdleState(golem));
        }
        
        else
        {
            // 플레이어를 추적
            golem.navMeshAgent.SetDestination(golem.playerTransform.position);

            // 플레이어가 시야에서 벗어나더라도 일정 시간 동안 추적
            if(golem.chaseTrigger.inSight) lostSightChaseDuration = 0;
            else lostSightChaseDuration += Time.deltaTime;

            // 플레이어가 몬스터와 가까이 있으면
            if (golem.navMeshAgent.remainingDistance <= golem.navMeshAgent.stoppingDistance)
            {
    
                // 대상을 바라보는 방향 계산
                Vector3 direction = (golem.playerTransform.position - golem.transform.position).normalized;
                direction.y = 0;

                // 몬스터의 바라볼 방향으로 쿼터니언을 계산
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                // 회전을 부드럽게 조절
                golem.transform.rotation = Quaternion.Slerp(golem.transform.rotation, lookRotation, Time.deltaTime * 5f);

                // 공격 딜레이 갱신
                attackDelay += Time.deltaTime;
                if(attackDelay > maxAttackDelay)
                {
                    golem.animator.SetTrigger("Attack");
                    attackDelay = 0;
                }
            }
        }
    }

    public void OnEnterState()
    {
        golem.navMeshAgent.speed = moveSpeed;
    }

    public void OnExitState()
    {
        
    }
}


// 이 클래스는 골렘 몬스터의 사망 상태를 담당합니다.
public class GolemDeadState : IMonsterState
{
    private Golem golem;
    private float deadStateDuration = 0;
    private const float maxDeadStateDuration = 5;

    public GolemDeadState(Monster monster)
    {
        golem = monster as Golem;
    }

    public void OnFixedUpdate()
    {
        deadStateDuration += Time.fixedDeltaTime;
        if(deadStateDuration > maxDeadStateDuration)
        {
            GameObject.Destroy(golem.gameObject);
            // 여기에 게임 엔딩 이벤트를 추가
        }
    }

    public void OnEnterState()
    {
        golem.animator.SetFloat("Speed", golem.navMeshAgent.velocity.magnitude);
        golem.animator.SetTrigger("Die");
        golem.navMeshAgent.ResetPath();
        golem.navMeshAgent.speed = 1;

        // 몬스터가 죽었을 때, 드랍 테이블에 따라 아이템을 필드에 생성합니다.
        var dropTable = golem.monsterStat.GetDropTable();
        foreach (var dropItem in dropTable)
        {
            if(Random.Range(0f, 1f) < dropItem.dropRate)
            {
                // 현재 몬스터의 위치에서 일정 범위 안에 랜덤한 위치에 아이템 생성
                var randomPos = Random.insideUnitCircle;
                var itemPos = golem.transform.position + new Vector3(randomPos.x, 0, randomPos.y);
                var go = FieldItem.CreateFieldItem(dropItem.item, itemPos);
            }
        }
    }

    public void OnExitState()
    {
        golem.animator.SetTrigger("DieComplete");
    }
}
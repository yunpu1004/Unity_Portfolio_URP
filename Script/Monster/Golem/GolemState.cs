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
        golem.Move(golem.idleWayPoint.position);
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

        // 플레이어가 일정 시간 동안 시야에서 벗어나면 대기 상태로 전환
        else if(lostSightChaseDuration > maxLostSightChaseDuration)
        {
            golem.SetMonsterState(new GolemIdleState(golem));
        }
        
        else
        {
            // 플레이어를 추적
            golem.Move(golem.playerTransform.position);

            // 플레이어가 시야에서 벗어나있는 시간을 측정
            if(golem.chaseTrigger.inSight) lostSightChaseDuration = 0;
            else lostSightChaseDuration += Time.deltaTime;

            // 플레이어가 몬스터와 가까이 있으면
            if (golem.navMeshAgent.remainingDistance <= golem.navMeshAgent.stoppingDistance)
            {
                // 공격 딜레이 갱신
                attackDelay += Time.deltaTime;
                if(attackDelay > maxAttackDelay)
                {
                    golem.Attack();
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
        }
    }

    public void OnEnterState()
    {
        golem.Die();
        golem.DropItem();
    }

    public void OnExitState()
    {

    }
}
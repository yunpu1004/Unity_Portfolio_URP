using UnityEngine;
using UnityEngine.AI;

// 이 클래스는 뱀 몬스터의 대기 상태를 담당합니다.
public class SnakeIdleState : IMonsterState
{
    private Snake snake;
    private float idleTime = 0;
    private const float maxIdleTime = 3;

    public SnakeIdleState(Monster monster)
    {
        snake = monster as Snake;
    }

    public void OnFixedUpdate()
    {
        if(snake.monsterStat.IsDead())
        {
            snake.SetMonsterState(new SnakeDeadState(snake));
        }
        else if(snake.chaseTrigger.inSight)
        {
            snake.SetMonsterState(new SnakeFightState(snake));
        }
        else if(idleTime > maxIdleTime)
        {
            snake.SetMonsterState(new SnakeWanderState(snake));
        }
        else
        {
            idleTime += Time.deltaTime;
        }
    }

    public void OnEnterState()
    {
        snake.Stop();
    }

    public void OnExitState()
    {

    }
}


// 이 클래스는 뱀 몬스터의 전투 상태를 담당합니다.
public class SnakeFightState : IMonsterState
{
    private Snake snake;
    private float attackDelay = 0;
    private const float maxAttackDelay = 3;
    private float lostSightChaseDuration = 0;
    private const float maxLostSightChaseDuration = 10;
    private const float moveSpeed = 3;

    public SnakeFightState(Monster monster)
    {
        snake = monster as Snake;
    }

    public void OnFixedUpdate()
    {
        snake.animator.SetFloat("Speed", snake.navMeshAgent.velocity.magnitude);

        if(snake.monsterStat.IsDead())
        {
            snake.SetMonsterState(new SnakeDeadState(snake));
        }

        // 플레이어가 일정 시간 동안 시야에서 벗어나면 대기 상태로 전환
        else if(lostSightChaseDuration > maxLostSightChaseDuration)
        {
            snake.SetMonsterState(new SnakeIdleState(snake));
        }
        
        else
        {
            // 플레이어를 추적
            snake.Move(snake.playerTransform.position);

            // 플레이어가 시야에서 벗어나있는 시간을 측정
            if(snake.chaseTrigger.inSight) lostSightChaseDuration = 0;
            else lostSightChaseDuration += Time.deltaTime;

            // 플레이어가 몬스터와 가까이 있으면
            if (snake.navMeshAgent.remainingDistance <= snake.navMeshAgent.stoppingDistance)
            {
                // 공격 딜레이 갱신
                attackDelay += Time.deltaTime;
                if(attackDelay > maxAttackDelay)
                {
                    snake.Attack();
                    attackDelay = 0;
                }
            }
        }
    }

    public void OnEnterState()
    {
        snake.navMeshAgent.speed = moveSpeed;
    }

    public void OnExitState()
    {
        
    }
}


// 이 클래스는 뱀 몬스터의 방황 상태를 담당합니다.
public class SnakeWanderState : IMonsterState
{
    private Snake snake;

    public SnakeWanderState(Monster monster)
    {
        snake = monster as Snake;
    }

    public void OnFixedUpdate()
    {
        snake.animator.SetFloat("Speed", snake.navMeshAgent.velocity.magnitude);

        if(snake.monsterStat.IsDead())
        {
            snake.SetMonsterState(new SnakeDeadState(snake));
        }
        else if(snake.chaseTrigger.inSight)
        {
            snake.SetMonsterState(new SnakeFightState(snake));
        }
        else if(snake.navMeshAgent.remainingDistance <= snake.navMeshAgent.stoppingDistance)
        {
            snake.SetMonsterState(new SnakeIdleState(snake));
        }
    }

    // 몬스터가 방황을 시작할때, 일정 범위 내의 랜덤한 위치로 목적지를 설정합니다.
    public void OnEnterState()
    {
        snake.navMeshAgent.speed = 1;
        snake.animator.SetFloat("Speed", snake.navMeshAgent.velocity.magnitude);
        int maxTryCount = 10;
        float wanderRange = 20;

        // 적절한 목적지를 찾을 때까지 최대 10번 시도
        while(maxTryCount > 0)
        {
            maxTryCount--;
            wanderRange += 1f;

            // agent가 현재 위치로부터 일정거리 안에 있는 랜덤한 위치로 이동
            var randomPos = Random.insideUnitCircle * wanderRange;
            var destPos = snake.transform.position + new Vector3(randomPos.x, 0, randomPos.y);

            // destPos가 NavMesh 위에 있는지 확인
            NavMeshHit hit;
            if (NavMesh.SamplePosition(destPos, out hit, 1.5f, NavMesh.AllAreas))
            {
                snake.Move(hit.position);
                return;
            }
        }
    }

    public void OnExitState()
    {
        
    }
}


// 이 클래스는 뱀 몬스터의 사망 상태를 담당합니다.
public class SnakeDeadState : IMonsterState
{
    private Snake snake;
    private float deadStateDuration = 0;
    private const float maxDeadStateDuration = 5;

    public SnakeDeadState(Monster monster)
    {
        snake = monster as Snake;
    }

    public void OnFixedUpdate()
    {
        deadStateDuration += Time.fixedDeltaTime;
        if(deadStateDuration > maxDeadStateDuration)
        {
            snake.SetMonsterState(new SnakeRespawnState(snake));
        }
    }

    public void OnEnterState()
    {
        snake.Die();
        snake.DropItem();
    }

    public void OnExitState()
    {
        
    }
}


// 이 클래스는 뱀 몬스터의 리스폰 상태를 담당합니다.
public class SnakeRespawnState : IMonsterState
{
    private Snake snake;
    private float respawnIdle = 0;
    private const float maxRespawnIdle = 1;

    public SnakeRespawnState(Monster monster)
    {
        snake = monster as Snake;
    }

    public void OnFixedUpdate()
    {
        if(snake.monsterStat.IsDead())
        {
            snake.SetMonsterState(new SnakeDeadState(snake));
        }
        else if(respawnIdle > maxRespawnIdle)
        {
            snake.SetMonsterState(new SnakeIdleState(snake));
        }
        else
        {
            respawnIdle += Time.deltaTime;
        }
    }

    public void OnEnterState()
    {
        snake.Respawn();
    }

    public void OnExitState()
    {

    }
}
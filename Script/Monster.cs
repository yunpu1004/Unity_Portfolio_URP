using UnityEngine;
using UnityEngine.AI;

// 이 스크립트는 몬스터의 행동, 상태 변화 및 플레이어와의 전투를 지시 및 제어합니다.
public class Monster : MonoBehaviour
{
    private MonsterState state = MonsterState.Idle;
    private float idleTime = 0;
    private float maxIdleTime = 3;
    private float lostSightChaseDuration = 0;
    private float maxLostSightChaseDuration = 5;
    private float attackDelay = 0;
    private float maxAttackDelay = 3;
    private float respawnIdle = 0;
    private float maxRespawnIdle = 1;
    
    private Animator animator;
    private NavMeshAgent agent;
    private ChaseTrigger chaseTrigger;
    private Transform playerTransform;
    private MonsterStat monsterStat;
    private Weapon weapon;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        chaseTrigger = GetComponentInChildren<ChaseTrigger>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        monsterStat = GetComponent<MonsterStat>();
        weapon = GetComponentInChildren<Weapon>(true);

        // 이벤트 등록
        var playerStat = playerTransform.GetComponent<PlayerStat>();
        var playerActivity = playerTransform.GetComponent<PlayerActivity>();
        monsterStat.AddOnDamageEvent((damage) => animator.SetTrigger("Hit"));
        monsterStat.AddOnDeathEvent(() => playerStat.GainExp(monsterStat.GetExp()));
        monsterStat.AddOnDeathEvent(() => animator.SetTrigger("Die"));
        monsterStat.AddOnDeathEvent(() => DropItem());
        monsterStat.AddOnDeathEvent(() => playerActivity.SetRecentActivity(new PlayerActivityData{activityType = PlayerActivityData.ActivityType.Kill, activityTarget = name}));

    }

    // 몬스터의 상태를 갱신하고 행동을 결정합니다.
    private void FixedUpdate() 
    {
        // 몬스터의 속도를 애니메이터에 전달
        var velocity = agent.velocity;
        animator.SetFloat("Speed", velocity.magnitude);

        // 몬스터의 상태를 갱신
        bool stateChanged = false;
        if(monsterStat.IsDead())
        {
            stateChanged = state != MonsterState.Dead;
            state = MonsterState.Dead;
        }
        else if(respawnIdle > 0)
        {
            stateChanged = state != MonsterState.Respawn;
            state = MonsterState.Respawn;
        }
        else if(chaseTrigger.IsInSight() || lostSightChaseDuration > 0)
        {
            stateChanged = state != MonsterState.Engagement;
            state = MonsterState.Engagement;
        }
        else if(idleTime > maxIdleTime)
        {
            idleTime = 0;
            stateChanged = state != MonsterState.Wander;
            state = MonsterState.Wander;
        }
        else if(Mathf.Approximately(velocity.magnitude, 0))
        {
            stateChanged = state != MonsterState.Idle;
            state = MonsterState.Idle;
        }

        // 몬스터의 상태에 따라 행동을 결정
        if(state == MonsterState.Dead)
        {
            if(stateChanged)
            {
                agent.ResetPath();
                agent.speed = 1;
            }
        }
        else if(state == MonsterState.Engagement)
        {
            if(stateChanged)
            {
                agent.speed = 5;
                attackDelay = 0;
            }

            // 몬스터가 전투중이면 플레이어를 추적
            agent.SetDestination(playerTransform.position);

            // 플레이어를 추적하다가 시야에서 벗어나더라도 일정 시간 동안 추적
            if(chaseTrigger.IsInSight()) lostSightChaseDuration = maxLostSightChaseDuration;
            else lostSightChaseDuration -= Time.deltaTime;

            // 플레이어가 몬스터의 목표 지점에 가까이 있으면
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                // 대상을 바라보는 방향 계산 (단, y축은 고려하지 않음)
                Vector3 direction = (playerTransform.position - transform.position).normalized;
                direction.y = 0;

                // 몬스터의 바라볼 방향으로 쿼터니언을 계산
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                // 에이전트의 회전을 부드럽게 조절
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

                // 공격 딜레이 갱신
                attackDelay += Time.deltaTime;
                if(attackDelay > maxAttackDelay)
                {
                    animator.SetTrigger("Attack");
                    attackDelay = 0;
                }
            }
        }
        else if(state == MonsterState.Idle)
        {
            idleTime += Time.deltaTime;
        }
        else if(state == MonsterState.Wander)
        {
            if(stateChanged) StartWander();
        }
        else if(state == MonsterState.Respawn)
        {
            respawnIdle -= Time.deltaTime;
            if(respawnIdle < 0) respawnIdle = 0;
        }
    }

    // 몬스터가 방황하는 상태일때, 일정 범위 안에 랜덤한 위치로 이동을 시작합니다.
    public void StartWander()
    {
        agent.speed = 1;
        int maxTryCount = 10;
        float wanderRange = 20;
        while(maxTryCount > 0)
        {
            maxTryCount--;
            wanderRange += 1f;

            // agent가 현재 위치로부터 일정거리 안에 있는 랜덤한 위치로 이동
            var randomPos = UnityEngine.Random.insideUnitCircle * wanderRange;
            var destPos = transform.position + new Vector3(randomPos.x, 0, randomPos.y);

            // destPos가 NavMesh 위에 있는지 확인
            NavMeshHit hit;
            if (NavMesh.SamplePosition(destPos, out hit, 1.5f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                return;
            }
        }
    }

    // 이 메소드는 애니메이션 이벤트로 호출됨
    private void Respawn()
    {
        monsterStat.RestoreHP();
        state = MonsterState.Respawn;
        lostSightChaseDuration = 0;
        attackDelay = 0;
        agent.ResetPath();
        respawnIdle = maxRespawnIdle;
        
        while(true)
        {
            var center = new Vector3(UnityEngine.Random.Range(-80, -10), 5, UnityEngine.Random.Range(85, 125));
            NavMeshHit hit;
            if (NavMesh.SamplePosition(center, out hit, 10, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
                return;
            }
        }
    }

    // 몬스터가 죽었을 때, 드랍 테이블에 따라 아이템을 필드에 생성합니다.
    public void DropItem()
    {
        var array = monsterStat.GetDropItemArray();
        foreach (var itemAndRate in array)
        {
            if(UnityEngine.Random.Range(0f, 1f) < itemAndRate.Item2)
            {
                // 현재 몬스터의 위치에서 일정 범위 안에 랜덤한 위치에 아이템 생성
                var randomPos = UnityEngine.Random.insideUnitCircle;
                var itemPos = transform.position + new Vector3(randomPos.x, 0, randomPos.y);
                var go = FieldItem.CreateFieldItem(itemAndRate.Item1, itemPos);
            }
        }
    }

    // 이 메소드는 애니메이션 이벤트로 호출됨
    private void ActivateWeapon()
    {
        weapon.ActivateWeapon();
    }

    // 이 메소드는 애니메이션 이벤트로 호출됨
    private void DeactivateWeapon()
    {
        weapon.DeactivateWeapon();
    }
}

public enum MonsterState
{
    Idle,
    Engagement,
    Wander,
    Dead,
    Respawn,
}

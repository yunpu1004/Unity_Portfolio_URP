using UnityEngine;
using UnityEngine.AI;

// 이 스크립트는 몬스터의 기본적인 행동을 정의합니다.
public abstract class Monster : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent navMeshAgent;
    public ChaseTrigger chaseTrigger;
    public Transform playerTransform;
    public MonsterStat monsterStat;
    public Rigidbody rigidbody;
    private Weapon weapon;
    private IMonsterState monsterState;

    void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        chaseTrigger = GetComponentInChildren<ChaseTrigger>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        monsterStat = GetComponent<MonsterStat>();
        rigidbody = GetComponent<Rigidbody>();
        weapon = GetComponentInChildren<Weapon>(true);
        monsterState = SetMonsterStateOnAwake();

        // 이벤트 등록
        var playerStat = playerTransform.GetComponent<PlayerStat>();
        var playerActivity = playerTransform.GetComponent<PlayerActivity>();
        monsterStat.AddOnDamageEvent((damage) => animator.SetTrigger("Hit"));
        monsterStat.AddOnDeathEvent(() => playerStat.GainExp(monsterStat.GetExp()));
        monsterStat.AddOnDeathEvent(() => animator.SetTrigger("Die"));
        monsterStat.AddOnDeathEvent(() => playerActivity.SetRecentActivity(new PlayerActivityData{activityType = PlayerActivityData.ActivityType.Kill, activityTarget = name}));
    }

    // Awake 시점에 몬스터의 종류에 알맞는 상태 객체를 할당합니다.
    protected abstract IMonsterState SetMonsterStateOnAwake();

    // 몬스터의 상태를 변경합니다.
    public void SetMonsterState(IMonsterState state)
    {
        // 새 상태가 null이거나 현재 상태와 같은 상태일 경우 아무것도 하지 않음
        if(state == null) return;
        else if(monsterState != null && monsterState.GetType() == state.GetType()) return;

        // 현재 상태가 null인 경우의 처리
        else if(monsterState == null)
        {
            monsterState = state;
            monsterState.OnEnterState();
        }

        // 현 상태가 null이 아닌 경우의 처리
        else
        {
            monsterState.OnExitState();
            monsterState = state;
            monsterState.OnEnterState();
        }
        
    }

    // 상태 객체의 OnFixedUpdate 메소드를 호출합니다.
    private void FixedUpdate() 
    {
        monsterState.OnFixedUpdate();
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
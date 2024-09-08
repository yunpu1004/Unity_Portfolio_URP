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
    protected Weapon weapon;
    private IMonsterState monsterState;
    public AudioClip attackAudioClip;

    void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        chaseTrigger = GetComponentInChildren<ChaseTrigger>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        monsterStat = GetComponent<MonsterStat>();
        rigidbody = GetComponent<Rigidbody>();
        weapon = GetComponentInChildren<Weapon>(true);
        monsterState = SetMonsterInitialState();
        SetAwakeStrategy().OnAwake(gameObject);
    }

    // 몬스터의 초기화를 실행하는 객체를 반환합니다.
    protected abstract IAwakeStrategy SetAwakeStrategy();

    // 몬스터의 초기 상태에 대한 객체를 반환합니다.
    protected abstract IMonsterState SetMonsterInitialState();

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

    // 몬스터가 공격합니다.
    // 애니메이션이 재생되면, 지정된 시점에 ActivateWeapon과 DeactivateWeapon 메서드가 실행됩니다.
    public virtual void Attack()
    {
        animator.SetTrigger("Attack");
    }

    // 몬스터가 사망합니다.
    public virtual void Die()
    {
        animator.SetTrigger("Die");
        navMeshAgent.ResetPath();
        animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
        navMeshAgent.speed = 1;
    }

    // 몬스터가 리스폰합니다.
    public virtual void Respawn()
    {
        animator.SetTrigger("Respawn");
        monsterStat.RestoreHP();
        navMeshAgent.ResetPath();

        // 일정 범위 내의 랜덤한 위치로 몬스터를 이동
        while(true)
        {
            var center = new Vector3(Random.Range(-80, -10), 5, Random.Range(85, 125));
            NavMeshHit hit;
            if (NavMesh.SamplePosition(center, out hit, 10, NavMesh.AllAreas))
            {
                navMeshAgent.Warp(hit.position);
                break;
            }
        }
    }

    // 몬스터가 아이템을 드랍합니다.
    public virtual void DropItem()
    {
        // 몬스터가 죽었을 때, 드랍 테이블에 따라 아이템을 필드에 생성
        var dropTable = monsterStat.GetDropTable();
        foreach (var dropItem in dropTable)
        {
            if(Random.Range(0f, 1f) < dropItem.dropRate)
            {
                // 현재 몬스터의 위치에서 일정 범위 안에 랜덤한 위치에 아이템 생성
                var randomPos = Random.insideUnitCircle;
                var itemPos = transform.position + new Vector3(randomPos.x, 0, randomPos.y);
                var go = FieldItem.CreateFieldItem(dropItem.item, itemPos);
            }
        }
    }

    // 몬스터가 이동합니다.
    public virtual void Move(Vector3 destination)
    {
        // 몬스터의 이동 목적지 설정
        navMeshAgent.SetDestination(destination);

        // 목적지를 바라보는 방향 계산
        Vector3 direction = (destination - transform.position).normalized;
        direction.y = 0;

        // 몬스터의 바라볼 방향으로 쿼터니언을 계산
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // 회전을 부드럽게 조절
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // 몬스터가 목적지를 초기화 하고 멈춥니다.
    public virtual void Stop()
    {
        navMeshAgent.ResetPath();
        navMeshAgent.speed = 0;
        animator.SetFloat("Speed", 0);
    }

    // 공격 애니메이션중 실행되는 이벤트
    // 무기를 활성화합니다.
    protected virtual void ActivateWeapon()
    {
        weapon.ActivateWeapon();
        AudioSource.PlayClipAtPoint(attackAudioClip, transform.position);
    }

    // 공격 애니메이션중 실행되는 이벤트
    // 무기를 비활성화합니다.
    protected virtual void DeactivateWeapon()
    {
        weapon.DeactivateWeapon();
    }
}
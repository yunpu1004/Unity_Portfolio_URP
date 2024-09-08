using System;
using Cinemachine;
using UnityEngine;

// 이 스크립트는 플레이어의 스킬을 관리합니다.
// 옵저버 패턴을 사용하여 스킬 쿨타임이 변경될 때마다 이벤트를 발생시킵니다.
public class Skill : MonoBehaviour
{
    private const float SKILL_Q_COOLTIME = 10f;
    private const float SKILL_E_COOLTIME = 5f;

    private const float SKILL_Q_Damage_Multiplier = 4f;
    private const float SKILL_E_Damage_Multiplier = 2.5f;

    private float skillQCooltime;
    private float skillECooltime;

    private Action<float> onSkillQCooltimeChanged;
    private Action<float> onSkillECooltimeChanged;

    private int skillQAnimationHash;
    private int skillEAnimationHash;
    private int idleAnimationHash;

    public Animator animator;
    public PlayerStat playerStat;
    public CinemachineVirtualCamera playerCamera;

    public GameObject skillQEffect;
    public GameObject skillEEffect;

    public AudioClip[] skillQAudioClips;
    public AudioClip[] skillEAudioClips;

    public float GetSkillQCooltime() => skillQCooltime;
    public float GetSkillECooltime() => skillECooltime;


    private void Awake() 
    {
        skillQAnimationHash = Animator.StringToHash("SkillQ");
        skillEAnimationHash = Animator.StringToHash("SkillE");
        idleAnimationHash = Animator.StringToHash("Idle");
    }

    public void AddOnSkillQCooltimeChangedEvent(Action<float> action)
    {
        onSkillQCooltimeChanged += action;
    }

    public void AddOnSkillECooltimeChangedEvent(Action<float> action)
    {
        onSkillECooltimeChanged += action;
    }

    // Q 스킬을 사용합니다.
    public void UseSkillQ()
    {
        if(skillQCooltime > 0) return;
        if(!IsSkillAvailable()) return;
        skillQCooltime = SKILL_Q_COOLTIME;
        onSkillQCooltimeChanged?.Invoke(skillQCooltime);
        animator.SetTrigger("SkillQ");
    }

    // E 스킬을 사용합니다.
    public void UseSkillE()
    {
        if(skillECooltime > 0) return;
        if(!IsSkillAvailable()) return;
        skillECooltime = SKILL_E_COOLTIME;
        onSkillECooltimeChanged?.Invoke(skillECooltime);
        animator.SetTrigger("SkillE");
    }

    // 스킬 쿨타임을 업데이트합니다.
    public void UpdateSkillCooltime()
    {
        if(skillQCooltime > 0)
        {
            skillQCooltime -= Time.deltaTime;
            if(skillQCooltime < 0) skillQCooltime = 0;
            onSkillQCooltimeChanged?.Invoke(skillQCooltime);
        }

        if(skillECooltime > 0)
        {
            skillECooltime -= Time.deltaTime;
            if(skillECooltime < 0) skillECooltime = 0;
            onSkillECooltimeChanged?.Invoke(skillECooltime);
        }
    }

    // 현재 애니메이션이 스킬 사용 가능한 상태인지 확인합니다.
    public bool IsSkillAvailable()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.shortNameHash == idleAnimationHash;
    }

    // 현재 애니메이션이 Q스킬 사용 중인지 확인합니다.
    public bool IsSkillQAnimationPlaying()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.shortNameHash == skillQAnimationHash;
    }

    // 현재 애니메이션이 E스킬 사용 중인지 확인합니다.
    public bool IsSkillEAnimationPlaying()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.shortNameHash == skillEAnimationHash;
    }

    // 이 메소드는 애니메이터의 이벤트로 호출됩니다.
    private void Init_SkillQ()
    {
        AudioSource.PlayClipAtPoint(skillQAudioClips[0], transform.position);
        VisualEffectManager.instance.PlaySkillQEffect();
    }

    // 이 메소드는 애니메이터의 이벤트로 호출됩니다.
    private void Hit_SkillQ()
    {
        // 플레이어의 위치에 구형 콜라이더를 생성
        // 콜라이더의 반지름은 2f
        var center = transform.position + new Vector3(0f, 0.5f, 0f);
        Collider[] colliders = Physics.OverlapSphere(center, 2f);

        foreach (var collider in colliders)
        {
            if(collider.gameObject.layer == LayerMask.NameToLayer("Monster"))
            {
                var monsterStat = collider.GetComponent<MonsterStat>();
                if(monsterStat.IsDead()) continue;
                int damage = (int)(playerStat.GetAtk() * SKILL_Q_Damage_Multiplier);
                monsterStat.AddHP(-damage);
            }
        }

        // 스킬 이펙트 생성
        skillQEffect.SetActive(true);

        // 스킬 사운드 재생
        AudioSource.PlayClipAtPoint(skillQAudioClips[1], transform.position);
    }

    // 이 메소드는 애니메이터의 이벤트로 호출됩니다.
    private void Init_SkillE()
    {
        
    }
    

    // 이 메소드는 애니메이터의 이벤트로 호출됩니다.
    private void Hit_SkillE()
    {
        // 플레이어가 바라보는 시선방향으로 구형 콜라이더를 생성
        // 이때, 콜라이더의 중심은 플레이어의 위치에서 0.5f만큼 높이고, 0.5f만큼 앞에 위치
        // 콜라이더의 반지름은 1.5f
        var center = transform.position + transform.forward * 0.5f + new Vector3(0f, 0.5f, 0f);
        Collider[] colliders = Physics.OverlapSphere(center, 1f);

        foreach (var collider in colliders)
        {
            if(collider.gameObject.layer == LayerMask.NameToLayer("Monster"))
            {
                var monsterStat = collider.GetComponent<MonsterStat>();
                if(monsterStat.IsDead()) continue;
                int damage = (int)(playerStat.GetAtk() * SKILL_E_Damage_Multiplier);
                monsterStat.AddHP(-damage);
            }
        }

        /// 스킬 이펙트 생성
        skillEEffect.SetActive(true);

        // 스킬 사운드 재생
        AudioSource.PlayClipAtPoint(skillEAudioClips[1], transform.position);

        // 카메라의 이펙트 재생
        VisualEffectManager.instance.PlaySkillEEffect();
    }

    // 스킬 범위를 시각적으로 표현하기 위한 기즈모
    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0f, 0.5f, 0f), 2f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 0.5f + new Vector3(0f, 0.5f, 0f), 1.5f);
    }
}
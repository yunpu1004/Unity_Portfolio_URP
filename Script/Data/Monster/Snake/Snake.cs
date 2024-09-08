using UnityEngine;

// 이 스크립트는 뱀 몬스터를 담당하며, 몬스터 클래스에서 상속되어 뱀 몬스터 유형의 초기화와 처음 상태를 설정합니다.
// 전략 패턴과 상태 패턴을 사용하여 몬스터의 초기화 전략과 상태를 설정합니다.
public class Snake : Monster
{
    // 뱀의 초기화 전략을 설정합니다.
    protected override IAwakeStrategy SetAwakeStrategy()
    {
        return new NormalMonsterAwakeStrategy();
    }

    // 뱀의 초기 상태를 설정합니다.
    protected override IMonsterState SetMonsterInitialState()
    {
        return new SnakeIdleState(this);
    }

    // 공격 애니메이션중 실행되는 이벤트
    // 투사체를 발사합니다.
    protected override void ActivateWeapon()
    {
        Transform player = GameObject.Find("Player").transform;
        GameObject go = Instantiate(Resources.Load("Prefabs/뱀 투사체"), transform.position, Quaternion.identity) as GameObject;
        go.transform.Translate(new Vector3(0, 1.1f, 0));
        Projectile projectile = go.GetComponent<Projectile>();
        MonsterStat monsterStat = GetComponent<MonsterStat>();
        projectile.damage = monsterStat.GetAtk();
        projectile.rigidbody.AddForce((player.position + new Vector3(0, 0.8f, 0) - go.transform.position).normalized * 10f, ForceMode.Impulse);
        AudioSource.PlayClipAtPoint(attackAudioClip, transform.position);
    }
}
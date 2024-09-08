using System;
using System.Collections.Generic;
using UnityEngine;

// 이 스크립트는 플레이어 또는 몬스터가 근접 공격할때의 판정을 담당합니다.
public class Weapon : MonoBehaviour
{
    public LayerMask targetLayer;
    private HashSet<Collider> alreadyHit = new HashSet<Collider>();
    private BoxCollider boxCollider;
    private PlayerStat playerStat;
    private MonsterStat monsterStat;
    private bool isPlayer;
    public AudioClip hitSound;

    private void Awake() 
    {
        boxCollider = GetComponent<BoxCollider>();
        playerStat = FindObjectOfType<PlayerStat>();
        monsterStat = GetComponentInParent<MonsterStat>();
        isPlayer = monsterStat == null;
        boxCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 상대 레이어가 targetLayer에 포함되어 있지 않으면 무시
        if((targetLayer.value & 1 << other.gameObject.layer) == 0) return;
        if(alreadyHit.Contains(other)) return;

        // 이 공격이 플레이어의 공격이라면
        if(isPlayer)
        {
            int damage = playerStat.GetAtk();
            monsterStat = other.GetComponent<MonsterStat>();
            if(monsterStat.IsDead()) return;
            monsterStat.AddHP(-damage); 
        }

        // 이 공격이 몬스터의 공격이라면
        else
        {
            int damage = playerStat.GetDef()-monsterStat.GetAtk();
            playerStat.AddHP(Math.Min(damage, 0));
        }

        // 공격 히트 사운드 재생
        if(hitSound != null) AudioSource.PlayClipAtPoint(hitSound, other.transform.position);

        // 이미 피격한 적이라면 다시 피격하지 않도록 리스트에 추가
        alreadyHit.Add(other);
    }

    // 피격 판정을 활성화 합니다.
    public void ActivateWeapon()
    {
        boxCollider.enabled = true;
        alreadyHit.Clear();
    }

    // 피격 판정을 비활성화 합니다.
    public void DeactivateWeapon()
    {
        boxCollider.enabled = false;
        alreadyHit.Clear();
    }
}
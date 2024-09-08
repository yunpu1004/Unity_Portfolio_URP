using System;
using System.Collections.Generic;
using UnityEngine;

// 이 스크립트는 플레이어 또는 몬스터가 원거리 공격할때의 판정을 담당합니다.
public class Projectile : MonoBehaviour
{
    public int damage;
    public LayerMask targetLayer;
    public Rigidbody rigidbody;

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 만약 지형에 부딪히면 투사체를 제거함
        if(other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            Destroy(gameObject);
            return;
        }

        // 상대 레이어가 targetLayer에 포함되어 있지 않으면 무시
        if((targetLayer.value & 1 << other.gameObject.layer) == 0) return;

        else
        {
            // 만약 공격 타겟이 몬스터라면
            if(other.gameObject.layer == LayerMask.NameToLayer("Monster"))
            {
                MonsterStat monsterStat = other.GetComponent<MonsterStat>();
                if(monsterStat.IsDead()) return;
                monsterStat.AddHP(-damage); 
                Destroy(gameObject);
            }

            // 만약 공격 타겟이 플레이어라면
            else if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                PlayerStat playerStat = other.GetComponent<PlayerStat>();
                playerStat.AddHP(-damage);
                Destroy(gameObject);
            }
        }
    }
}
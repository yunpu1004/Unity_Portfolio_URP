using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Monster
{
    protected override IMonsterState SetMonsterStateOnAwake()
    {
        return new WolfIdleState(this);
    }
    
    // 이 메소드는 애니메이션 이벤트로 호출됨
    private void Respawn()
    {
        SetMonsterState(new WolfRespawnState(this));
    }
}
 
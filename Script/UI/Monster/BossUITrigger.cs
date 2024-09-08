using UnityEngine;

// 이 스크립트는 보스 몬스터의 트리거와 닿으면 보스 몬스터의 UI를 활성화 시킵니다.
public class BossUITrigger : MonoBehaviour
{
    // 플레이어가 보스 몬스터의 트리거 이내로 접근하면 보스 몬스터의 UI를 활성화 시킵니다.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster") && other.CompareTag("Boss"))
        {
            MonsterStat monsterStat = other.GetComponent<MonsterStat>();
            BossUI.instance.ShowUI();
            BossUI.instance.SetHPBar(monsterStat.GetHP(), monsterStat.GetMaxHP());
            BossUI.instance.SetNameText(monsterStat.name);
        }
    }

    // 플레이어가 보스 몬스터의 트리거에서 벗어나면 보스 몬스터의 UI를 숨깁니다.
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster") && other.CompareTag("Boss"))
        {
            MonsterStat monsterStat = other.GetComponent<MonsterStat>();
            BossUI.instance.HideUI();
            monsterStat.RestoreHP();
        }
    }
}

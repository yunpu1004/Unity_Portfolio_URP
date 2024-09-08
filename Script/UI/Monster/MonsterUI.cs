using UnityEngine;
using UnityEngine.UI;

// 이 스크립트는 몬스터의 체력바를 표시합니다. (MonsterStat의 OnHPChangedEvent에 등록됨)
public class MonsterUI : MonoBehaviour
{
    public Camera mainCamera;
    public Image hpBar;

    // 체력바를 갱신합니다.
    public void SetHPBar(int hp, int maxHp)
    {
        hpBar.fillAmount = (float)hp / maxHp;
    }

    // 몬스터의 체력바가 항상 카메라를 정면으로 바라보도록 합니다.
    void Update()
    {
        var cameraPosition = mainCamera.transform.position;
        cameraPosition.y = transform.position.y;

        // 몬스터의 체력바를 카메라의 위치를 기준으로 회전 시킴
        transform.LookAt(cameraPosition);
        transform.rotation = Quaternion.LookRotation(transform.position - cameraPosition);
    }
}
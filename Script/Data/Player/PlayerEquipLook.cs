using UnityEngine;

// 이 스크립트는 플레이어가 장착한 장비 오브젝트를 활성화합니다. (PlayerStat의 OnWeaponChangedEvent, OnShieldChangedEvent에 등록됨)
public class PlayerEquipLook : MonoBehaviour
{
    public GameObject[] weapons;
    public GameObject[] shields;

    // 플레이어가 장착한 무기를 활성화합니다.
    public void ShowWeapon(Item item)
    {
        if(item.IsEmpty()) return;
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(weapon.name == item.name);
        }
    }

    // 플레이어가 장착한 방패를 활성화합니다.
    public void ShowShield(Item item)
    {
        if(item.IsEmpty()) return;
        foreach (GameObject shield in shields)
        {
            shield.SetActive(shield.name == item.name);
        }
    }
}

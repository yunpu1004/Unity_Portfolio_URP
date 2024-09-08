using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerInit : MonoBehaviour
{
    private PlayerStat playerStat;
    private PlayerStatUI playerStatUI;
    private PlayerUI playerUI;
    private PlayerEquipLook playerEquipLook;
    private MenuUI menuUI;
    private Volume volume;

    private void Awake()
    {
        playerStat = FindObjectOfType<PlayerStat>(true);
        playerStatUI = FindObjectOfType<PlayerStatUI>(true);
        playerUI = FindObjectOfType<PlayerUI>(true);
        playerEquipLook = FindObjectOfType<PlayerEquipLook>(true);
        menuUI = FindObjectOfType<MenuUI>(true);
        volume = FindObjectOfType<Volume>(true);

        Init();
    }

    private void Init()
    {
        playerStat.OnLevelUpEvent += playerStatUI.SetLevelText;
        playerStat.OnHPChangedEvent += (hp, maxHp) => playerStatUI.SetMaxHpText(maxHp);
        playerStat.OnAtkChangedEvent += playerStatUI.SetAtkText;
        playerStat.OnDefChangedEvent += playerStatUI.SetDefText;
        playerStat.OnWeaponChangedEvent += playerStatUI.weaponSlot.UpdateSlot;
        playerStat.OnShieldChangedEvent += playerStatUI.shieldSlot.UpdateSlot;

        playerStat.OnLevelUpEvent += playerUI.SetLevelText;
        playerStat.OnLevelUpEvent += playerUI.ShowLevelUp;
        playerStat.OnHPChangedEvent += playerUI.SetHPText;

        playerStat.OnWeaponChangedEvent += playerEquipLook.ShowWeapon;
        playerStat.OnShieldChangedEvent += playerEquipLook.ShowShield;

        playerStat.OnLevelUpEvent += menuUI.SetLevelText;
        playerStat.OnExpChangedEvent += menuUI.SetExpText;
    }
}

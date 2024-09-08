using UnityEngine;
using TMPro;


/// 이 스크립트는 PlayerStat 클래스의 플레이어 스탯을 표시하는 UI를 담당합니다 (PlayerStat 클래스의 이벤트에 연결되어 있습니다.)
public class PlayerStatUI : MonoBehaviour
{
    public bool isPlayerStatOpen { get; private set;} = false;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI defText;
    public TextMeshProUGUI maxHpText;
    public PlayerStatSlot weaponSlot;
    public PlayerStatSlot shieldSlot;
    public GameObject content;

    private PlayerStat playerStat;
    private Inventory playerInventory;

    private void Awake() 
    {
        playerStat = FindObjectOfType<PlayerStat>();
        playerInventory = GameObject.Find("Player").GetComponent<Inventory>();

        weaponSlot.AddOnDoubleClick(() => 
        {
            if(playerStat.IsCurrentDefaultWeapon()) return;
            if(!playerInventory.HasEmptySlot()) return;
            playerInventory.AddItem(playerStat.GetWeapon());
            playerStat.SetWeapon(Item.GetDefaultSword());
        });

        shieldSlot.AddOnDoubleClick(() => 
        {
            if(playerStat.IsCurrentDefaultShield()) return;
            if(!playerInventory.HasEmptySlot()) return;
            playerInventory.AddItem(playerStat.GetShield());
            playerStat.SetShield(Item.GetDefaultShield());
        });
    }

    // 레벨 텍스트를 업데이트합니다.
    public void SetLevelText(int level)
    {
        levelText.text = level.ToString();
    }

    // 공격력 텍스트를 업데이트합니다.
    public void SetAtkText(int value)
    {
        atkText.text = value.ToString();
    }

    // 방어력 텍스트를 업데이트합니다.
    public void SetDefText(int value)
    {
        defText.text = value.ToString();
    }

    // 최대 체력 텍스트를 업데이트합니다.
    public void SetMaxHpText(int maxHp)
    {
        maxHpText.text = maxHp.ToString();
    }

    // 플레이어 스탯 UI를 엽니다.
    public void OpenPlayerStat()
    {
        isPlayerStatOpen = true;
        content.SetActive(true);
    }

    // 플레이어 스탯 UI를 닫습니다.
    public void ClosePlayerStat()
    {
        isPlayerStatOpen = false;
        content.SetActive(false);
    }
}

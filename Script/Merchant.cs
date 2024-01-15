using UnityEngine;

// 이 스트립트는 상인NPC의 Interaction에 거래 이벤트를 등록합니다.
public class Merchant : MonoBehaviour
{
    private Inventory playerInventory;
    private Inventory merchantInventory;
    private Interaction interaction;
    private Trade trade;

    private void Awake() 
    {
        playerInventory = GameObject.Find("Player").GetComponent<Inventory>();
        merchantInventory = GetComponent<Inventory>();
        interaction = GetComponent<Interaction>();
        trade = FindObjectOfType<Trade>(true);
        interaction.SetInteraction(name, () => trade.StartTrade(playerInventory, merchantInventory));
    }
}

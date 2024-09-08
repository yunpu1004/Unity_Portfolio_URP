using TMPro;
using UnityEngine;

// 이 스크립트는 거래 UI를 표시합니다. (Trade의 OnTradeStartEvent, OnTradeConfirmEvent에 연결되어 있습니다.)
public class TradeUI : MonoBehaviour
{
    public bool onInteraction = false;
    public PlayerInput input;
    public TradeSlot[] merchantSlots;
    public TradeSlot[] inventorySlots;
    public TextMeshProUGUI goldText;
    public GameObject content;
    
    public Trade trade;

    private void Awake() 
    {
        trade.AddOnTradeStartEvent(ShowTradeUI);
        trade.AddOnTradeConfirmEvent(UpdateTradeUI_Inventory);
        trade.AddOnTradeConfirmEvent(UpdateTradeUI_Merchant);
    }

    // 거래 UI를 표시합니다.
    private void ShowTradeUI(Inventory playerInventory, Inventory merchantInventory)
    {
        input.SetCursorLock(false);
        onInteraction = true;
        content.gameObject.SetActive(true);

        UpdateTradeUI_Inventory();
        UpdateTradeUI_Merchant();
    }

    // 플레이어의 거래 UI를 업데이트합니다.
    private void UpdateTradeUI_Inventory() 
    {
        var playerInventory = trade.GetPlayerInventory();
        goldText.text = $"소지골드 : {playerInventory.GetGold()}";
        foreach (var slot in inventorySlots)
        {
            slot.UpdateSlot();
        }
    }

    // 상인의 거래 UI를 업데이트합니다.
    private void UpdateTradeUI_Merchant() 
    {
        foreach (var slot in merchantSlots)
        {
            slot.UpdateSlot();
        }
    }

    // 거래 UI를 닫습니다.
    public void HideTradeUI()
    {
        input.SetCursorLock(true);
        onInteraction = false;
        content.gameObject.SetActive(false);
    }
}
using System;
using UnityEngine;

// 이 스크립트는 거래 데이터를 관리합니다.
// 옵저버 패턴을 사용하여 거래 데이터가 변경될 때마다 이벤트를 발생시킵니다.
public class Trade : MonoBehaviour
{
    private Inventory playerInventory;
    private Inventory merchantInventory;
    private event Action<Inventory, Inventory> OnTradeStart;

    private bool isBuy;
    private Item currentTradeItem;
    private int currentTradeCount;
    private event Action<bool, Item, int> OnTradeItemChanged;
    private event Action OnTradeConfirm;


    public Inventory GetPlayerInventory() => playerInventory;
    public Inventory GetMerchantInventory() => merchantInventory;
    public bool IsBuy() => isBuy;
    public Item GetCurrentTradeItem() => currentTradeItem;
    public int GetCurrentTradeCount() => currentTradeCount;

    // 상인과의 거래를 시작합니다.
    public void StartTrade(Inventory playerInventory, Inventory merchantInventory)
    {
        this.playerInventory = playerInventory;
        this.merchantInventory = merchantInventory;
        OnTradeStart?.Invoke(playerInventory, merchantInventory);
    }

    // 거래할 아이템을 설정합니다.
    public void SetTradeItem(bool isBuy, Item item, int count)
    {
        this.isBuy = isBuy;
        currentTradeItem = item;
        currentTradeCount = count;
        OnTradeItemChanged?.Invoke(isBuy, item, count);
    }

    // 아이템 거래를 진행합니다.
    public void ConfirmTrade()
    {
        if(currentTradeCount == 0) return;
        if(currentTradeItem.IsEmpty()) return;

        int totalPrice = isBuy ? currentTradeItem.buyPrice * currentTradeCount : currentTradeItem.sellPrice * currentTradeCount;
        Item goldChange = new Item{type = ItemType.Gold, count = totalPrice};
        Item itemChange = currentTradeItem;
        itemChange.count = currentTradeCount;

        if(isBuy)
        {
            playerInventory.RemoveItem(goldChange);
            playerInventory.AddItem(itemChange);
        }
        else
        {
            playerInventory.RemoveItem(itemChange);
            playerInventory.AddItem(goldChange);
        }

        OnTradeConfirm?.Invoke();
    }

    public void AddOnTradeStartEvent(Action<Inventory, Inventory> action)
    {
        OnTradeStart += action;
    }

    public void AddOnTradeItemChangedEvent(Action<bool, Item, int> action)
    {
        OnTradeItemChanged += action;
    }

    public void AddOnTradeConfirmEvent(Action action)
    {
        OnTradeConfirm += action;
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 이 스크립트는 거래 아이템 슬롯을 표시합니다.
public class TradeSlot : MonoBehaviour
{
    public Button button;
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI priceText;
    public TradePopup tradePopup;

    public Trade trade;
    public int itemIndex;
    public bool isPlayerSlot;

    private void Awake() 
    {
        button.onClick.AddListener(SlotClick);
    }

    // 거래 아이템 슬롯을 업데이트합니다.
    public void UpdateSlot()
    {
        var inventory = isPlayerSlot ? trade.GetPlayerInventory() : trade.GetMerchantInventory();
        var item = inventory.GetItem(itemIndex);
        if(!item.IsEmpty())
        {
            gameObject.SetActive(true);
            icon.sprite = item.sprite;
            nameText.text = item.name;
            priceText.text = isPlayerSlot ? $"가격 : {item.sellPrice}" : $"가격 : {item.buyPrice}";
            if(isPlayerSlot) countText.text = $"보유개수 : {item.count}";
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // 슬롯을 클릭하면 팝업을 표시합니다.
    private void SlotClick()
    {
        var inventory = isPlayerSlot ? trade.GetPlayerInventory() : trade.GetMerchantInventory();
        trade.SetTradeItem(!isPlayerSlot, inventory.GetItem(itemIndex), 0);
        tradePopup.ShowPopup();
    }
}
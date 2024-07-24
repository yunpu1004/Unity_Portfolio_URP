using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 이 스크립트는 거래진행시 팝업을 표시합니다. 
public class TradePopup : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Button confirmButton;
    public Slider slider;
    public GameObject content;

    public Trade trade;
    

    private void Awake() 
    {
        trade.AddOnTradeItemChangedEvent(UpdatePopup);
        confirmButton.onClick.AddListener(Confirm);
        slider.onValueChanged.AddListener((value) => trade.SetTradeItem(trade.IsBuy(), trade.GetCurrentTradeItem(), (int)value));
    }

    // 팝업을 표시합니다.
    public void ShowPopup()
    {
        content.gameObject.SetActive(true);
        slider.SetValueWithoutNotify(0);
        slider.maxValue = trade.IsBuy() ? trade.GetPlayerInventory().GetGold() / trade.GetCurrentTradeItem().buyPrice : trade.GetCurrentTradeItem().count;
    }

    // 팝업을 업데이트합니다.
    private void UpdatePopup(bool isBuy, Item item, int count)
    {
        if(item.IsEmpty()) return;

        if(trade.IsBuy())
        {
            titleText.text = "아이템을 구매하시겠습니까?";
            descriptionText.text = $"{item.name} {count}개 구매시 잃는 골드\n{item.buyPrice * count}골드";
        }
        else
        {
            titleText.text = "아이템을 판매하시겠습니까?";
            descriptionText.text = $"{item.name} {count}개 판매시 얻는 골드\n{item.sellPrice * count}골드";
        }
    }

    // 거래를 진행합니다.
    private void Confirm()
    {
        trade.ConfirmTrade();
        content.gameObject.SetActive(false);
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 이 스크립트는 인벤토리 슬롯을 표시합니다. (InventoryUI의 inventorySlots에 연결되어 있습니다.)
public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public Image image;
    public TextMeshProUGUI countText;
    public int index;
    public Inventory inventory;

    // 인벤토리 슬롯을 업데이트합니다.
    public void UpdateSlot(Item item)
    {
        if(!item.IsEmpty())
        {
            gameObject.SetActive(true);
            image.sprite = item.sprite;
            countText.text = item.count.ToString();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // 인벤토리 슬롯을 더블 클릭하면 아이템을 사용합니다.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2) 
        {
            inventory.UseItem(index);
        }
    }
}
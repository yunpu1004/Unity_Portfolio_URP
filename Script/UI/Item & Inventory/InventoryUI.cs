using TMPro;
using UnityEngine;

// 이 스크립트는 인벤토리 UI를 표시합니다. (Inventory의 골드와 아이템 변경 이벤트에 연결되어 있습니다.)
public class InventoryUI : MonoBehaviour
{
    public bool isInventoryOpen { get; private set;} = false;
    public TextMeshProUGUI goldText;
    public InventorySlot[] inventorySlots = new InventorySlot[36];
    private Inventory inventory;
    public GameObject content;

    private void Awake() {
        inventory = GameObject.Find("Player").GetComponent<Inventory>();
        inventory.AddOnGoldChangedEvent(UpdateGoldText);
        inventory.AddOnInventoryChangedEvent(UpdateSlot);
    }

    // 인벤토리 UI의 골드 텍스트를 업데이트합니다.
    private void UpdateGoldText(int gold)
    {
        goldText.text = "소지골드 : " + gold.ToString();
    }

    // 인벤토리 UI의 각 슬롯을 업데이트합니다.
    private void UpdateSlot(int index, Item item)
    {
        inventorySlots[index].UpdateSlot(item);
    }

    // 인벤토리 UI를 엽니다.
    public void OpenInventory()
    {
        isInventoryOpen = true;
        content.SetActive(true);
    }

    // 인벤토리 UI를 닫습니다.
    public void CloseInventory()
    {
        isInventoryOpen = false;
        content.SetActive(false);
    }
}
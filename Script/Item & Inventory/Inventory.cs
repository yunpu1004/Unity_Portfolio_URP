using UnityEngine;
using System;

// 이 스크립트는 인벤토리 데이터를 관리합니다.
// 옵저버 패턴을 사용하여 인벤토리가 변경될 때마다 이벤트를 발생시킵니다.
public class Inventory : MonoBehaviour
{
    [SerializeField] private int gold = 0;
    [SerializeField] private Item[] items;
    private event Action<int, Item> onInventoryChanged;
    private event Action<int> onGoldChanged;

    public int GetGold() => gold;

    public Item GetItem(int index)
    {
        if(index < 0 || index >= items.Length) return Item.GetEmptyItem();
        return items[index];
    }

    // 인벤토리에 아이템을 추가합니다.
    public bool AddItem(Item value)
    {
        if(value.IsEmpty()) return false;

        // 장비 아이템인 경우, 빈 슬롯에 추가
        if(value.type == ItemType.Weapon || value.type == ItemType.Shield)
        {
            int index = Array.FindIndex(items, x => x.IsEmpty());
            if(index != -1)
            {
                items[index] = value;
                onInventoryChanged?.Invoke(index, value);
                return true;
            }
        }

        // 골드인 경우, 더하기
        else if(value.type == ItemType.Gold)
        {
            gold += value.count;
            onGoldChanged?.Invoke(gold);
            return true;
        }

        // 소비, 기타 아이템인 경우, 같은 아이템이 있으면 더하고, 없으면 추가
        else
        {
            int index = Array.FindIndex(items, x => x.name == value.name);
            if(index != -1)
            {
                items[index].count += value.count;
                onInventoryChanged?.Invoke(index, items[index]);
                return true;
            }
            else
            {
                index = Array.FindIndex(items, x => x.IsEmpty());
                if(index != -1)
                {
                    items[index] = value;
                    onInventoryChanged?.Invoke(index, value);
                    return true;
                }
            }
        }

        // 인벤토리가 가득 찼을 때
        return false; 
    }

    // 인벤토리에서 아이템을 제거합니다.    
    public bool RemoveItem(Item value)
    {   
        if(value.IsEmpty()) return false;

        if(value.type == ItemType.Weapon || value.type == ItemType.Shield)
        {
            int index = Array.FindIndex(items, x => x.name == value.name);
            if(index != -1)
            {
                items[index] = Item.GetEmptyItem();
                onInventoryChanged?.Invoke(index, value);
                return true;
            }
        }

        else if(value.type == ItemType.Gold)
        {
            if(gold >= value.count)
            {
                gold -= value.count;
                onGoldChanged?.Invoke(gold);
                return true;
            }
        }

        else
        {
            int index = Array.FindIndex(items, x => x.name == value.name);
            if(index != -1)
            {
                if(items[index].count >= value.count)
                {
                    items[index].count -= value.count;
                    onInventoryChanged?.Invoke(index, items[index]);
                    return true;
                }
            }
        }

        // 인벤토리에 해당 아이템이 없을 때
        return false; 
    }

    // 인벤토리에서 아이템을 사용합니다.
    public void UseItem(int index)
    {
        if(index < 0 || index >= 36) return;
        items[index].Use();
        onInventoryChanged?.Invoke(index, items[index]);
    }

    // 인벤토리에 빈 슬롯이 있는지 확인합니다.
    public bool HasEmptySlot()
    {
        return Array.FindIndex(items, x => x.IsEmpty()) != -1;
    }

    public void AddOnInventoryChangedEvent(Action<int, Item> action)
    {
        onInventoryChanged += action;
    }

    public void AddOnGoldChangedEvent(Action<int> action)
    {
        onGoldChanged += action;
    }
}

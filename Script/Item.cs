using System;
using System.Text.RegularExpressions;
using UnityEngine;

// 이 스크립트는 아이템 데이터를 관리합니다.
// 아이템 습득, 거래, 사용 등의 기능은 이 구조체를 생성 또는 수정하는 방식으로 구현됩니다.
[Serializable]
public struct Item
{
    public string id;
    public string name;
    public ItemType type;
    public Sprite sprite;
    public int count;
    public int sellPrice;
    public int buyPrice;
    public int atk_equip;
    public int def_equip;
    public int hp_recover;

    public bool IsEmpty() => count == 0;
    public bool IsNull() => string.IsNullOrEmpty(id);

    public Item(string csvLine, int count = 0)
    {
        var pattern = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";
        string[] arr = Regex.Split(csvLine, pattern);
        for(int i = 0; i < arr.Length; i++)
        {
            arr[i] = arr[i].Trim(' ', '"', '\r', '\n');
        }

        if(arr.Length != 8) throw new Exception("Item 생성자의 인자로 전달되는 arr의 길이가 8이 아닙니다.");
        id = arr[0];
        name = arr[1];
        type = Enum.Parse<ItemType>(arr[2]);
        sprite = Resources.Load<Sprite>("Image/Items/" + name); 
        sellPrice = int.Parse(arr[3]);
        buyPrice = int.Parse(arr[4]);
        atk_equip = int.Parse(arr[5]);
        def_equip = int.Parse(arr[6]);
        hp_recover = int.Parse(arr[7]); 
        this.count = count;
    }

    // 아이템을 사용합니다. (아이템의 종류에 따라 다른 동작을 수행합니다.)
    public void Use()
    {
        if(count == 0) return;
        if(type == ItemType.Misc) return;

        // 장비 아이템인 경우 장착합니다.
        else if(type == ItemType.Weapon || type == ItemType.Shield)
        {
            var inventory = GameObject.Find("Player").GetComponent<Inventory>();
            var playerStat = GameObject.FindObjectOfType<PlayerStat>(true);

            if(type == ItemType.Weapon)
            {
                // 기본 무기가 아닌 경우, 장착하고 있던 무기를 인벤토리에 추가합니다.
                if(!playerStat.IsCurrentDefaultWeapon())
                {
                    inventory.AddItem(playerStat.GetWeapon());
                }

                // 새로운 무기를 장착합니다.
                playerStat.SetWeapon(this);
            }
            else
            {
                // 기본 방패가 아닌 경우, 장착하고 있던 방패를 인벤토리에 추가합니다.
                if(!playerStat.IsCurrentDefaultShield())
                {
                    inventory.AddItem(playerStat.GetShield());
                }

                // 새로운 방패를 장착합니다.
                playerStat.SetShield(this);
            }
        }

        // 소비 아이템인 경우, 체력을 회복합니다. (소비아이템이 체력포션인 경우 뿐이지만, 추후에 다른 소비 아이템이 추가될 수 있습니다.)
        else if(type == ItemType.Consumable)
        {
            var playerStat = GameObject.FindObjectOfType<PlayerStat>(true);
            playerStat.AddHP(hp_recover);
        }

        count--;
    }

    public static Item GetEmptyItem()
    {
        var item = new Item();
        return item;
    }

    public static Item GetDefaultSword()
    {
        var item = new Item
        {
            name = "기본검",
            count = 1,
            type = ItemType.Weapon,
            sprite = Resources.Load<Sprite>("Image/Items/기본검"),
        };
        return item;
    }

    public static Item GetDefaultShield()
    {
        var item = new Item
        {
            name = "기본방패",
            count = 1,
            type = ItemType.Shield,
            sprite = Resources.Load<Sprite>("Image/Items/기본방패"),
        };
        return item;
    }

    public static Item GetGold(int count)
    {
        var item = new Item
        {
            name = "골드",
            sellPrice = 1,
            buyPrice = 1,
            count = count,
            type = ItemType.Gold,
            sprite = null,
        };
        return item;
    }
}

public enum ItemType
{
    Misc,
    Consumable,
    Weapon,
    Shield,
    Gold
}
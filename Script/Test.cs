using UnityEngine;

// 이 스크립트는 기능 테스트를 위해 사용됩니다.
public class Test : MonoBehaviour
{
    public void EditorTestMethod()
    {
        DataLoader.InitData();
    }

    public void TestMethod()
    {
        InventoryTest();
    }

    public void CSVTest()
    {

    }


    private void QuestTest()
    {
        var playerActivity = FindObjectOfType<PlayerActivity>(true);
        playerActivity.SetRecentActivity(new PlayerActivityData{activityType = PlayerActivityData.ActivityType.Kill, activityTarget = "늑대"}); 
    }

    private void InventoryTest()
    {
        var Inventory = GameObject.Find("Player").GetComponent<Inventory>();

        var potion = Item.GetHealthPotion(20);
        Inventory.AddItem(potion);

        var powerSword = Item.GetPowerSword();
        powerSword.count = 1;
        Inventory.AddItem(powerSword);

        var powerShield = Item.GetPowerShield();
        powerShield.count = 1;
        Inventory.AddItem(powerShield);

        var glaive = Item.GetGlaive();
        glaive.count = 1;
        Inventory.AddItem(glaive);

        var royalShield = Item.GetRoyalShield();
        royalShield.count = 1;
        Inventory.AddItem(royalShield);

        var wolfToe = new Item
        {
            name = "늑대의발톱",
            count = 10,
            sellPrice = 10,
            buyPrice = 0,
            sprite = Resources.Load<Sprite>("Image/Items/늑대의발톱"),
        };
        Inventory.AddItem(wolfToe);

        var gold = Item.GetGold(100);
        Inventory.AddItem(gold);
    }
}
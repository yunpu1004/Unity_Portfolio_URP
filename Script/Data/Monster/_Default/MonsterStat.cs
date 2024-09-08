using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using UnityEngine;

// 이 스크립트는 몬스터의 스테이터스를 관리합니다.
// 옵저버 패턴을 사용하여 데이터가 변경될 때마다 이벤트를 발생시킵니다.
public class MonsterStat : MonoBehaviour
{
    [SerializeField] private string id;
    private int maxHP;
    private int hp;
    private int atk;
    private int exp;
    private (Item item, float dropRate)[] dropTable;

    public int GetMaxHP() => maxHP;
    public int GetHP() => hp;
    public int GetAtk() => atk;
    public int GetExp() => exp;
    public bool IsDead() => hp <= 0;

    public event Action<int, int> OnHPChangedEvent;
    public event Action<int> OnDamageEvent;
    public event Action OnDeathEvent;

    private void Start() 
    {
        var csvLine = DataManager.instance.GetMonsterStat(id);
        var pattern = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";
        string[] arr = Regex.Split(csvLine, pattern);
        for(int i = 0; i < arr.Length; i++)
        {
            arr[i] = arr[i].Trim(' ', '"', '\r', '\n');
        }

        if(arr.Length != 5) throw new Exception("Item 생성자의 인자로 전달되는 arr의 길이가 5가 아닙니다.");
        
        maxHP = int.Parse(arr[1]);
        hp = maxHP;
        atk = int.Parse(arr[2]);
        exp = int.Parse(arr[3]);

        var dropTableStrArray = arr[4].Split('#', StringSplitOptions.RemoveEmptyEntries);
        dropTable = new (Item item, float dropRate)[dropTableStrArray.Length];
        for(int i = 0; i < dropTableStrArray.Length; i++)
        {
            var dropInfo = dropTableStrArray[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Item item = DataManager.instance.GetItem(dropInfo[0]);
            item.count = int.Parse(dropInfo[1]);
            dropTable[i] = (item, float.Parse(dropInfo[2]));
        }
    }

    // HP를 변경합니다.
    public void AddHP(int value)
    {
        if(hp <= 0) return;
        int newHP = Mathf.Clamp(hp + value, 0, maxHP);
        int deltaHP = newHP - hp;
        if(deltaHP == 0) return;
        hp = newHP;
        if (hp <= 0)
        {
            hp = 0;
            OnDeathEvent?.Invoke();
        }
        if(deltaHP < 0) OnDamageEvent?.Invoke(-deltaHP);
        OnHPChangedEvent?.Invoke(hp, maxHP);
    }

    // HP를 최대치로 회복합니다.
    public void RestoreHP()
    {
        hp = maxHP;
        OnHPChangedEvent?.Invoke(hp, maxHP);
    }

    // 몬스터가 죽었을 때 생성되는 아이템의 드롭 테이블을 반환합니다.
    public ReadOnlyCollection<(Item item, float dropRate)> GetDropTable()
    {
        return Array.AsReadOnly(dropTable);
    }
}

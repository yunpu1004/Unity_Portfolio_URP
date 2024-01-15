using System.Collections.ObjectModel;
using UnityEngine;

// 이 스크립트는 몬스터의 스테이터스를 관리합니다.
// 옵저버 패턴을 사용하여 데이터가 변경될 때마다 이벤트를 발생시킵니다.
public class MonsterStat : MonoBehaviour
{
    [SerializeField] private int maxHP = 1;
    [SerializeField] private int hp = 1;
    [SerializeField] private int atk = 1;
    [SerializeField] private int exp = 1;
    [SerializeField] private int gold = 1;

    [Header("아이템 이름과 드랍 확률을 공백을 두고 입력하세요. \n예시) 골드 0.3")]
    [SerializeField] private string[] dropItemAndRate;
    private (string, float)[] dropItemArray;

    public int GetMaxHP() => maxHP;
    public int GetHP() => hp;
    public int GetAtk() => atk;
    public int GetExp() => exp;
    public int GetGold() => gold;
    public bool IsDead() => hp <= 0;

    private event System.Action<int, int> OnHPChangedEvent;
    private event System.Action<int> OnDamageEvent;
    private event System.Action OnDeathEvent;

    private void Awake() 
    {
        dropItemArray = new (string, float)[dropItemAndRate.Length];
        for (int i = 0; i < dropItemAndRate.Length; i++)
        {
            var itemAndRate = dropItemAndRate[i].Split(' ');
            dropItemArray[i] = (itemAndRate[0], float.Parse(itemAndRate[1]));
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

    public void AddOnHPChangedEvent(System.Action<int, int> action)
    {
        OnHPChangedEvent += action;
    }

    public void AddOnDamageEvent(System.Action<int> action)
    {
        OnDamageEvent += action;
    }

    public void AddOnDeathEvent(System.Action action)
    {
        OnDeathEvent += action;
    }

    // HP를 최대치로 회복합니다.
    public void RestoreHP()
    {
        hp = maxHP;
        OnHPChangedEvent?.Invoke(hp, maxHP);
    }

    public ReadOnlyCollection<(string, float)> GetDropItemArray()
    {
        return System.Array.AsReadOnly(dropItemArray);
    }
}

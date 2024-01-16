using UnityEngine;

// 이 스크립트는 플레이어의 스테이터스를 관리합니다.
public class PlayerStat : MonoBehaviour
{
    private int level = 1;
    private int exp = 0;
    private int maxExp = 10;
    private int hp = 100;
    private int maxHP = 100;
    private int atk_player = 10;
    private int atk_equip = 0;
    private int def_player = 0;
    private int def_equip = 1;
    private Item weapon;
    private Item shield;


    private event System.Action<int> OnLevelUpEvent;
    private event System.Action<int, int> OnExpChangedEvent;
    private event System.Action<int, int> OnHPChangedEvent;
    private event System.Action<int> OnDamageEvent;
    private event System.Action<int> OnHealEvent;
    private event System.Action<Item> OnWeaponChangedEvent;
    private event System.Action<Item> OnShieldChangedEvent;
    private event System.Action<int> OnAtkChangedEvent;
    private event System.Action<int> OnDefChangedEvent;
    private event System.Action OnDeathEvent;

    private Player player;
    private Animator animator;
    private Skill skill;
    
    public int GetLevel() => level;
    public int GetExp() => exp;
    public int GetMaxExp() => maxExp;
    public int GetHP() => hp;
    public int GetMaxHP() => maxHP;
    public int GetPlayerAtk() => atk_player;
    public int GetPlayerDef() => def_player;
    public int GetEquipAtk() => atk_equip;
    public int GetEquipDef() => def_equip;
    public int GetAtk() => atk_player + atk_equip;
    public int GetDef() => def_player + def_equip;
    public Item GetWeapon() => weapon;
    public Item GetShield() => shield;
    
    private void Awake() 
    {
        animator = GetComponent<Animator>();
        skill = GetComponent<Skill>();
        player = GetComponent<Player>();
    }

    private void Start()
    {
        level = 1;
        exp = 0;
        maxExp = 10;
        hp = 100;
        maxHP = 100;
        atk_player = 10;
        def_player = 0;

        SetWeapon(Item.GetDefaultSword());
        SetShield(Item.GetDefaultShield());
    }

    // 경험치를 획득합니다.
    public void GainExp(int value)
    {
        exp += value;
        if (exp >= maxExp)
        {
            level += 1;

            maxExp = level * 10;
            exp = 0;

            maxHP = 100 + (level - 1) * 20;
            hp = maxHP;

            atk_player = 10 + (level - 1) * 2;
            def_player = 0;

            OnLevelUpEvent?.Invoke(level);
            OnHPChangedEvent?.Invoke(hp, maxHP);
            OnAtkChangedEvent?.Invoke(GetAtk());
            OnDefChangedEvent?.Invoke(GetDef());
        }
        OnExpChangedEvent?.Invoke(exp, maxExp);
    }

    // HP를 변경합니다.
    public void AddHP(int value)
    {
        int oldHP = hp;
        int newHP = Mathf.Clamp(hp + value, 0, maxHP);
        hp = newHP;

        if (oldHP != 0 && newHP == 0)
        {
            OnDeathEvent?.Invoke();
            animator.SetTrigger("Die");
        }

        if(newHP != oldHP) OnHPChangedEvent?.Invoke(newHP, maxHP);
        if(newHP < oldHP)
        {
            if(skill.IsSkillEAnimationPlaying()) return;
            if(skill.IsSkillQAnimationPlaying()) return;
            if(player.onAttackAnimation) return;
            animator.SetTrigger("Hit");
        }
        if(newHP > oldHP) OnHealEvent?.Invoke(newHP - oldHP);
    }

    // 무기를 변경합니다.
    public void SetWeapon(Item item)
    {
        if(item.IsEmpty())
        {
            Debug.Log("weapon is null");
        }
        else
        {
            if(item.type != ItemType.Weapon) return;
            weapon = item;
            atk_equip = weapon.atk_equip;
            OnWeaponChangedEvent?.Invoke(weapon);
            OnAtkChangedEvent?.Invoke(GetAtk());
        }
    }

    // 방패를 변경합니다.
    public void SetShield(Item item)
    {
        if(item.IsEmpty())
        {
            Debug.Log("shield is null");
        }
        else
        {
            if(item.type != ItemType.Shield) return;
            shield = item;
            def_equip = shield.def_equip;
            OnShieldChangedEvent?.Invoke(shield);
            OnDefChangedEvent?.Invoke(GetDef());
        }
    }

    // 현재 무기가 기본 무기인지 확인합니다.
    public bool IsCurrentDefaultWeapon()
    {
        return weapon.name == "기본검";
    }

    // 현재 방패가 기본 방패인지 확인합니다.
    public bool IsCurrentDefaultShield()
    {
        return shield.name == "기본방패";
    }


    public void AddOnLevelChangedEvent(System.Action<int> action)
    {
        OnLevelUpEvent += action;
    }

    public void AddOnExpChangedEvent(System.Action<int, int> action)
    {
        OnExpChangedEvent += action;
    }

    public void AddOnHPChangedEvent(System.Action<int, int> action)
    {
        OnHPChangedEvent += action;
    }

    public void AddOnDamageEvent(System.Action<int> action)
    {
        OnDamageEvent += action;
    }

    public void AddOnHealEvent(System.Action<int> action)
    {
        OnHealEvent += action;
    }

    public void AddOnDeathEvent(System.Action action)
    {
        OnDeathEvent += action;
    }

    public void AddOnWeaponChangedEvent(System.Action<Item> action)
    {
        OnWeaponChangedEvent += action;
    }

    public void AddOnShieldChangedEvent(System.Action<Item> action)
    {
        OnShieldChangedEvent += action;
    }

    public void AddOnAtkChangedEvent(System.Action<int> action)
    {
        OnAtkChangedEvent += action;
    }

    public void AddOnDefChangedEvent(System.Action<int> action)
    {
        OnDefChangedEvent += action;
    }


    // HP를 최대치로 회복합니다.
    public void RestoreFullHP()
    {
        hp = maxHP;
        OnHPChangedEvent?.Invoke(hp, maxHP);
    }
}

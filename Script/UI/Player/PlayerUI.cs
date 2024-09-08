using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 이 스크립트는 플레이어 UI를 표시합니다. (playerStat, playerSkill에 연결되어 있습니다.)
public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI hpText;
    public Image hpBar;

    public Image skillQ_Gray;
    public TextMeshProUGUI skillQ_CoolTime;
    public Image skillE_Gray;
    public TextMeshProUGUI skillE_CoolTime;

    public GameObject levelUp;

    private Skill playerSkill;


    private void Awake() 
    {
        playerSkill = FindObjectOfType<Skill>();
        playerSkill.AddOnSkillQCooltimeChangedEvent(SetSkillQ);
        playerSkill.AddOnSkillECooltimeChangedEvent(SetSkillE);
    }

    // 레벨 텍스트를 업데이트합니다.
    public void SetLevelText(int level)
    {
        levelText.text = "Lv." + level;
    }

    // 체력 텍스트를 업데이트합니다.
    public void SetHPText(int hp, int maxHp)
    {
        hpText.text = hp + " / " + maxHp;
        hpBar.fillAmount = (float)hp / maxHp;
    }

    // 레벨업 UI를 표시합니다.
    public void ShowLevelUp(int level)
    {
        levelUp.SetActive(true);
    }

    // 스킬 Q의 쿨타임 텍스트와 이미지를 업데이트합니다.
    private void SetSkillQ(float coolTime)
    {
        if(coolTime > 0) 
        {
            skillQ_Gray.gameObject.SetActive(true);
            skillQ_CoolTime.gameObject.SetActive(true);
            skillQ_CoolTime.text = coolTime.ToString("N1");
        }
        else
        {
            skillQ_Gray.gameObject.SetActive(false);
            skillQ_CoolTime.gameObject.SetActive(false);
        }
    }

    // 스킬 E의 쿨타임 텍스트와 이미지를 업데이트합니다.
    private void SetSkillE(float coolTime)
    {
        if(coolTime > 0) 
        {
            skillE_Gray.gameObject.SetActive(true);
            skillE_CoolTime.gameObject.SetActive(true);
            skillE_CoolTime.text = coolTime.ToString("N1");
        }
        else
        {
            skillE_Gray.gameObject.SetActive(false);
            skillE_CoolTime.gameObject.SetActive(false);
        }
    }
}
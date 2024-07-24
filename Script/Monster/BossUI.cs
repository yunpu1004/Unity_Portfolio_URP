using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 이 스크립트는 보스 몬스터의 UI를 표시합니다.
// 보스 몬스터의 트리거와 닿으면 BossUITrigger 스크립트에서 UI를 활성화 시킵니다.
// 체력바, 이름에 대한 이벤트는 MonsterStat의 이벤트에 연결되어 있습니다. (BossMonsterAwakeStrategy 참고)
public class BossUI : MonoBehaviour
{
    public static BossUI instance { get; private set; }
    public Canvas canvas;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI hpText;
    public Image hpBar;

    private void Awake() 
    {
        instance = this;
        canvas.enabled = false;
    }

    // 이름 텍스트를 수정합니다.
    public void SetNameText(string name)
    {
        nameText.text = name;
    }

    // 체력바를 수정합니다.
    public void SetHPBar(int hp, int maxHp)
    {
        hpText.text = hp + " / " + maxHp;
        hpBar.fillAmount = (float)hp / maxHp;
    }

    // UI를 표시합니다.
    public void ShowUI()
    {
        canvas.enabled = true;
    }

    // UI를 숨깁니다.
    public void HideUI()
    {
        canvas.enabled = false;
    }
}
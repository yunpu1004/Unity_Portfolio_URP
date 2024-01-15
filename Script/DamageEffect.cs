using TMPro;
using UnityEngine;

// 이 스크립트는 몬스터가 플레이어에게 데미지를 입었을 때 데미지 텍스트를 표시합니다.
public class DamageEffect : MonoBehaviour
{
    public TextMeshProUGUI text;
    public RectTransform textRectTransform;
    public MonsterStat monsterStat;
    private Color color;

    // 이벤트 등록이 끝나면 오브젝트를 비활성화 시킵니다.
    private void Awake() 
    {
        monsterStat.AddOnDamageEvent(ShowDamage);
        color = text.color;
        gameObject.SetActive(false);
    }

    // 데미지 텍스트를 표시합니다.
    public void ShowDamage(int damage)
    {
        if(!gameObject.activeSelf) gameObject.SetActive(true);

        text.text = damage.ToString();
        color.a = 1;
        text.color = color;

        /// 텍스트 위치를 랜덤하게 설정
        textRectTransform.anchoredPosition = new Vector2(Random.Range(-0.8f, 0.8f), Random.Range(0.3f, 1f));
    }

    // 시간에 따라 페이드 아웃 효과를 줍니다.
    private void Update() {
        color.a -= Time.deltaTime;
        text.color = color;

        if(color.a <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}

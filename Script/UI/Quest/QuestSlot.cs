using TMPro;
using UnityEngine;

// 이 스크립트는 퀘스트 슬롯을 표시합니다.
public class QuestSlot : MonoBehaviour
{
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;

    // 퀘스트 슬롯을 업데이트합니다.
    public void UpdateUI(QuestData quest)
    {
        title.text = quest.title;
        description.text = quest.description + "\n(" + quest.progressCount + " / " + quest.targetCount + ")";
    }
}

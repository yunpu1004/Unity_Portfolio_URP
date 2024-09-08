using UnityEngine;

// 이 스크립트는 퀘스트 UI를 표시합니다. (Quest의 currentQuestList에 연결되어 있습니다.)
public class QuestUI : MonoBehaviour
{
    public QuestSlot[] questSlots;

    private void Start() 
    {
        Quest.instance.OnCurrentQuestsUpdateEvent += (string questID) => UpdateUI();
    }

    // 퀘스트 UI를 업데이트합니다.
    private void UpdateUI()
    {
        foreach (var questSlot in questSlots)
        {
            questSlot.gameObject.SetActive(false);
        }

        int i = 0;
        var quests = Quest.instance.currentQuests.Values;
        foreach (var quest in quests)
        {
            questSlots[i].gameObject.SetActive(true);
            questSlots[i].UpdateUI(quest);
            i++;
            if(i >= questSlots.Length) break;
        }
    }
}

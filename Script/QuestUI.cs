using UnityEngine;

// 이 스크립트는 퀘스트 UI를 표시합니다. (Quest의 currentQuestList에 연결되어 있습니다.)
public class QuestUI : MonoBehaviour
{
    public Quest quest;
    public QuestSlot[] questSlots;

    private void Awake() 
    {
        quest.AddOnCurrentQuestListUpdatedEvent(UpdateUI);
    }

    // 퀘스트 UI를 업데이트합니다.
    private void UpdateUI(QuestData[] array)
    {
        for (int i = 0; i < questSlots.Length; i++)
        {
            if (i < array.Length)
            {
                questSlots[i].gameObject.SetActive(true);
                questSlots[i].UpdateUI(array[i]);
            }
            else
            {
                questSlots[i].gameObject.SetActive(false);
            }
        }
    }
}

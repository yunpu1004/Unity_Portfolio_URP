using System;
using UnityEngine;
using UnityEngine.UI;

// 이 스크립트는 NPC의 머리위에 표시되는 퀘스트 아이콘을 관리합니다.
public class QuestIconUI : MonoBehaviour
{
    private Sprite defaultNpcIcon;
    private Sprite questRunningIcon;
    private Sprite questCompletableIcon;
    private Image iconImage;
    private DialogueNPC dialogueNPC;

    private void Awake() 
    {
        defaultNpcIcon = Resources.Load<Sprite>("Image/defaultNpcIcon");
        questRunningIcon = Resources.Load<Sprite>("Image/questRunningIcon");
        questCompletableIcon = Resources.Load<Sprite>("Image/questCompletableIcon");
        iconImage = GetComponent<Image>();
        dialogueNPC = GetComponentInParent<DialogueNPC>();
        Quest.instance.OnCurrentQuestsUpdateEvent += UpdateNpcQuestIcon;
    }

    // 현재 퀘스트 상태에 따라 아이콘을 변경합니다.
    private void UpdateNpcQuestIcon(string questId)
    {
        if(dialogueNPC == null) return;

        // questId 가 만약 이 NPC와 관련된 퀘스트가 아니면 리턴
        if(Array.IndexOf(dialogueNPC.questIdArray, questId) == -1) return;

        // questId 가 완료 가능한 퀘스트라면 questCompletableIcon 로 변경
        if(Quest.instance.IsCompletableQuest(questId))
        {
            iconImage.sprite = questCompletableIcon;
            return;
        }

        // questId 가 현재 진행중인 퀘스트라면 questRunningIcon 로 변경
        else if(Quest.instance.IsCurrentQuest(questId))
        {
            iconImage.sprite = questRunningIcon;
            return;
        }

        // questId 가 취소되거나 완료된 퀘스트라면, 이 NPC에게 받은 다른 퀘스트가 있는지 확인
        else
        {
            foreach (var quest in Quest.instance.currentQuests.Values)
            {
                if(Array.IndexOf(dialogueNPC.questIdArray, quest.id) != -1)
                {
                    iconImage.sprite = questRunningIcon;
                    return;
                }
            }

            iconImage.sprite = defaultNpcIcon;
        }
    }
}
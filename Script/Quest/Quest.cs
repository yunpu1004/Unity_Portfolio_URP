using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;

// 이 스크립트는 퀘스트를 관리합니다.
// 이벤트 패턴을 사용하여 퀘스트 데이터 변경시 이벤트를 발생시킵니다.
public class Quest : MonoBehaviour
{
    public static Quest instance { get; private set; }

    public Dictionary<string, QuestData> currentQuests;
    public Dictionary<string, QuestData> completedQuests;
    private PlayerActivity playerActivity;
    private Inventory inventory;

    public event Action<string> OnCurrentQuestsUpdateEvent;

    private void Awake() 
    {
        instance = this;
        currentQuests = new Dictionary<string, QuestData>();
        completedQuests = new Dictionary<string, QuestData>();
        playerActivity = GetComponent<PlayerActivity>();
        inventory = GetComponent<Inventory>();
        playerActivity.OnPlayerActivityChangedEvent += UpdateQuestProgress;
    }

    // 퀘스트를 추가합니다.
    public void AddQuest(string questId)
    {
        if(currentQuests.ContainsKey(questId)) return;
        if(completedQuests.ContainsKey(questId)) return;
        var quest = DataManager.instance.GetQuestData(questId);
        currentQuests.Add(questId, quest);
        quest.onQuestAcceptEvent?.Invoke();
        OnCurrentQuestsUpdateEvent?.Invoke(questId);
    }

    // 퀘스트를 제거합니다.
    public void RemoveQuest(string questId)
    {
        if(!currentQuests.ContainsKey(questId)) return;
        currentQuests.Remove(questId);
        OnCurrentQuestsUpdateEvent?.Invoke(questId);
    }

    // 퀘스트를 완료합니다.
    public bool CompleteQuest(string questId)
    {
        if(!currentQuests.ContainsKey(questId)) return false;
        var quest = currentQuests[questId];
        if(quest.IsCompletable() == false) return false;
        completedQuests.Add(questId, quest);
        currentQuests.Remove(questId);
        inventory.AddItem(Item.GetGold(quest.goldReward));
        OnCurrentQuestsUpdateEvent?.Invoke(questId);
        quest.onQuestCompleteEvent?.Invoke();
        return true;
    }

    // 현제 진행중인 퀘스트인지 확인합니다.
    public bool IsCurrentQuest(string questId)
    {
        return currentQuests.ContainsKey(questId);
    }

    // 완료한 퀘스트인지 확인합니다.
    public bool IsCompletedQuest(string questId)
    {
        return completedQuests.ContainsKey(questId);
    }

    // 퀘스트를 완료할 수 있는지 확인합니다.
    public bool IsCompletableQuest(string questId)
    {
        if(!currentQuests.ContainsKey(questId)) return false;
        else
        {
            var quest = currentQuests[questId];
            return quest.progressCount >= quest.targetCount;
        }
    }

    // PlayerActivityData에 따라 퀘스트 진행도를 업데이트합니다.
    private void UpdateQuestProgress(PlayerActivityData playerActivityLog)
    {
        var values = currentQuests.Values.ToArray();
        foreach(var quest in values)
        {
            quest.UpdateProgressCount(playerActivityLog);
            OnCurrentQuestsUpdateEvent?.Invoke(quest.id);
        }
    }
}


// 이 클래스는 CSV 파일에서 읽어온 퀘스트 데이터를 저장합니다.
public class QuestData
{
    public string id { get; private set; }
    public string title { get; private set; }
    public string npcName { get; private set; }
    public string description { get; private set; }
    public PlayerActivityData targetActivityData { get; private set; }
    public int targetCount { get; private set; }
    public int progressCount { get; private set; }
    public int goldReward { get; private set; }
    public bool markTarget { get; private set; }
    public Action onQuestAcceptEvent { get; private set; }
    public Action onQuestCompleteEvent { get; private set; }

    public float GetProgress() => (float)progressCount / targetCount;
    public bool IsCompletable() => progressCount >= targetCount;

    public QuestData(string csvLine)
    {
        var pattern = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";
        string[] arr = Regex.Split(csvLine, pattern);
        for(int i = 0; i < arr.Length; i++)
        {
            arr[i] = arr[i].Trim(' ', '"', '\r', '\n');
        }
        id = arr[0];
        title = arr[1];
        npcName = arr[2];
        description = arr[3];
        var temp = arr[4].Split(' ');
        targetActivityData = new PlayerActivityData{activityType = (PlayerActivityData.ActivityType)Enum.Parse(typeof(PlayerActivityData.ActivityType), temp[0]), activityTarget = temp[1]};
        targetCount = int.Parse(arr[5]);
        progressCount = 0;
        goldReward = int.Parse(arr[6]);
        markTarget = bool.Parse(arr[7]);
        
        if(!string.IsNullOrWhiteSpace(arr[8]))
        {
            string input = arr[8];
            string[] parts = input.Split(' ');
            string[] parameters = parts[1..];
            string[] methodParts = parts[0].Split('.');

            Type type = Type.GetType(methodParts[0]);
            MethodInfo method = type.GetMethod(methodParts[1], BindingFlags.Static | BindingFlags.Public);
            onQuestAcceptEvent = () => method.Invoke(null, parameters);
        }
        
        if(!string.IsNullOrWhiteSpace(arr[9]))
        {
            string input = arr[9];
            string[] parts = input.Split(' ');
            string[] parameters = parts[1..];
            string[] methodParts = parts[0].Split('.');

            Type type = Type.GetType(methodParts[0]);
            MethodInfo method = type.GetMethod(methodParts[1], BindingFlags.Static | BindingFlags.Public);
            onQuestCompleteEvent = () => method.Invoke(null, parameters);
        }
    }

    public void UpdateProgressCount(PlayerActivityData playerActivityData)
    {
        if(IsCompletable()) return;
        if(playerActivityData.Equals(targetActivityData))
        {
            progressCount++;
        }
    }
}
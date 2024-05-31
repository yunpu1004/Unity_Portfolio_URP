using System;
using System.Text.RegularExpressions;
using UnityEngine;

// 이 스크립트는 옵저버 패턴을 사용하여 대화 데이터가 변경될 때마다 이벤트를 발생시킵니다.
public class Dialogue : MonoBehaviour
{
    private DialogueData dialogueData;
    private Action<DialogueData> onDialogueDataChanged;

    public void SetDialogue(DialogueData dialogueData)
    {
        this.dialogueData = dialogueData;
        onDialogueDataChanged?.Invoke(dialogueData);

        /// 퀘스트 데이터가 있으면 이를 플레이어의 현재 퀘스트로 추가합니다.
        if(dialogueData != null && dialogueData.HasQuestData())
        {
            if(!Quest.instance.IsCurrentQuest(dialogueData.questDataId) && !Quest.instance.IsCompletedQuest(dialogueData.questDataId))
            {
                Quest.instance.AddQuest(dialogueData.questDataId);
                return;
            }
                
            if(Quest.instance.IsCompletableQuest(dialogueData.questDataId))
            {
                Quest.instance.CompleteQuest(dialogueData.questDataId);
                return;
            }
        }
    }

    /// 이 메소드는 에디터에 의해서 Continue 오브젝트의 Button.OnClick 이벤트에 연결되어 있습니다.
    public void SetNextDialogue()
    {
        if(dialogueData == null) return;
        if(dialogueData.IsEnd()) SetDialogue(null);
        else if(!dialogueData.HasNextDialogue()) return;
        else SetDialogue(DataManager.instance.GetDialogueData(dialogueData.nextDialogueId));
    }

    /// 이 메소드는 에디터에 의해서 각 옵션 오브젝트의 Button.OnClick 이벤트에 연결되어 있습니다.
    public void SetOption1Dialogue()
    {
        if(dialogueData == null) return;
        if(dialogueData.IsEnd()) SetDialogue(null);
        else SetDialogue(DataManager.instance.GetDialogueData(dialogueData.GetOptionNextDialogueId(0)));
    }

    /// 이 메소드는 에디터에 의해서 각 옵션 오브젝트의 Button.OnClick 이벤트에 연결되어 있습니다.
    public void SetOption2Dialogue()
    {
        if(dialogueData == null) return;
        if(dialogueData.IsEnd()) SetDialogue(null);
        else SetDialogue(DataManager.instance.GetDialogueData(dialogueData.GetOptionNextDialogueId(1)));
    }

    /// 이 메소드는 에디터에 의해서 각 옵션 오브젝트의 Button.OnClick 이벤트에 연결되어 있습니다.
    public void SetOption3Dialogue()
    {
        if(dialogueData == null) return;
        if(dialogueData.IsEnd()) SetDialogue(null);
        else SetDialogue(DataManager.instance.GetDialogueData(dialogueData.GetOptionNextDialogueId(2)));
    }

    /// 이 메소드는 에디터에 의해서 각 옵션 오브젝트의 Button.OnClick 이벤트에 연결되어 있습니다.
    public void SetOption4Dialogue()
    {
        if(dialogueData == null) return;
        if(dialogueData.IsEnd()) SetDialogue(null);
        else SetDialogue(DataManager.instance.GetDialogueData(dialogueData.GetOptionNextDialogueId(3)));
    }

    public void AddOnDialogueDataChangedEvent(Action<DialogueData> onDialogueDataChanged)
    {
        this.onDialogueDataChanged += onDialogueDataChanged;
    }
}


// 이 클래스는 CSV 파일에서 읽어온 대화 데이터를 저장합니다.
public class DialogueData
{
    public string id { get; private set; }
    public string npcName { get; private set; }
    public string dialogueText { get; private set; }
    public string nextDialogueId { get; private set; }
    private (string, string)[] option_Text_nextId_Pairs;
    public string questDataId { get; private set; }

    public DialogueData(string csvLine)
    {
        var pattern = ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";
        string[] arr = Regex.Split(csvLine, pattern);
        for(int i = 0; i < arr.Length; i++)
        {
            arr[i] = arr[i].Trim(' ', '"', '\r', '\n');
        }
        if(arr.Length != 13) throw new Exception("DialogueData 생성자의 인자로 전달되는 arr의 길이가 13이 아닙니다.");
        id = arr[0];
        npcName = arr[1];
        dialogueText = arr[2];
        nextDialogueId = arr[3];
        option_Text_nextId_Pairs = new (string, string)[4];
        option_Text_nextId_Pairs[0] = (arr[4], arr[5]);
        option_Text_nextId_Pairs[1] = (arr[6], arr[7]);
        option_Text_nextId_Pairs[2] = (arr[8], arr[9]);
        option_Text_nextId_Pairs[3] = (arr[10], arr[11]);
        questDataId = arr[12];
    }

    public bool IsEnd()
    {
        return !HasNextDialogue() && GetOptionCount() == 0;
    }

    public bool HasNextDialogue()
    {
        return !string.IsNullOrWhiteSpace(nextDialogueId);
    }

    public bool HasQuestData()
    {
        return !string.IsNullOrWhiteSpace(questDataId);
    }

    public int GetOptionCount()
    {
        int result = 0;
        for(int i = 0; i < 4; i++)
        {
            if(!string.IsNullOrWhiteSpace(option_Text_nextId_Pairs[i].Item1))
            {
                result++;
            }
        }
        return result;
    }

    public string GetOptionText(int optionIndex)
    {
        var text = option_Text_nextId_Pairs[optionIndex].Item1;
        if(!text.Contains("#")) return text;

        string[] parts = text.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
        string questId = parts[0].Trim(' ', '"', '\r', '\n');
        string questText_UnAccepted = parts[1].Trim(' ', '"', '\r', '\n');
        string questText_Accepted = parts[2].Trim(' ', '"', '\r', '\n');
        string questText_Completable = parts[3].Trim(' ', '"', '\r', '\n');
        string questText_Completed = parts[4].Trim(' ', '"', '\r', '\n');

        if(!Quest.instance.IsCurrentQuest(questId) && !Quest.instance.IsCompletedQuest(questId))
        {
            return questText_UnAccepted;
        }
        else if(Quest.instance.IsCompletedQuest(questId))
        {
            return questText_Completed;
        }
        else if(Quest.instance.IsCompletableQuest(questId))
        {
            return questText_Completable;
        }
        else
        {
            return questText_Accepted;
        }
    }

    public string GetOptionNextDialogueId(int optionIndex)
    {
        var text = option_Text_nextId_Pairs[optionIndex].Item2;
        if(!text.Contains("#")) return text;

        string[] parts = text.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
        string questId = parts[0].Trim(' ', '"', '\r', '\n');
        string questText_UnAccepted = parts[1].Trim(' ', '"', '\r', '\n');
        string questText_Accepted = parts[2].Trim(' ', '"', '\r', '\n');    
        string questText_Completable = parts[3].Trim(' ', '"', '\r', '\n');
        string questText_Completed = parts[4].Trim(' ', '"', '\r', '\n');

        if(!Quest.instance.IsCurrentQuest(questId) && !Quest.instance.IsCompletedQuest(questId))
        {
            return questText_UnAccepted;
        }
        else if(Quest.instance.IsCompletedQuest(questId))
        {
            return questText_Completed;
        }
        else if(Quest.instance.IsCompletableQuest(questId))
        {
            return questText_Completable;
        }
        else
        {
            return questText_Accepted;
        }
    }
}
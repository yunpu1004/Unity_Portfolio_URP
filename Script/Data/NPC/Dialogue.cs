using System;
using System.Text.RegularExpressions;
using UnityEngine;

// 이 스크립트는 옵저버 패턴을 사용하여 대화 데이터가 변경될 때마다 이벤트를 발생시킵니다.
public class Dialogue : MonoBehaviour
{
    public static Dialogue instance { get; private set; }
    private DialogueData dialogueData;
    private Action<DialogueData> onDialogueDataChanged;

    private void Awake() 
    {
        instance = this;
    }

    public void SetDialogue(DialogueData dialogueData)
    {
        this.dialogueData = dialogueData;
        onDialogueDataChanged?.Invoke(dialogueData);

        // 퀘스트 데이터가 있으면 이를 플레이어의 현재 퀘스트로 추가하거나 완료합니다.
        if(dialogueData != null && dialogueData.IsQuestDataExist())
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

    // 이 메소드는 에디터에 의해서 Continue 오브젝트의 Button.OnClick 이벤트에 연결되어 있습니다.
    public void SetNextDialogue()
    {
        if(dialogueData == null) return;
        if(dialogueData.IsEnd()) SetDialogue(null);
        else if(!dialogueData.IsNextDialogueIdExist()) return;
        else SetDialogue(DataManager.instance.GetDialogueData(dialogueData.nextDialogueId));
    }

    // 이 메소드는 에디터에 의해서 각 옵션 오브젝트의 Button.OnClick 이벤트에 연결되어 있습니다.
    public void SetOption1Dialogue()
    {
        if(dialogueData == null) return;
        if(dialogueData.IsEnd()) SetDialogue(null);
        else SetDialogue(DataManager.instance.GetDialogueData(dialogueData.GetOptionNextDialogueId(0)));
    }

    // 이 메소드는 에디터에 의해서 각 옵션 오브젝트의 Button.OnClick 이벤트에 연결되어 있습니다.
    public void SetOption2Dialogue()
    {
        if(dialogueData == null) return;
        if(dialogueData.IsEnd()) SetDialogue(null);
        else SetDialogue(DataManager.instance.GetDialogueData(dialogueData.GetOptionNextDialogueId(1)));
    }

    // 이 메소드는 에디터에 의해서 각 옵션 오브젝트의 Button.OnClick 이벤트에 연결되어 있습니다.
    public void SetOption3Dialogue()
    {
        if(dialogueData == null) return;
        if(dialogueData.IsEnd()) SetDialogue(null);
        else SetDialogue(DataManager.instance.GetDialogueData(dialogueData.GetOptionNextDialogueId(2)));
    }

    // 이 메소드는 에디터에 의해서 각 옵션 오브젝트의 Button.OnClick 이벤트에 연결되어 있습니다.
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
    public string id { get; private set; }                // 대화 데이터의 ID
    public string npcName { get; private set; }           // NPC 이름
    public string dialogueText { get; private set; }      // 대화 텍스트
    public string nextDialogueId { get; private set; }    // 다음 대화 데이터의 ID
    private (string optionText, string nextDialogueId)[] options;  // 각 선택지의 텍스트와 클릭시 다음 대화 데이터의 ID
    public string questDataId { get; private set; }       // 대화 데이터와 연결된 퀘스트 데이터의 ID

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
        options = new (string, string)[4];
        options[0] = (arr[4], arr[5]);
        options[1] = (arr[6], arr[7]);
        options[2] = (arr[8], arr[9]);
        options[3] = (arr[10], arr[11]);
        questDataId = arr[12];
    }

    // 현재 대화 데이터가 마지막 대화 데이터인지 확인합니다.
    public bool IsEnd()
    {
        return !IsNextDialogueIdExist() && GetOptionCount() == 0;
    }

    // 다음 대화 데이터가 있는지 확인합니다.
    public bool IsNextDialogueIdExist()
    {
        return !string.IsNullOrWhiteSpace(nextDialogueId);
    }

    // 퀘스트 데이터가 있는지 확인합니다.
    public bool IsQuestDataExist()
    {
        return !string.IsNullOrWhiteSpace(questDataId);
    }

    // 선택지의 개수를 반환합니다. (선택지의 텍스트가 비어있는 경우는 선택지가 없는 것으로 간주합니다.)
    public int GetOptionCount()
    {
        int result = 0;
        for(int i = 0; i < 4; i++)
        {
            if(!string.IsNullOrWhiteSpace(GetOptionText(i)))
            {
                result++;
            }
        }
        return result;
    }

    // 퀘스트 상태에 따라 적절하게 선택지의 텍스트를 반환합니다.
    public string GetOptionText(int optionIndex)
    {
        var text = options[optionIndex].optionText;
        if(!text.Contains("#")) return text;

        string[] parts = text.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
        string questId = parts[0].Trim(' ', '"', '\r', '\n');
        string questText_UnAccepted = parts.Length>=2 ?parts[1].Trim(' ', '"', '\r', '\n') : null;
        string questText_Accepted = parts.Length>=3 ?parts[2].Trim(' ', '"', '\r', '\n') : null;
        string questText_Completable = parts.Length>=4 ?parts[3].Trim(' ', '"', '\r', '\n') : null;
        string questText_Completed = parts.Length>=5 ?parts[4].Trim(' ', '"', '\r', '\n') : null;

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

    // 퀘스트 상태에 따라 적절하게 선택지 클릭시 다음 대화 데이터의 ID를 반환합니다.
    public string GetOptionNextDialogueId(int optionIndex)
    {
        var text = options[optionIndex].nextDialogueId;
        if(!text.Contains("#")) return text;

        string[] parts = text.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
        string questId = parts[0].Trim(' ', '"', '\r', '\n');
        string questText_UnAccepted = parts.Length>=2 ?parts[1].Trim(' ', '"', '\r', '\n') : null;
        string questText_Accepted = parts.Length>=3 ?parts[2].Trim(' ', '"', '\r', '\n') : null;
        string questText_Completable = parts.Length>=4 ?parts[3].Trim(' ', '"', '\r', '\n') : null;
        string questText_Completed = parts.Length>=5 ?parts[4].Trim(' ', '"', '\r', '\n') : null;

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
using System.Collections.Generic;
using UnityEngine;

// 이 스크립트는 CSV 파일에서 데이터를 읽어와서 저장합니다.
public class DataLoader : MonoBehaviour
{
    public static DataLoader instance { get; private set; }
    private static Dictionary<string, DialogueData> dialogueDataDict;
    private static Dictionary<string, QuestData> questDataDict;

    private void Awake() 
    {
        instance = this;
        InitData();
    }

    public static void InitData()
    {
        dialogueDataDict = new Dictionary<string, DialogueData>();
        questDataDict = new Dictionary<string, QuestData>();

        var csv = Resources.Load<TextAsset>("DialogueCSV");
        var lines = csv.text.Split('\n');

        for(var i = 1; i < lines.Length; i++)
        {
            if(lines[i].Length == 0) break;
            if(lines[i][0] == ',') break;
            var dialogueData = new DialogueData(lines[i]);
            dialogueDataDict.Add(dialogueData.id, dialogueData);
        }

        csv = Resources.Load<TextAsset>("QuestCSV");
        lines = csv.text.Split('\n');

        for(var i = 1; i < lines.Length; i++)
        {
            if(lines[i].Length == 0) break;
            if(lines[i][0] == ',') break;
            var questData = new QuestData(lines[i]);
            questDataDict.Add(questData.id, questData);
        }
    }

    public DialogueData GetDialogueData(string dialogueDataId)
    {
        if(!dialogueDataDict.ContainsKey(dialogueDataId)) return null;
        else
        {            
            return dialogueDataDict[dialogueDataId];
        }
    }

    public QuestData GetQuestData(string questDataId)
    {
        if(!questDataDict.ContainsKey(questDataId)) return null;
        else return questDataDict[questDataId];
    }
}

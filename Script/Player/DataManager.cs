using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 이 스크립트는 CSV 파일에서 대화, 퀘스트, 아이템, 몬스터 데이터를 읽어와서 저장합니다.
public class DataManager : MonoBehaviour
{
    public static DataManager instance { get; private set; }
    private static Dictionary<string, DialogueData> dialogueDataDict;
    private static Dictionary<string, QuestData> questDataDict;
    private static Dictionary<string, Item> itemDict;
    private static Dictionary<string, string> monsterStatDict;
    private static Dictionary<string, GameObject> itemPrefabDict;

    private void Awake() 
    {
        instance = this;
        InitData();
    }

    public static void InitData()
    {
        dialogueDataDict = new Dictionary<string, DialogueData>();
        questDataDict = new Dictionary<string, QuestData>();
        itemDict = new Dictionary<string, Item>();
        monsterStatDict = new Dictionary<string, string>();

        var csv = Resources.Load<TextAsset>("CSV/DialogueCSV");
        var lines = csv.text.Split('\n');
        for(var i = 1; i < lines.Length; i++)
        {
            if(lines[i].Length == 0) continue;
            if(lines[i][0] == ',') continue;
            var dialogueData = new DialogueData(lines[i]);
            dialogueDataDict.Add(dialogueData.id, dialogueData);
        }

        csv = Resources.Load<TextAsset>("CSV/QuestCSV");
        lines = csv.text.Split('\n');
        for(var i = 1; i < lines.Length; i++)
        {
            if(lines[i].Length == 0) break;
            if(lines[i][0] == ',') break;
            var questData = new QuestData(lines[i]);
            questDataDict.Add(questData.id, questData);
        }

        csv = Resources.Load<TextAsset>("CSV/ItemCSV");
        lines = csv.text.Split('\n');
        for(var i = 1; i < lines.Length; i++)
        {
            if(lines[i].Length == 0) break;
            if(lines[i][0] == ',') break;
            var item = new Item(lines[i]);
            itemDict.Add(item.id, item);
        }

        csv = Resources.Load<TextAsset>("CSV/MonsterStatCSV");
        lines = csv.text.Split('\n',System.StringSplitOptions.RemoveEmptyEntries);
        for(var i = 1; i < lines.Length; i++)
        {
            if(lines[i].Length == 0) break;
            if(lines[i][0] == ',') break; 
            monsterStatDict.Add(lines[i].Split(',')[0], lines[i]);
        }

        itemPrefabDict = new Dictionary<string, GameObject>();
        
        GameObject[] prefabs = Resources.LoadAll<GameObject>("FieldItems");
        foreach(GameObject prefab in prefabs) 
        {
            var fieldItem = prefab.GetComponent<FieldItem>();
            itemPrefabDict.Add(fieldItem.item.id, prefab);
        }
    }

    // id에 해당하는 대화 데이터를 반환합니다.
    public DialogueData GetDialogueData(string dialogueDataId)
    {
        if(!dialogueDataDict.ContainsKey(dialogueDataId)) return null;
        else
        {            
            return dialogueDataDict[dialogueDataId];
        }
    }

    // id에 해당하는 퀘스트 데이터를 반환합니다.
    public QuestData GetQuestData(string questDataId)
    {
        if(!questDataDict.ContainsKey(questDataId)) return null;
        else return questDataDict[questDataId];
    }

    // id에 해당하는 아이템 데이터를 반환합니다.
    public Item GetItem(string itemId)
    {
        if(!itemDict.ContainsKey(itemId)) return Item.GetEmptyItem();
        else return itemDict[itemId];
    }

    // id에 해당하는 몬스터 스탯 데이터를 반환합니다.
    public string GetMonsterStat(string monsterId)
    {
        if(!monsterStatDict.ContainsKey(monsterId)) return null;
        else return monsterStatDict[monsterId];
    }

    // id에 해당하는 아이템 프리팹을 반환합니다.
    public GameObject GetItemPrefab(string itemId)
    {
        if(!itemPrefabDict.ContainsKey(itemId)) return null;
        else return itemPrefabDict[itemId];
    }

    // 특정 NPC가 주는 퀘스트 id들을 반환합니다.
    public string[] GetNpcQuestIds(string npcName)
    {
        return questDataDict.Values.Where(questData => questData.npcName == npcName).Select(questData => questData.id).ToArray();
    }
}

using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// 이 스크립트는 퀘스트 마커 UI를 관리합니다.
// 이 UI는 퀘스트 목표 대상의 위치와 플레이어의 거리를 표시합니다. (ex : 골렘처치 퀘스트의 경우 골렘의 위치와 플레이어와의 거리를 표시)
public class QuestMarkerUI : MonoBehaviour
{
    private List<(RectTransform rectTransform, Image image, TextMeshProUGUI text)> allQuestMarkers;
    private List<(QuestData questData, GameObject target)> questTargetList;
    private Transform player;
    private Camera mainCamera;

    private void Awake()
    {
        allQuestMarkers = new List<(RectTransform, Image, TextMeshProUGUI)>();
        questTargetList = new List<(QuestData, GameObject)>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = Camera.main;

        for (int i = 0; i < transform.childCount; i++)
        {
            var questMarker = transform.GetChild(i);
            var rectTransform = questMarker.GetComponent<RectTransform>();
            var image = questMarker.GetComponentInChildren<Image>(true);
            var text = questMarker.GetComponentInChildren<TextMeshProUGUI>(true);
            allQuestMarkers.Add((rectTransform, image, text));
        }

        Quest.instance.OnCurrentQuestsUpdateEvent += UpdateQuestMarkers;
    }

    // 현재 진행중인 퀘스트 목록을 읽고, 마커를 업데이트합니다.
    private void UpdateQuestMarkers(string questId)
    {
        if(Quest.instance.currentQuests.ContainsKey(questId))
        {
            var questData = Quest.instance.currentQuests[questId];
            if(!questData.markTarget) return;
            var target = GameObject.Find(questData.targetActivityData.activityTarget);
            questTargetList.Add((questData, target));
        }
        else
        {
            questTargetList.RemoveAll(x => x.questData.id == questId);
        }

        int count = questTargetList.Count;
        foreach(var (rt, _, _) in allQuestMarkers)
        {
            rt.gameObject.SetActive(count > 0);
            count--;
        }
    }

    public void Update()
    {
        int index = 0;
        foreach (var (questData, target) in questTargetList)
        {
            var (rectTransform, image, text) = allQuestMarkers[index];

            if(questData.IsCompletable() == true)
            {
                rectTransform.gameObject.SetActive(false);
            }
            else
            {
                rectTransform.position = mainCamera.WorldToScreenPoint(target.transform.position + Vector3.up * 2f);
                float distance = Vector3.Distance(player.position, target.transform.position);
                text.text = $"{distance:F1}M";
                rectTransform.gameObject.SetActive(rectTransform.position.z > 0);
                rectTransform.localScale = Vector3.one * Mathf.Clamp((distance - 10) / 20, 0, 1f);
            }

            index++;
            if (index >= allQuestMarkers.Count) break;
        }
    }
}
